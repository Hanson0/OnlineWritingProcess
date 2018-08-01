using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.DelegateUI;
using FactoryAuto;
using System.Threading;
using OnlineWritingProcess;

namespace DownLoad
{
    class httpDown
    {
        public string ProFile;
        public System.Windows.Forms.Control ctr;
        public System.Windows.Forms.Control ctr2;
        public System.Windows.Forms.Control ctr3;
        public System.Windows.Forms.Control progressBar;
        delegate void UpdateUI();
        UpdateUI updateUI;

        private StopWatch stopWatch = new StopWatch();

        /// <summary>
        /// http下载文件
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="path">文件存放地址，包含文件名</param>
        /// <returns></returns>
        public bool HttpDownload(string url, string path)
        {
            string tempPath = path;
            //Directory.CreateDirectory(tempPath);                                      //创建临时文件目录
            string ss = url.Split('/').Last();

            string tempFile = string.Format("{0}/{1}.{2}", tempPath, ss, "temp");   //临时文件

            ProFile = string.Format("{0}\\{1}", tempPath, ss);

            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);    //存在则删除
            }
            try
            {
                #region
                FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                int contentLenth;
                contentLenth = (int)(response.ContentLength / 1024);

                UIDelegate.setMaxUIControl(progressBar, contentLenth);//最大的基础上再加40



                //updateUI = delegate
                //{
                //    progressBar.
                //};

                //创建本地文件写入流
                //Stream stream = new FileStream(tempFile, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                long sizeAll = 0;
                int sizeAll_kb = 0;
                float percent_kb = 0.0f;
                double testTime;
                int downLoadSpeed;
                stopWatch.Start();
                while (size > 0)
                {

                    //stream.Write(bArr, 0, size);
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                    sizeAll += size;
                    sizeAll_kb = (int)(sizeAll / 1024);
                    percent_kb = (int)(((float)sizeAll_kb / contentLenth) * 101);
                    testTime = stopWatch.getTotoleSeconds();
                    downLoadSpeed = testTime > 0 ? (int)((float)sizeAll_kb / testTime) : 0;

                    //UIDelegate.writeUIControl(ctr3, string.Format("{0}kB/s ", downLoadSpeed.ToString()));
                    //UIDelegate.writeUIControl(ctr2, string.Format("已下载  {0}% ", percent_kb.ToString()));
                    //UIDelegate.writeUIControl(ctr, string.Format("已下载  {0}  kB", sizeAll_kb.ToString()));
                    //Form1.mainForm.Invoke((EventHandler) delegate{


                    //});

                    //System.Threading.Thread.Sleep(0);
                    //Form1.mainForm.Invoke((EventHandler)delegate {
                    //    Form1.mainForm.label1.Text = string.Format("{0}kB/s ", downLoadSpeed);
                    //    Form1.mainForm.label2.Text = string.Format("已下载  {0}% ", percent_kb);
                    //    Form1.mainForm.label3.Text = string.Format("已下载  {0}  kB", sizeAll_kb);
                    //});
                    AutoWriteProcess.autoWriteForm.label1.Text = string.Format("{0}kB/s ", downLoadSpeed);
                    AutoWriteProcess.autoWriteForm.label1.Update();
                    AutoWriteProcess.autoWriteForm.label2.Text = string.Format("已下载  {0}% ", percent_kb);
                    AutoWriteProcess.autoWriteForm.label2.Update();
                    AutoWriteProcess.autoWriteForm.label3.Text = string.Format("已下载  {0}  kB", sizeAll_kb);
                    AutoWriteProcess.autoWriteForm.label3.Update();

                    UIDelegate.setValueUIControl(progressBar, sizeAll_kb);//在线程中不会出现显示不完的现象
                    Thread.Sleep(0);
                }
                stopWatch.Stop();
                //stream.Close();
                #endregion
                fs.Close();
                responseStream.Close();
                if (System.IO.File.Exists(ProFile))
                {
                    System.IO.File.Delete(ProFile);    //前面删除的是.temp
                }
                try
                {
                    System.IO.File.Move(tempFile, tempFile.Replace(".temp", ""));

                }
                catch (Exception ex)
                {
                    UIDelegate.writeUIControl(ctr, ex.Message);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
    }
}


#region
//先把有问题的代码贴出来吧,

//using System;
//using System.Data;
//using System.Configuration;
//using System.Collections;
//using System.Web;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;
//using System.IO;

//namespace infoPlatClient.NetDisk
//{
//    public partial class downLoad : Com.DRPENG.Common.WebStruct.BaseForm
//    {

//        /// <summary>
//        /// 取得要下载文件的路径
//        /// </summary>
//        private string fileRpath
//        {
//            get
//            {
//                return Request["fileRpath"] == null ? "" : Request["fileRpath"];
//            }
//        }
//        /// <summary>
//        /// 取得要下载文件的名称
//        /// </summary>

//        protected void Page_Load(object sender, EventArgs e)
//        {
//                if (!IsPostBack)
//                this.DownloadFile();
//        }
//        public void DownloadFile()
//        {

//                Response.ClearHeaders();
//                Response.Clear();
//                Response.Expires = 0;
//                Response.Buffer =true;
//                Response.AddHeader("Accept-Language", "zh-tw");
//                string name = System.IO.Path.GetFileName(fileRpath);
//                System.IO.FileStream files = new FileStream(fileRpath, FileMode.Open, FileAccess.Read, FileShare.Read);
//                byte[] byteFile=null;
//                if (files.Length == 0)
//                {
//                    byteFile=new byte[1];
//                }
//                else
//                {
//                    byteFile = new byte[files.Length];
//                }
//                files.Read(byteFile, 0, (int)byteFile.Length);
//                files.Close();

//                Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(name, System.Text.Encoding.UTF8));
//                Response.ContentType = "application/octet-stream;charset=gbk";
//                Response.BinaryWrite(byteFile);
//                Response.End();

//        }
//    }
//}

// 之前一直用这种下载方式，可是有一次用户上传了一个700Mb的文件时报内存溢出的问题，分析了一下原因，用户的内存只有256M，而下载文件时要创建内存流，导致了内存溢出。

//解决方案:1>WriteFile分块下载，就是每次下载指定数量的多件;

//             2>通过超链接的方式;

//             lblDownLoad.Text = "<a href='" + drv["VPath"].ToString() + "'>下载</a>"

//下面是四种实现文件下载的方式:

//using System;
//using System.Data;
//using System.Configuration;
//using System.Web;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;
//using System.IO;

//public partial class _Default : System.Web.UI.Page 
//{
//    protected void Page_Load(object sender, EventArgs e)
//    {

//    }

//    //TransmitFile实现下载
//    protected void Button1_Click(object sender, EventArgs e)
//    {
//        /*
//         微软为Response对象提供了一个新的方法TransmitFile来解决使用Response.BinaryWrite
//         下载超过400mb的文件时导致Aspnet_wp.exe进程回收而无法成功下载的问题。
//         代码如下：
//         */

//        Response.ContentType = "application/x-zip-compressed";
//        Response.AddHeader("Content-Disposition", "attachment;filename=z.zip");
//        string filename = Server.MapPath("DownLoad/z.zip");
//        Response.TransmitFile(filename);
//    }

//    //WriteFile实现下载
//    protected void Button2_Click(object sender, EventArgs e)
//    {
//        /*
//         using System.IO;

//         */

//        string fileName ="asd.txt";//客户端保存的文件名
//        string filePath=Server.MapPath("DownLoad/aaa.txt");//路径

//        FileInfo fileInfo = new FileInfo(filePath);
//        Response.Clear();
//        Response.ClearContent();
//        Response.ClearHeaders();
//        Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
//        Response.AddHeader("Content-Length", fileInfo.Length.ToString());
//        Response.AddHeader("Content-Transfer-Encoding", "binary");
//        Response.ContentType = "application/octet-stream";
//        Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
//        Response.WriteFile(fileInfo.FullName);
//        Response.Flush();
//        Response.End();
//    }

//    //WriteFile分块下载
//    protected void Button3_Click(object sender, EventArgs e)
//    {

//        string fileName = "aaa.txt";//客户端保存的文件名
//        string filePath = Server.MapPath("DownLoad/aaa.txt");//路径

//        System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);

//        if (fileInfo.Exists == true)
//        {
//            const long ChunkSize = 102400;//100K 每次读取文件，只读取100K，这样可以缓解服务器的压力
//            byte[] buffer = new byte[ChunkSize];

//            Response.Clear();
//            System.IO.FileStream iStream = System.IO.File.OpenRead(filePath);
//            long dataLengthToRead = iStream.Length;//获取下载的文件总大小
//            Response.ContentType = "application/octet-stream";
//            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName));
//            while (dataLengthToRead > 0 && Response.IsClientConnected)
//            {
//                int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(ChunkSize));//读取的大小
//                Response.OutputStream.Write(buffer, 0, lengthRead);
//                Response.Flush();
//                dataLengthToRead = dataLengthToRead - lengthRead;
//            }
//            Response.Close();
//        }
//    }

//    //流方式下载
//    protected void Button4_Click(object sender, EventArgs e)
//    {
//        string fileName = "aaa.txt";//客户端保存的文件名
//        string filePath = Server.MapPath("DownLoad/aaa.txt");//路径

//        //以字符流的形式下载文件
//        FileStream fs = new FileStream(filePath, FileMode.Open);
//        byte[] bytes = new byte[(int)fs.Length];
//        fs.Read(bytes, 0, bytes.Length);
//        fs.Close();
//        Response.ContentType = "application/octet-stream";
//        //通知浏览器下载文件而不是打开
//        Response.AddHeader("Content-Disposition", "attachment;  filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
//        Response.BinaryWrite(bytes);
//        Response.Flush();
//        Response.End();

//    }
//}
#endregion