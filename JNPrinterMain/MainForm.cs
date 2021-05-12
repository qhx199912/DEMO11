using BaseCommon;
using DBHelp;
using IDCodePrinter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JNPrinterMain
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Common._db =  new DBHelp.DBHelp();
            //var sd = Common._db.Queryable<Model.t_weighact>();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
         



        }
        private List<Model.t_weighact> t_WeighactsLsit = new List<Model.t_weighact>();
        private void btnStart_Click(object sender, EventArgs e)
        {
            t_WeighactsLsit = Common._db.Queryable<Model.t_weighact>()
                .Where(r=> long.Parse(r.INSERT_TIME) >= long.Parse(dateTimePicker1.Value.ToString("yyyyMMddHHmmss")) && long.Parse(r.INSERT_TIME) <= long.Parse(dateTimePicker2.Value.ToString("yyyyMMddHHmmss"))).ToList();
            DGV.DataSource = t_WeighactsLsit;
        }
        JNZGPrint jNPrinter = new JNZGPrint();
        private void btnPrint_Click(object sender, EventArgs e)
        {
            foreach (var item in t_WeighactsLsit.Where(r => r.status))
            {
                jNPrinter.AutoPNGJNPrint(item.SG_SIGN, item.HEAT_NO, item.ROLL_PLAN_NO.ToString(),  "",
                       item.SPEC, item.MAT_ACT_WT.ToString(), item.SG_STD, DateTime.Now.ToString("yyyyMMdd"),
                       "0", "", "XK05-001-00042");
            }    

        }
    }
}
