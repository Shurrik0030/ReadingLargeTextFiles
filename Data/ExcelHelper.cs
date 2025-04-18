using System;
using System.Collections.Generic;
using System.IO;
using ReadingLargeTextFiles.Core;

namespace ReadingLargeTextFiles.Data
{
    /// <summary>
    /// Excel辅助类，提供Excel相关的通用操作
    /// </summary>
    public static class ExcelHelper
    {
        #region 文件类型检查

        /// <summary>
        /// 检查文件是否为Excel文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否为Excel文件</returns>
        public static bool IsExcelFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return false;

            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".xlsx" || extension == ".xls" || extension == ".csv";
        }

        /// <summary>
        /// 检查文件是否为XLSX格式
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否为XLSX格式</returns>
        public static bool IsXlsxFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".xlsx";
        }

        /// <summary>
        /// 检查文件是否为XLS格式
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否为XLS格式</returns>
        public static bool IsXlsFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".xls";
        }

        /// <summary>
        /// 检查文件是否为CSV格式
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否为CSV格式</returns>
        public static bool IsCsvFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".csv";
        }

        #endregion

        #region 列名处理

        /// <summary>
        /// 处理列名，确保没有重复的列名
        /// </summary>
        /// <param name="headers">原始列名列表</param>
        /// <returns>处理后的列名列表</returns>
        public static List<string> ProcessColumnHeaders(List<string> headers)
        {
            if (headers == null || headers.Count == 0)
                return new List<string>();

            List<string> result = new List<string>(headers.Count);
            Dictionary<string, int> columnNameCount = new Dictionary<string, int>();

            for (int i = 0; i < headers.Count; i++)
            {
                string headerText = headers[i];

                if (string.IsNullOrWhiteSpace(headerText))
                {
                    headerText = $"Column {i+1}";
                }

                if (columnNameCount.ContainsKey(headerText))
                {
                    columnNameCount[headerText]++;
                    headerText = $"{headerText}_{columnNameCount[headerText]}";
                }
                else
                {
                    columnNameCount[headerText] = 1;
                }

                result.Add(headerText);
            }

            return result;
        }

        #endregion

        #region 文件信息

        /// <summary>
        /// 获取Excel文件的大小信息
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件大小信息</returns>
        public static string GetFileSizeInfo(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return "文件不存在";

            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                long fileSizeBytes = fileInfo.Length;

                if (fileSizeBytes < 1024)
                    return $"{fileSizeBytes} B";
                else if (fileSizeBytes < 1024 * 1024)
                    return $"{fileSizeBytes / 1024.0:F2} KB";
                else if (fileSizeBytes < 1024 * 1024 * 1024)
                    return $"{fileSizeBytes / (1024.0 * 1024):F2} MB";
                else
                    return $"{fileSizeBytes / (1024.0 * 1024 * 1024):F2} GB";
            }
            catch (Exception ex)
            {
                LogManager.Error($"获取文件大小信息时出错: {ex.Message}", ex);
                return "无法获取文件大小";
            }
        }

        #endregion
    }
}
