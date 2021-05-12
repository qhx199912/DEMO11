using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class BaseData : t_weighact
    {
        public bool status { get; set; }
        public string NewROLLCode { get; set; }
        public string NewHEAT_NO { get; set; }
        /// <summary>
        /// 新增时间
        /// </summary>
        public DateTime create_Time { get; set; }
        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime Print_Time { get; set; }
       /// <summary>
       /// 捆号
       /// </summary>
        public int NewHEATNo { get; set; }
    }
}
