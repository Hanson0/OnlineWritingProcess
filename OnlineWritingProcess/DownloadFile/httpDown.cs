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
using Newtonsoft.Json;

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
        public bool HttpDownload(string url, string path,out string msg)
        {
            msg = "";
            string tempPath = path;
            //Directory.CreateDirectory(tempPath);                                      //创建临时文件目录
            string ss = url.Split('=').Last();

            string tempFile = string.Format("{0}{1}.{2}", tempPath, ss, "temp");   //临时文件

            ProFile = tempFile.Replace(".temp",".bin");// string.Format("{0}{1}", tempPath, ss);

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

                string headers = response.Headers.ToString();
                if (!headers.Contains("filename"))
                {
                    using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        string httpRespose = streamReader.ReadToEnd();
                        int start = httpRespose.IndexOf("{");
                        int end = httpRespose.LastIndexOf("}");
                        int length = end - start + 1;
                        httpRespose = httpRespose.Substring(start, length);

                        ResponseInfo responseInfo = JsonConvert.DeserializeObject(httpRespose, typeof(ResponseInfo)) as ResponseInfo;
                        if (responseInfo.code == -1)
                        {
                            msg = responseInfo.msg + "\r\n";
                            return false;
                        }
                    }
                }
                int startIndex = headers.IndexOf('=');
                int endIndex = headers.IndexOf("\r\n");
                string fileName = headers.Substring(startIndex + 1, endIndex - startIndex-1).Replace("\"","");
                
                ProFile = ProFile.Replace(ss+".bin", fileName);
                contentLenth = 20;/*每个bin文件都是20K*/

                UIDelegate.setMaxUIControl(progressBar, contentLenth);//最大的基础上再加40

                //if (response.ContentLength<1)
                //{
                //    using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                //    {
                //        string httpRespose = streamReader.ReadToEnd();
                //        int start = httpRespose.IndexOf("{");
                //        int end = httpRespose.LastIndexOf("}");
                //        int length = end - start + 1;
                //        httpRespose = httpRespose.Substring(start, length);

                //        ResponseInfo responseInfo = JsonConvert.DeserializeObject(httpRespose, typeof(ResponseInfo)) as ResponseInfo;
                //        if (responseInfo.code == -1)
                //        {
                //            msg = responseInfo.msg + "\r\n";
                //            return false;
                //        }
                //    }
                //}


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
                    percent_kb = (int)(((float)sizeAll_kb / contentLenth) * 100);
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
                    if (sizeAll_kb<=contentLenth)
                    {
                        UIDelegate.setValueUIControl(progressBar, sizeAll_kb);//在线程中不会出现显示不完的现象
                    }
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
                    System.IO.File.Move(tempFile, ProFile);

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

    [Serializable]
    class ResponseInfo
    {
        public int code { get; set; }
        public string msg { get; set; }
        public string data { get; set; }
    }

}


