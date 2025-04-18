using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using ReadingLargeTextFiles.Core;

namespace ReadingLargeTextFiles.UI
{
    public partial class DbToExcelForm : Form
    {
        private BackgroundWorker exportWorker;
        private CancellationTokenSource cancellationTokenSource;
        private const string REGISTRY_KEY = @"Software\ReadingLargeTextFiles\DbExport";
        private const int MAX_ROWS_PER_SHEET = 1000000;
        private const int BATCH_SIZE = 100000;
        private int totalExportedRows = 0;
        private int sheetCount = 0;
        private bool isPaused = false;
        private List<string> columnNames = new List<string>();
        private string exportBasePath = string.Empty;
        private string connectionString = string.Empty;
        private SqlConnection connection = null;
        private string exportFileName = string.Empty;

        public DbToExcelForm()
        {
            InitializeComponent();

            InitializeBackgroundWorker();

            UpdateUIState(false);

            LoadConnectionSettings();

            this.FormClosing += DbToExcelForm_FormClosing;
        }

        private void InitializeBackgroundWorker()
        {
            exportWorker = new BackgroundWorker();
            exportWorker.WorkerReportsProgress = true;
            exportWorker.WorkerSupportsCancellation = true;
            exportWorker.DoWork += ExportWorker_DoWork;
            exportWorker.ProgressChanged += ExportWorker_ProgressChanged;
            exportWorker.RunWorkerCompleted += ExportWorker_RunWorkerCompleted;
        }

        private void DbToExcelForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.TopLevel == false)
            {
                e.Cancel = true;
                this.Hide();
                LogManager.Debug("数据库导出器已隐藏");
                return;
            }

            SaveConnectionSettings();

            if (exportWorker.IsBusy)
            {
                DialogResult result = MessageBox.Show("导出操作正在进行中，确定要关闭窗口吗？",
                    "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    CancelExport();
                }
                else
                {
                    e.Cancel = true;
                }
            }

