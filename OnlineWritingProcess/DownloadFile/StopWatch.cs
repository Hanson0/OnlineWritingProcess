using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.DelegateUI;


namespace FactoryAuto
{
    class StopWatch
    {
        private TimeSpan span;
        private DateTime startTime;
        private Timer timer = new Timer();
        public System.Windows.Forms.Control ctr;

        public delegate void UpdateUi(string time);
        //public UpdateUi UpdateUiCallBack;

        private int seq = 0;

        private bool isRun;

        public string TestTime
        {
            get
            {
                return double.Parse(span.TotalSeconds.ToString("#.000")).ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double getTotoleSeconds()
        {
            return span.TotalSeconds;
        }

        public int Seq
        {
            get
            {
                return seq;
            }

            set
            {
                seq = value;
            }
        }

        public void Start()
        {
            startTime = DateTime.Now;
            timer.Enabled = true;
            timer.Interval = 50;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            isRun = true;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isRun)
            {
                span = DateTime.Now - startTime;
                DateTime spanDateTime = new DateTime(span.Ticks);
                //UpdateUiCallBack(spanDateTime.ToString("HH:mm:ss.fff"));
                //UIDelegate.writeUIControl(ctr, spanDateTime.ToString("HH:mm:ss.fff"));

            }
        }

        public void Stop()
        {
            isRun = false;
            timer.Stop();
        }
    }
}
