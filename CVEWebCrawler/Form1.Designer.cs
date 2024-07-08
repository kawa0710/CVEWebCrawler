namespace CVEWebCrawler
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnGo = new Button();
            label1 = new Label();
            txbInput = new TextBox();
            btnInput = new Button();
            label2 = new Label();
            txbOutput = new TextBox();
            btnOutput = new Button();
            ckbDownload = new CheckBox();
            label3 = new Label();
            lblBrowser = new Label();
            btnBrowser = new Button();
            txbBrowser = new TextBox();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            SuspendLayout();
            // 
            // btnGo
            // 
            btnGo.Location = new Point(242, 183);
            btnGo.Name = "btnGo";
            btnGo.Size = new Size(100, 25);
            btnGo.TabIndex = 0;
            btnGo.Text = "Go";
            btnGo.UseVisualStyleBackColor = true;
            btnGo.Click += btnGo_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(63, 63);
            label1.Name = "label1";
            label1.Size = new Size(98, 15);
            label1.TabIndex = 1;
            label1.Text = "Read a CSV File:";
            // 
            // txbInput
            // 
            txbInput.Location = new Point(163, 60);
            txbInput.Name = "txbInput";
            txbInput.Size = new Size(300, 23);
            txbInput.TabIndex = 2;
            // 
            // btnInput
            // 
            btnInput.Location = new Point(469, 59);
            btnInput.Name = "btnInput";
            btnInput.Size = new Size(75, 24);
            btnInput.TabIndex = 3;
            btnInput.Text = "Select";
            btnInput.UseVisualStyleBackColor = true;
            btnInput.Click += btnInput_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(59, 105);
            label2.Name = "label2";
            label2.Size = new Size(102, 15);
            label2.TabIndex = 1;
            label2.Text = "Output Location:";
            // 
            // txbOutput
            // 
            txbOutput.Location = new Point(163, 102);
            txbOutput.Name = "txbOutput";
            txbOutput.Size = new Size(300, 23);
            txbOutput.TabIndex = 2;
            // 
            // btnOutput
            // 
            btnOutput.Location = new Point(469, 101);
            btnOutput.Name = "btnOutput";
            btnOutput.Size = new Size(75, 24);
            btnOutput.TabIndex = 3;
            btnOutput.Text = "Select";
            btnOutput.UseVisualStyleBackColor = true;
            btnOutput.Click += btnOutput_Click;
            // 
            // ckbDownload
            // 
            ckbDownload.AutoSize = true;
            ckbDownload.Location = new Point(432, 148);
            ckbDownload.Name = "ckbDownload";
            ckbDownload.Size = new Size(112, 19);
            ckbDownload.TabIndex = 5;
            ckbDownload.Text = "Download Files";
            ckbDownload.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(71, 23);
            label3.Name = "label3";
            label3.Size = new Size(90, 15);
            label3.TabIndex = 1;
            label3.Text = "Chrome/Edge:";
            // 
            // lblBrowser
            // 
            lblBrowser.AutoSize = true;
            lblBrowser.Location = new Point(163, 23);
            lblBrowser.Name = "lblBrowser";
            lblBrowser.Size = new Size(67, 15);
            lblBrowser.TabIndex = 1;
            lblBrowser.Text = "(Detected)";
            // 
            // btnBrowser
            // 
            btnBrowser.Location = new Point(469, 19);
            btnBrowser.Name = "btnBrowser";
            btnBrowser.Size = new Size(75, 24);
            btnBrowser.TabIndex = 7;
            btnBrowser.Text = "Select";
            btnBrowser.UseVisualStyleBackColor = true;
            btnBrowser.Visible = false;
            btnBrowser.Click += btnBrowser_Click;
            // 
            // txbBrowser
            // 
            txbBrowser.Location = new Point(163, 20);
            txbBrowser.Name = "txbBrowser";
            txbBrowser.Size = new Size(300, 23);
            txbBrowser.TabIndex = 6;
            txbBrowser.Visible = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.Red;
            label4.Location = new Point(63, 23);
            label4.Name = "label4";
            label4.Size = new Size(12, 15);
            label4.TabIndex = 8;
            label4.Text = "*";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = Color.Red;
            label5.Location = new Point(55, 64);
            label5.Name = "label5";
            label5.Size = new Size(12, 15);
            label5.TabIndex = 9;
            label5.Text = "*";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = Color.Red;
            label6.Location = new Point(51, 106);
            label6.Name = "label6";
            label6.Size = new Size(12, 15);
            label6.TabIndex = 10;
            label6.Text = "*";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(594, 241);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(lblBrowser);
            Controls.Add(txbBrowser);
            Controls.Add(btnBrowser);
            Controls.Add(ckbDownload);
            Controls.Add(btnOutput);
            Controls.Add(txbOutput);
            Controls.Add(btnInput);
            Controls.Add(label2);
            Controls.Add(txbInput);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(btnGo);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CVEWebCrawler";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnGo;
        private Label label1;
        private TextBox txbInput;
        private Button btnInput;
        private Label label2;
        private TextBox txbOutput;
        private Button btnOutput;
        private CheckBox ckbDownload;
        private Label label3;
        private Label lblBrowser;
        private Button btnBrowser;
        private TextBox txbBrowser;
        private Label label4;
        private Label label5;
        private Label label6;
    }
}
