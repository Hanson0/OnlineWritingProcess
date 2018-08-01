using System.Windows.Forms;

namespace System.DelegateUI
{
    public static class UIDelegate
    {
        //------------------
        private delegate void myDelegateW(Control str, string s);
        private delegate string myDelegateR(Control str);

        private delegate void myDelegateSV(Control str, int value);
        private delegate void myDelegateSM(Control str, int max);
        //------------------

        /// <summary>
        /// 可以实现对带有Text属性的标准控件进行写操作
        /// </summary>
        /// <param name="ctr"></param>
        /// <param name="str"></param>
        public static void writeUIControl(Control ctr, string str)
        {
            if (ctr.InvokeRequired)
            {
                myDelegateW mydelegate = new myDelegateW(writeUIControl);
                ctr.Invoke(mydelegate, new object[] { ctr, str });
            }
            else
            {
                try
                {
                    ctr.Text = str;
                }
                catch (Exception ex)
                {
                    string ss = ex.Message;

                }
            }
        }
        public static string readUIControl(Control ctr)
        {
            string ret = string.Empty;
            if (ctr.InvokeRequired)
            {
                myDelegateR mydelegate = new myDelegateR(readUIControl); //递归
                ctr.Invoke(mydelegate, new object[] { ctr });
            }
            else
            {
                try
                {
                    ret = ctr.Text;
                }
                catch { }
            }
            return ret;
        }

        public static void setValueUIControl(Control ctr, int value)
        {
            if (ctr.InvokeRequired)
            {
                myDelegateSV mydelegate = new myDelegateSV(setValueUIControl);
                ctr.Invoke(mydelegate, new object[] { ctr, value });
            }
            else
            {
                try
                {
                    (ctr as ProgressBar).Value = value;
                    //(ctr as ProgressBar).Update();

                }
                catch { }
            }
        }
        public static void setMaxUIControl(Control ctr, int max)
        {
            if (ctr.InvokeRequired)
            {
                myDelegateSM mydelegate = new myDelegateSM(setMaxUIControl);
                ctr.Invoke(mydelegate, new object[] { ctr, max });
            }
            else
            {
                try
                {
                   // (ctr as ProgressBar).Maximum = max;
                    ProgressBar control = ctr as ProgressBar;
                    control.Maximum = max;

                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
        }


        public static void writeUI(Control ctr, string str)
        {
            if (ctr.InvokeRequired)
            {
                ctr.Invoke((EventHandler)delegate
                {
                    ctr.Text = str;
                });
            }
            else
            {
                ctr.Text = str;
            }
        }
        public static string readUI(Control ctr)
        {
            //if (ctr.InvokeRequired)
            //{
            //    ctr.Invoke((EventHandler)delegate
            //    {
            //         return ctr.Text ;
            //    });
            //}
            //else
            //{
            //     return ctr.Text ;
            //}
            return string.Empty;
        }
    }
}


