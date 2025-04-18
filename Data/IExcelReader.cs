using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReadingLargeTextFiles.Data
{
    /// <summary>
    /// Excel读取器接口，用于统一不同的Excel读取器实现
    /// </summary>
    public interface IExcelReader : IDisposable
    {
        /// <summary>
        /// 初始化Excel读取器，获取工作表列表
        /// </summary>
        /// <returns>工作表名称列表</returns>
        Task<List<string>> InitializeAsync();

        /// <summary>
        /// 获取工作表的行数
        /// </summary>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>行数</returns>
        int GetRowCount(string sheetName);

        /// <summary>
        /// 获取工作表的列标题
        /// </summary>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>列标题列表</returns>
        List<string> GetColumnHeaders(string sheetName);

        /// <summary>
        /// 读取工作表的指定范围数据
        /// </summary>
        /// <param name="sheetName">工作表名称</param>
        /// <param name="startRow">起始行（从0开始）</param>
        /// <param name="rowCount">读取的行数</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>数据行列表</returns>
        Task<List<string[]>> ReadSheetChunkAsync(string sheetName, int startRow, int rowCount, CancellationToken cancellationToken);
    }
}
