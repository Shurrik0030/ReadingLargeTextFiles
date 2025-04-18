using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReadingLargeTextFiles.Core;

namespace ReadingLargeTextFiles.Data
{
    /// <summary>
    /// 使用OleDb读取Excel文件的实现
    /// </summary>
    public class OleDbExcelReader : IExcelReader
    {
        private string filePath;
        private Dictionary<string, int> sheetRowCounts = new Dictionary<string, int>();
        private List<string> sheetNames = new List<string>();
        private Dictionary<string, List<string>> columnHeaders = new Dictionary<string, List<string>>();
        private bool isDisposed = false;

        public OleDbExcelReader(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// 初始化Excel读取器，获取工作表列表
        /// </summary>
        /// <returns>工作表名称列表</returns>
        public async Task<List<string>> InitializeAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    LogManager.Info($"正在初始化OleDb Excel读取器: {filePath}");

                    sheetNames.Clear();
                    sheetRowCounts.Clear();
                    columnHeaders.Clear();

                    string connectionString = GetConnectionString(filePath);

                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();

                        DataTable schemasTable = connection.GetOleDbSchemaTable(
                            OleDbSchemaGuid.Tables,
                            new object[] { null, null, null, "TABLE" });

                        foreach (DataRow row in schemasTable.Rows)
                        {
                            string sheetName = row["TABLE_NAME"].ToString();

                            sheetName = sheetName.Replace("'", "").Replace("$", "");

                            if (!sheetName.StartsWith("_") && !sheetName.Contains("FilterDatabase"))
                            {
                                sheetNames.Add(sheetName);

                                GetSheetInfo(connection, sheetName);
                            }
                        }

                        connection.Close();
                    }

                    LogManager.Info($"OleDb Excel读取器初始化完成，共找到 {sheetNames.Count} 个工作表");
                    return sheetNames;
                }
                catch (Exception ex)
                {
                    LogManager.Error($"初始化OleDb Excel读取器时出错: {ex.Message}", ex);
                    throw;
                }
            });
        }

        /// <summary>
        /// 获取工作表的行数和列标题
        /// </summary>
        private void GetSheetInfo(OleDbConnection connection, string sheetName)
        {
            try
            {
                string countQuery = $"SELECT COUNT(*) FROM [{sheetName}$]";
                using (OleDbCommand command = new OleDbCommand(countQuery, connection))
                {
                    int rowCount = Convert.ToInt32(command.ExecuteScalar());
                    sheetRowCounts[sheetName] = rowCount > 0 ? rowCount - 1 : 0;
                }

                string headerQuery = $"SELECT TOP 1 * FROM [{sheetName}$]";
                using (OleDbCommand command = new OleDbCommand(headerQuery, connection))
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    List<string> headers = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        headers.Add(reader.GetName(i));
                    }
                    columnHeaders[sheetName] = headers;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"获取工作表 {sheetName} 信息时出错: {ex.Message}", ex);
                sheetRowCounts[sheetName] = 0;
                columnHeaders[sheetName] = new List<string>();
            }
        }

        /// <summary>
        /// 获取工作表的行数
        /// </summary>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>行数</returns>
        public int GetRowCount(string sheetName)
        {
            if (sheetRowCounts.ContainsKey(sheetName))
            {
                return sheetRowCounts[sheetName];
            }
            return 0;
        }

        /// <summary>
        /// 获取工作表的列标题
        /// </summary>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>列标题列表</returns>
        public List<string> GetColumnHeaders(string sheetName)
        {
            if (columnHeaders.ContainsKey(sheetName))
            {
                return columnHeaders[sheetName];
            }
            return new List<string>();
        }

        /// <summary>
        /// 读取工作表的指定范围数据
        /// </summary>
        /// <param name="sheetName">工作表名称</param>
        /// <param name="startRow">起始行（从0开始）</param>
        /// <param name="rowCount">读取的行数</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>数据行列表</returns>
        public async Task<List<string[]>> ReadSheetChunkAsync(string sheetName, int startRow, int rowCount, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                List<string[]> result = new List<string[]>();

                try
                {
                    LogManager.Debug($"正在读取工作表 {sheetName} 的数据，起始行: {startRow}，行数: {rowCount}");

                    string connectionString = GetConnectionString(filePath);

                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();

                        int oleDbStartRow = startRow + 2;
                        int oleDbEndRow = oleDbStartRow + rowCount - 1;

                        int columnCount = columnHeaders.ContainsKey(sheetName) ?
                            columnHeaders[sheetName].Count : 0;

                        if (columnCount == 0)
                        {
                            string headerQuery = $"SELECT TOP 1 * FROM [{sheetName}$]";
                            using (OleDbCommand command = new OleDbCommand(headerQuery, connection))
                            using (OleDbDataReader reader = command.ExecuteReader())
                            {
                                columnCount = reader.FieldCount;
                            }
                        }

                        string query = $"SELECT * FROM [{sheetName}$] WHERE (ROW >= {oleDbStartRow} AND ROW <= {oleDbEndRow})";

                        if (!SupportsRowFunction())
                        {
                            query = $"SELECT * FROM [{sheetName}$] LIMIT {rowCount} OFFSET {startRow + 1}";

                            if (!SupportsSqlLimitOffset())
                            {
                                query = $"SELECT * FROM [{sheetName}$]";
                            }
                        }

                        using (OleDbCommand command = new OleDbCommand(query, connection))
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            if (!SupportsRowFunction() && !SupportsSqlLimitOffset())
                            {
                                for (int i = 0; i <= startRow; i++)
                                {
                                    if (!reader.Read())
                                        break;
                                }
                            }

                            int rowIndex = 0;
                            while (reader.Read() && rowIndex < rowCount)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    LogManager.Debug("读取操作已取消");
                                    break;
                                }

                                string[] rowData = new string[columnCount];
                                for (int i = 0; i < columnCount; i++)
                                {
                                    if (i < reader.FieldCount)
                                    {
                                        rowData[i] = reader.IsDBNull(i) ? string.Empty : reader.GetValue(i).ToString();
                                    }
                                    else
                                    {
                                        rowData[i] = string.Empty;
                                    }
                                }

                                result.Add(rowData);
                                rowIndex++;
                            }
                        }

                        connection.Close();
                    }

                    LogManager.Debug($"成功读取 {result.Count} 行数据");
                }
                catch (Exception ex)
                {
                    LogManager.Error($"读取工作表 {sheetName} 数据时出错: {ex.Message}", ex);
                }

                return result;
            }, cancellationToken);
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

        /// <summary>
        /// 检查是否支持ROW函数
        /// </summary>
        private bool SupportsRowFunction()
        {
            return false;
        }

        /// <summary>
        /// 检查是否支持SQL LIMIT和OFFSET
        /// </summary>
        private bool SupportsSqlLimitOffset()
        {
            return false;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                sheetNames.Clear();
                sheetRowCounts.Clear();
                columnHeaders.Clear();

                isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
