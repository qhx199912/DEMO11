using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Citizen_Label_sample_CS
{
    public class SoftConfig
    {
        public static string BudleNo { get; set; } //捆号
        public static string PrinterIP { get; set; } //打印机IP
        public static string PrinterName { get; set; } //打印机名称

        public static string CompanyName { get; set; }//公司名

        public static string DStatus { get; set; }//交付状态
        public static string ProductName { get; set; }//产品名称
        public static string SqlConnnection { get; internal set; }

        public static string HeatNo { get; set; }//炉号

        public static string LastPlanNum { get; set; }//上一个计划号

        public static string RollPlanNo { get; set; }//当前所用轧制号

        public static string UnitCode { get; set; }//机组代码D103,代表A线，D104代表B线
        public static string UsbName { get; set; }//打印机名称
        public static string Specifications { get; set; }//生产规格
        public static int hRatio { get; set; }
        public static int vRatio { get; set; }
        public static int buff = 0;
        public static int BuleStart = 1;
        public static int MaxBult = 45;
        public static bool b;
    }
}
