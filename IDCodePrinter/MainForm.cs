using BaseCommon;
using DBHelp;
using IDCodePrinter;
using LogR;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IDCodePrinter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Logger.Info("程序开始...");
            //DGV.AutoGenerateColumns = false;
            Common._db =  new DBHelp.DBHelp(SoftConfig.SqlConnnection);
            //var sd = Common._db.Queryable<Model.t_weighact>();
            timer1.Start();
            DeletePng();//定时删除超过一天的照片
        }
        private Dictionary<string, PlateConfig> PlateDiction { get; set; }

        class PlateConfig
        {
            public int NewNoIndex { get; set; } = 1;
            //炉号
        }
        
        private Dictionary<string, string> codeByROLLCode { get; set; } = new Dictionary<string, string>();
        private Dictionary<string, string> codeByHEATCode { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// 捆号
        /// </summary>
        private Dictionary<string, int> codeBybundle { get; set; } = new Dictionary<string, int>();


        public int ROLLIndex = 1;
        public int NewNoIndex = 0;
        private DateTime dtStart { get; set; } = DateTime.Now.AddHours(-2);
        private DateTime SoftTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 轧制号
        /// </summary>
        static string NewROLLCode = string.Empty;
        /// <summary>
        /// 炉号
        /// </summary>
        static string NewHEATCode = string.Empty;
        //炉序号
        static int NewHEATNo = 70001;


        private void timer1_Tick(object sender, EventArgs e)
        {
            //自动打印逻辑
            _Start();
            #region 老肖
            //List<Model.t_weighact> t_WeighactsLsit = Common._db.Queryable<Model.t_weighact>()
            //     .Where(r => r.MAT_ACT_WT > 0 && long.Parse(r.INSERT_TIME) >= long.Parse(dtStart.Date.ToString("yyyyMMddHHmmss"))).ToList();//1,
            //                                                                                                       //1,
            //if (t_WeighactsLsit.Count > 0)
            //{
            //    string MaxDate = t_WeighactsLsit.Max(r => r.INSERT_TIME);
            //    if (IsAutoPrint)
            //        dtStart = DateTime.Parse($"{MaxDate.Substring(0, 4)}/{MaxDate.Substring(4, 2)}/{MaxDate.Substring(6, 2)} {MaxDate.Substring(8, 2)}:{MaxDate.Substring(10, 2)}:{MaxDate.Substring(12, 2)} ");
            //}
            //foreach (Model.t_weighact item in t_WeighactsLsit)
            //{
            //    if (t_BaseDatasList.Count(r => r.INSERT_TIME == item.INSERT_TIME) > 0)
            //    {
            //        continue;
            //    }
            //    //合同号更新逻辑轧制号变动
            //    if (!codeByROLLCode.ContainsKey(item.ORDER_NO))
            //    {
            //        if (++NewNoIndex > 99)
            //        {
            //            ROLLIndex++;
            //            NewNoIndex = 0;
            //        }
            //        NewROLLCode = $"C{DateTime.Now.ToString("yy").Substring(1, 1)}1{ROLLIndex.ToString().PadLeft(4, '0')}{ NewNoIndex.ToString().PadLeft(2, '0')}";
            //        //合同号和最新
            //        codeByROLLCode.Add(item.ORDER_NO, NewROLLCode);
            //        codeBybundle.Add(item.ORDER_NO, 7001);
            //    }
            //    else
            //    {
            //        codeBybundle[item.ORDER_NO]++;
            //    }
            //    if (!codeByHEATCode.ContainsKey(item.HEAT_NO))
            //    {
            //        NewHEATCode = $"{DateTime.Now.ToString("yy")}6{NewHEATNo++}";
            //        codeByHEATCode.Add(item.HEAT_NO, NewHEATCode);
            //    }

            //    Model.BaseData baseData = new Model.BaseData();
            //    string weighJson = JsonConvert.SerializeObject(item);
            //    baseData = JsonConvert.DeserializeObject<Model.BaseData>(weighJson);
              //baseData.NewROLLCode = codeByROLLCode[item.ORDER_NO].Trim();
            //    baseData.NewHEAT_NO = codeByHEATCode[item.HEAT_NO].Trim();
            //    baseData.NewHEATNo = codeBybundle[item.ORDER_NO];
            //    baseData.create_Time = DateTime.Parse($"{item.INSERT_TIME.Substring(0, 4)}/{item.INSERT_TIME.Substring(4, 2)}/{item.INSERT_TIME.Substring(6, 2)} {item.INSERT_TIME.Substring(8, 2)}:{item.INSERT_TIME.Substring(10, 2)}:{item.INSERT_TIME.Substring(12, 2)} ");
            //    //baseData.Print_Time = DateTime.Now; 
            //    if (IsAutoPrint)
            //    {
            //        baseData.Print_Time = DateTime.Now;
            //        if (item.MAT_ACT_WT > 0)
            //        {
            //            double _W =Convert.ToDouble(item.MAT_ACT_WT * 1000);
            //            string _str = _W.ToString().Split('.').ToArray()[0];//重量信息
            //            jNPrinter.AutoPNGJNPrint(item.SG_SIGN, baseData.NewHEAT_NO.Trim(), baseData.NewROLLCode.Trim(), baseData.NewHEATNo.ToString(),
            //               "10.0X0", _str, "GB/T 1499.2-2018", DateTime.Now.ToString("yyyyMMdd"),
            //               "0", "0", "XK05-001-00042");
            //        }
            //    }
            //    t_BaseDatasList.Add(baseData);
            //    Thread.Sleep(10);
            //}
            #endregion
            Thread.Sleep(100);
            DGV.DataSource = t_BaseDatasList;
        }
        private string LastDateTime = string.Empty;
        private string LastROLL_PLAN_NO = string.Empty;
        private void _Start()
        {
            try
            {
                if (IsAutoPrint)
                {
                    //Model.t_weighact _Weighact = Common._db.Queryable<Model.t_weighact>().Where(r => r.MAT_ACT_WT > 0&&r.UNIT_CODE==SoftConfig.UnitCode).OrderByDescending(r => r.INSERT_TIME).First();
                    Model.t_weighact _Weighact = Common._db.Queryable<Model.t_weighact>().Where(r => r.MAT_ACT_WT > 0 && r.UNIT_CODE == SoftConfig.UnitCode).OrderByDescending(r => r.PROD_TIME).First();
                    if (_Weighact != null)
                    {
                        Logger.Info("_Start " + "当前的插表时刻：" + _Weighact.PROD_TIME);
                        Logger.Info("_Start " + "上次有效的插表时刻：" + LastDateTime);
                        if (LastDateTime != _Weighact.PROD_TIME)
                        {
                            if (LastROLL_PLAN_NO != _Weighact.ROLL_PLAN_NO)
                            {
                                SoftConfig.RollPlanNo = jNPrinter.GetRollingNo();//轧制号
                                SoftConfig.HeatNo = jNPrinter.GetHeatNo();
                                SoftConfig.BudleNo = "7001";
                            }
                            else
                            {
                                SoftConfig.BudleNo = jNPrinter.GetBudleNo();
                            }
                            LastDateTime = _Weighact.PROD_TIME;
                            Model.BaseData baseData = new Model.BaseData();
                            string weighJson = JsonConvert.SerializeObject(_Weighact);
                            baseData = JsonConvert.DeserializeObject<Model.BaseData>(weighJson);
                            baseData.NewROLLCode = SoftConfig.RollPlanNo;
                            baseData.NewHEAT_NO = SoftConfig.HeatNo;
                            baseData.NewHEATNo = Convert.ToInt32(SoftConfig.BudleNo);
                            baseData.create_Time = DateTime.Parse($"{_Weighact.INSERT_TIME.Substring(0, 4)}/{_Weighact.INSERT_TIME.Substring(4, 2)}/{_Weighact.INSERT_TIME.Substring(6, 2)} {_Weighact.INSERT_TIME.Substring(8, 2)}:{_Weighact.INSERT_TIME.Substring(10, 2)}:{_Weighact.INSERT_TIME.Substring(12, 2)} ");
                            t_BaseDatasList.Add(baseData);
                            if (_Weighact.MAT_ACT_WT > 0)
                            {
                                double _W = Convert.ToDouble(_Weighact.MAT_ACT_WT * 1000);
                                string _str = _W.ToString().Split('.').ToArray()[0];//重量信息
                                //if (_Weighact.MAT_ACT_THICK != null)
                                //    SoftConfig.Specifications = _Weighact.MAT_ACT_THICK.ToString() + ".0X0";
                                jNPrinter.AutoPNGJNPrint(/*_Weighact.SG_SIGN*/ "HRB400E", SoftConfig.HeatNo.Trim(), SoftConfig.RollPlanNo.Trim(), SoftConfig.BudleNo,
                                   SoftConfig.Specifications, _str, "GB/T1499.2-2018", DateTime.Now.ToString("yyyyMMdd"),
                                   "0", "0", "XK05-001-00042");
                            }
                        }
                    }
                }
            }
            catch (Exception EX)
            {

            }
        }
        private BindingList<Model.BaseData> t_BaseDatasList = new BindingList<Model.BaseData>();
        private BindingList<Model.BaseData> New_BaseDatasList = new BindingList<Model.BaseData>();

        //private BindingList<Model.BaseData> t_BaseDataLsit = new BindingList<Model.BaseData>();


        //List<Model.t_weighact> t_WeighactsLsit = Common._db.Queryable<Model.t_weighact>()
        //    .Where(r => long.Parse(r.INSERT_TIME) >= long.Parse(dateTimePicker1.Value.ToString("yyyyMMddHHmmss")) && long.Parse(r.INSERT_TIME) <= long.Parse(dateTimePicker2.Value.ToString("yyyyMMddHHmmss"))).ToList();
        //string weighJson = JsonConvert.SerializeObject(t_WeighactsLsit);
        //t_BaseDataLsit = JsonConvert.DeserializeObject<BindingList<Model.BaseData>>(weighJson);

        JNZGPrint jNPrinter = new JNZGPrint();
        private void btnPrint_Click(object sender, EventArgs e)
        {
            foreach (var item in t_BaseDatasList.Where(r=>r.status))
            {
                item.Print_Time = DateTime.Now;
                double _W = Convert.ToDouble(item.MAT_ACT_WT * 1000);
                string _str = _W.ToString().Split('.').ToArray()[0];//重量信息
                //if (item.MAT_ACT_THICK != null)
                //    SoftConfig.Specifications = item.MAT_ACT_THICK.ToString() + ".0X0";
                jNPrinter.AutoPNGJNPrint(/*item.SG_SIGN*/ "HRB400E", item.NewHEAT_NO.Trim(), item.NewROLLCode.Trim(), item.NewHEATNo.ToString(),
                              SoftConfig.Specifications, _str, "GB/T1499.2-2018".Replace("0","o"), DateTime.Now.ToString("yyyyMMdd"),
                              "0", "0", "XK05-001-00042");
            }
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            DGV.DataSource = t_BaseDatasList;
        }
        bool IsAutoPrint = false;
        private void btnAutoPrint_Click(object sender, EventArgs e)
        {
            if (btnAutoPrint.Text.Equals("自动打印"))
            {
                btnAutoPrint.Text = "停止";
                dtStart = DateTime.Now;
                IsAutoPrint = true;
            }
            else
            {
                btnAutoPrint.Text = "自动打印";
                IsAutoPrint = false;

            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Info("程序关闭...");
        }

        private void DeletePng()
        {
            Thread t = new Thread(StartDelete);
            t.IsBackground = true;
            t.Start();
        }
        private void StartDelete()
        {
            while (true)
            {
                try
                {
                    string Path= Application.StartupPath;
                    DirectoryInfo fdir = new DirectoryInfo(Path);
                    FileInfo[] file = fdir.GetFiles();
                    foreach(FileInfo file1 in file)
                    {
                        if(file1.Extension == ".png")
                        {
                            DateTime NowTime = DateTime.Now;
                            if ((NowTime - file1.CreationTime).TotalDays >= 1)
                            {
                                file1.Delete();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                Thread.Sleep(10000);
            }
        }
    }
}
