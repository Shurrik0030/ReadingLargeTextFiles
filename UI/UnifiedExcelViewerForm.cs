using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReadingLargeTextFiles.Core;
using ReadingLargeTextFiles.Data;

namespace ReadingLargeTextFiles.UI
{
    public partial class UnifiedExcelViewerForm : Form
    {
        #region 字段

        private IExcelReader excelReader;
        private string filePath;
        private string currentSheetName;
        private DataTable dataTable;
        private int pageSize = 100;
        private int currentPage = 0;
        private int totalPages = 0;
        private int totalRows = 0;
        private bool isLoading = false;
        private CancellationTokenSource cts = new CancellationTokenSource();

        #endregion

        public UnifiedExcelViewerForm()
        {
            InitializeComponent();

            InitializeControls();

            this.Text = "Excel查看器";

            this.FormClosing += UnifiedExcelViewerForm_FormClosing;

            LogManager.Info("统一Excel查看器已初始化");
        }

        #region 初始化方法

        private void InitializeControls()
        {
            try
            {
                numPageSize.Items.Clear();
                numPageSize.Items.AddRange(new object[] { "100", "500", "1000", "5000", "10000" });
                numPageSize.SelectedIndex = 0;

                cmbSheets.Enabled = false;
                numPageSize.Enabled = false;
                btnFirstPage.Enabled = false;
                btnPreviousPage.Enabled = false;
                btnNextPage.Enabled = false;
                btnLastPage.Enabled = false;

                progressBar.Visible = false;

                dataGridView.ReadOnly = true;
                dataGridView.AllowUserToAddRows = false;
                dataGridView.AllowUserToDeleteRows = false;
                dataGridView.AllowUserToResizeRows = false;
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView.MultiSelect = true;
                dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;

                toolStripStatusLabel.Text = "就绪";
            }
            catch (Exception ex)
            {
                LogManager.Error($"初始化控件时出错: {ex.Message}", ex);
                MessageBox.Show($"初始化控件时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 事件处理程序

        private void UnifiedExcelViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.TopLevel == false)
            {
                e.Cancel = true;
                this.Hide();
                LogManager.Debug("Excel查看器已隐藏");
                return;
            }

            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
            }

            DisposeResources();
        }

