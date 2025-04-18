using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ReadingLargeTextFiles.Core;

namespace ReadingLargeTextFiles.Data
{
    /// <summary>
    /// 使用OleDb导出Excel文件
    /// </summary>
    public class OleDbExcelExporter
    {
        private const int MAX_ROWS_PER_SHEET = 1000000;
        private string filePath;
        private string connectionString;
        private int totalExportedRows = 0;
        private int sheetCount = 0;

        /// <summary>
        /// 创建一个新的OleDbExcelExporter实例
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        public OleDbExcelExporter(string filePath)
        {
            this.filePath = filePath;
            this.connectionString = GetConnectionString(filePath);
        }

        /// <summary>
        /// 导出DataTable到Excel文件
        /// </summary>
        /// <param name="dataTable">要导出的数据表</param>
        /// <param name="sheetName">工作表名称</param>
        /// <param name="progress">进度报告回调</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>导出的行数</returns>
        public async Task<int> ExportDataTableAsync(DataTable dataTable, string sheetName, IProgress<int> progress, CancellationToken cancellationToken)
        {
            totalExportedRows = 0;
            sheetCount = 0;

            try
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await CreateExcelFileAsync();

                int totalRows = dataTable.Rows.Count;
                int totalSheets = (int)Math.Ceiling((double)totalRows / MAX_ROWS_PER_SHEET);

                for (int sheetIndex = 0; sheetIndex < totalSheets; sheetIndex++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        LogManager.Info("导出操作已取消");
                        break;
                    }

                    int startRow = sheetIndex * MAX_ROWS_PER_SHEET;
                    int rowCount = Math.Min(MAX_ROWS_PER_SHEET, totalRows - startRow);

                    string currentSheetName = totalSheets > 1 ? $"{sheetName}_{sheetIndex + 1}" : sheetName;

                    await CreateSheetAsync(currentSheetName, dataTable.Columns);

                    int exportedRows = await ExportDataToSheetAsync(dataTable, startRow, rowCount, currentSheetName, progress, cancellationToken);
                    totalExportedRows += exportedRows;
                    sheetCount++;

                    int progressPercentage = (int)((double)(startRow + rowCount) / totalRows * 100);
                    progress?.Report(progressPercentage);
                }

                LogManager.Info($"导出完成，共导出 {totalExportedRows} 行数据到 {sheetCount} 个工作表");
                return totalExportedRows;
            }
            catch (Exception ex)
            {
                LogManager.Error($"导出数据时出错: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// 创建Excel文件
        /// </summary>
        private async Task CreateExcelFileAsync()
        {
            try
            {
                using (FileStream fs = File.Create(filePath))
                {
                    byte[] excelHeader = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
                    await fs.WriteAsync(excelHeader, 0, excelHeader.Length);
                }

                LogManager.Info($"已创建Excel文件: {filePath}");
            }
            catch (Exception ex)
            {
                LogManager.Error($"创建Excel文件时出错: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// 创建工作表
        /// </summary>
        /// <param name="sheetName">工作表名称</param>
        /// <param name="columns">列集合</param>
        private async Task CreateSheetAsync(string sheetName, DataColumnCollection columns)
        {
            try
            {
                string createTableSql = $"CREATE TABLE [{sheetName}] (";
                for (int i = 0; i < columns.Count; i++)
                {
                    string columnName = columns[i].ColumnName;
                    createTableSql += $"[{columnName}] TEXT";
                    if (i < columns.Count - 1)
                    {
                        createTableSql += ", ";
                    }
                }
                createTableSql += ")";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (OleDbCommand command = new OleDbCommand(createTableSql, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }

                LogManager.Info($"已创建工作表: {sheetName}");
            }
            catch (Exception ex)
            {
                LogManager.Error($"创建工作表时出错: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// 导出数据到工作表
        /// </summary>
        /// <param name="dataTable">数据表</param>
        /// <param name="startRow">起始行</param>
        /// <param name="rowCount">行数</param>
        /// <param name="sheetName">工作表名称</param>
        /// <param name="progress">进度报告回调</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>导出的行数</returns>
        private async Task<int> ExportDataToSheetAsync(DataTable dataTable, int startRow, int rowCount, string sheetName, IProgress<int> progress, CancellationToken cancellationToken)
        {
            int exportedRows = 0;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string insertSql = $"INSERT INTO [{sheetName}] (";
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        string columnName = dataTable.Columns[i].ColumnName;
                        insertSql += $"[{columnName}]";
                        if (i < dataTable.Columns.Count - 1)
                        {
                            insertSql += ", ";
                        }
                    }
                    insertSql += ") VALUES (";
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        insertSql += $"@p{i}";
                        if (i < dataTable.Columns.Count - 1)
                        {
                            insertSql += ", ";
                        }
                    }
                    insertSql += ")";

                    const int batchSize = 1000;
                    for (int i = startRow; i < startRow + rowCount; i += batchSize)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            LogManager.Info("导出操作已取消");
                            break;
                        }

                        int currentBatchSize = Math.Min(batchSize, startRow + rowCount - i);

                        using (OleDbTransaction transaction = connection.BeginTransaction())
                        {
                            using (OleDbCommand command = new OleDbCommand(insertSql, connection, transaction))
                            {
                                for (int j = 0; j < dataTable.Columns.Count; j++)
                                {
                                    command.Parameters.Add($"@p{j}", OleDbType.VarChar);
                                }

                                for (int j = i; j < i + currentBatchSize; j++)
                                {
                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        LogManager.Info("导出操作已取消");
                                        break;
                                    }

                                    for (int k = 0; k < dataTable.Columns.Count; k++)
                                    {
                                        command.Parameters[$"@p{k}"].Value = dataTable.Rows[j][k].ToString();
                                    }

                                    await command.ExecuteNonQueryAsync();
                                    exportedRows++;
                                }
                            }

                            transaction.Commit();
                        }

                        int progressPercentage = (int)((double)(i + currentBatchSize - startRow) / rowCount * 100);
                        progress?.Report(progressPercentage);
                    }
                }

                LogManager.Info($"已导出 {exportedRows} 行数据到工作表 {sheetName}");
                return exportedRows;
            }
            catch (Exception ex)
            {
                LogManager.Error($"导出数据到工作表时出错: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// 获取Excel文件的连接字符串
        /// </summary>
        private string GetConnectionString(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            string connectionString;

            if (extension == ".xlsx" || extension == ".xlsm")
            {
                connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"";
            }
            else
            {
                connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            }

            return connectionString;
        }
    }
}
