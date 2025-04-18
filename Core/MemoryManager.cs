using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReadingLargeTextFiles.Core
{
    /// <summary>
    /// 内存管理类，用于优化内存使用
    /// </summary>
    public static class MemoryManager
    {
        // 上次垃圾回收的时间
        private static DateTime lastGCTime = DateTime.MinValue;

        // 最小垃圾回收间隔（毫秒）
        private static readonly int MIN_GC_INTERVAL = 5000; // 5秒

        /// <summary>
        /// 检查内存状态，如果内存不足则清理缓存
        /// </summary>
        public static void CheckMemoryStatus()
        {
            try
            {
                // 获取当前进程的内存使用情况
                Process currentProcess = Process.GetCurrentProcess();
                long memoryUsed = currentProcess.WorkingSet64;

                // 如果内存使用超过阈值，进行垃圾回收
                if (memoryUsed > AppConfig.MemoryThreshold)
                {
                    // 检查是否已经过了最小垃圾回收间隔
                    if ((DateTime.Now - lastGCTime).TotalMilliseconds > MIN_GC_INTERVAL)
                    {
                        LogManager.Debug($"内存使用超过阈值 ({memoryUsed / (1024 * 1024)} MB > {AppConfig.MemoryThreshold / (1024 * 1024)} MB)，执行垃圾回收");
                        
                        // 强制进行完整垃圾回收
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        
                        // 更新最后垃圾回收时间
                        lastGCTime = DateTime.Now;
                        
                        // 记录垃圾回收后的内存使用情况
                        long memoryAfterGC = Process.GetCurrentProcess().WorkingSet64;
                        LogManager.Debug($"垃圾回收后内存使用: {memoryAfterGC / (1024 * 1024)} MB，释放了 {(memoryUsed - memoryAfterGC) / (1024 * 1024)} MB");
                    }
                }
                else if (AppConfig.EnableMemoryOptimization)
                {
                    // 如果启用了内存优化模式，进行轻量级垃圾回收
                    if ((DateTime.Now - lastGCTime).TotalMilliseconds > MIN_GC_INTERVAL * 2)
                    {
                        LogManager.Debug($"执行轻量级垃圾回收，当前内存使用: {memoryUsed / (1024 * 1024)} MB");
                        GC.Collect(0, GCCollectionMode.Optimized, false);
                        lastGCTime = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("检查内存状态时出错", ex);
                
                // 忽略错误，使用默认的垃圾回收机制
                GC.Collect(0, GCCollectionMode.Optimized, false);
            }
        }

        /// <summary>
        /// 异步检查内存状态
        /// </summary>
        public static async Task CheckMemoryStatusAsync()
        {
            await Task.Run(() => CheckMemoryStatus());
        }

        /// <summary>
        /// 强制进行垃圾回收
        /// </summary>
        /// <param name="full">是否进行完整垃圾回收</param>
        public static void ForceGarbageCollection(bool full = false)
        {
            try
            {
                LogManager.Debug($"强制进行{(full ? "完整" : "轻量级")}垃圾回收");
                
                if (full)
                {
                    // 完整垃圾回收
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
                else
                {
                    // 轻量级垃圾回收
                    GC.Collect(0, GCCollectionMode.Optimized, false);
                }
                
                lastGCTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                LogManager.Error("强制垃圾回收时出错", ex);
            }
        }

        /// <summary>
        /// 获取当前内存使用情况
        /// </summary>
        /// <returns>内存使用信息</returns>
        public static string GetMemoryUsageInfo()
        {
            try
            {
                Process currentProcess = Process.GetCurrentProcess();
                long workingSet = currentProcess.WorkingSet64;
                long privateMemory = currentProcess.PrivateMemorySize64;
                long peakWorkingSet = currentProcess.PeakWorkingSet64;
                
                return $"工作集: {workingSet / (1024 * 1024)} MB, 私有内存: {privateMemory / (1024 * 1024)} MB, 峰值工作集: {peakWorkingSet / (1024 * 1024)} MB";
            }
            catch (Exception ex)
            {
                LogManager.Error("获取内存使用信息时出错", ex);
                return "无法获取内存使用信息";
            }
        }
    }
}
