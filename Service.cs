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
        private int isSign;

        private int count = 0;




        private static object obj;

        public Service()
        {
            InitializeComponent();
            InitParameter();
        }

        /// <summary>
        /// 初始化私有变量
        /// </summary>
        private void InitParameter()
        {

            string[] links = ConfigurationManager.AppSettings["TargetLink"].Split(',');
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
        }

        protected override void OnStart(string[] args)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\log.txt", true))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start."+ count);
                        count++;
                    }
                    await Task.Delay(1000);
                }
            });
        }

        protected override void OnStop()
        {
            //var s = HttpTools.GetHttpResult();
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\log.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Stop.");
            }
        }
    }
}
