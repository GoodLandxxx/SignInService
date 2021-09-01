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
        /// <summary>
        /// 轮询时间 单位:秒
        /// </summary>
        private DateTime[] regularTime;


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
        }

        protected override void OnStart(string[] args)
        {

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\log.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "Start.");
            }
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
