using System;
using System.Drawing;
using System.Windows.Forms;
using ReadingLargeTextFiles.Core;
using ReadingLargeTextFiles.UI;

namespace ReadingLargeTextFiles
{
    public partial class MainForm : Form
    {
        // 当前活动按钮
        private Button currentButton;

        // 当前活动窗体
        private Form activeForm;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "数据处理工具";
            this.MinimumSize = new Size(800, 500);

            // 初始化完成后，显示首页
            this.Load += MainForm_Load;

            // 添加窗体关闭事件
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 关闭时清理临时文件
            AppConfig.CleanupTempFiles();

            // 记录日志
            LogManager.Info("应用程序关闭");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 默认显示首页
            btnHome_Click(btnHome, EventArgs.Empty);

            // 检查OleDb提供程序
            CheckOleDbProviders();
        }

        /// <summary>
        /// 检查OleDb提供程序
        /// </summary>
        private void CheckOleDbProviders()
        {
            // 不再主动检查OleDb提供程序
            // 当用户尝试使用Excel功能时，如果出错会显示提示
            LogManager.Info("不主动检查OleDb提供程序，当需要时会显示安装提示");
        }

        // 激活按钮
        private void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {
                if (currentButton != (Button)btnSender)
                {
                    // 重置其他按钮
                    DisableButton();

                    // 设置当前按钮样式
                    currentButton = (Button)btnSender;
                    currentButton.BackColor = Color.FromArgb(0, 150, 255);
                    currentButton.ForeColor = Color.White;
                    currentButton.Font = new Font("微软雅黑", 12F, FontStyle.Bold);
                }
            }
        }

        // 重置按钮样式
        private void DisableButton()
        {
            foreach (Control previousBtn in panelMenu.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = Color.FromArgb(0, 122, 204);
                    previousBtn.ForeColor = Color.White;
                    previousBtn.Font = new Font("微软雅黑", 12F, FontStyle.Regular);
                }
            }
        }

        // 打开子窗体
        private void OpenChildForm(Form childForm, object btnSender)
        {
            if (activeForm != null)
            {
                activeForm.Close();
            }

            // 激活按钮
            ActivateButton(btnSender);

            // 设置子窗体
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            // 添加到内容面板
            this.panelContent.Controls.Add(childForm);
            this.panelContent.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();

            // 更新标题
            this.Text = $"数据处理工具 - {childForm.Text}";
        }

        // 首页按钮点击
        private void btnHome_Click(object sender, EventArgs e)
        {
            if (activeForm != null)
            {
                activeForm.Close();
                this.Text = "数据处理工具";
            }

            // 重置按钮样式
            DisableButton();
            currentButton = null;

            // 显示欢迎信息
            Label welcomeLabel = new Label();
            welcomeLabel.Text = "欢迎使用数据处理工具\r\n\r\n请从左侧菜单选择功能";
            welcomeLabel.Font = new Font("微软雅黑", 16F, FontStyle.Bold);
            welcomeLabel.ForeColor = Color.FromArgb(0, 122, 204);
            welcomeLabel.Dock = DockStyle.Fill;
            welcomeLabel.TextAlign = ContentAlignment.MiddleCenter;

            panelContent.Controls.Clear();
            panelContent.Controls.Add(welcomeLabel);
        }

        private void btnTextReader_Click(object sender, EventArgs e)
        {
            OpenChildForm(new UI.TextReaderForm(), sender);
        }

        private void btnDbExporter_Click(object sender, EventArgs e)
        {
            OpenChildForm(new UI.DbToExcelForm(), sender);
        }

        private void btnExcelViewer_Click(object sender, EventArgs e)
        {
            // 直接打开统一的Excel查看器
            OpenChildForm(new UI.UnifiedExcelViewerForm(), sender);
        }
    }
}