            LogManager.Info("数据库导出器已关闭");
        }

        private void LoadConnectionSettings()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY))
                {
                    if (key != null)
                    {
                        txtServer.Text = key.GetValue("Server", "").ToString();
                        chkWindowsAuth.Checked = Convert.ToBoolean(key.GetValue("WindowsAuth", true));
                        txtUsername.Text = key.GetValue("Username", "").ToString();
                        txtSavePath.Text = key.GetValue("SavePath", "").ToString();
                        nudTimeout.Value = Convert.ToDecimal(key.GetValue("Timeout", 30));
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("加载连接设置失败", ex);
            }

            UpdateAuthenticationUI();
        }

        private void SaveConnectionSettings()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(REGISTRY_KEY))
                {
                    if (key != null)
                    {
                        key.SetValue("Server", txtServer.Text);
                        if (cmbDatabase.SelectedItem != null)
                        {
                            key.SetValue("Database", cmbDatabase.SelectedItem.ToString());
                        }
                        key.SetValue("WindowsAuth", chkWindowsAuth.Checked);
                        key.SetValue("Username", txtUsername.Text);
                        key.SetValue("SavePath", txtSavePath.Text);
                        key.SetValue("Timeout", (int)nudTimeout.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("保存连接设置失败", ex);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            cmbDatabase.Items.Clear();
            lstTables.Items.Clear();

            connectionString = BuildConnectionString();

            try
            {
                this.Cursor = Cursors.WaitCursor;
                lblStatus.Text = "正在连接数据库...";
                LogManager.Info($"正在连接到服务器: {txtServer.Text}");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    DataTable databases = conn.GetSchema("Databases");

                    foreach (DataRow row in databases.Rows)
                    {
                        string dbName = row["database_name"].ToString();
                        if (dbName != "master" && dbName != "tempdb" && dbName != "model" && dbName != "msdb")
                        {
                            cmbDatabase.Items.Add(dbName);
                        }
                    }

                    string savedDatabase = string.Empty;
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY))
                    {
                        if (key != null)
                        {
                            savedDatabase = key.GetValue("Database", "").ToString();
                        }
                    }

                    if (!string.IsNullOrEmpty(savedDatabase))
                    {
                        int index = cmbDatabase.Items.IndexOf(savedDatabase);
                        if (index >= 0)
                        {
                            cmbDatabase.SelectedIndex = index;
                        }
                        else if (cmbDatabase.Items.Count > 0)
                        {
                            cmbDatabase.SelectedIndex = 0;
                        }
                    }
                    else if (cmbDatabase.Items.Count > 0)
                    {
                        cmbDatabase.SelectedIndex = 0;
                    }

                    lblStatus.Text = "已连接到服务器";
                    grpDatabaseOptions.Enabled = true;
                    LogManager.Info("已成功连接到服务器");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "连接失败";
                grpDatabaseOptions.Enabled = false;
                LogManager.Error($"连接数据库失败: {ex.Message}", ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private string BuildConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = txtServer.Text.Trim();

            if (cmbDatabase.SelectedItem != null)
            {
                builder.InitialCatalog = cmbDatabase.SelectedItem.ToString();
            }

            if (chkWindowsAuth.Checked)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = txtUsername.Text.Trim();
                builder.Password = txtPassword.Text;
            }

            builder.ConnectTimeout = (int)nudTimeout.Value;

            builder.ApplicationName = "ReadingLargeTextFiles.DbExporter";

            return builder.ConnectionString;
        }

        private void cmbDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDatabase.SelectedItem == null) return;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                lblStatus.Text = "正在读取表和视图...";
                string dbName = cmbDatabase.SelectedItem.ToString();
                LogManager.Info($"正在加载数据库 {dbName} 的表和视图");

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                builder.InitialCatalog = dbName;
                connectionString = builder.ConnectionString;

                lstTables.Items.Clear();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    DataTable tables = conn.GetSchema("Tables");

                    foreach (DataRow row in tables.Rows)
                    {
                        string schema = row["TABLE_SCHEMA"].ToString();
                        string tableName = row["TABLE_NAME"].ToString();

                        lstTables.Items.Add($"[{schema}].[{tableName}]");
                    }

                    DataTable views = conn.GetSchema("Views");

                    foreach (DataRow row in views.Rows)
                    {
                        string schema = row["TABLE_SCHEMA"].ToString();
                        string viewName = row["TABLE_NAME"].ToString();

                        lstTables.Items.Add($"[{schema}].[{viewName}] (视图)");
                    }

                    lblStatus.Text = $"已加载 {lstTables.Items.Count} 个表和视图";
                    grpExportOptions.Enabled = true;
                    LogManager.Info($"已加载 {lstTables.Items.Count} 个表和视图");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取表结构失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "读取表和视图失败";
                grpExportOptions.Enabled = false;
                LogManager.Error($"获取表结构失败: {ex.Message}", ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "选择导出文件保存位置";

                if (!string.IsNullOrEmpty(txtSavePath.Text) && Directory.Exists(txtSavePath.Text))
                {
                    folderDialog.SelectedPath = txtSavePath.Text;
                }

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtSavePath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void chkWindowsAuth_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAuthenticationUI();
        }

        private void UpdateAuthenticationUI()
        {
            bool sqlAuth = !chkWindowsAuth.Checked;
            lblUsername.Enabled = sqlAuth;
            lblPassword.Enabled = sqlAuth;
            txtUsername.Enabled = sqlAuth;
            txtPassword.Enabled = sqlAuth;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtServer.Text))
            {
                MessageBox.Show("请输入服务器地址", "验证", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtServer.Focus();
                return;
            }

            if (!chkWindowsAuth.Checked && string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("请输入用户名", "验证", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (lstTables.SelectedItem == null && string.IsNullOrEmpty(txtCustomSql.Text))
            {
                MessageBox.Show("请选择要导出的表或输入自定义SQL", "验证", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtSavePath.Text))
            {
                MessageBox.Show("请选择保存路径", "验证", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnBrowse.Focus();
                return;
            }

            if (!Directory.Exists(txtSavePath.Text))
            {
                try
                {
                    Directory.CreateDirectory(txtSavePath.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"创建保存目录失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogManager.Error($"创建保存目录失败: {ex.Message}", ex);
                    return;
                }
            }

            exportBasePath = txtSavePath.Text;
            UpdateUIState(true);
            totalExportedRows = 0;
            sheetCount = 0;

            string dbName = cmbDatabase.SelectedItem?.ToString() ?? "database";
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            exportFileName = $"导出_{dbName}_{timestamp}.xlsx";

            cancellationTokenSource = new CancellationTokenSource();
            exportWorker.RunWorkerAsync();

            LogManager.Info($"开始导出数据到 {exportFileName}");
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (isPaused)
            {
                isPaused = false;
                btnPause.Text = "暂停";
                LogManager.Info("恢复导出");
            }
            else
            {
                isPaused = true;
                btnPause.Text = "继续";
                LogManager.Info("暂停导出");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelExport();
        }

        private void radSelectedTable_CheckedChanged(object sender, EventArgs e)
        {
            txtCustomSql.Enabled = !radSelectedTable.Checked;
        }

        private void radCustomSql_CheckedChanged(object sender, EventArgs e)
        {
            txtCustomSql.Enabled = radCustomSql.Checked;
        }

        private void CancelExport()
        {
            if (exportWorker.IsBusy)
            {
                DialogResult result = MessageBox.Show("确定要取消导出操作吗？", "确认",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    lblStatus.Text = "正在取消...";
                    cancellationTokenSource.Cancel();
                    exportWorker.CancelAsync();
                    LogManager.Info("用户取消了导出操作");
                }
            }
        }

        private void UpdateUIState(bool isExporting)
        {
            grpConnection.Enabled = !isExporting;
            grpDatabaseOptions.Enabled = !isExporting;
            grpExportOptions.Enabled = !isExporting;
            btnExport.Enabled = !isExporting;
            btnPause.Enabled = isExporting;
            btnCancel.Enabled = isExporting;
            progressExport.Visible = isExporting;
        }

        private async void ExportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            try
            {
                string commandText;
                if (radSelectedTable.Checked && lstTables.SelectedItem != null)
                {
                    string tableName = lstTables.SelectedItem.ToString();
                    tableName = tableName.Split(' ')[0];
                    commandText = $"SELECT * FROM {tableName}";
                    worker.ReportProgress(0, $"将导出表: {tableName}");
                }
                else
                {
                    commandText = txtCustomSql.Text;
                    worker.ReportProgress(0, $"将执行自定义SQL查询");
                }

                if (string.IsNullOrWhiteSpace(commandText))
                {
                    worker.ReportProgress(0, $"错误: SQL查询为空");
                    throw new Exception("SQL查询为空，请选择表或输入自定义SQL");
                }

                worker.ReportProgress(0, $"正在连接数据库...");
                string excelFilePath = Path.Combine(exportBasePath, exportFileName);

                using (connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    worker.ReportProgress(0, $"已连接到数据库，准备导出数据");

                    Data.OleDbExcelExporter exporter = null;
                    try
                    {
                        exporter = new Data.OleDbExcelExporter(excelFilePath);
                        worker.ReportProgress(0, $"已创建 Excel 导出器");

                        using (SqlCommand dataCommand = new SqlCommand(commandText, connection))
                        {
                            dataCommand.CommandTimeout = (int)nudTimeout.Value;

                            worker.ReportProgress(0, $"正在获取数据...");
                            DataTable dataTable = new DataTable();
                            using (SqlDataAdapter adapter = new SqlDataAdapter(dataCommand))
                            {
                                if (worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                    return;
                                }

                                adapter.Fill(dataTable);
                                worker.ReportProgress(0, $"已获取 {dataTable.Rows.Count} 行数据");
                            }

                            var progress = new Progress<int>(percent => {
                                while (isPaused && !worker.CancellationPending)
                                {
                                    Thread.Sleep(100);
                                }

                                worker.ReportProgress(percent, $"导出进度: {percent}%, 已处理 {totalExportedRows} 行");
                            });

                            worker.ReportProgress(0, $"正在导出数据到 Excel 文件...");
                            totalExportedRows = await exporter.ExportDataTableAsync(
                                dataTable,
                                "Sheet",
                                progress,
                                cancellationTokenSource.Token);

                            sheetCount = (int)Math.Ceiling((double)totalExportedRows / MAX_ROWS_PER_SHEET);

                            worker.ReportProgress(100, $"数据导出完成，共 {totalExportedRows} 行");
                        }
                    }
                    finally
                    {
                        if (exporter != null)
                        {
                            exporter = null;
                        }
                    }
                }

                e.Result = new ExportResult
                {
                    Success = true,
                    FilePath = excelFilePath,
                    RowCount = totalExportedRows,
                    SheetCount = sheetCount
                };
            }
            catch (Exception ex)
            {
                worker.ReportProgress(0, $"导出失败: {ex.Message}");
                LogManager.Error($"导出失败: {ex.Message}", ex);
                e.Result = ex;
            }
        }

        private void ExportWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is string message)
            {
                lblStatus.Text = message;
                LogManager.Debug(message);
            }

            progressExport.Value = e.ProgressPercentage;
        }

        private void ExportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateUIState(false);

            if (e.Cancelled)
            {
                lblStatus.Text = "导出已取消";
                LogManager.Info("导出已取消");
            }
            else if (e.Error != null)
            {
                lblStatus.Text = $"导出失败: {e.Error.Message}";
                MessageBox.Show($"导出失败: {e.Error.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.Error($"导出失败: {e.Error.Message}", e.Error);
            }
            else if (e.Result is Exception ex)
            {
                lblStatus.Text = $"导出失败: {ex.Message}";
                MessageBox.Show($"导出失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.Error($"导出失败: {ex.Message}", ex);
            }
            else if (e.Result is ExportResult result)
            {
                lblStatus.Text = $"导出完成，共 {result.RowCount} 行数据，{result.SheetCount} 个工作表";

                string message = $"导出完成！\n\n文件: {result.FilePath}\n行数: {result.RowCount}\n工作表数: {result.SheetCount}\n\n是否打开文件所在目录？";
                DialogResult dialogResult = MessageBox.Show(message, "导出成功", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{result.FilePath}\"");
                    }
                    catch (Exception openEx)
                    {
                        LogManager.Error($"打开文件所在目录失败: {openEx.Message}", openEx);
                    }
                }

                LogManager.Info($"导出完成，文件: {result.FilePath}，行数: {result.RowCount}，工作表数: {result.SheetCount}");
            }
        }

        private class ExportResult
        {
            public bool Success { get; set; }
            public string FilePath { get; set; }
            public int RowCount { get; set; }
            public int SheetCount { get; set; }
        }
    }
}
