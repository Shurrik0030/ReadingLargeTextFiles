using System;
using System.Collections.Generic;

namespace ReadingLargeTextFiles.Data
{
    /// <summary>
    /// 工作表信息类，用于存储Excel工作表的元数据
    /// </summary>
    public class SheetInfo
    {
        #region 属性

        /// <summary>
        /// 工作表名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 工作表索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 行数
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// 列数
        /// </summary>
        public int ColumnCount { get; set; }

        /// <summary>
        /// 列标题
        /// </summary>
        public List<string> ColumnHeaders { get; set; } = new List<string>();

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime LastAccessed { get; set; }

        /// <summary>
        /// 是否已加载列标题
        /// </summary>
        public bool HeadersLoaded => ColumnHeaders.Count > 0;

        /// <summary>
        /// 是否已加载行数
        /// </summary>
        public bool RowCountLoaded => RowCount > 0;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">工作表名称</param>
        /// <param name="index">工作表索引</param>
        public SheetInfo(string name, int index)
        {
            Name = name;
            Index = index;
            LastAccessed = DateTime.Now;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 更新最后访问时间
        /// </summary>
        public void UpdateLastAccessed()
        {
            LastAccessed = DateTime.Now;
        }

        /// <summary>
        /// 重置列标题
        /// </summary>
        public void ResetHeaders()
        {
            ColumnHeaders.Clear();
        }

        /// <summary>
        /// 添加列标题
        /// </summary>
        /// <param name="header">列标题</param>
        public void AddHeader(string header)
        {
            ColumnHeaders.Add(header);
        }

        /// <summary>
        /// 获取工作表信息的字符串表示
        /// </summary>
        /// <returns>工作表信息的字符串表示</returns>
        public override string ToString()
        {
            return $"{Name} (索引: {Index}, 行数: {RowCount}, 列数: {ColumnCount})";
        }

        #endregion
    }
}
