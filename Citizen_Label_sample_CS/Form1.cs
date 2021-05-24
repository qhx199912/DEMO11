using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using com.citizen.sdk.LabelPrint;

namespace Citizen_Label_sample_CS
{
    public partial class Form1 : Form
    {
        delegate void PrintJNDelegate(string Grade, string HeatNo, string RollingNo, string BudleNo, string Size,
                                     string Weight, string Standard, string Date, string DStatus,
                                     string ProductName, string Licence);
        public Form1()
        {

            InitializeComponent();
            InitConfig();
            // ComboBoxColumnで使用するDataTableの作成
            DataTable dataTable1 = new DataTable("ComboBox");
            dataTable1.Columns.Add("Display", typeof(string));
            dataTable1.Columns.Add("Value", typeof(int));
            dataTable1.Rows.Add("NET", LabelConst.CLS_PORT_NET);
            dataTable1.Rows.Add("USB", LabelConst.CLS_PORT_USB);
            dataTable1.Rows.Add("COM", LabelConst.CLS_PORT_COM);
            dataTable1.Rows.Add("LPT", LabelConst.CLS_PORT_LPT);
            dataTable1.Rows.Add("Bluetooth", LabelConst.CLS_PORT_Bluetooth);
            comboBox1.DataSource = dataTable1;
            comboBox1.DisplayMember = "Display";
            comboBox1.ValueMember = "Value";
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 2;
            checkBox2.Checked = true;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            // Create an instance( LabelPrinter class )
            LabelPrinter printer = new LabelPrinter();
            
            // Get Type
            int type = (int)comboBox1.SelectedValue;

            // Get Address
            String addr = txt_Address.Text;

            // Set COMM Properties( COMM only )
            if (LabelConst.CLS_PORT_COM == type)
            {
                printer.SetCommProperties(LabelConst.CLS_COM_BAUDRATE_9600, LabelConst.CLS_COM_PARITY_NONE, LabelConst.CLS_COM_HANDSHAKE_DTRDSR);
            }

            // Connect printer
            int result = printer.Connect(type, addr);
            if (LabelConst.CLS_SUCCESS == result)
            {
                // Printer Check
                result = printer.PrinterCheck();

                // Disconnect
                printer.Disconnect();

                if (LabelConst.CLS_SUCCESS == result)
                {
                    String msg = "PrinterCheck() : Success\n";

                    // CommandInterpreterInAction
                    int status = printer.GetCommandInterpreterInAction();
                    msg += "\n CommandInterpreterInAction -> " + status;

                    // PaperError
                    status = printer.GetPaperError();
                    msg += "\n PaperError -> " + status;

                    // RibbonEnd
                    status = printer.GetRibbonEnd();
                    msg += "\n RibbonEnd -> " + status;

                    // BatchProcessing
                    status = printer.GetBatchProcessing();
                    msg += "\n BatchProcessing -> " + status;

                    // Printing
                    status = printer.GetPrinting();
                    msg += "\n Printing -> " + status;

                    // Pause
                    status = printer.GetPause();
                    msg += "\n Pause -> " + status;

                    // WaitingForPeeling
                    status = printer.GetWaitingForPeeling();
                    msg += "\n WaitingForPeeling -> " + status;

                    // PrintHeadLowTemp
                    status = printer.GetPrintHeadLowTemp();
                    msg += "\n PrintHeadLowTemp -> " + status;

                    // PrintHeadFailure
                    status = printer.GetPrintHeadFailure();
                    msg += "\n PrintHeadFailure -> " + status;

                    // PrintHeadOverheat
                    status = printer.GetPrintHeadOverheat();
                    msg += "\n PrintHeadOverheat -> " + status;

                    // MechanismOpen
                    status = printer.GetMechanismOpen();
                    msg += "\n MechanismOpen -> " + status;

                    // AutoCutterError
                    status = printer.GetAutoCutterError();
                    msg += "\n AutoCutterError -> " + status;

                    // FanMotorError
                    status = printer.GetFanMotorError();
                    msg += "\n FanMotorError -> " + status;

                    // MiscError
                    status = printer.GetMiscError();
                    msg += "\n MiscError -> " + status;

                    
                    // Show Status
                    MessageBox.Show(msg, "Citizen_Label_sample", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                else
                {
                    // Printer Check Error
                    MessageBox.Show("PrinterCheck Error : " + result.ToString(), "Citizen_Label_sample", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Connect Error
                MessageBox.Show("Connect Error : " + result.ToString(), "Citizen_Label_sample", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Create an instance( LabelDesign class )
            LabelDesign design = new LabelDesign();

            // Text
            design.DrawTextPtrFont("Sample Print", LabelConst.CLS_LOCALE_JP, LabelConst.CLS_PRT_FNT_TRIUMVIRATE_B, LabelConst.CLS_RT_NORMAL, 1, 1, LabelConst.CLS_PRT_FNT_SIZE_24, 20, 300);

            // QRCode
            design.DrawQRCode("DrawQRCode", LabelConst.CLS_ENC_CDPG_IBM850, LabelConst.CLS_RT_NORMAL, 4, LabelConst.CLS_QRCODE_EC_LEVEL_H, 20, 220);

            // Rect(fill)
            design.FillRect(20, 150, 350, 40, LabelConst.CLS_SHADED_PTN_11);

            // BarCode
            design.DrawBarCode("0123456789", LabelConst.CLS_BCS_CODE128, LabelConst.CLS_RT_NORMAL, 3, 3, 30, 20, 70, LabelConst.CLS_BCS_TEXT_SHOW);

            
            // Create an instance( LabelPrinter class )
            LabelPrinter printer = new LabelPrinter();

            // Get Type
            int type = (int)comboBox1.SelectedValue;

            // Get Address
            String addr = txt_Address.Text;

            // Set COMM Properties( COMM only )
            if (LabelConst.CLS_PORT_COM == type)
            {
                printer.SetCommProperties(LabelConst.CLS_COM_BAUDRATE_9600, LabelConst.CLS_COM_PARITY_NONE, LabelConst.CLS_COM_HANDSHAKE_DTRDSR);
            }
            int result = printer.Connect(type, addr);
            if (LabelConst.CLS_SUCCESS == result) 
            {
                int printDarkness = printer.GetPrintDarkness();
                if (LabelConst.CLS_PROPERTY_DEFAULT == printDarkness) 
                {
                    printer.SetPrintDarkness(10);
                }
                result = printer.Print(design, 0001);//
                if (LabelConst.CLS_SUCCESS != result)
                {
                    MessageBox.Show("Print Error : " + result.ToString(), "Citizen_Label_sample", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                printer.Disconnect();
            }
            else 
            {
                MessageBox.Show("Connect Error : " + result.ToString(), "Citizen_Label_sample", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void InitConfig()
        {
            try
            {
                SoftConfig.PrinterIP = System.Configuration.ConfigurationManager.AppSettings["PrinterIP"].ToString();
                SoftConfig.PrinterName = System.Configuration.ConfigurationManager.AppSettings["PrinterName"].ToString();
                SoftConfig.CompanyName = System.Configuration.ConfigurationManager.AppSettings["CompanyName"].ToString();
                SoftConfig.BudleNo = System.Configuration.ConfigurationManager.AppSettings["BudleNo"].ToString();
                SoftConfig.DStatus = System.Configuration.ConfigurationManager.AppSettings["DStatus"].ToString();
                SoftConfig.ProductName = System.Configuration.ConfigurationManager.AppSettings["ProductName"].ToString();
                SoftConfig.HeatNo = System.Configuration.ConfigurationManager.AppSettings["HeatNo"].ToString();
                SoftConfig.LastPlanNum = System.Configuration.ConfigurationManager.AppSettings["LastPlanNum"].ToString();
                SoftConfig.UnitCode = System.Configuration.ConfigurationManager.AppSettings["UnitCode"].ToString();//机组代码
                SoftConfig.Specifications = System.Configuration.ConfigurationManager.AppSettings["Specifications"].ToString();
                SoftConfig.SqlConnnection = System.Configuration.ConfigurationManager.AppSettings["SqlConntion"].ToString();
                SoftConfig.UsbName= System.Configuration.ConfigurationManager.AppSettings["UsbName"].ToString();
                SoftConfig.hRatio =Convert.ToInt16( System.Configuration.ConfigurationManager.AppSettings["hRatio"].ToString());
                SoftConfig.vRatio =Convert.ToInt16( System.Configuration.ConfigurationManager.AppSettings["hRatio"].ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private string DataMatrixStr = string.Empty;
        private string BottomData = string.Empty;

        /// </summary>
        /// <param name="Grade">牌号</param>
        /// <param name="HeatNo">炉号</param>
        /// <param name="RollingNo">轧制号</param>
        /// <param name="BudleNo">捆号</param>
        /// <param name="Size">规格</param>
        /// <param name="Weight">重量</param>
        /// <param name="Standard">标准</param>
        /// <param name="Date">日期</param>
        /// <param name="DStatus"></param>
        /// <param name="ProductName">产品名称</param>
        /// <param name="Licence">许可证</param>
        public void AutoPNGJNPrint( string Grade, string HeatNo, string RollingNo, string BudleNo, string Size,
                             string Weight, string Standard, string Date, string DStatus, string ProductName,
                             string Licence)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new PrintJNDelegate(AutoPNGJNPrint), new object[] {  Grade,  HeatNo,  RollingNo,  BudleNo,  Size,
                                                                            Weight,  Standard,  Date,  DStatus,  ProductName,
                                                                            Licence});
                }
                else
                {
                    DStatus = SoftConfig.DStatus;//热轧
                    ProductName = SoftConfig.ProductName;//钢筋混凝
                    BottomData = RollingNo + BudleNo;//轧钢号+捆号
                    //公司名→牌号→规格→轧制号捆号→重量→生产日期→炉号→标准号→许可证号
                    DataMatrixStr = SoftConfig.CompanyName + ";" + Grade + ";" + Size + ";" + BottomData + ";" + Weight + ";" + Date + ";" + HeatNo + ";" + Standard + ";" + Licence;
                    LabelDesign design = new LabelDesign();
                    #region 默认代码
                    // Text (数据,字体名称，是否旋转，水平占比，垂直占比，字体大小，?，x坐标，y坐标，DPI，计量单位)
                    //design.DrawTextPCFont(Grade, "Arial", LabelConst.CLS_RT_NORMAL, 50, 50, 12, LabelConst.CLS_FNT_DEFAULT, 85, 555,LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);//牌号
                    //design.DrawTextPCFont(HeatNo, "Arial", LabelConst.CLS_RT_NORMAL, 50, 50, 12, LabelConst.CLS_FNT_DEFAULT, 85, 555, LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);//炉号
                    //design.DrawTextPCFont(RollingNo, "Arial", LabelConst.CLS_RT_NORMAL, 50, 50, 12, LabelConst.CLS_FNT_DEFAULT, 85, 555, LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);//轧制号
                    //design.DrawTextPCFont(BudleNo, "Arial", LabelConst.CLS_RT_NORMAL, 50, 50, 12, LabelConst.CLS_FNT_DEFAULT, 85, 555, LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);//捆号
                    //design.DrawTextPCFont(Size, "Arial", LabelConst.CLS_RT_NORMAL, 50, 50, 12, LabelConst.CLS_FNT_DEFAULT, 85, 555, LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);//规格
                    //design.DrawTextPCFont(Weight, "Arial", LabelConst.CLS_RT_NORMAL, 50, 50, 12, LabelConst.CLS_FNT_DEFAULT, 85, 555, LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);//重量
                    //design.DrawTextPCFont(Standard, "Arial", LabelConst.CLS_RT_NORMAL, 50, 50, 12, LabelConst.CLS_FNT_DEFAULT, 85, 555, LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);//标准
                    //design.DrawTextPCFont(Date, "Arial", LabelConst.CLS_RT_NORMAL, 50, 50, 12, LabelConst.CLS_FNT_DEFAULT, 85, 555, LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);//日期
                    //design.DrawTextPCFont(DStatus, "Arial", LabelConst.CLS_RT_NORMAL, 50, 50, 12, LabelConst.CLS_FNT_DEFAULT, 85, 555, LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);//交付状态
                    //design.DrawTextPCFont(ProductName, "Arial", LabelConst.CLS_RT_NORMAL, 50, 50, 12, LabelConst.CLS_FNT_DEFAULT, 85, 555, LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);//产品名称
                    //design.DrawTextPCFont(Licence, "Arial", LabelConst.CLS_RT_NORMAL, 50, 50, 12, LabelConst.CLS_FNT_DEFAULT, 85, 555, LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);//许可证
                    //design.DrawTextPCFont(BottomData, "Arial", LabelConst.CLS_RT_NORMAL, 50, 50, 12, LabelConst.CLS_FNT_DEFAULT, 85, 555, LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);//底部数据
                    #endregion
                    if (checkBox1.Checked)
                    {
                        string  select = comboBox2.Text;
                        int angle = 0;
                        switch (select)
                        {
                            case "90":
                                   angle = 2;
                                break;
                            case "180":
                                angle = 3;
                                break;
                            case "270":
                                angle = 4;
                                break;
                            default:
                                break;
                        }
                        //牌号
                        if (!string.IsNullOrEmpty(txt_Grade_x.Text) && !string.IsNullOrEmpty(txt_Grade_y.Text))
                            design.DrawTextPCFont(Grade, "Arial", angle, 100, 100, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_Grade_x.Text),
                                Convert.ToInt16(txt_Grade_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //炉号
                        if (!string.IsNullOrEmpty(txt_HeatNo_x.Text) && !string.IsNullOrEmpty(txt_HeatNo_y.Text))
                            design.DrawTextPCFont(HeatNo, "Arial", angle, 100, 100, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_HeatNo_x.Text),
                                Convert.ToInt16(txt_HeatNo_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //轧制号
                        if (!string.IsNullOrEmpty(txt_RollingNo_x.Text) && !string.IsNullOrEmpty(txt_RollingNo_y.Text))
                            design.DrawTextPCFont(RollingNo, "Arial", angle, 100, 100, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_RollingNo_x.Text),
                                Convert.ToInt16(txt_RollingNo_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //捆号
                        if (!string.IsNullOrEmpty(txt_BudleNo_x.Text) && !string.IsNullOrEmpty(txt_BudleNo_y.Text))
                            design.DrawTextPCFont(BudleNo, "Arial", angle, 100, 100, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_BudleNo_x.Text),
                                Convert.ToInt16(txt_BudleNo_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //规格
                        if (!string.IsNullOrEmpty(txt_Size_x.Text) && !string.IsNullOrEmpty(txt_Size_y.Text))
                            design.DrawTextPCFont(Size, "Arial", angle, 100, 100, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_Size_x.Text),
                                Convert.ToInt16(txt_Size_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //重量
                        if (!string.IsNullOrEmpty(txt_Weight_x.Text) && !string.IsNullOrEmpty(txt_Weight_y.Text))
                            design.DrawTextPCFont(Weight, "Arial", angle, 100, 100, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_Weight_x.Text),
                                Convert.ToInt16(txt_Weight_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //标准
                        if (!string.IsNullOrEmpty(txt_Standard_x.Text) && !string.IsNullOrEmpty(txt_Standard_y.Text))
                            design.DrawTextPCFont(Standard, "Arial", angle, 100, 100, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_Standard_x.Text),
                                Convert.ToInt16(txt_Standard_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //日期Date
                        if (!string.IsNullOrEmpty(txt_Date_x.Text) && !string.IsNullOrEmpty(txt_Date_y.Text))
                            design.DrawTextPCFont(Date, "Arial", angle, 100, 100, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_Date_x.Text),
                                Convert.ToInt16(txt_Date_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //交付状态
                        if (!string.IsNullOrEmpty(txt_DSstatus_x.Text) && !string.IsNullOrEmpty(txt_DSstatus_y.Text))
                            design.DrawTextPCFont(DStatus, "Arial", angle, 100, 100, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_DSstatus_x.Text),
                                Convert.ToInt16(txt_DSstatus_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //产品名称
                        if (!string.IsNullOrEmpty(txt_ProductName_x.Text) && !string.IsNullOrEmpty(txt_ProductName_y.Text))
                            design.DrawTextPCFont(ProductName, "Arial", angle, 100, 100, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_ProductName_x.Text),
                                Convert.ToInt16(txt_ProductName_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //许可证
                        if (!string.IsNullOrEmpty(txt_Licence_x.Text) && !string.IsNullOrEmpty(txt_Licence_y.Text))
                            design.DrawTextPCFont(Licence, "Arial", angle, 100, 100, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_Licence_x.Text),
                                Convert.ToInt16(txt_Licence_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //底部数据
                        if (!string.IsNullOrEmpty(txt_BottomData_x.Text) && !string.IsNullOrEmpty(txt_BottomData_y.Text))
                            design.DrawTextPCFont(BottomData, "Arial", angle, 100, 100, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_BottomData_x.Text),
                                Convert.ToInt16(txt_BottomData_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);

                        //条形码(数据，编码格式，是否旋转，厚宽，窄宽，高度，x坐标，y坐标)

                        if (!string.IsNullOrEmpty(txt_BarCode_x.Text) && !string.IsNullOrEmpty(txt_BarCode_y.Text))
                            design.DrawBarCode(BottomData, LabelConst.CLS_BCS_CODE128, angle, 3, 3, 15,
                                Convert.ToInt16(txt_BarCode_x.Text),
                                Convert.ToInt16(txt_BarCode_y.Text), LabelConst.CLS_BCS_TEXT_HIDE);


                        //二维码(数据，编码格式，是否旋转，放大倍数，等级，x坐标，y坐标)
                        if (!string.IsNullOrEmpty(txt_MatriStr_x.Text) && !string.IsNullOrEmpty(txt_MatriStr_y.Text))
                            design.DrawQRCode(DataMatrixStr, LabelConst.CLS_ENC_CDPG_IBM850, angle, 4,
                                LabelConst.CLS_QRCODE_EC_LEVEL_H,
                                Convert.ToInt16(txt_MatriStr_x.Text), Convert.ToInt16(txt_MatriStr_y.Text));
                    }
                    else
                    {
                        //牌号
                        if (!string.IsNullOrEmpty(txt_Grade_x.Text) && !string.IsNullOrEmpty(txt_Grade_y.Text))
                            design.DrawTextPCFont(Grade, "Arial", LabelConst.CLS_RT_NORMAL, SoftConfig.hRatio, SoftConfig.vRatio, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_Grade_x.Text),
                                Convert.ToInt16(txt_Grade_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //炉号
                        if (!string.IsNullOrEmpty(txt_HeatNo_x.Text) && !string.IsNullOrEmpty(txt_HeatNo_y.Text))
                            design.DrawTextPCFont(HeatNo, "Arial", LabelConst.CLS_RT_NORMAL, SoftConfig.hRatio, SoftConfig.vRatio, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_HeatNo_x.Text),
                                Convert.ToInt16(txt_HeatNo_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //轧制号
                        if (!string.IsNullOrEmpty(txt_RollingNo_x.Text) && !string.IsNullOrEmpty(txt_RollingNo_y.Text))
                            design.DrawTextPCFont(RollingNo, "Arial", LabelConst.CLS_RT_NORMAL, SoftConfig.hRatio, SoftConfig.vRatio, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_RollingNo_x.Text),
                                Convert.ToInt16(txt_RollingNo_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //捆号
                        if (!string.IsNullOrEmpty(txt_BudleNo_x.Text) && !string.IsNullOrEmpty(txt_BudleNo_y.Text))
                            design.DrawTextPCFont(BudleNo, "Arial", LabelConst.CLS_RT_NORMAL, SoftConfig.hRatio, SoftConfig.vRatio, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_BudleNo_x.Text),
                                Convert.ToInt16(txt_BudleNo_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //规格
                        if (!string.IsNullOrEmpty(txt_Size_x.Text) && !string.IsNullOrEmpty(txt_Size_y.Text))
                            design.DrawTextPCFont(Size, "Arial", LabelConst.CLS_RT_NORMAL, SoftConfig.hRatio, SoftConfig.vRatio, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_Size_x.Text),
                                Convert.ToInt16(txt_Size_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //重量
                        if (!string.IsNullOrEmpty(txt_Weight_x.Text) && !string.IsNullOrEmpty(txt_Weight_y.Text))
                            design.DrawTextPCFont(Weight, "Arial", LabelConst.CLS_RT_NORMAL, SoftConfig.hRatio, SoftConfig.vRatio, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_Weight_x.Text),
                                Convert.ToInt16(txt_Weight_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //标准
                        if (!string.IsNullOrEmpty(txt_Standard_x.Text) && !string.IsNullOrEmpty(txt_Standard_y.Text))
                            design.DrawTextPCFont(Standard, "Arial", LabelConst.CLS_RT_NORMAL, SoftConfig.hRatio, SoftConfig.vRatio, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_Standard_x.Text),
                                Convert.ToInt16(txt_Standard_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //日期Date
                        if (!string.IsNullOrEmpty(txt_Date_x.Text) && !string.IsNullOrEmpty(txt_Date_y.Text))
                            design.DrawTextPCFont(Date, "Arial", LabelConst.CLS_RT_NORMAL, SoftConfig.hRatio, SoftConfig.vRatio, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_Date_x.Text),
                                Convert.ToInt16(txt_Date_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //交付状态
                        if (!string.IsNullOrEmpty(txt_DSstatus_x.Text) && !string.IsNullOrEmpty(txt_DSstatus_y.Text))
                            design.DrawTextPCFont(DStatus, "Arial", LabelConst.CLS_RT_NORMAL, SoftConfig.hRatio, SoftConfig.vRatio, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_DSstatus_x.Text),
                                Convert.ToInt16(txt_DSstatus_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //产品名称
                        if (!string.IsNullOrEmpty(txt_ProductName_x.Text) && !string.IsNullOrEmpty(txt_ProductName_y.Text))
                            design.DrawTextPCFont(ProductName, "Arial", LabelConst.CLS_RT_NORMAL, SoftConfig.hRatio, SoftConfig.vRatio, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_ProductName_x.Text),
                                Convert.ToInt16(txt_ProductName_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //许可证
                        if (!string.IsNullOrEmpty(txt_Licence_x.Text) && !string.IsNullOrEmpty(txt_Licence_y.Text))
                            design.DrawTextPCFont(Licence, "Arial", LabelConst.CLS_RT_NORMAL, SoftConfig.hRatio, SoftConfig.vRatio, 12, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_Licence_x.Text),
                                Convert.ToInt16(txt_Licence_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                        //底部数据
                        if (!string.IsNullOrEmpty(txt_BottomData_x.Text) && !string.IsNullOrEmpty(txt_BottomData_y.Text))
                        {
                            if (!string.IsNullOrEmpty(txt_Size.Text))
                            {
                                int size = Convert.ToInt16(txt_Size.Text);
                                design.DrawTextPCFont(BottomData, "Arial", LabelConst.CLS_RT_NORMAL, SoftConfig.hRatio, SoftConfig.vRatio, size, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_BottomData_x.Text),
                                Convert.ToInt16(txt_BottomData_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                            }
                            else
                            {
                                design.DrawTextPCFont(BottomData, "Arial", LabelConst.CLS_RT_NORMAL, SoftConfig.hRatio, SoftConfig.vRatio, 11, LabelConst.CLS_FNT_DEFAULT,
                                Convert.ToInt16(txt_BottomData_x.Text),
                                Convert.ToInt16(txt_BottomData_y.Text), LabelConst.CLS_PRT_RES_300, LabelConst.CLS_UNIT_MILLI);
                            }
                        }


                        //条形码(数据，编码格式，是否旋转，厚宽，窄宽，高度，x坐标，y坐标)

                        if (!string.IsNullOrEmpty(txt_BarCode_x.Text) && !string.IsNullOrEmpty(txt_BarCode_y.Text))
                            design.DrawBarCode(BottomData, LabelConst.CLS_BCS_CODE128, LabelConst.CLS_RT_NORMAL, 3, 3, 15,
                                Convert.ToInt16(txt_BarCode_x.Text),
                                Convert.ToInt16(txt_BarCode_y.Text), LabelConst.CLS_BCS_TEXT_HIDE);


                        //二维码(数据，编码格式，是否旋转，放大倍数，等级，x坐标，y坐标)
                        if (!string.IsNullOrEmpty(txt_MatriStr_x.Text) && !string.IsNullOrEmpty(txt_MatriStr_y.Text))
                            design.DrawQRCode(DataMatrixStr, LabelConst.CLS_ENC_CDPG_IBM850, LabelConst.CLS_RT_NORMAL, 4,
                                LabelConst.CLS_QRCODE_EC_LEVEL_H,
                                Convert.ToInt16(txt_MatriStr_x.Text), Convert.ToInt16(txt_MatriStr_y.Text));
                    }

                    int type = (int)comboBox1.SelectedValue;
                    //string address = txt_Address.Text;
                    LabelPrinter printer = new LabelPrinter();
                    int result = printer.Connect(type, SoftConfig.UsbName);
                    // Set COMM Properties( COMM only )
                    if (LabelConst.CLS_PORT_COM == type)
                    {
                        printer.SetCommProperties(LabelConst.CLS_COM_BAUDRATE_9600, LabelConst.CLS_COM_PARITY_NONE, LabelConst.CLS_COM_HANDSHAKE_DTRDSR);
                    }
                    if (LabelConst.CLS_SUCCESS == result)//
                    {
                        int printDarkness = printer.GetPrintDarkness();
                        if (LabelConst.CLS_PROPERTY_DEFAULT == printDarkness)
                        {
                            if (!string.IsNullOrEmpty(txt_DarkNess.Text))
                            {
                                int darkNess = Convert.ToInt16(txt_DarkNess.Text);
                                printer.SetPrintDarkness(darkNess);
                            }
                            else
                                printer.SetPrintDarkness(10);
                        }
                        if (checkBox2.Checked)
                            result = printer.Print(design, 0002);
                        else
                            result = printer.Print(design, 0001);//测试先打印一张  打印两张
                        if (LabelConst.CLS_SUCCESS != result)
                        {
                            MessageBox.Show("Print Error : " + result.ToString(), "手动打印", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        printer.Disconnect();
                    }
                    else
                        MessageBox.Show("打印机连接失败，请查看原因...错误代码：" + result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("打印出错:" + ex.Message);
            }
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                LabelPrinter printer = new LabelPrinter();
                int type = (int)comboBox1.SelectedValue;
                // Set COMM Properties( COMM only )
                if (LabelConst.CLS_PORT_COM == type)
                {
                    printer.SetCommProperties(LabelConst.CLS_COM_BAUDRATE_9600, LabelConst.CLS_COM_PARITY_NONE, LabelConst.CLS_COM_HANDSHAKE_DTRDSR);
                }
                int result= printer.Connect(type, txt_Address.Text);
                if(LabelConst.CLS_SUCCESS == result)
                {
                    btn_Connect.BackColor = Color.Green;
                    printer.Disconnect();
                }
                else
                    btn_Connect.BackColor = Color.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show("异常:"+ex.ToString());
            }
          
        }

        private void btn_Printer_Click(object sender, EventArgs e)
        {
            AutoPNGJNPrint("HRB400E", "21495036","C17000136", "7014",
                        "8.0X00", "1965", "GB/T 1499.2-2018", DateTime.Now.ToString("yyyyMMdd"),
                        "热轧", "盘螺", "XK05-001-00042");
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void txt_Address_TextChanged(object sender, EventArgs e)
        {

        }

        public string GetBudleNo(int buff)
        {
            string Result = string.Empty;
            try
            {
                Result = (Convert.ToInt32(SoftConfig.BudleNo) + 1).ToString();
                SoftConfig.BudleNo = Result;//原来数量上加一捆
            }
            catch (Exception ex)
            {
                Result = null;
            }
            return Result;
        }
    }
}
