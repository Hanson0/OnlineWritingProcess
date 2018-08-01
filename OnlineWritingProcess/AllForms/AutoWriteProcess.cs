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
using Production.Windows;
using Production;
using System.Threading;
using System.IO.Ports;
using System.IO;
using DownLoad;
using System.Text.RegularExpressions;

namespace OnlineWritingProcess
{

    public partial class AutoWriteProcess : DevExpress.XtraEditors.XtraForm
    {
        private delegate void UpdateUI();
        public static AutoWriteProcess autoWriteForm;

        CmdProcess cmdProcess = new CmdProcess();   //cmd类
        private List<string> preDevList = new List<string>() { };
        private static string configPath = ConfigInfo.ConfigPath;            //配置文件路径
        StringBuilder tempStringBuilder = new StringBuilder();      //全局可变字符串实例，用于读取配置文件内容
        //Infomation
        private string strLogPath;                      //log路径
        private string strBinPath;                      //log路径
        
        //Result 
        private int iPass;
        private int iFail;


        private const string commCmd = "SerialDownloader_cpp.exe -";
        private const string DevicesRead = "check devices";
        private const string CheckSumValueRead = "check checksumvalue";

        private string SkipsysSet = "set skipsys {0}";//True
        private string SkipcalSet = "set skipcal {0}";//True

        private string ResetSet = "set reset {0}";//True

        private string UartSetX = "set uartset {0}";

        private string FirmwarePathSet = "set firmware {0}";//path:""

        private string OffsetSet = "set userdataoffset {0}";//hex_addr

        private string ChecksumSet = "set checksum {0}";//True


        private const string DownloadAll = "download";//True
        private string DownloadSome = "download -port={0}";//True

        private const string idKeySubstr = "ID:";

        //private bool isFindDev;
        private int preDevCount;//初始未发现设备
        private bool isChanged;
        private bool hasSelectCom;
        private bool addValue;//processbar 的value+
        //private bool getcheckSumValue;
        private System.Windows.Forms.ProgressBar selectProgressBar;
        private PictureBox selectPictureBox;
        private string comValue;
        private string strIp;
        private string macUrl = "http://{0}:7778/haier_ge/download?mac={1}";

        enum readCmd
        {
            ReadCmd,
            WriteCmd
        }
        //private List<string> deviceList;
        private bool startSearchDev = true;
        public bool StartSearchDev
        {
            get { return startSearchDev; }
            set { startSearchDev = value; }
        }

