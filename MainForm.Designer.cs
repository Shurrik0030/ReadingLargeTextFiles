namespace ReadingLargeTextFiles
{
    partial class MainForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.panelContent = new System.Windows.Forms.Panel();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnDbExporter = new System.Windows.Forms.Button();
            this.btnTextReader = new System.Windows.Forms.Button();
            this.btnExcelViewer = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelMenu.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.SuspendLayout();
            //
            // panel1
            //
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1024, 80);
            this.panel1.TabIndex = 0;
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(30, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据处理工具";
            //
            // panel2
            //
            this.panel2.Controls.Add(this.panelContent);
            this.panel2.Controls.Add(this.panelMenu);

            //
            // panelContent
            //
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(200, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(824, 520);
            this.panelContent.TabIndex = 1;

            //
            // panelMenu
            //
            this.panelMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelMenu.Controls.Add(this.btnExcelViewer);
            this.panelMenu.Controls.Add(this.btnDbExporter);
            this.panelMenu.Controls.Add(this.btnTextReader);
            this.panelMenu.Controls.Add(this.btnHome);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(200, 520);
            this.panelMenu.TabIndex = 0;

            //
            // panel2
            //
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 80);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1024, 520);
            this.panel2.TabIndex = 1;
            //
            // btnDbExporter
            //
            this.btnDbExporter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnDbExporter.FlatAppearance.BorderSize = 0;
            this.btnDbExporter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDbExporter.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDbExporter.ForeColor = System.Drawing.Color.White;
            this.btnDbExporter.Text = "  [DB] 数据库导出到Excel";
            this.btnDbExporter.Location = new System.Drawing.Point(0, 80);
            this.btnDbExporter.Name = "btnDbExporter";
            this.btnDbExporter.Size = new System.Drawing.Size(200, 40);
            this.btnDbExporter.TabIndex = 2;
            this.btnDbExporter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDbExporter.UseVisualStyleBackColor = false;
            this.btnDbExporter.Click += new System.EventHandler(this.btnDbExporter_Click);
            //
            // btnHome
            //
            this.btnHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnHome.FlatAppearance.BorderSize = 0;
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnHome.ForeColor = System.Drawing.Color.White;
            this.btnHome.Text = "首页";
            this.btnHome.Location = new System.Drawing.Point(0, 0);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(200, 40);
            this.btnHome.TabIndex = 0;
            this.btnHome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHome.UseVisualStyleBackColor = false;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            //
            // btnExcelViewer
            //
            this.btnExcelViewer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnExcelViewer.FlatAppearance.BorderSize = 0;
            this.btnExcelViewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcelViewer.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExcelViewer.ForeColor = System.Drawing.Color.White;
            this.btnExcelViewer.Text = "  [XLS] 大型Excel文件查看";
            this.btnExcelViewer.Location = new System.Drawing.Point(0, 120);
            this.btnExcelViewer.Name = "btnExcelViewer";
            this.btnExcelViewer.Size = new System.Drawing.Size(200, 40);
            this.btnExcelViewer.TabIndex = 3;
            this.btnExcelViewer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExcelViewer.UseVisualStyleBackColor = false;
            this.btnExcelViewer.Click += new System.EventHandler(this.btnExcelViewer_Click);
            //
            // btnTextReader
            //
            this.btnTextReader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnTextReader.FlatAppearance.BorderSize = 0;
            this.btnTextReader.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTextReader.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnTextReader.ForeColor = System.Drawing.Color.White;
            this.btnTextReader.Text = "  [TXT] 大文件分块读取";
            this.btnTextReader.Location = new System.Drawing.Point(0, 40);
            this.btnTextReader.Name = "btnTextReader";
            this.btnTextReader.Size = new System.Drawing.Size(200, 40);
            this.btnTextReader.TabIndex = 1;
            this.btnTextReader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTextReader.UseVisualStyleBackColor = false;
            this.btnTextReader.Click += new System.EventHandler(this.btnTextReader_Click);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 600);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据处理工具";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panelMenu.ResumeLayout(false);
            this.panelContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnTextReader;
        private System.Windows.Forms.Button btnDbExporter;
        private System.Windows.Forms.Button btnExcelViewer;
    }
}