        private async void btnOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel文件|*.xlsx;*.xls|所有文件|*.*";
                openFileDialog.Title = "选择Excel文件";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    await OpenExcelFileAsync(openFileDialog.FileName);
                }
            }
        }

        private async void cmbSheets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSheets.SelectedIndex >= 0 && !isLoading)
            {
                currentSheetName = cmbSheets.SelectedItem.ToString();
                currentPage = 0;
                await LoadCurrentPageAsync();
            }
        }

        private async void numPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(numPageSize.Text, out int newPageSize) && newPageSize > 0)
            {
                if (pageSize != newPageSize)
                {
                    pageSize = newPageSize;
                    LogManager.Info($"用户设置每页行数: {pageSize}");

                    if (excelReader != null && !string.IsNullOrEmpty(currentSheetName))
                    {
                        currentPage = 0;
                        await LoadCurrentPageAsync();
                    }
                }
            }
        }

        private async void btnFirstPage_Click(object sender, EventArgs e)
        {
            if (!isLoading && currentPage > 0)
            {
                currentPage = 0;
                await LoadCurrentPageAsync();
            }
        }

        private async void btnPreviousPage_Click(object sender, EventArgs e)
        {
            if (!isLoading && currentPage > 0)
            {
                currentPage--;
                await LoadCurrentPageAsync();
            }
        }

        private async void btnNextPage_Click(object sender, EventArgs e)
        {
            if (!isLoading && currentPage < totalPages - 1)
            {
                currentPage++;
                await LoadCurrentPageAsync();
            }
        }

        private async void btnLastPage_Click(object sender, EventArgs e)
        {
            if (!isLoading && currentPage < totalPages - 1)
            {
                currentPage = totalPages - 1;
                await LoadCurrentPageAsync();
            }
        }

        #endregion

        #region 文件操作方法

        private async Task OpenExcelFileAsync(string fileName)
        {
            try
            {
                if (cts != null && !cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }
                cts = new CancellationTokenSource();

                SetLoadingState(true);
                UpdateStatusBar($"正在打开文件: {Path.GetFileName(fileName)}...");

                filePath = fileName;

                DisposeResources();

                await InitializeReaderAsync();

                List<string> sheets = await excelReader.InitializeAsync();

                cmbSheets.Items.Clear();
                foreach (string sheet in sheets)
                {
                    cmbSheets.Items.Add(sheet);
                }

                if (cmbSheets.Items.Count > 0)
                {
                    cmbSheets.SelectedIndex = 0;
                    currentSheetName = cmbSheets.Items[0].ToString();
                }
                else
                {
                    MessageBox.Show("未找到工作表", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateStatusBar("未找到工作表");
                    SetLoadingState(false);
                    return;
                }

                this.Text = $"Excel查看器 (OleDb) - {Path.GetFileName(filePath)}";

                cmbSheets.Enabled = true;
                numPageSize.Enabled = true;

                UpdateStatusBar($"已加载文件: {Path.GetFileName(filePath)}，正在加载第一页数据...");

                await LoadCurrentPageAsync();

                UpdateStatusBar($"已加载文件: {Path.GetFileName(filePath)}");

                LogManager.Info($"Excel文件已成功加载: {filePath}");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Microsoft.ACE.OLEDB.12.0") || ex.Message.Contains("Provider"))
                {
                    LogManager.Error($"打开Excel文件时出错: {ex.Message}", ex);

                    OleDbHelper.ShowInstallDialog();

                    UpdateStatusBar($"打开文件失败: 缺少Excel提供程序");
                }
                else
                {
                    MessageBox.Show($"打开Excel文件时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatusBar($"打开文件失败: {ex.Message}");
                    LogManager.Error($"打开Excel文件时出错: {ex.Message}", ex);
                }
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private async Task InitializeReaderAsync()
        {
            excelReader = new OleDbExcelReader(filePath);
            LogManager.Info($"正在初始化OleDb Excel读取器: {filePath}");
        }

        private async Task LoadCurrentPageAsync()
        {
            if (excelReader == null || string.IsNullOrEmpty(currentSheetName))
            {
                UpdateStatusBar("请先打开Excel文件");
                return;
            }

            try
            {
                SetLoadingState(true);
                UpdateStatusBar($"正在加载工作表 '{currentSheetName}' 第 {currentPage + 1} 页...");

                LogManager.Debug($"正在加载工作表 '{currentSheetName}' 第 {currentPage + 1} 页");

                await Task.Delay(1);
                Application.DoEvents();

                int rows = 0;
                int pages = 0;
                List<string> headers = null;

                await Task.Run(() => {
                    rows = excelReader.GetRowCount(currentSheetName);
                    pages = (int)Math.Ceiling((double)rows / pageSize);

                    headers = excelReader.GetColumnHeaders(currentSheetName);
                });

                totalRows = rows;
                totalPages = Math.Max(1, pages);

                int startRow = currentPage * pageSize;
                int rowsToRead = Math.Min(pageSize, totalRows - startRow);

                LogManager.Debug($"读取工作表: {currentSheetName}, 起始行: {startRow}, 行数: {rowsToRead}");
                LogManager.Debug($"列数: {headers.Count}");

                await MemoryManager.CheckMemoryStatusAsync();

                await Task.Delay(1);
                Application.DoEvents();

                UpdateStatusBar($"正在读取数据: 从第 {startRow + 1} 行开始，读取 {rowsToRead} 行...");

                List<string[]> data = await excelReader.ReadSheetChunkAsync(
                    currentSheetName,
                    startRow,
                    rowsToRead,
                    cts.Token).ConfigureAwait(true);

                LogManager.Debug($"读取到的数据行数: {data.Count}");

                Application.DoEvents();

                if (data.Count == 0)
                {
                    if (totalRows > 0)
                    {
                        currentPage = 0;
                        UpdateStatusBar($"当前页超出范围，已重置到第一页");
                        LogManager.Warning($"当前页超出范围，已重置到第一页");
                        await LoadCurrentPageAsync();
                        return;
                    }
                    else
                    {
                        MessageBox.Show($"工作表为空。\n工作表: {currentSheetName}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateStatusBar($"工作表 '{currentSheetName}' 没有数据");
                        LogManager.Warning($"工作表 '{currentSheetName}' 没有数据");

                        CreateDataTable(headers, new List<string[]>());
                        UpdatePagingInfo();
                        SetLoadingState(false);
                        return;
                    }
                }

                CreateDataTable(headers, data);

                UpdatePagingInfo();

                UpdateStatusBar($"工作表 '{currentSheetName}' 第 {currentPage + 1}/{totalPages} 页，共 {totalRows} 行");

                LogManager.Info($"已加载工作表 '{currentSheetName}' 第 {currentPage + 1}/{totalPages} 页，共 {totalRows} 行");

                data.Clear();

                await Task.Run(() => {
                    MemoryManager.CheckMemoryStatus();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatusBar($"加载数据失败: {ex.Message}");
                LogManager.Error($"加载数据时出错: {ex.Message}", ex);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private void CreateDataTable(List<string> headers, List<string[]> data)
        {
            try
            {
                LogManager.Debug("创建数据表");

                dataTable = new DataTable();

                List<string> uniqueHeaders = ExcelHelper.ProcessColumnHeaders(headers);

                for (int i = 0; i < uniqueHeaders.Count; i++)
                {
                    dataTable.Columns.Add(uniqueHeaders[i], typeof(string));
                }

                foreach (string[] row in data)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int i = 0; i < Math.Min(row.Length, uniqueHeaders.Count); i++)
                    {
                        dataRow[i] = row[i];
                    }
                    dataTable.Rows.Add(dataRow);
                }

                dataGridView.DataSource = null;
                dataGridView.DataSource = dataTable;

                dataGridView.Refresh();

                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }

                UpdateStatusBar($"已加载 {data.Count} 行数据");

                LogManager.Debug($"数据表创建完成，共 {data.Count} 行数据");
            }
            catch (Exception ex)
            {
                LogManager.Error($"创建数据表时出错: {ex.Message}", ex);
                MessageBox.Show($"创建数据表时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 辅助方法

        private void UpdatePagingInfo()
        {
            try
            {
                lblPageInfo.Text = $"第 {currentPage + 1} / {totalPages} 页";

                btnFirstPage.Enabled = currentPage > 0;
                btnPreviousPage.Enabled = currentPage > 0;
                btnNextPage.Enabled = currentPage < totalPages - 1;
                btnLastPage.Enabled = currentPage < totalPages - 1;
            }
            catch (Exception ex)
            {
                LogManager.Error($"更新分页信息时出错: {ex.Message}", ex);
            }
        }

        private void UpdateStatusBar(string message)
        {
            try
            {
                toolStripStatusLabel.Text = message;
                statusStrip.Refresh();
            }
            catch (Exception ex)
            {
                LogManager.Error($"更新状态栏时出错: {ex.Message}", ex);
            }
        }

        private void SetLoadingState(bool loading)
        {
            try
            {
                isLoading = loading;
                progressBar.Visible = loading;
                btnOpenFile.Enabled = !loading;
                cmbSheets.Enabled = !loading && excelReader != null;
                numPageSize.Enabled = !loading && excelReader != null;

                if (!loading)
                {
                    UpdatePagingInfo();
                }
                else
                {
                    btnFirstPage.Enabled = false;
                    btnPreviousPage.Enabled = false;
                    btnNextPage.Enabled = false;
                    btnLastPage.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"设置加载状态时出错: {ex.Message}", ex);
            }
        }

        private void DisposeResources()
        {
            try
            {
                if (excelReader != null)
                {
                    excelReader.Dispose();
                    excelReader = null;
                }

                if (dataTable != null)
                {
                    dataTable.Dispose();
                    dataTable = null;
                }

                dataGridView.DataSource = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                LogManager.Error($"释放资源时出错: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
