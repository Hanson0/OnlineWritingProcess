using Production.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Production.IdReadWrite
{
    static class ReadWriteIdHandleInfo
    {
        private static string configPath;
        private static int checkDeviceInterval;      //检测模块上掉电的时间间隔 ms

        public static int CheckDeviceInterval
        {
            get
            {
                return checkDeviceInterval;
            }

            set
            {
                checkDeviceInterval = value;
            }
        }



        public static void ReadConfig()
        {
            StringBuilder stringBuilder = new StringBuilder();
            configPath = ConfigInfo.ConfigPath;

            //产品型号
            Win32API.GetPrivateProfileString("Time", "CheckDeviceInterval", "", stringBuilder, 256, configPath);
            checkDeviceInterval = int.Parse(stringBuilder.ToString().Trim());
        }
    }
}
