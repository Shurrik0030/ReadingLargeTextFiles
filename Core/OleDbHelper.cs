using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ReadingLargeTextFiles.Core
{
    /// <summary>
    /// OleDb辅助类，提供OleDb相关的通用操作
    /// </summary>
    public static class OleDbHelper
    {
        /// <summary>
        /// 显示安装Microsoft Access Database Engine的提示
        /// </summary>
        public static void ShowInstallDialog()
        {
            string message = "未在本地计算机上注册 Microsoft.ACE.OLEDB.12.0 提供程序。\n\n" +
                "要使用Excel功能，请安装 Microsoft Access Database Engine 2016 Redistributable。\n\n" +
                "是否打开下载页面？";

            DialogResult result = MessageBox.Show(message, "缺少Excel提供程序",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // 打开下载页面
                    Process.Start("https://www.microsoft.com/en-us/download/details.aspx?id=54920");
                }
                catch (Exception ex)
                {
                    LogManager.Error($"无法打开下载页面: {ex.Message}", ex);
                    MessageBox.Show($"无法打开下载页面: {ex.Message}\n\n" +
                        "请手动访问以下网址下载安装程序：\n" +
                        "https://www.microsoft.com/en-us/download/details.aspx?id=54920",
                        "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
