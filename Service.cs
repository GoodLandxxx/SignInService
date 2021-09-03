using SignInService.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SignInService
{
    public partial class Service : ServiceBase
    {
        /// <summary>
        /// 请求的地址数组;
        /// </summary>
        private string[] linkArr;
        /// <summary>
        /// 轮询时间 单位:秒
        /// </summary>
        private int rotationTime;
        private int regularOpenHour;
        private int regularOpenMinute;
        private int regularCloseHour;
        private int regularCloseMinute;

        /// <summary>
        /// 0 <开始>签到，1 <结束> 签到
        /// </summary>
        private int signState;

        private int count = 0;




        private static object obj;

        public Service()
        {
            InitializeComponent();
            WriterSometing("Start SignService");
            InitParameter();
        }

        /// <summary>
        /// 初始化私有变量
        /// </summary>
        private void InitParameter()
        {

            string[] links = ConfigurationManager.AppSettings["TargetLink"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (links.Length > 0)
            {
                linkArr = links;
            }
            if (!int.TryParse(ConfigurationManager.AppSettings["RotationTime"], out rotationTime) || rotationTime == 0)
                rotationTime = 30;

            string[] regularOpenTimes = ConfigurationManager.AppSettings["RegularOpenTimes"].Split(':');
            if (!(regularOpenTimes.Length > 1
                && int.TryParse(regularOpenTimes[0], out regularOpenHour)
                && int.TryParse(regularOpenTimes[1], out regularOpenMinute)))
            {
                regularOpenHour = 08;
                regularOpenMinute = 40;
            }
            string[] regularCloseTimes = ConfigurationManager.AppSettings["RegularCloseTimes"].Split(':');
            if (!(regularCloseTimes.Length > 1
             && int.TryParse(regularCloseTimes[0], out regularCloseHour)
             && int.TryParse(regularCloseTimes[1], out regularCloseMinute)))
            {
                regularCloseHour = 18;
                regularCloseMinute = 10;
            }
            signState = 0;
        }

        protected override void OnStart(string[] args)
        {
            Rotation();
        }

        protected override void OnStop()
        {
            var nowHour = DateTime.Now.Hour;
            var nowMinute = DateTime.Now.Minute;
            var now = CreateDateTime(nowHour, nowMinute);
            var time = CreateDateTime(regularCloseHour, regularCloseMinute);
            if (time.CompareTo(now) <= 0)
            {
                var tools = new HttpTools();
                for (int i = 0; i < linkArr.Length && !string.IsNullOrWhiteSpace(linkArr[i]); i++)
                {
                    if (tools.GetHttpResult(linkArr[i]))
                    {
                        WriterSometing("OnStopfunc Succed SignCloseWork-" + linkArr[i]);
                    }
                    else
                    {
                        WriterSometing("OnStopfunc failed SignCloseWork-" + linkArr[i]);
                    }
                }
            }

            WriterSometing("End SignService");
        }


        private void Rotation()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    WriterSometing(signState.ToString());
                    if (signState == 0)
                        await Rotation(signState, regularOpenHour, regularOpenMinute);
                    if (signState == 1)
                        await Rotation(signState, regularCloseHour, regularCloseMinute);
                    await Task.Delay(rotationTime * 1000);
                }
            });

        }
        private async Task Rotation(int state, int hour, int minute)
        {
            var nowHour = DateTime.Now.Hour;
            var nowMinute = DateTime.Now.Minute;
            var str = state == 0 ? "OpenWork" : "CloseWork";
            var now = CreateDateTime(nowHour, nowMinute);
            var time = CreateDateTime(hour, minute);
            if (time.CompareTo(now) <= 0)
            {
                var tools = new HttpTools();
                int k = 0;
                for (int i = 0; i < linkArr.Length && !string.IsNullOrWhiteSpace(linkArr[i]); i++)
                {

                    await Task.Delay(new Random().Next(0, 10) * 1000);
                    if (tools.GetHttpResult(linkArr[i]))
                    {
                        k++;
                        WriterSometing(str + ",Succed, " + linkArr[i]);
                       
                    }
                    else
                    {
                        WriterSometing(str + ",Failed," + linkArr[i]);
                    }
                }
                if (linkArr.Length == k)
                {
                    signState++;
                }
            }
            return;
        }
        private void WriterSometing(string str)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\log.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-----" + str);
                count++;
            }
        }
        private DateTime CreateDateTime(int hour, int minute)
        {

            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
        }
    }
}
