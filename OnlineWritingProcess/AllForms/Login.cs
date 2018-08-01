using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using System.Threading;
//using Production;
//using Production.Windows;

namespace OnlineWritingProcess.AllForms
{
    public partial class Login : DevExpress.XtraEditors.XtraForm
    {
        private bool loginOk;

        public bool LoginOk
        {
            get { return loginOk; }
            set { loginOk = value; }
        }
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string errReason;
            int ret = LoginHelp.StartLogin(textUser.Text, textPassword.Text, out errReason);
            if (ret!=0)
            {
                labErrTip.BackColor =Color.FromArgb(251, 225, 227);//B：251, 225, 227  F：231, 61, 74
                labErrTip.ForeColor = Color.FromArgb(231, 61, 74);//B：251, 225, 227  F：231, 61, 74
                labErrTip.Text = errReason;
                textPassword.Text = "";
                textUser.Focus();
                textUser.SelectAll();
                return;
            }
            loginOk = true;
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormCaption("请稍后");
            splashScreenManager1.SetWaitFormDescription("登录中...");
            Thread.Sleep(1500);
            splashScreenManager1.CloseWaitForm();
            this.Close();
        }





    }
}