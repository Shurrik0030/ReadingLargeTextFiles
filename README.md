# 大型数据文件处理工具

一个专为处理大型数据文件设计的工具，针对低内存环境（8GB RAM）进行了优化，可以处理Excel、文本文件和数据库数据。采用流式处理和分页加载技术，能够在普通配置的计算机上处理超大型数据文件。

## 功能特点

- **Excel文件查看器**：采用OleDb技术，分页加载数据，避免内存溢出，支持XLSX和XLS格式
- **文本文件阅读器**：支持大型文本文件（>10GB），多种编码支持（UTF-8、GBK、ASCII等）
- **数据库导出工具**：支持SQL Server连接，自定义SQL查询，导出到Excel，支持超大数据量导出
- **多Sheet处理**：支持Excel多工作表处理，自动在同一Excel文件中创建多个Sheet
- **内存优化**：智能内存管理，根据系统内存自动调整处理策略


## 系统要求

- **操作系统**：Windows 7/8/10/11（64位）
- **运行环境**：.NET Framework 4.7.2
- **最低配置**：4GB RAM，20GB可用磁盘空间
- **推荐配置**：8GB+ RAM，SSD存储
- **必需组件**：Microsoft Access Database Engine 2016 Redistributable（用于Excel文件处理）

## 说明

