using OnlineWritingProcess;
//using Production.ProductionTest;
using Production.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Production
{
    class ConfigInfo
    {
        private static string configPath;    //配置文件的路径

        public static string ConfigPath
        {
            get
            {
                return configPath;
            }

            private set
            {
                configPath = value;
            }
        }


        static ConfigInfo()
        {
            configPath = System.Environment.CurrentDirectory + "\\SetUp.ini";
        }


        /// <summary>
        /// 初始化所有基础类的配置
        /// </summary>
        public static void Init()
        {
            try
            {
                //ResultInfo.ReadConfig();
                LoginHelp.ReadConfig();


                //ProductionInfo.ReadConfig();
                //Server.HttpServerInfo.ReadConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Environment.Exit(0);
            }

        }


    }
}
