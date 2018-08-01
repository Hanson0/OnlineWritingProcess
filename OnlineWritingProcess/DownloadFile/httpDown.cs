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
            string ss = url.Split('=').Last();

            string tempFile = string.Format("{0}{1}.{2}", tempPath, ss, "temp");   //临时文件

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


