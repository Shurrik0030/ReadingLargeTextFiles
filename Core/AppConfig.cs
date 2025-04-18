using System;
using System.IO;

namespace ReadingLargeTextFiles.Core
{
    /// <summary>
    /// 应用程序配置类，用于存储全局配置
    /// </summary>
    public static class AppConfig
    {
        #region 内存管理配置

        /// <summary>
        /// 内存阈值，超过该值时进行垃圾回收（默认为500MB）
        /// 对于8GB内存的电脑，设置为较小的值
        /// </summary>
        public static long MemoryThreshold = 500 * 1024 * 1024; // 500MB

        /// <summary>
        /// Excel读取时的默认页大小
        /// </summary>
        public static int DefaultPageSize = 1000;

        /// <summary>
        /// Excel读取时的最大缓存工作表数量
        /// </summary>
        public static int MaxCachedSheets = 5;

        /// <summary>
        /// 是否启用内存优化模式（针对低内存电脑）
        /// </summary>
        public static bool EnableMemoryOptimization = true;

        #endregion

        #region 文件路径配置

        /// <summary>
        /// 应用程序数据目录
        /// </summary>
        public static string AppDataPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "data");

        /// <summary>
        /// 临时文件目录
        /// </summary>
        public static string TempPath = Path.Combine(AppDataPath, "temp");

        /// <summary>
        /// 日志文件目录
        /// </summary>
        public static string LogPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "logs");

        #endregion

        #region UI配置

        /// <summary>
        /// 是否显示调试信息
        /// </summary>
        public static bool ShowDebugInfo = false;

        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public static bool EnableVerboseLogging = false;

        /// <summary>
        /// 是否启用文件日志
        /// </summary>
        public static bool EnableFileLogging = true;

        #endregion

        /// <summary>
        /// 初始化应用程序配置
        /// </summary>
        public static void Initialize()
        {
            // 创建必要的目录
            EnsureDirectoryExists(AppDataPath);
            EnsureDirectoryExists(TempPath);
            EnsureDirectoryExists(LogPath);

            // 加载用户设置
            LoadUserPageSize();

            // 根据系统内存调整配置
            AdjustConfigBasedOnSystemMemory();

            // 初始化日志管理器
            LogManager.Initialize(
                EnableVerboseLogging ? LogManager.LogLevel.Debug : LogManager.LogLevel.Info,
                EnableFileLogging,
                true);

            LogManager.Info("应用程序配置初始化完成");
        }

        /// <summary>
        /// 确保目录存在
        /// </summary>
        /// <param name="path">目录路径</param>
        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 根据系统内存调整配置
        /// </summary>
        private static void AdjustConfigBasedOnSystemMemory()
        {
            try
            {
                // 获取系统内存信息
                long totalMemory = GetTotalPhysicalMemory();
                long availableMemory = GetAvailablePhysicalMemory();

                LogManager.Info($"系统总内存: {totalMemory / (1024 * 1024)} MB, 可用内存: {availableMemory / (1024 * 1024)} MB");

                // 根据系统内存调整配置
                if (totalMemory < 8L * 1024 * 1024 * 1024) // 小于8GB
                {
                    // 针对低内存电脑的优化
                    MemoryThreshold = 200 * 1024 * 1024; // 200MB
                    DefaultPageSize = 500;
                    MaxCachedSheets = 2;
                    EnableMemoryOptimization = true;
                    LogManager.Info("检测到低内存环境，已启用内存优化模式");
                }
                else if (totalMemory < 16L * 1024 * 1024 * 1024) // 8GB-16GB
                {
                    // 针对中等内存电脑的优化
                    MemoryThreshold = 500 * 1024 * 1024; // 500MB
                    DefaultPageSize = 1000;
                    MaxCachedSheets = 5;
                    EnableMemoryOptimization = true;
                    LogManager.Info("检测到中等内存环境，已启用内存优化模式");
                }
                else // 大于16GB
                {
                    // 针对高内存电脑的优化
                    MemoryThreshold = 1024 * 1024 * 1024; // 1GB
                    DefaultPageSize = 2000;
                    MaxCachedSheets = 10;
                    EnableMemoryOptimization = false;
                    LogManager.Info("检测到高内存环境，已禁用内存优化模式");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("获取系统内存信息失败，使用默认配置", ex);
            }
        }

        /// <summary>
        /// 获取系统总物理内存
        /// </summary>
        /// <returns>总物理内存（字节）</returns>
        private static long GetTotalPhysicalMemory()
        {
            return (long)new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }

        /// <summary>
        /// 获取系统可用物理内存
        /// </summary>
        /// <returns>可用物理内存（字节）</returns>
        private static long GetAvailablePhysicalMemory()
        {
            return (long)new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory;
        }

        /// <summary>
        /// 保存用户设置的每页行数
        /// </summary>
        /// <param name="pageSize">每页行数</param>
        public static void SaveUserPageSize(int pageSize)
        {
            try
            {
                // 将用户设置保存到配置文件
                string configPath = Path.Combine(AppDataPath, "config.txt");
                File.WriteAllText(configPath, $"PageSize={pageSize}");
                LogManager.Debug($"已保存用户设置的每页行数: {pageSize}");

                // 更新默认值
                DefaultPageSize = pageSize;
            }
            catch (Exception ex)
            {
                LogManager.Error($"保存用户设置时出错: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 加载用户设置的每页行数
        /// </summary>
        /// <returns>用户设置的每页行数，如果没有设置则返回默认值</returns>
        public static int LoadUserPageSize()
        {
            try
            {
                string configPath = Path.Combine(AppDataPath, "config.txt");
                if (File.Exists(configPath))
                {
                    string content = File.ReadAllText(configPath);
                    if (content.StartsWith("PageSize="))
                    {
                        string pageSizeStr = content.Substring(9);
                        if (int.TryParse(pageSizeStr, out int pageSize) && pageSize > 0)
                        {
                            LogManager.Debug($"已加载用户设置的每页行数: {pageSize}");
                            DefaultPageSize = pageSize;
                            return pageSize;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"加载用户设置时出错: {ex.Message}", ex);
            }

            return DefaultPageSize;
        }

        /// <summary>
        /// 清理临时文件
        /// </summary>
        public static void CleanupTempFiles()
        {
            try
            {
                if (Directory.Exists(TempPath))
                {
                    LogManager.Info($"正在清理临时文件夹: {TempPath}");

                    // 删除临时目录中的所有文件
                    foreach (string file in Directory.GetFiles(TempPath))
                    {
                        try
                        {
                            File.Delete(file);
                            LogManager.Debug($"已删除临时文件: {file}");
                        }
                        catch (Exception ex)
                        {
                            LogManager.Warning($"无法删除临时文件 {file}: {ex.Message}");
                        }
                    }

                    // 删除临时目录中的所有子目录
                    foreach (string dir in Directory.GetDirectories(TempPath))
                    {
                        try
                        {
                            Directory.Delete(dir, true);
                            LogManager.Debug($"已删除临时目录: {dir}");
                        }
                        catch (Exception ex)
                        {
                            LogManager.Warning($"无法删除临时目录 {dir}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("清理临时文件时出错", ex);
            }
        }
    }
}
