using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using System.Threading.Tasks;
using ReadingLargeTextFiles.Core;

namespace ReadingLargeTextFiles
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                AppConfig.Initialize();

                AppConfig.CleanupTempFiles();

                LogSystemInfo();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.ApplicationExit += Application_ApplicationExit;

                LogManager.Info("应用程序启动");
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"应用程序启动失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.Error("应用程序启动失败", ex);
            }
        }

        /// <summary>
        /// 应用程序退出时清理临时文件
        /// </summary>
        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            LogManager.Info("应用程序退出");
            AppConfig.CleanupTempFiles();
        }

        /// <summary>
        /// 记录系统信息
        /// </summary>
        private static void LogSystemInfo()
        {
            try
            {
                ComputerInfo computerInfo = new ComputerInfo();
                long totalMemory = (long)computerInfo.TotalPhysicalMemory;
                long availableMemory = (long)computerInfo.AvailablePhysicalMemory;
                string osArchitecture = Environment.Is64BitOperatingSystem ? "64位" : "32位";
                string processorCount = Environment.ProcessorCount.ToString();
                string currentDirectory = Environment.CurrentDirectory;

                LogManager.Info($"CPU核心数: {processorCount}");
                LogManager.Info($"总内存: {totalMemory / (1024 * 1024)} MB");
                LogManager.Info($"可用内存: {availableMemory / (1024 * 1024)} MB");
                LogManager.Info($"当前目录: {currentDirectory}");

                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives)
                {
                    if (drive.IsReady)
                    {
                        LogManager.Info($"驱动器 {drive.Name} - 总空间: {drive.TotalSize / (1024 * 1024 * 1024)} GB, 可用空间: {drive.AvailableFreeSpace / (1024 * 1024 * 1024)} GB");
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("获取系统信息失败", ex);
            }
        }
    }
}