        private int uartSet;
        public int UartSet
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                Win32API.GetPrivateProfileString("Setting", "UartSet", "", stringBuilder, 256, configPath);
                uartSet = int.Parse(stringBuilder.ToString());
                return uartSet;
            }
        }
        private static string toolFolder;//工具文件夹名称
        public static string ToolFolder
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                Win32API.GetPrivateProfileString("Tool", "ToolFolderName", "", stringBuilder, 256, configPath);
                toolFolder = stringBuilder.ToString();
                return toolFolder;
            }
        }



        public AutoWriteProcess()
        {
            InitializeComponent();
            InitGlobleVariable();
            InitComBoxUart();

            InitForm();
            autoWriteForm = this;

        }
        public void InitGlobleVariable()
        {
            //!!!!!!!!!!!!!!上线后该地址要更改为：获取当前系统地址，再拼剪为formain的地址

            strLogPath = System.Environment.CurrentDirectory + "\\Log\\";
            if (!Directory.Exists(strLogPath))
            {
                Directory.CreateDirectory(strLogPath);
            }
            strBinPath = System.Environment.CurrentDirectory + "\\BinFile\\";
            if (!Directory.Exists(strBinPath))
            {
                Directory.CreateDirectory(strBinPath);
            }

            //HTTP
            #region
            Win32API.GetPrivateProfileString("Http", "IP", "", tempStringBuilder, 256, configPath);
            if (tempStringBuilder.ToString() == "")
            {
                Win32API.WritePrivateProfileString("Http", "IP", "10.42.1.4", configPath);
            }
            else
            {
                strIp = tempStringBuilder.ToString();
            }

            //Win32API.GetPrivateProfileString("Http", "Port", "", tempStringBuilder, 256, strCurrentDirectory);
            //if (tempStringBuilder.ToString() == "")
            //{
            //    Win32API.WritePrivateProfileString("Http", "Port", "8088", strCurrentDirectory);
            //}
            //else
            //{
            //    HttpServer.Port = tempStringBuilder.ToString();
            //}
            #endregion

            //Result
            #region
            Win32API.GetPrivateProfileString("Result", "Pass", "", tempStringBuilder, 256, configPath);
            if (tempStringBuilder.ToString() == "")
            {
                Win32API.WritePrivateProfileString("Result", "Pass", "6", configPath);
            }
            else
            {
                textPass.Text = tempStringBuilder.ToString();
                iPass = int.Parse(tempStringBuilder.ToString());
            }

            Win32API.GetPrivateProfileString("Result", "Fail", "", tempStringBuilder, 256, configPath);
            if (tempStringBuilder.ToString() == "")
            {
                Win32API.WritePrivateProfileString("Result", "Fail", "6", configPath);

            }
            else
            {
                textFail.Text = tempStringBuilder.ToString();
                iFail = int.Parse(tempStringBuilder.ToString());
                textTotal.Text = (iPass + iFail).ToString();
                if (iPass + iFail == 0)
                {
                    textRate.Text = "0.00%";
                }
                else
                {
                    textRate.Text = Math.Round((double)iPass * 100 / (iPass + iFail), 2).ToString() + "%";
                }
            }
            #endregion
        }
        void InitComBoxUart()
        {
            try
            {
                string[] uartPin = { "C0/C3", "A0/A4", "A6/A7", "D4/D7" };
                //, "D0/D3", "B4/B5"
                comBoxUart.Items.AddRange(uartPin);
                if (UartSet < 5 && UartSet >= 0)
                {
                    comBoxUart.SelectedIndex = uartSet;
                }
                else
                {
                    XtraMessageBox.Show("串口设置有误");
                    comBoxUart.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }
        void InitForm()
        {
            #region 读form ini
            #region skipSys
            //跳过系统
            Win32API.GetPrivateProfileString("Setting", "SkipSys", "", tempStringBuilder, 256, configPath);
            if (tempStringBuilder.ToString() == "True")
            {
                checkSkipSys.Checked = true;
            }
            else if (tempStringBuilder.ToString() == "False")
            {
                checkSkipSys.Checked = false;
            }
            #endregion
            #region skipCal
            //跳过计算
            Win32API.GetPrivateProfileString("Setting", "SkipCal", "", tempStringBuilder, 256, configPath);
            if (tempStringBuilder.ToString() == "True")
            {
                checkSkipCal.Checked = true;
            }
            else if (tempStringBuilder.ToString() == "False")
            {
                checkSkipCal.Checked = false;
            }
            #endregion
            #region reset
            //重启
            Win32API.GetPrivateProfileString("Setting", "checkReset", "", tempStringBuilder, 256, configPath);
            if (tempStringBuilder.ToString() == "True")
            {
                checkReset.Checked = true;
            }
            else if (tempStringBuilder.ToString() == "False")
            {
                checkReset.Checked = false;
            }
            #endregion
            #region offset
            Win32API.GetPrivateProfileString("Setting", "checkOffset", "", tempStringBuilder, 256, configPath);
            if (tempStringBuilder.ToString() == "True")
            {
                checkOffset.Checked = true;

                checkSkipSys.Enabled = false;
                checkSkipSys.Checked = false;

                checkSkipCal.Enabled = false;
                checkSkipCal.Checked = false;

                labOffset.Enabled = true;

                Win32API.GetPrivateProfileString("Setting", "UserDataOffset", "", tempStringBuilder, 256, configPath);
                labOffset.Text = tempStringBuilder.ToString();

            }
            else if (tempStringBuilder.ToString() == "False")
            {
                checkOffset.Checked = false;

                labOffset.Text = "";
                labOffset.Enabled = false;
                checkSkipSys.Enabled = true;
                checkSkipCal.Enabled = true;

            }
            #endregion
            #region checkSum
            //校验
            Win32API.GetPrivateProfileString("Setting", "checkSum", "", tempStringBuilder, 256, configPath);
            if (tempStringBuilder.ToString() == "True")
            {
                checkSum.Checked = true;
            }
            else if (tempStringBuilder.ToString() == "False")
            {
                checkSum.Checked = false;
            }
            #endregion
            #region factory
            //校验
            Win32API.GetPrivateProfileString("Setting", "Factory", "", tempStringBuilder, 256, configPath);
            if (tempStringBuilder.ToString() == "True")
            {
                //checkSum.Checked = true;
                btnProductMode_Click(null, null);
            }
            #endregion
            #endregion
        }

        private void btnSetID_Click(object sender, EventArgs e)
        {
            try
            {
                string strId = listDevices.SelectedItem.ToString();
                int position;
                if ((position = strId.IndexOf(idKeySubstr)) >= 0)
                {
                    string temp = strId.Substring(position + idKeySubstr.Length);

                    //listDevices.SelectedItem 
                    if (temp != "--")
                    {
                        return;
                    }
                    strId = strId.Replace(temp, textMbedId.Text);
                    listDevices.Items.Clear();
                    listDevices.Items.Add(strId);
                }
            }
            catch (Exception)
            {
                XtraMessageBox.Show("\r\n请选择设备");
                return;
            }
        }

        private void btnSearchDev_Click(object sender, EventArgs e)
        {
            updateDevice();
        }

        private void SearchDevThread()
        {
            while (true)
            {
                if (StartSearchDev)
                {
                    updateDevice();
                    Thread.Sleep(400);
                }
            }
        }
        /// <summary>
        /// cmd.exe发送命令并存储返回内容
        /// </summary>
        /// <param name="cmdType">命令类型—read、set</param>
        /// <param name="cmdKey">命令部分</param>
        /// <param name="idKeySubstr">返回值提取的关键字</param>
        /// <param name="deviceList">返回提取信息的存储表</param>
        /// <param name="cmdValue">命令参数部分</param>
        /// <returns></returns>
        private int SendCmd(int cmdType, string cmdKey, string idKeySubstr, out List<string> deviceList, bool getcheckSumValue, string cmdValue = "False")
        {
            int ret = -1;
            string strCmdReturn = "";
            string[] spit = { "\r\n" };
            string[] line;
            int positon;
            deviceList = new List<string>();
            switch (cmdType)
            {
                case (int)readCmd.ReadCmd://read
                    strCmdReturn = cmdProcess.ExeCommand(ToolFolder, commCmd + cmdKey);
                    break;
                case (int)readCmd.WriteCmd://set
                    strCmdReturn = cmdProcess.ExeCommand(ToolFolder, commCmd + string.Format(cmdKey, cmdValue));
                    break;
                default:
                    break;
            }
            line = strCmdReturn.Split(spit, StringSplitOptions.RemoveEmptyEntries);
            if (getcheckSumValue)//获取校验码
            {
                deviceList.Add(line[4]);
            }
            else//其他有关键字标识的
            {
                for (int i = 0; i < line.Length; i++)
                {
                    if ((positon = line[i].IndexOf(idKeySubstr)) >= 0)
                    {
                        string temp = line[i].Substring(positon);
                        deviceList.Add(temp);
                    }
                }
            }
            if (deviceList.Count == 1)
            {
                ret = 0;
            }

            return ret;
        }
        private void updateDevice()
        {
            //进入到工具文件目录下，执行命令
            //工具文件夹名称需设置成配置
            List<string> deviceList;
            SendCmd((int)readCmd.ReadCmd, DevicesRead, "COM", out deviceList, false);
            /****************************************/
            if (preDevList.Count == deviceList.Count)//设备数量相等
            {
                for (int i = 0; i < preDevList.Count; i++)
                {
                    if (preDevList[i] != deviceList[i])//此处可能有风险，数量相同，如果相对于前一次，第二次两个端口的顺序变了，就不同了，可实际内容是相同的
                    {//确保端口顺序都是 由小到大 的就行，观察一般都是
                        isChanged = true;
                        break;
                    }
                }
                if (!isChanged)//数量+设备内容相同
                {
                    return;
                }
            }
            //有不同之处 旧值赋新值
            isChanged = false;
            preDevList = deviceList;
            //更新
            UpdateUI updateUi = delegate
            {
                //清空
                //textComInfo.Text = "";
                listDevices.Items.Clear();
                panelControl3.Controls.Clear();
                //更新 log
                foreach (var device in deviceList)
                {
                    //textComInfo.Text += device + "\r\n";
                    listDevices.Items.Add(device);
                }
                //更新控件
                for (int i = 0; i < deviceList.Count; i++)
                {
                    #region 控件创建
                    #region 进度条动态亮光
                    System.Windows.Forms.ProgressBar progressBar = new System.Windows.Forms.ProgressBar();

                    // progressBar1
                    // 
                    progressBar.Location = new System.Drawing.Point(45, 89);
                    progressBar.Name = "progressBar" + i.ToString();
                    progressBar.Location = new Point(95, 35 * (i) + 5);//改变35：改变间距
                    progressBar.Size = new Size(220, 30);
                    progressBar.TabIndex = 30 + i;
                    //设置一个最小值
                    progressBar.Minimum = 0;
                    //设置一个最大值
                    progressBar.Maximum = 100;
                    //设置步长，即每次增加的数
                    progressBar.Step = 1;
                    //设置进度条的样式
                    progressBar.Style = ProgressBarStyle.Blocks;
                    //当前值
                    progressBar.Value = 0;
                    panelControl3.Controls.Add(progressBar);
                    #endregion

                    #region 复选框
                    // checkEdit
                    CheckEdit checkBoxx = new CheckEdit();
                    checkBoxx.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
                    checkBoxx.Properties.RadioGroupIndex = 1;
                    checkBoxx.Location = new Point(2, 35 * (i) + 5);

                    int position;
                    string[] comInfo = deviceList[i].Split(' ');
                    string comName = comInfo[0];
                    if ((position = deviceList[i].IndexOf(idKeySubstr)) >= 0)
                    {
                        string temp = deviceList[i].Substring(position + idKeySubstr.Length);
                        checkBoxx.Properties.Caption = comName + ":" + temp;//COM+temp
                    }
                    checkBoxx.Name = "checkBoxx" + i.ToString();
                    checkBoxx.Checked = true;
                    /******************COM口显示***********************/
                    /********************************************/
                    //checkBoxx.Properties.Caption = "COM" + i.ToString();//com[i];
                    checkBoxx.Size = new Size(90, 30);
                    this.panelControl3.Controls.Add(checkBoxx);
                    #endregion

                    #region 结果图
                    //创建结果显示
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Name = "picture" + i.ToString();

                    pictureBox.Image = Properties.Resources.白圆点;
                    //pictureBox.Dock = DockStyle.Top;
                    pictureBox.Location = new Point(330, 35 * (i) + 5);
                    //pictureBox.Size = new Size(300, 30);
                    pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                    //groupTestRet.Controls.Add(pictureBox);
                    panelControl3.Controls.Add(pictureBox);
                    #endregion
                    #endregion
                }

            };
            this.Invoke(updateUi);
        }

        private void btnProductMode_Click(object sender, EventArgs e)
        {

            //设置offset、reset、uart、checkSum
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormCaption("请稍后");
            splashScreenManager1.SetWaitFormDescription("设置中...");
            //关闭设备搜索
            StartSearchDev = false;
            //存储并设置
            btnProductMode.Enabled = false;
            panelControl1.Enabled = false;
            textMbedId.Enabled = false;
            btnSetID.Enabled = false;
            SaveConfigValue(true);
            splashScreenManager1.CloseWaitForm();
            StartSearchDev = true;

            Win32API.WritePrivateProfileString("Setting", "Factory", "True", configPath);
            //StartSearchDev = false;
            //panelControl2.Enabled = false;
            //listDevices.Items.Add("hello");
            //listDevices.Items.Add("222");
        }

        private void checkSum_CheckedChanged(object sender, EventArgs e)
        {
            if (checkSum.Checked)//勾选了只是cmd set checksum为True
            {

            }
        }

        private void AutoWriteProcess_Load(object sender, EventArgs e)
        {
            Thread detectDev = new Thread(new ThreadStart(SearchDevThread));
            detectDev.IsBackground = true;
            detectDev.Start();

        }
        private void listDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strId = listDevices.SelectedItem.ToString();
            int position;
            if ((position = strId.IndexOf(idKeySubstr)) >= 0)
            {
                string temp = strId.Substring(position + idKeySubstr.Length);
                textMbedId.Text = temp;
            }
        }

        public void SaveConfigValue(bool sendCmd)
        {
            #region 保存复选框+offset值
            #region skipSys
            //跳过系统
            if (checkSkipSys.Checked)
            {
                Win32API.WritePrivateProfileString("Setting", "SkipSys", "True", configPath);

            }
            else
            {
                Win32API.WritePrivateProfileString("Setting", "SkipSys", "False", configPath);
            }
            #endregion
            #region skipCal
            //跳过计算
            if (checkSkipCal.Checked)
            {
                Win32API.WritePrivateProfileString("Setting", "SkipCal", "True", configPath);
            }
            else
            {
                Win32API.WritePrivateProfileString("Setting", "SkipCal", "False", configPath);
            }
            #endregion
            #region reset
            //重启
            if (checkReset.Checked)
            {
                Win32API.WritePrivateProfileString("Setting", "checkReset", "True", configPath);
            }
            else
            {
                Win32API.WritePrivateProfileString("Setting", "checkReset", "False", configPath);
            }
            #endregion
            #region uart
            //串口
            Win32API.WritePrivateProfileString("Setting", "UartSet", comBoxUart.SelectedIndex.ToString(), configPath);
            #endregion
            #region offset
            //偏移地址  只保存一下状态，下次启动，读取状态，偏移为空的话，lab显示""
            if (checkOffset.Checked)
            {
                Win32API.WritePrivateProfileString("Setting", "checkOffset", "True", configPath);
                Win32API.WritePrivateProfileString("Setting", "UserDataOffset", labOffset.Text, configPath);
            }
            else
            {
                Win32API.WritePrivateProfileString("Setting", "checkOffset", "False", configPath);

            }
            #endregion
            #region checkSum
            //校验
            if (checkSum.Checked)
            {
                Win32API.WritePrivateProfileString("Setting", "checkSum", "True", configPath);

            }
            else
            {
                Win32API.WritePrivateProfileString("Setting", "checkSum", "False", configPath);
            }
            #endregion
            #endregion

            if (sendCmd)
            {
                Thread.Sleep(500);
                #region 启动cmd命令
                int Ret;
                List<string> deviceList;

                #region offset
                //偏移地址  只保存一下状态，下次启动，读取状态，偏移为空的话，lab显示""
                if (checkOffset.Checked)
                {
                    Ret = SendCmd((int)readCmd.WriteCmd, OffsetSet, "USERDATAOFFSET", out deviceList, false, labOffset.Text);
                    if (Ret != 0)
                    {
                        XtraMessageBox.Show("设置失败");
                    }
                }
                else
                {
                    Ret = SendCmd((int)readCmd.WriteCmd, OffsetSet, "USERDATAOFFSET", out deviceList, false, "NULL");
                    if (Ret != 0)
                    {
                        XtraMessageBox.Show("设置失败");
                    }
                    Thread.Sleep(500);
                    #region skipSys
                    //跳过系统
                    if (checkSkipSys.Checked)
                    {
                        Ret = SendCmd((int)readCmd.WriteCmd, SkipsysSet, "SKIPSYS", out deviceList, false, "True");
                        if (Ret != 0)
                        {
                            XtraMessageBox.Show("设置失败");
                        }
                    }
                    else
                    {
                        Ret = SendCmd((int)readCmd.WriteCmd, SkipsysSet, "SKIPSYS", out deviceList, false, "False");
                        if (Ret != 0)
                        {
                            XtraMessageBox.Show("设置失败");
                        }
                    }
                    #endregion
                    Thread.Sleep(500);
                    #region skipCal
                    //跳过计算
                    if (checkSkipCal.Checked)
                    {
                        Ret = SendCmd((int)readCmd.WriteCmd, SkipcalSet, "SKIPCAL", out deviceList, false, "True");
                        if (Ret != 0)
                        {
                            XtraMessageBox.Show("设置失败");
                        }
                    }
                    else
                    {
                        Ret = SendCmd((int)readCmd.WriteCmd, SkipcalSet, "SKIPCAL", out deviceList, false, "False");
                        if (Ret != 0)
                        {
                            XtraMessageBox.Show("设置失败");
                        }
                    }
                    #endregion

                }
                #endregion
                Thread.Sleep(500);
                #region reset
                //重启
                if (checkReset.Checked)
                {
                    Ret = SendCmd((int)readCmd.WriteCmd, ResetSet, "RESET", out deviceList, false, "True");
                    if (Ret != 0)
                    {
                        XtraMessageBox.Show("设置失败");
                    }
                }
                else
                {
                    Ret = SendCmd((int)readCmd.WriteCmd, ResetSet, "RESET", out deviceList, false, "False");
                    if (Ret != 0)
                    {
                        XtraMessageBox.Show("设置失败");
                    }
                }
                #endregion
                Thread.Sleep(500);

                #region uart
                //串口
                Ret = SendCmd((int)readCmd.WriteCmd, UartSetX, "UARTSET", out deviceList, false, comBoxUart.SelectedIndex.ToString());
                if (Ret != 0)
                {
                    XtraMessageBox.Show("设置失败");
                }

                #endregion
                Thread.Sleep(500);


                #region checkSum
                //校验
                if (checkSum.Checked)
                {
                    Ret = SendCmd((int)readCmd.WriteCmd, ChecksumSet, "CHECKSUM", out deviceList, false, "True");
                    if (Ret != 0)
                    {
                        XtraMessageBox.Show("设置失败");
                    }
                }
                else
                {
                    Ret = SendCmd((int)readCmd.WriteCmd, ChecksumSet, "CHECKSUM", out deviceList, false, "False");
                    if (Ret != 0)
                    {
                        XtraMessageBox.Show("设置失败");
                    }
                }
                #endregion
                #endregion
            }

        }


        private void checkOffset_CheckedChanged(object sender, EventArgs e)
        {
            if (checkOffset.Checked)
            {
                checkSkipSys.Enabled = false;
                checkSkipSys.Checked = false;

                checkSkipCal.Enabled = false;
                checkSkipCal.Checked = false;

                labOffset.Enabled = true;
                labOffset.Text = "0x0000000";
            }
            else
            {
                labOffset.Text = "";
                labOffset.Enabled = false;
                checkSkipSys.Enabled = true;
                checkSkipCal.Enabled = true;
            }
        }

        private void textMac_TextChanged(object sender, EventArgs e)
        {
            if (textMac.Text.Length > 11)
            {

                //正则表达式
                if (Regex.IsMatch(textMac.Text, "^[0-9a-zA-Z]{12}$"))//@"(\d{15})[,，]{1}(\w{15})"
                {
                    #region 根据所选下载，更新，写程
                    try
                    {
                        //清空历史数据
                        textLog.Text = "";
                        textLog.BackColor = Color.White;
                        if (selectPictureBox != null)
                        {
                            selectPictureBox.Image = Properties.Resources.白圆点;
                        }


                        splashScreenManager1.ShowWaitForm();
                        splashScreenManager1.SetWaitFormCaption("下载并写程中");
                        splashScreenManager1.SetWaitFormDescription("请稍后...");

                        #region 下载写程
                        string checkName = "";
                        int Ret = -1;
                        List<string> deviceList;

                        #region 根据所勾选的radiobox，寻找到对应行的 progressbar,picturebox
                        foreach (var item in panelControl3.Controls)
                        {
                            if (item is CheckEdit)
                            {
                                CheckEdit b = (CheckEdit)item;
                                if (b.Checked)
                                {
                                    hasSelectCom = true;
                                    checkName = b.Name;
                                    int startIndex = b.Text.IndexOf('M');
                                    int endIndex = b.Text.IndexOf(':');
                                    //得COM几
                                    comValue = b.Text.Substring(startIndex + 1, endIndex - startIndex - 1);
                                    checkName = checkName.Substring(checkName.Length - 1, 1);

                                    //ProgressBar+i
                                    string progressBarName = "progressBar" + checkName;
                                    foreach (var control in panelControl3.Controls)
                                    {
                                        if (control is System.Windows.Forms.ProgressBar)
                                        {
                                            //得到selectProgressBar
                                            selectProgressBar = (System.Windows.Forms.ProgressBar)control;
                                            if (selectProgressBar.Name == progressBarName)
                                            {
                                                selectProgressBar.Value = 0;
                                                //addValue = true;
                                                //Thread progresThread = new Thread(new ParameterizedThreadStart(proThread));
                                                //progresThread.IsBackground = false;
                                                //progresThread.Start((object)selectProgressBar);
                                            }
                                        }
                                    }

                                    //"picture" + i
                                    foreach (var control in panelControl3.Controls)
                                    {
                                        if (control is PictureBox)
                                        {
                                            //得到selectProgressBar
                                            selectPictureBox = (PictureBox)control;
                                        }
                                    }


                                }
                            }
                        }
                        #endregion
                        if (!hasSelectCom)
                        {
                            XtraMessageBox.Show("请选择烧录的COM口");
                            goto END;
                        }
                        hasSelectCom = false;
                        StartSearchDev = false;

                        //根据MAC号,下载bin文件
                        //string urlDownload = url+textMac.Text;
                        string binPath;
                        string msg;
                        bool result = update(string.Format(macUrl, strIp, textMac.Text), strBinPath, out binPath, out msg);//"http://down10.zol.com.cn/xiazai/utorrentv3.5.0.44246.b.zip"
                        if (!result)
                        {
                            PutResult(false, msg + "bin文件下载失败");
                            goto END;
                        }
                        //设置bin文件路径
                        //string binPath=strBinPath+fileName+
                        Ret = SendCmd((int)readCmd.WriteCmd, FirmwarePathSet, "FIRMWARE", out deviceList, false, binPath);// @"C:\Users\ZJH\Desktop\237669683672908_D828C911B74C.bin"
                        if (Ret != 0)
                        {
                            PutResult(false, "MAC:" + textMac.Text + "  cmd获取bin文件路径失败");
                            textMac.Text = "";
                            addValue = false;//processbar持续增长
                            goto END;
                        }
                        //计算cheksum并显示
                        Thread.Sleep(500);
                        Ret = SendCmd((int)readCmd.ReadCmd, CheckSumValueRead, "", out deviceList, true);
                        if (Ret != 0)
                        {
                            XtraMessageBox.Show("获取checksum校验失败");
                            textMac.Text = "";
                            addValue = false;//processbar持续增长
                            goto END;
                        }
                        labChecksumValue.Text = deviceList[0];

                        ////设置offset、reset、uart、checkSum
                        //SaveConfigValue(true);

                        //程序烧录
                        Thread.Sleep(500);
                        Ret = SendCmd((int)readCmd.WriteCmd, DownloadSome, "COM", out deviceList, false, comValue);
                        if (Ret != 0)
                        {
                            PutResult(false, "MAC:" + textMac.Text + "  程序烧录cmd命令失败");
                            selectPictureBox.Image = Properties.Resources.红色圆;
                            textMac.Text = "";
                            addValue = false;
                            goto END;
                        }
                        if (deviceList[0].Contains("fail"))
                        {
                            //progressBar
                            selectPictureBox.Image = Properties.Resources.红色圆;

                            PutResult(false, "MAC:" + textMac.Text + "  写程失败");
                        }
                        else if (deviceList[0].Contains("success"))
                        {
                            selectProgressBar.Value = 100;
                            selectPictureBox.Image = Properties.Resources.绿色小圆;
                            PutResult(true, "MAC:" + textMac.Text + "   写程成功");

                        }
                        #endregion
                    END:

                        addValue = false;//processbar增加值
                        StartSearchDev = true;
                        textMac.Text = "";
                        if (selectProgressBar != null)
                        {
                            selectProgressBar.Value = 0;
                        }

                        splashScreenManager1.CloseWaitForm();
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(ex.Message);
                    }
                    #endregion
                }
                else
                {
                    PutResult(false, "MAC:" + textMac.Text + "   不合法", false);
                }
                //最后
                textMac.Text = "";

            }

            


        }
        private bool update(string url, string path,out string  proFile,out string msg)
        {
            bool flag=false;
            httpDown downloader = new httpDown();
            downloader.ctr = label1;
            downloader.ctr2 = label2;
            downloader.ctr3 = label3;
            downloader.progressBar = selectProgressBar;
            //开始下载
            flag = downloader.HttpDownload(url, path,out msg);
            //下载结束
            Thread.Sleep(100);
            proFile = downloader.ProFile;

            return flag;
        }
        private void proThread(object progressbar)
        {
            while (addValue)
            {
                UpdateUI updateUi = delegate
                {
                    System.Windows.Forms.ProgressBar bar = (System.Windows.Forms.ProgressBar)progressbar;
                    if (bar.Value < 90)
                    {
                        bar.Value++;
                        Thread.Sleep(1000);
                    }

                };
                this.Invoke(updateUi);

            }

        }


        public void PutResult(bool result, string log)
        {
            string logStr;

            if (result)
            {
                iPass++;
                logStr = "\r\n";
                logStr += "########     ###     ######   ######\r\n";
                logStr += "##     ##   ## ##   ##    ## ##    ##\r\n";
                logStr += "##     ##  ##   ##  ##       ##\r\n";
                logStr += "########  ##     ##  ######   ######\r\n";
                logStr += "##        #########       ##       ##\r\n";
                logStr += "##        ##     ## ##    ## ##    ##\r\n";
                logStr += "##        ##     ##  ######   ######\r\n";
                textLog.Text = logStr;
                textLog.Text += "====================================\r\n";
                textLog.Text += "====================================\r\n";
                textLog.Text += "LOG\r\n";
                textLog.Text += log;
                textLog.BackColor = Color.Green;

                //结果显示： 
                textPass.Text = iPass.ToString();
                Win32API.WritePrivateProfileString("Result", "Pass", iPass.ToString(), configPath);
                textTotal.Text = (iPass + iFail).ToString();
                textRate.Text = Math.Round((double)iPass * 100 / (iPass + iFail), 2).ToString() + "%";

                WritePassResult(textMac.Text, textLog.Text, strLogPath, true);
            }
            else
            {
                iFail++;
                logStr = "\r\n";
                logStr += "########    ###     ####  ##\r\n";
                logStr += "##         ## ##     ##   ##\r\n";
                logStr += "##        ##   ##    ##   ##\r\n";
                logStr += "######   ##     ##   ##   ##\r\n";
                logStr += "##       #########   ##   ##\r\n";
                logStr += "##       ##     ##   ##   ##\r\n";
                logStr += "##       ##     ##  ####  ########\r\n";

                textLog.Text = logStr;
                textLog.Text += "====================================\r\n";
                textLog.Text += "====================================\r\n";
                textLog.Text += "LOG\r\n";
                textLog.Text += log;
                textLog.BackColor = Color.Red;

                //结果显示： 
                textFail.Text = iFail.ToString();
                Win32API.WritePrivateProfileString("Result", "Fail", iFail.ToString(), configPath);
                textTotal.Text = (iPass + iFail).ToString();
                textRate.Text = Math.Round((double)iPass * 100 / (iPass + iFail), 2).ToString() + "%";

                WritePassResult(textMac.Text, textLog.Text, strLogPath, false);
            }
        }
        public void PutResult(bool result, string log, bool isWriteToRet)
        {
            string logStr;

            if (result)
            {
                iPass++;
                logStr = "\r\n";
                logStr += "########     ###     ######   ######\r\n";
                logStr += "##     ##   ## ##   ##    ## ##    ##\r\n";
                logStr += "##     ##  ##   ##  ##       ##\r\n";
                logStr += "########  ##     ##  ######   ######\r\n";
                logStr += "##        #########       ##       ##\r\n";
                logStr += "##        ##     ## ##    ## ##    ##\r\n";
                logStr += "##        ##     ##  ######   ######\r\n";
                textLog.Text = logStr;
                textLog.Text += "====================================\r\n";
                textLog.Text += "====================================\r\n";
                textLog.Text += "LOG\r\n";
                textLog.Text += log;
                textLog.BackColor = Color.Green;

                if (isWriteToRet)
                {
                    //结果显示： 
                    textPass.Text = iPass.ToString();
                    Win32API.WritePrivateProfileString("Result", "Pass", iPass.ToString(), configPath);
                    textTotal.Text = (iPass + iFail).ToString();
                    textRate.Text = Math.Round((double)iPass * 100 / (iPass + iFail), 2).ToString() + "%";

                    WritePassResult(textMac.Text, textLog.Text, strLogPath, true);
                }
            }
            else
            {
                iFail++;
                logStr = "\r\n";
                logStr += "########    ###     ####  ##\r\n";
                logStr += "##         ## ##     ##   ##\r\n";
                logStr += "##        ##   ##    ##   ##\r\n";
                logStr += "######   ##     ##   ##   ##\r\n";
                logStr += "##       #########   ##   ##\r\n";
                logStr += "##       ##     ##   ##   ##\r\n";
                logStr += "##       ##     ##  ####  ########\r\n";

                textLog.Text = logStr;
                textLog.Text += "====================================\r\n";
                textLog.Text += "====================================\r\n";
                textLog.Text += "LOG\r\n";
                textLog.Text += log;
                textLog.BackColor = Color.Red;

                if (isWriteToRet)
                {
                    //结果显示： 
                    textFail.Text = iFail.ToString();
                    Win32API.WritePrivateProfileString("Result", "Fail", iFail.ToString(), configPath);
                    textTotal.Text = (iPass + iFail).ToString();
                    textRate.Text = Math.Round((double)iPass * 100 / (iPass + iFail), 2).ToString() + "%";

                    WritePassResult(textMac.Text, textLog.Text, strLogPath, false);
                }
            }
        }

        public void WritePassResult(string Sn, string content, string path, bool IsPassOrFail)
        {
            string strTempName = null;
            string strTimeNow = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            if (IsPassOrFail)
            {
                strTempName = path + Sn.ToUpper() + "_" + strTimeNow + "_PASS.LOG";//文件夹的路径+文件
            }
            else
            {
                if (Sn == null)
                {
                    Sn = "FFFFFFFFF...";
                    //updateUI = delegate {
                    textMac.Text = Sn;
                    //};
                    //SN_text.Invoke(updateUI);
                }
                strTempName = path + Sn.ToUpper() + "_" + strTimeNow + "_FAIL.LOG";
            }

            FileStream fs = new FileStream(strTempName, FileMode.OpenOrCreate, FileAccess.Write);
            byte[] byteWrite = Encoding.Default.GetBytes(content);
            fs.Write(byteWrite, 0, byteWrite.Length);
            fs.Close();
        }



    }
}