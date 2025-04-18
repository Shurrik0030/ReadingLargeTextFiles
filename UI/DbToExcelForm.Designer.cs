namespace ReadingLargeTextFiles.UI
{
    partial class DbToExcelForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.nudTimeout = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.chkWindowsAuth = new System.Windows.Forms.CheckBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpDatabaseOptions = new System.Windows.Forms.GroupBox();
            this.cmbDatabase = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lstTables = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.grpExportOptions = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtSavePath = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtCustomSql = new System.Windows.Forms.TextBox();
            this.radCustomSql = new System.Windows.Forms.RadioButton();
            this.radSelectedTable = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressExport = new System.Windows.Forms.ToolStripProgressBar();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeout)).BeginInit();
            this.grpDatabaseOptions.SuspendLayout();
            this.grpExportOptions.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConnection
            // 
            this.grpConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConnection.Controls.Add(this.label5);
            this.grpConnection.Controls.Add(this.nudTimeout);
            this.grpConnection.Controls.Add(this.label4);
            this.grpConnection.Controls.Add(this.txtPassword);
            this.grpConnection.Controls.Add(this.lblPassword);
            this.grpConnection.Controls.Add(this.txtUsername);
            this.grpConnection.Controls.Add(this.lblUsername);
            this.grpConnection.Controls.Add(this.chkWindowsAuth);
            this.grpConnection.Controls.Add(this.btnConnect);
            this.grpConnection.Controls.Add(this.txtServer);
            this.grpConnection.Controls.Add(this.label1);
            this.grpConnection.Location = new System.Drawing.Point(12, 12);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(776, 141);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "数据库连接";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(482, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "秒";
            // 
            // nudTimeout
            // 
            this.nudTimeout.Location = new System.Drawing.Point(426, 29);
            this.nudTimeout.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudTimeout.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudTimeout.Name = "nudTimeout";
            this.nudTimeout.Size = new System.Drawing.Size(50, 21);
            this.nudTimeout.TabIndex = 11;
            this.nudTimeout.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(361, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "连接超时:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(371, 101);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(145, 21);
            this.txtPassword.TabIndex = 9;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(306, 104);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(35, 12);
            this.lblPassword.TabIndex = 8;
            this.lblPassword.Text = "密码:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(90, 101);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(145, 21);
            this.txtUsername.TabIndex = 7;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(25, 104);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(47, 12);
            this.lblUsername.TabIndex = 6;
            this.lblUsername.Text = "用户名:";
            // 
            // chkWindowsAuth
            // 
            this.chkWindowsAuth.AutoSize = true;
            this.chkWindowsAuth.Checked = true;
            this.chkWindowsAuth.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWindowsAuth.Location = new System.Drawing.Point(90, 66);
            this.chkWindowsAuth.Name = "chkWindowsAuth";
            this.chkWindowsAuth.Size = new System.Drawing.Size(114, 16);
            this.chkWindowsAuth.TabIndex = 5;
            this.chkWindowsAuth.Text = "Windows身份验证";
            this.chkWindowsAuth.UseVisualStyleBackColor = true;
            this.chkWindowsAuth.CheckedChanged += new System.EventHandler(this.chkWindowsAuth_CheckedChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(610, 101);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(90, 28);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(193, 21);
            this.txtServer.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器名称:";
            // 
            // grpDatabaseOptions
            // 
            this.grpDatabaseOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDatabaseOptions.Controls.Add(this.cmbDatabase);
            this.grpDatabaseOptions.Controls.Add(this.label6);
            this.grpDatabaseOptions.Controls.Add(this.lstTables);
            this.grpDatabaseOptions.Controls.Add(this.label7);
            this.grpDatabaseOptions.Enabled = false;
            this.grpDatabaseOptions.Location = new System.Drawing.Point(12, 159);
            this.grpDatabaseOptions.Name = "grpDatabaseOptions";
            this.grpDatabaseOptions.Size = new System.Drawing.Size(776, 143);
            this.grpDatabaseOptions.TabIndex = 1;
            this.grpDatabaseOptions.TabStop = false;
            this.grpDatabaseOptions.Text = "数据库选择";
            // 
            // cmbDatabase
            // 
            this.cmbDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDatabase.FormattingEnabled = true;
            this.cmbDatabase.Location = new System.Drawing.Point(90, 28);
            this.cmbDatabase.Name = "cmbDatabase";
            this.cmbDatabase.Size = new System.Drawing.Size(193, 20);
            this.cmbDatabase.TabIndex = 3;
            this.cmbDatabase.SelectedIndexChanged += new System.EventHandler(this.cmbDatabase_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "数据库:";
            // 
            // lstTables
            // 
            this.lstTables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstTables.FormattingEnabled = true;
            this.lstTables.ItemHeight = 12;
            this.lstTables.Location = new System.Drawing.Point(90, 58);
            this.lstTables.Name = "lstTables";
            this.lstTables.Size = new System.Drawing.Size(670, 64);
            this.lstTables.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "表/视图:";
            // 
            // grpExportOptions
            // 
            this.grpExportOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpExportOptions.Controls.Add(this.btnBrowse);
            this.grpExportOptions.Controls.Add(this.txtSavePath);
            this.grpExportOptions.Controls.Add(this.label10);
            this.grpExportOptions.Controls.Add(this.txtCustomSql);
            this.grpExportOptions.Controls.Add(this.radCustomSql);
            this.grpExportOptions.Controls.Add(this.radSelectedTable);
            this.grpExportOptions.Controls.Add(this.label8);
            this.grpExportOptions.Enabled = false;
            this.grpExportOptions.Location = new System.Drawing.Point(12, 316);
            this.grpExportOptions.Name = "grpExportOptions";
            this.grpExportOptions.Size = new System.Drawing.Size(776, 171);
            this.grpExportOptions.TabIndex = 2;
            this.grpExportOptions.TabStop = false;
            this.grpExportOptions.Text = "导出选项";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(685, 136);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 7;
            this.btnBrowse.Text = "浏览...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtSavePath
            // 
            this.txtSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSavePath.Location = new System.Drawing.Point(90, 138);
            this.txtSavePath.Name = "txtSavePath";
            this.txtSavePath.Size = new System.Drawing.Size(589, 21);
            this.txtSavePath.TabIndex = 6;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 141);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 12);
            this.label10.TabIndex = 5;
            this.label10.Text = "保存位置:";
            // 
            // txtCustomSql
            // 
            this.txtCustomSql.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomSql.Enabled = false;
            this.txtCustomSql.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCustomSql.Location = new System.Drawing.Point(90, 51);
            this.txtCustomSql.Multiline = true;
            this.txtCustomSql.Name = "txtCustomSql";
            this.txtCustomSql.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCustomSql.Size = new System.Drawing.Size(670, 75);
            this.txtCustomSql.TabIndex = 4;
            // 
            // radCustomSql
            // 
            this.radCustomSql.AutoSize = true;
            this.radCustomSql.Location = new System.Drawing.Point(215, 29);
            this.radCustomSql.Name = "radCustomSql";
            this.radCustomSql.Size = new System.Drawing.Size(101, 16);
            this.radCustomSql.TabIndex = 2;
            this.radCustomSql.Text = "自定义SQL语句";
            this.radCustomSql.UseVisualStyleBackColor = true;
            this.radCustomSql.CheckedChanged += new System.EventHandler(this.radCustomSql_CheckedChanged);
            // 
            // radSelectedTable
            // 
            this.radSelectedTable.AutoSize = true;
            this.radSelectedTable.Checked = true;
            this.radSelectedTable.Location = new System.Drawing.Point(90, 29);
            this.radSelectedTable.Name = "radSelectedTable";
            this.radSelectedTable.Size = new System.Drawing.Size(119, 16);
            this.radSelectedTable.TabIndex = 1;
            this.radSelectedTable.TabStop = true;
            this.radSelectedTable.Text = "导出上面选择的表";
            this.radSelectedTable.UseVisualStyleBackColor = true;
            this.radSelectedTable.CheckedChanged += new System.EventHandler(this.radSelectedTable_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 31);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "导出内容:";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.progressExport});
            this.statusStrip1.Location = new System.Drawing.Point(0, 528);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(140, 17);
            this.lblStatus.Text = "准备就绪，请连接数据库";
            // 
            // progressExport
            // 
            this.progressExport.Name = "progressExport";
            this.progressExport.Size = new System.Drawing.Size(200, 16);
            this.progressExport.Visible = false;
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExport.Location = new System.Drawing.Point(533, 493);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(83, 32);
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "开始导出";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnPause
            // 
            this.btnPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPause.Enabled = false;
            this.btnPause.Location = new System.Drawing.Point(622, 493);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 32);
            this.btnPause.TabIndex = 5;
            this.btnPause.Text = "暂停";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(703, 493);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 32);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // DbToExcelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 550);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.grpExportOptions);
            this.Controls.Add(this.grpDatabaseOptions);
            this.Controls.Add(this.grpConnection);
            this.MinimumSize = new System.Drawing.Size(600, 500);
            this.Name = "DbToExcelForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据库导出到Excel";
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeout)).EndInit();
            this.grpDatabaseOptions.ResumeLayout(false);
            this.grpDatabaseOptions.PerformLayout();
            this.grpExportOptions.ResumeLayout(false);
            this.grpExportOptions.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkWindowsAuth;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudTimeout;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox grpDatabaseOptions;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListBox lstTables;
        private System.Windows.Forms.ComboBox cmbDatabase;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox grpExportOptions;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton radCustomSql;
        private System.Windows.Forms.RadioButton radSelectedTable;
        private System.Windows.Forms.TextBox txtCustomSql;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtSavePath;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar progressExport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnCancel;
    }
}