1. 安装Microsoft Access Database Engine 2016 Redistributable
___[下载地址](https://www.microsoft.com/en-us/download/details.aspx?id=54920)___

   **注意**：
   如果已安装64位Office，请下载64位版本，否则请下载32位版本


## 架构设计

```
项目架构
├── 核心组件
│   ├── AppConfig - 应用程序配置管理
│   ├── LogManager - 日志管理
│   ├── MemoryManager - 内存管理
│   └── OleDbHelper - OleDb操作辅助类
├── 数据处理组件
│   ├── OleDbExcelReader - 基于OleDb的Excel读取器
│   ├── OleDbExcelExporter - 基于OleDb的Excel导出器
│   ├── ExcelHelper - Excel操作辅助类
│   └── SheetInfo - 工作表信息类
└── 用户界面组件
    ├── MainForm - 主窗体
    ├── UnifiedExcelViewerForm - 统一Excel查看器
    ├── TextReaderForm - 文本阅读器
    └── DbToExcelForm - 数据库导出工具
```

## 使用指南

### Excel文件查看器

1. 点击主界面的"Excel查看器"按钮
2. 点击"打开文件"按钮，选择要打开的Excel文件
3. 从下拉列表中选择要查看的工作表
4. 使用分页控件浏览数据
5. 可以通过下拉列表调整每页显示的行数（100-10000行）

### 文本文件阅读器

1. 点击主界面的"文本阅读器"按钮
2. 点击"打开文件"按钮，选择要打开的文本文件
3. 选择文件编码（如果自动检测不正确）
4. 使用分页控件浏览数据
5. 可以通过输入框调整每页显示的行数

### 数据库导出工具

1. 点击主界面的"数据库导出"按钮
2. 输入数据库连接信息（服务器地址、用户名、密码）
3. 点击"连接"按钮，连接到数据库
4. 选择要导出的数据库
5. 选择导出方式：选择表或自定义SQL
6. 设置导出路径
7. 点击"导出"按钮开始导出
8. 导出过程中可以暂停或取消操作

## 内存优化策略

为了在低内存环境下高效处理大型文件，本应用采用以下策略：

1. **分块读取**：按需加载数据，而不是一次性加载整个文件
2. **内存监控**：实时监控内存使用情况，超过阈值时进行垃圾回收
3. **资源释放**：及时释放不再使用的资源
4. **临时文件清理**：定期清理临时文件，避免磁盘空间不足
5. **自适应配置**：根据系统内存自动调整处理参数

### 内存使用优化

程序会根据系统内存大小自动调整以下参数：

| 系统内存 | 内存阈值 | 默认页大小 | 缓存工作表数量 |
|---------|---------|-----------|------------|
| <8GB    | 200MB   | 500行     | 2个          |
| 8-16GB  | 500MB   | 1000行    | 5个          |
| >16GB   | 1GB     | 2000行    | 10个         |

## 技术实现

### Excel文件处理

使用OleDb技术处理Excel文件，相比NPOI等库有以下优势：

1. **内存占用更低**：OleDb不需要将整个Excel文件加载到内存中
2. **处理速度更快**：对于大型文件，OleDb的分页读取性能更好
3. **支持更大文件**：理论上可以处理任意大小的Excel文件

### 数据库连接

使用ADO.NET技术连接SQL Server数据库，支持以下功能：

1. **自定义SQL查询**：支持复杂的SQL查询语句
2. **大数据量导出**：采用流式处理技术，支持导出大量数据
3. **多Sheet导出**：自动将超过单个Sheet限制的数据分散到多个Sheet

### 文本文件处理

采用流式读取技术处理大型文本文件：

1. **按需加载**：只加载当前查看的部分内容
2. **多编码支持**：自动检测并支持多种文件编码
3. **行号显示**：显示准确的行号，便于定位

## 自定义修改指南

### 修改Excel读取的内存阈值

在`AppConfig.cs`中修改内存阈值：

```csharp
// 修改内存阈值（默认为500MB）
public static long MemoryThreshold = 500 * 1024 * 1024; // 500MB
```


### 修改每页显示的行数

用户可以在界面上直接设置每页显示的行数，设置将自动保存并在下次启动时自动加载。

如果需要修改默认值，可以在`AppConfig.cs`中修改：

```csharp
// 修改默认页大小
public static int DefaultPageSize = 1000; // 默认每页显示1000行
```

用户设置保存在程序根目录的`data/config.txt`文件中。

### 添加新的日志级别

在`LogManager.cs`中添加新的日志级别：

```csharp
public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Fatal
    // 添加新的日志级别
}
```

### 日志文件位置

日志文件保存在程序根目录的`logs`文件夹中，文件名格式为`log_yyyyMMdd.txt`。

例如：
```
C:\Program Files\ReadBigTextFile\logs\log_20240520.txt
```

日志文件默认已启用，可以在`AppConfig.cs`中修改：

```csharp
// 禁用日志文件
public static bool EnableFileLogging = false;
```

## 常见问题

### Excel相关问题

**问题**：打开Excel文件时出现"未在本地计算机上注册Microsoft.ACE.OLEDB.12.0提供程序"错误
**解决方案**：安装Microsoft Access Database Engine 2016 Redistributable：[下载地址](https://www.microsoft.com/en-us/download/details.aspx?id=54920)

**问题**：打开大型Excel文件时出现"内存不足"错误
**解决方案**：减小每页显示的行数，关闭其他内存密集型应用程序

**问题**：Excel文件中的日期格式显示不正确
**解决方案**：确保系统区域设置与Excel文件的区域设置一致

### 数据库相关问题

**问题**：导出大量数据时程序卡顿
**解决方案**：增加命令超时时间，减少每批处理的记录数

**问题**：导出的Excel文件中有些数据显示为科学计数法
**解决方案**：在SQL查询中使用CAST或CONVERT将数字转换为字符串

### 系统相关问题

**问题**：应用程序启动缓慢
**解决方案**：检查临时文件夹是否需要清理，确保系统资源充足

**问题**：程序崩溃或无响应
**解决方案**：检查日志文件，确认是否有异常记录；尝试重新安装程序

**问题**：无法保存设置
**解决方案**：确保程序有足够的权限访问配置文件夹

## 性能优化建议

1. **使用SSD存储**：SSD比HDD有更快的读写速度，可以显著提高大文件处理性能
2. **增加系统内存**：更多的系统内存可以提高程序处理大文件的能力
3. **关闭不必要的程序**：处理大文件时，关闭其他内存密集型应用程序
4. **定期清理临时文件**：程序会自动清理临时文件，但也可以手动清理系统临时文件夹
5. **使用64位系统**：64位系统可以使用更多内存，提高大文件处理能力

## 开发者信息

使用C#语言开发，基于.NET Framework 4.7.2平台。主要使用了以下技术：

- **WinForms**：用户界面开发
- **ADO.NET**：数据库连接和操作
- **OleDb**：Excel文件读写
- **Task Parallel Library**：异步操作和并行处理
- **Memory Management**：内存优化和资源管理

## 许可证

本项目采用 `MIT` 许可证 ___[LICENSE](LICENSE)___

## 联系方式

如有问题或建议，请通过 Issues 页面提交。

## 更新日志

### 版本 1.0.0 (2024-05-20)
- 初始版本发布
- 支持Excel文件查看
- 支持文本文件阅读
- 支持数据库导出到Excel

### 版本 1.1.0 (2024-05-25)
- 将Excel处理引擎从NPOI更换为OleDb，提高性能和内存效率
- 统一Excel查看器界面，不再区分高低性能版本
- 优化内存管理，提高大文件处理能力
- 修复已知问题和bug