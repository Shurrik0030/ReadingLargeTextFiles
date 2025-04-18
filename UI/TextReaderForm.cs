using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ReadingLargeTextFiles.Core;

namespace ReadingLargeTextFiles.UI
{
    public partial class TextReaderForm : Form
    {
        private string selectedFilePath = string.Empty;
        private long currentPosition = 0;
        private long fileSize = 0;
        private Encoding currentEncoding = Encoding.UTF8;
        private CancellationTokenSource readCancellationToken;
        private const int MAX_CHARS_PER_READ = 1024 * 1024;
        private FileType currentFileType = FileType.Text;
        private const string REGISTRY_KEY = @"Software\ReadingLargeTextFiles";
        private const string LAST_PATH_VALUE = "LastFilePath";
        private const string LAST_ENCODING_VALUE = "LastEncoding";
        private const string LAST_LINES_VALUE = "LastLines";
        private int currentRowIndex = 0;

        private bool isProcessing = false;
        private CancellationTokenSource cancellationTokenSource;
        private long previousChunkEndPosition = 0;

        private enum FileType
        {
            Text
        }

        public TextReaderForm()
        {
            InitializeComponent();

            btnRestart.Enabled = false;
            rtbContent.ReadOnly = true;
            txtFilePath.ReadOnly = true;

            LoadSettings();

            this.FormClosing += TextReaderForm_FormClosing;
        }

