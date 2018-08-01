using Production;
using Production.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineWritingProcess
{
    class LoginHelp
    {
        private static string configPath;

        //private string userName;
        //private string passWord;

        //private int loginResult;
        //private string errReason;

        private static string correctUserName;
        private static string correctPassword;


        public static int StartLogin(string userName, string passWord,out string errReason)
        {
            int ret=-1;
            errReason = "";
            if (string.IsNullOrEmpty(userName))
            {
                errReason = "账号为空";
                return ret;
            }
            if (string.IsNullOrEmpty(passWord))
            {
                errReason = "密码为空";
                return ret;
            }
            if (userName != correctUserName || passWord != correctPassword)
            {
                errReason = "账号或密码错误";
                return ret;
            }
            ret = 0;
            return ret;
        }

        public static void ReadConfig()
        {
            configPath = ConfigInfo.ConfigPath;            //配置文件路径
            StringBuilder stringBuilder = new StringBuilder();

            Win32API.GetPrivateProfileString("Login", "user", "", stringBuilder, 256, configPath);
            correctUserName = stringBuilder.ToString().Trim();

            Win32API.GetPrivateProfileString("Login", "password", "", stringBuilder, 256, configPath);
            correctPassword = stringBuilder.ToString().Trim();
        }

    }
}
