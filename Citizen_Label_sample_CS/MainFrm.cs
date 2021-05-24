using BaseCommon;
using Citizen_Label_sample_CS.Modle;
using LogR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Model;

namespace Citizen_Label_sample_CS
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
        }
        private Dictionary<string, PlateConfig> PlateDiction { get; set; }

        class PlateConfig
        {
            public int NewNoIndex { get; set; } = 1;
            //炉号
        }
        private void btnQuery_Click(object sender, EventArgs e)
        {
            DGV.DataSource = t_BaseDatasList;
        }
        public int ROLLIndex = 1;
        public int NewNoIndex = 0;
       
        private void btnPrint_Click(object sender, EventArgs e)
        {
            foreach (var item in t_BaseDatasList.Where(r => r.status))
            {
                item.Print_Time = DateTime.Now;
                double _W = Convert.ToDouble(item.MAT_ACT_WT * 1000);
                string _str = _W.ToString().Split('.').ToArray()[0];//重量信息
                //if (item.MAT_ACT_THICK != null)
                //    SoftConfig.Specifications = item.MAT_ACT_THICK.ToString() + ".0X0";
                jNPrinter.AutoPNGJNPrint(/*item.SG_SIGN*/ "HRB400E", item.NewHEAT_NO.Trim(), item.NewROLLCode.Trim(), item.NewHEATNo.ToString(),
                              SoftConfig.Specifications, _str, "GB/T1499.2-2018", DateTime.Now.ToString("yyyyMMdd"),
                              "0", "0", "XK05-001-00042");
            }
        }
        private DateTime dtStart { get; set; } = DateTime.Now.AddHours(-2);
        private DateTime SoftTime { get; set; } = DateTime.Now
            
            ;
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("确定使用该参数?", "警告", messButton);

            if (dr == DialogResult.OK)//如果点击“确定”按钮
            {
                //JNZGPrint._h = (Convert.ToInt32(txtF.Text)-1).ToString();
                //JNZGPrint.Rolling = Convert.ToInt32(txtR.Text);
                //JNZGPrint.Order = txtOrder.Text;
                //SoftConfig.BuleStart = Convert.ToInt32(txtBule.Text) - 1;
                if (SoftConfig.b)
                {
                    SoftConfig.BuleStart = 7000;
                    txtBule.Text = "7001";
                }
                else
                    SoftConfig.BuleStart = Convert.ToInt32(txtBule.Text) - 1;

                SoftConfig.b = false;

                SoftConfig.MaxBult = 999;
                SoftConfig.buff = 0;
                SoftConfig.BudleNo = SoftConfig.BuleStart.ToString();
            }
        }

        private string LastDateTime = string.Empty;
        private string LastROLL_PLAN_NO = string.Empty;
        bool IsAutoPrint = false;
        private void _Start()
        {
            try
            {
                if (IsAutoPrint)
                {
                    Model.t_weighact _Weighact = Common._db.Queryable<Model.t_weighact>().Where(r => r.MAT_ACT_WT > 0 && r.UNIT_CODE == SoftConfig.UnitCode).OrderByDescending(r => r.PROD_TIME).FirstOrDefault();
                    if (_Weighact != null)
                    {
                        Logger.Info("_Start " + "当前的插表时刻：" + _Weighact.PROD_TIME);
                        Logger.Info("_Start " + "上次有效的插表时刻：" + LastDateTime);
                        if (LastDateTime != _Weighact.PROD_TIME)
                        {
                            SoftConfig.buff++;

                            SoftConfig.BudleNo = jNPrinter.GetBudleNo(SoftConfig.buff);
                            txtBule.Text = SoftConfig.BudleNo.ToString();
                            //if (SoftConfig.buff >= SoftConfig.MaxBult+ 2 - SoftConfig.BuleStart)
                            //{
                            //    SoftConfig.BudleNo = "7001";
                            //    SoftConfig.buff = 1;
                            //}

                            //SoftConfig.RollPlanNo = jNPrinter.GetRollingNo(SoftConfig.buff == 1);//轧制号
                            //SoftConfig.HeatNo = jNPrinter.GetHeatNo(SoftConfig.buff == 1);
                            SoftConfig.RollPlanNo = txtR.Text;
                            SoftConfig.HeatNo = txtF.Text;
                            //SoftConfig.BudleNo = "7001";
                            //SoftConfig.BudleNo = jNPrinter.GetBudleNo(SoftConfig.buff);


                            //if (LastROLL_PLAN_NO != _Weighact.ROLL_PLAN_NO)
                            //{
                            //    SoftConfig.RollPlanNo = jNPrinter.GetRollingNo(SoftConfig.buff == 1);//轧制号
                            //    SoftConfig.HeatNo = jNPrinter.GetHeatNo();
                            //    SoftConfig.BudleNo = "7001";
                            //}
                            //else
                            //{
                            //    SoftConfig.BudleNo = jNPrinter.GetBudleNo();
                            //}
                            LastDateTime = _Weighact.PROD_TIME;
                            Model.BaseData baseData = new Model.BaseData();
                            string weighJson = JsonConvert.SerializeObject(_Weighact);
                            baseData = JsonConvert.DeserializeObject<Model.BaseData>(weighJson);
                            baseData.NewROLLCode = SoftConfig.RollPlanNo;
                            baseData.NewHEAT_NO = SoftConfig.HeatNo;
                            baseData.NewHEATNo = Convert.ToInt32(SoftConfig.BudleNo);
                            baseData.create_Time = DateTime.Parse($"{_Weighact.INSERT_TIME.Substring(0, 4)}/{_Weighact.INSERT_TIME.Substring(4, 2)}/{_Weighact.INSERT_TIME.Substring(6, 2)} {_Weighact.INSERT_TIME.Substring(8, 2)}:{_Weighact.INSERT_TIME.Substring(10, 2)}:{_Weighact.INSERT_TIME.Substring(12, 2)} ");
                            t_BaseDatasList.Add(baseData);
                            SoftConfig.Specifications = cmbSpec.Text;
                            //理重不打印
                            string strWeight = Convert.ToDouble(_Weighact.MAT_ACT_WT).ToString();

                            if (_Weighact.MAT_ACT_WT > 0&& Convert.ToDouble(_Weighact.MAT_ACT_WT).ToString().Length>5)
                            {
                                double _W = Convert.ToDouble(_Weighact.MAT_ACT_WT * 1000);
                                string _str = _W.ToString().Split('.').ToArray()[0];//重量信息

                                //jNPrinter.AutoPNGJNPrint(/*_Weighact.SG_SIGN*/ "HRB400E", SoftConfig.HeatNo.Trim(), SoftConfig.RollPlanNo.Trim(), SoftConfig.BudleNo,
                                //   SoftConfig.Specifications, _str, "GB/T1499.2-2018", DateTime.Now.ToString("yyyyMMdd"),
                                //   "0", "0", "XK05-001-00042");

                                jNPrinter.AutoPNGJNPrint(cmbGrade.Text, SoftConfig.HeatNo.Trim(), SoftConfig.RollPlanNo.Trim(), SoftConfig.BudleNo,
                                   SoftConfig.Specifications, _str, "GB/T1499.2-2018", DateTime.Now.ToString("yyyyMMdd"),
                                   "0", "0", "XK05-001-00042");
                            }
                        }
                    }
                }
            }
            catch (Exception EX)
            {
                Logger.Info("方法异常:" + EX.ToString());
            }
        }
        private BindingList<Model. BaseData> t_BaseDatasList = new BindingList<Model.BaseData>();
        private BindingList<Model.BaseData> New_BaseDatasList = new BindingList<Model.BaseData>();
        Form1 jNPrinter = new Form1();
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

        private void MainFrm_Load(object sender, EventArgs e)
        {
            Logger.Info("程序开始...");
            //DGV.AutoGenerateColumns = false;
            Common._db = new DBHelp.DBHelp(SoftConfig.SqlConnnection);
            //var sd = Common._db.Queryable<Model.t_weighact>();
            timer1.Start();
        }

        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Info("程序关闭...");
        }

        private void txtF_TextChanged(object sender, EventArgs e)
        {
            SoftConfig.b = true;
        }

        private void txtBule_TextChanged(object sender, EventArgs e)
        {
            SoftConfig.b = false;
        }
    }
}