        private void LoadSettings()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY))
                {
                    if (key != null)
                    {
                        object lastPathValue = key.GetValue(LAST_PATH_VALUE);
                        if (lastPathValue != null && !string.IsNullOrEmpty(lastPathValue.ToString()) && File.Exists(lastPathValue.ToString()))
                        {
                            txtFilePath.Text = lastPathValue.ToString();
                        }

                        object encodingIndexValue = key.GetValue(LAST_ENCODING_VALUE);
                        if (encodingIndexValue != null && int.TryParse(encodingIndexValue.ToString(), out int encodingIndex))
                        {
                            if (encodingIndex >= 0 && encodingIndex < cmbEncoding.Items.Count)
                            {
                                cmbEncoding.SelectedIndex = encodingIndex;
                            }
                        }

                        object linesPerChunkValue = key.GetValue(LAST_LINES_VALUE);
                        if (linesPerChunkValue != null && decimal.TryParse(linesPerChunkValue.ToString(), out decimal linesValue))
                        {
                            nudLinesPerChunk.Value = Math.Min(Math.Max(linesValue, nudLinesPerChunk.Minimum), nudLinesPerChunk.Maximum);
                        }
                    }
                    else
                    {
                        cmbEncoding.SelectedIndex = 0;
                        nudLinesPerChunk.Value = 100;
                    }
                }
            }
            catch
            {
                cmbEncoding.SelectedIndex = 0;
                nudLinesPerChunk.Value = 100;
            }
        }

        private void SaveSettings()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(REGISTRY_KEY))
                {
                    if (key != null)
                    {
                        if (!string.IsNullOrEmpty(selectedFilePath))
                        {
                            key.SetValue(LAST_PATH_VALUE, selectedFilePath);
                        }

                        key.SetValue(LAST_ENCODING_VALUE, cmbEncoding.SelectedIndex);

                        key.SetValue(LAST_LINES_VALUE, (int)nudLinesPerChunk.Value);
                    }
                }
            }
            catch
            {
            }
        }

        private void TextReaderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.TopLevel == false)
            {
                e.Cancel = true;
                this.Hide();
                LogManager.Debug("文本阅读器已隐藏");
                return;
            }

            SaveSettings();
            LogManager.Info("文本阅读器已关闭");
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    if (!string.IsNullOrEmpty(selectedFilePath))
                    {
                        string lastDirectory = Path.GetDirectoryName(selectedFilePath);
                        if (Directory.Exists(lastDirectory))
                        {
                            openFileDialog.InitialDirectory = lastDirectory;
                        }
                    }

                    openFileDialog.Filter = "文本文件 (*.txt;*.sql)|*.txt;*.sql|所有文件 (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        currentPosition = 0;
                        previousChunkEndPosition = 0;
                        currentRowIndex = 0;
                        rtbContent.Clear();

                        selectedFilePath = openFileDialog.FileName;
                        txtFilePath.Text = selectedFilePath;

                        currentFileType = FileType.Text;

                        FileInfo fileInfo = new FileInfo(selectedFilePath);
                        fileSize = fileInfo.Length;

                        cmbEncoding.Enabled = true;

                        SaveSettings();

                        btnRestart.Enabled = true;
                        lblStatus.Text = "文件已加载，准备读取";

                        UpdateFileIcon();

                        LogManager.Info($"已选择文件: {selectedFilePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"选择文件时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.Error($"选择文件时发生错误: {ex.Message}", ex);
            }
        }

        private async void btnContinue_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedFilePath))
                {
                    MessageBox.Show("请先选择一个文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (isProcessing)
                {
                    MessageBox.Show("正在处理，请稍候...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string fileExt = Path.GetExtension(selectedFilePath).ToLower();

                if (readCancellationToken != null)
                {
                    readCancellationToken.Cancel();
                    readCancellationToken.Dispose();
                }
                readCancellationToken = new CancellationTokenSource();

                await ReadNextTextChunkAsync(readCancellationToken.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.Error($"读取下一段时发生错误: {ex.Message}", ex);
            }
        }

        private async Task ReadNextTextChunkAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                lblStatus.Text = "没有选择文件";
                return;
            }

            if (!File.Exists(selectedFilePath))
            {
                lblStatus.Text = "所选文件不存在";
                return;
            }

            isProcessing = true;
            btnBrowse.Enabled = false;
            btnPrevious.Enabled = false;
            btnRestart.Enabled = false;

            lblStatus.Text = "正在读取下一段...";

            try
            {
                int linesToRead = (int)nudLinesPerChunk.Value;

                Encoding encoding = GetEncoding();

                if (currentPosition >= fileSize && fileSize > 0)
                {
                    lblStatus.Text = "已到达文件末尾";
                    return;
                }

                using (FileStream fs = new FileStream(selectedFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fs.Position = currentPosition;

                    StringBuilder contentBuilder = new StringBuilder();
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    int lineCount = 0;

                    string incompleteLine = string.Empty;

                    while (lineCount < linesToRead && (bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        string chunk = encoding.GetString(buffer, 0, bytesRead);

                        if (!string.IsNullOrEmpty(incompleteLine))
                        {
                            chunk = incompleteLine + chunk;
                            incompleteLine = string.Empty;
                        }

                        int lastNewLinePos = chunk.LastIndexOf('\n');
                        if (lastNewLinePos < 0)
                        {
                            incompleteLine = chunk;
                        }
                        else if (lastNewLinePos < chunk.Length - 1)
                        {
                            incompleteLine = chunk.Substring(lastNewLinePos + 1);
                            chunk = chunk.Substring(0, lastNewLinePos + 1);
                        }

                        string[] lines = chunk.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                        if (lines.Length > 0 && !string.IsNullOrEmpty(incompleteLine))
                        {
                            lines[0] = incompleteLine + lines[0];
                            incompleteLine = string.Empty;
                        }

                        int linesToAdd = Math.Min(linesToRead - lineCount, lines.Length - (lastNewLinePos < 0 ? 0 : 1));
                        for (int i = 0; i < linesToAdd; i++)
                        {
                            contentBuilder.AppendLine(lines[i]);
                            lineCount++;
                        }

                        if (lineCount >= linesToRead)
                            break;
                    }

                    previousChunkEndPosition = currentPosition;
                    currentPosition = fs.Position;

                    string content = contentBuilder.ToString();
                    rtbContent.Text = content;

                    double progressPercent = (double)currentPosition / fileSize * 100;
                    lblStatus.Text = $"读取了 {lineCount} 行, 进度 {progressPercent:0.0}%, 位置: {currentPosition}/{fileSize} 字节";

                    LogManager.Debug($"读取了 {lineCount} 行, 进度 {progressPercent:0.0}%, 位置: {currentPosition}/{fileSize} 字节");
                }
            }
            catch (IOException ioEx)
            {
                lblStatus.Text = $"文件读取错误: {ioEx.Message}";
                MessageBox.Show($"文件读取错误: {ioEx.Message}", "I/O错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.Error($"文件读取错误: {ioEx.Message}", ioEx);
            }
            catch (UnauthorizedAccessException uaEx)
            {
                lblStatus.Text = $"没有权限访问文件: {uaEx.Message}";
                MessageBox.Show($"没有权限访问文件: {uaEx.Message}", "权限错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.Error($"没有权限访问文件: {uaEx.Message}", uaEx);
            }
            catch (OutOfMemoryException memEx)
            {
                lblStatus.Text = "内存不足，请减少每次读取的行数";
                MessageBox.Show($"内存不足，请减少每次读取的行数: {memEx.Message}", "内存错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.Error($"内存不足，请减少每次读取的行数: {memEx.Message}", memEx);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"读取文件时发生错误: {ex.Message}";
                MessageBox.Show($"读取文件时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.Error($"读取文件时发生错误: {ex.Message}", ex);
            }
            finally
            {
                btnBrowse.Enabled = true;
                btnPrevious.Enabled = currentPosition > 0;
                btnRestart.Enabled = true;
                isProcessing = false;
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return $"{number:n2} {suffixes[counter]}";
        }

        private void cmbEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbEncoding.SelectedItem.ToString())
            {
                case "UTF-8":
                    currentEncoding = Encoding.UTF8;
                    break;
                case "GB2312":
                    currentEncoding = Encoding.GetEncoding("GB2312");
                    break;
                case "GBK":
                    currentEncoding = Encoding.GetEncoding("GBK");
                    break;
                case "Unicode":
                    currentEncoding = Encoding.Unicode;
                    break;
                case "ASCII":
                    currentEncoding = Encoding.ASCII;
                    break;
            }

            if (currentFileType == FileType.Text && !string.IsNullOrEmpty(selectedFilePath))
            {
                ResetState();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            SaveSettings();

            if (readCancellationToken != null)
            {
                readCancellationToken.Cancel();
                readCancellationToken.Dispose();
            }
        }

        private void UpdateFileIcon()
        {
            string fileExt = Path.GetExtension(selectedFilePath).ToLower();
            string asciiIcon;

            switch (fileExt)
            {
                case ".txt":
                case ".log":
                case ".csv":
                    asciiIcon = "[TXT]";
                    lblStatus.Text = $"{asciiIcon} 文本文件已加载";
                    break;
                case ".sql":
                    asciiIcon = "[SQL]";
                    lblStatus.Text = $"{asciiIcon} SQL文件已加载";
                    break;
                default:
                    asciiIcon = "[???]";
                    lblStatus.Text = $"{asciiIcon} 未知类型文件已加载";
                    break;
            }

            Text = $"大文本阅读器 - {asciiIcon} {Path.GetFileName(selectedFilePath)}";
        }

        private async Task ReadPreviousTextChunkAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                lblStatus.Text = "没有选择文件";
                return;
            }

            if (!File.Exists(selectedFilePath))
            {
                lblStatus.Text = "所选文件不存在";
                return;
            }

            if (previousChunkEndPosition <= 0)
            {
                lblStatus.Text = "已经是文件开头";
                return;
            }

            isProcessing = true;
            btnBrowse.Enabled = false;
            btnContinue.Enabled = false;
            btnRestart.Enabled = false;

            lblStatus.Text = "正在读取上一段...";

            try
            {
                int linesToRead = (int)nudLinesPerChunk.Value;
                Encoding encoding = GetEncoding();

                currentPosition = previousChunkEndPosition;
                previousChunkEndPosition = Math.Max(0, previousChunkEndPosition - (linesToRead * 100));

                using (FileStream fs = new FileStream(selectedFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fs.Position = previousChunkEndPosition;

                    StringBuilder contentBuilder = new StringBuilder();
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    int lineCount = 0;
                    long bytesReadTotal = 0;

                    string incompleteLine = string.Empty;

                    while (bytesReadTotal < (currentPosition - previousChunkEndPosition) && 
                           (bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        bytesReadTotal += bytesRead;
                        string chunk = encoding.GetString(buffer, 0, bytesRead);

                        if (!string.IsNullOrEmpty(incompleteLine))
                        {
                            chunk = incompleteLine + chunk;
                            incompleteLine = string.Empty;
                        }

                        int lastNewLinePos = chunk.LastIndexOf('\n');
                        if (lastNewLinePos < 0)
                        {
                            incompleteLine = chunk;
                        }
                        else if (lastNewLinePos < chunk.Length - 1)
                        {
                            incompleteLine = chunk.Substring(lastNewLinePos + 1);
                            chunk = chunk.Substring(0, lastNewLinePos + 1);
                        }

                        string[] lines = chunk.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                        if (lines.Length > 0 && !string.IsNullOrEmpty(incompleteLine))
                        {
                            lines[0] = incompleteLine + lines[0];
                            incompleteLine = string.Empty;
                        }

                        for (int i = 0; i < lines.Length - (lastNewLinePos < 0 ? 0 : 1); i++)
                        {
                            contentBuilder.AppendLine(lines[i]);
                            lineCount++;
                        }
                    }

                    string content = contentBuilder.ToString();
                    rtbContent.Text = content;

                    double progressPercent = (double)previousChunkEndPosition / fileSize * 100;
                    lblStatus.Text = $"读取了上一段 {lineCount} 行, 进度 {progressPercent:0.0}%, 位置: {previousChunkEndPosition}/{fileSize} 字节";

                    LogManager.Debug($"读取了上一段 {lineCount} 行, 进度 {progressPercent:0.0}%, 位置: {previousChunkEndPosition}/{fileSize} 字节");
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"读取上一段时发生错误: {ex.Message}";
                MessageBox.Show($"读取上一段时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.Error($"读取上一段时发生错误: {ex.Message}", ex);
            }
            finally
            {
                btnBrowse.Enabled = true;
                btnContinue.Enabled = true;
                btnRestart.Enabled = true;
                btnPrevious.Enabled = previousChunkEndPosition > 0;
                isProcessing = false;
            }
        }

        private void ResetState()
        {
            currentPosition = 0;
            previousChunkEndPosition = 0;
            rtbContent.Clear();
            lblStatus.Text = "已重置，准备读取";
            btnPrevious.Enabled = false;
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            ResetState();
        }

        private async void btnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedFilePath))
                {
                    MessageBox.Show("请先选择一个文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (isProcessing)
                {
                    MessageBox.Show("正在处理，请稍候...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (readCancellationToken != null)
                {
                    readCancellationToken.Cancel();
                    readCancellationToken.Dispose();
                }
                readCancellationToken = new CancellationTokenSource();

                await ReadPreviousTextChunkAsync(readCancellationToken.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.Error($"读取上一段时发生错误: {ex.Message}", ex);
            }
        }

        private Encoding GetEncoding()
        {
            if (cmbEncoding.SelectedItem == null)
            {
                return Encoding.UTF8;
            }

            switch (cmbEncoding.SelectedItem.ToString())
            {
                case "UTF-8":
                    return Encoding.UTF8;
                case "GB2312":
                    return Encoding.GetEncoding("GB2312");
                case "GBK":
                    return Encoding.GetEncoding("GBK");
                case "Unicode":
                    return Encoding.Unicode;
                case "ASCII":
                    return Encoding.ASCII;
                default:
                    return Encoding.UTF8;
            }
        }
    }
}
