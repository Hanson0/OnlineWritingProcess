namespace OnlineWritingProcess.AllForms
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.textUser = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.textPassword = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.btnLogin = new DevExpress.XtraEditors.SimpleButton();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::OnlineWritingProcess.AllForms.WaitForm1), true, true, true);
            this.labErrTip = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.SuspendLayout();
            // 
            // textUser
            // 
            this.textUser.BackColor = System.Drawing.Color.Gainsboro;
            // 
            // 
            // 
            this.textUser.Border.Class = "TextBoxBorder";
            this.textUser.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.textUser.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textUser.Location = new System.Drawing.Point(38, 173);
            this.textUser.Multiline = true;
            this.textUser.Name = "textUser";
            this.textUser.PreventEnterBeep = true;
            this.textUser.Size = new System.Drawing.Size(420, 46);
            this.textUser.TabIndex = 0;
            this.textUser.WatermarkBehavior = DevComponents.DotNetBar.eWatermarkBehavior.HideNonEmpty;
            this.textUser.WatermarkText = "账号";
            // 
            // textPassword
            // 
            this.textPassword.BackColor = System.Drawing.Color.Gainsboro;
            // 
            // 
            // 
            this.textPassword.Border.Class = "TextBoxBorder";
            this.textPassword.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.textPassword.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textPassword.Location = new System.Drawing.Point(38, 253);
            this.textPassword.Multiline = true;
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '*';
            this.textPassword.PreventEnterBeep = true;
            this.textPassword.Size = new System.Drawing.Size(420, 46);
            this.textPassword.TabIndex = 1;
            this.textPassword.WatermarkText = "密码";
            // 
            // btnLogin
            // 
            this.btnLogin.Appearance.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnLogin.Appearance.Options.UseFont = true;
            this.btnLogin.Appearance.Options.UseForeColor = true;
            this.btnLogin.Location = new System.Drawing.Point(38, 358);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(88, 54);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "登录";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // labErrTip
            // 
            this.labErrTip.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labErrTip.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labErrTip.Location = new System.Drawing.Point(38, 96);
            this.labErrTip.Name = "labErrTip";
            this.labErrTip.Size = new System.Drawing.Size(420, 60);
            this.labErrTip.TabIndex = 3;
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Font = new System.Drawing.Font("微软雅黑", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelX1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(179)))), ((int)(((byte)(165)))));
            this.labelX1.Location = new System.Drawing.Point(38, 28);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(420, 60);
            this.labelX1.TabIndex = 4;
            this.labelX1.Text = "Manual Writing Process";
            this.labelX1.TextAlignment = System.Drawing.StringAlignment.Center;
            // 
            // Login
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 453);
            this.ControlBox = false;
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.labErrTip);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.textPassword);
            this.Controls.Add(this.textUser);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "访问权限";
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.TextBoxX textUser;
        private DevComponents.DotNetBar.Controls.TextBoxX textPassword;
        private DevExpress.XtraEditors.SimpleButton btnLogin;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevComponents.DotNetBar.LabelX labErrTip;
        private DevComponents.DotNetBar.LabelX labelX1;
    }
}