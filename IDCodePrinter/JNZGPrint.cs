using com.google.zxing;
using com.google.zxing.common;
using com.google.zxing.oned;
using DataMatrix.net;
using Microsoft.Reporting.WinForms;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using ByteMatrix = com.google.zxing.common.ByteMatrix;
using EAN8Writer = com.google.zxing.oned.EAN8Writer;
using Code39Writer = com.google.zxing.oned.Code39Writer;
using System.Windows.Forms;
using ThoughtWorks.QRCode.Codec;
using System.Drawing.Printing;
using System.Threading;

namespace IDCodePrinter
{
    public partial class JNZGPrint : Form
    {
        delegate void PrintJNDelegate(string Grade, string HeatNo, string RollingNo, string BudleNo, string Size,
                                      string Weight, string Standard, string Date, string DStatus, 
                                      string ProductName, string Licence);
        public JNZGPrint()
        {
            pd.PrintPage += PrintDocument_PrintPage;
            InitializeComponent();
            InitConfig();
            comboBox1.SelectedIndex = 2;
            checkBox1.Checked = true;
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
                SoftConfig.Specifications= System.Configuration.ConfigurationManager.AppSettings["Specifications"].ToString();
                SoftConfig.SqlConnnection = System.Configuration.ConfigurationManager.AppSettings["SqlConntion"].ToString(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string printerName=string.Empty;
        private string DataMatrixStr=string.Empty;
        private string BottomData = string.Empty;
        private void AutoJNPrint(string Grade, string HeatNo, string RollingNo, string BudleNo, string Size,
                                 string Weight, string Standard, string Date, string DStatus, string ProductName, 
                
                                 string Licence)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new PrintJNDelegate(AutoJNPrint), new object[] {  Grade,  HeatNo,  RollingNo,  BudleNo,  Size,
                                                                            Weight,  Standard,  Date,  DStatus,  ProductName,  
                                                                            Licence});
                }
                else
                {
                    BottomData = RollingNo + BudleNo;//轧钢号+捆号
                    //公司名→牌号→规格→轧制号捆号→重量→生产日期→炉号→标准号→许可证号
                    DataMatrixStr = SoftConfig.CompanyName+";"+Grade +";"+Size+";"+ BottomData+";"+Weight+";"+Date+";"+HeatNo+";"+Standard+";"+Licence;
                    LocalReport report = new LocalReport();
                    DStatus = SoftConfig.DStatus;//热轧
                    ProductName = SoftConfig.ProductName;//钢筋混凝
                    report.ReportPath = @".\Report\Report2.rdlc";
                    //Image img = Encode_Code_39("wwww"+DateTime.Now.ToString("yyyyMMddHHmmss"));//SVWPEA1AB5A001B5A0000001
                    //Image img = Encode_Code_39(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Image img = Encode_Code_128(DataMatrixStr);
                    Bitmap imgBit = new Bitmap(img);
                    byte[] imgBytes = BitmapToBytes(imgBit);


                    //Image img2 = Encode_DM(DataMatrixStr, 5, 5);
                    Image img2 = GetDimensionalCode(DataMatrixStr);
                    Bitmap imgBit2 = new Bitmap(img2);
                    byte[] imgBytes2 = BitmapToBytes(imgBit2);
                    ReportParameter ReportParam = new ReportParameter("ReportParameter1", Convert.ToBase64String(imgBytes) );//二维码
                    ReportParameter ReportParam2 = new ReportParameter("ReportParameter2", Convert.ToBase64String(imgBytes2));//条形码
                    ReportParameter ReportParam3 = new ReportParameter("ReportParameter3", "3");
                    ReportParameter ReportParam4 = new ReportParameter("ReportParameter4", Grade );
                    ReportParameter ReportParam5 = new ReportParameter("ReportParameter5", HeatNo );
                    ReportParameter ReportParam6 = new ReportParameter("ReportParameter6", RollingNo );
                    ReportParameter ReportParam7 = new ReportParameter("ReportParameter7", BudleNo );
                    ReportParameter ReportParam8 = new ReportParameter("ReportParameter8", Size );
                    ReportParameter ReportParam9 = new ReportParameter("ReportParameter9", Weight );
                    ReportParameter ReportParam10 = new ReportParameter("ReportParameter10", Standard );
                    ReportParameter ReportParam11 = new ReportParameter("ReportParameter11", Date );
                    ReportParameter ReportParam12 = new ReportParameter("ReportParameter12", DStatus );
                    ReportParameter ReportParam13 = new ReportParameter("ReportParameter13", ProductName );
                    ReportParameter ReportParam14 = new ReportParameter("ReportParameter14", Licence );
                    ReportParameter ReportParam15 = new ReportParameter("ReportParameter15", BottomData);
                    ReportParameter ReportParam16 = new ReportParameter("ReportParameter16", "16");
                    report.SetParameters(new ReportParameter[] { ReportParam, ReportParam2,
                    ReportParam3, ReportParam4, ReportParam5, ReportParam6, ReportParam7, ReportParam8,
                    ReportParam9, ReportParam10, ReportParam11, ReportParam12, ReportParam13, ReportParam14,
                    ReportParam15, ReportParam16 });

                    report.Refresh();

                    string deviceInfo = "<DeviceInfo>" +
                        "  <OutputFormat>EMF</OutputFormat>" +
                        "  <PageWidth>11.5cm</PageWidth>" +
                        "  <PageHeight>7.5cm</PageHeight>" +
                        "  <MarginTop>0.1cm</MarginTop>" +
                        "  <MarginLeft>0.1cm</MarginLeft>" +
                        "  <MarginRight>0.1cm</MarginRight>" +
                        "  <MarginBottom>0.1cm</MarginBottom>" +
                        "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;

                    byte[] bytes = report.Render(
                       "PDF", deviceInfo, out mimeType, out encoding, out extension,
                       out streamids, out warnings);

                    FileStream fs = new FileStream(@"output.pdf", FileMode.Create);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                    ///设置旋转打印

                    if (checkBox1.Checked)
                    {
                        PdfDocument doc = new PdfDocument();
                        doc.LoadFromFile(@"output.pdf");

                        PdfPageBase page = doc.Pages[0];

                        int rotation = (int)page.Rotation;
                        string str = comboBox1.Text;
                        switch (str)
                        {
                            case "90":
                                {
                                    rotation += (int)PdfPageRotateAngle.RotateAngle90;
                                }
                                break;
                            case "180":
                                {
                                    rotation += (int)PdfPageRotateAngle.RotateAngle180;
                                }
                                break;
                            case "270":
                                {
                                    rotation += (int)PdfPageRotateAngle.RotateAngle270;
                                }
                                break;
                            default:
                                {
                                    break;
                                }
                        }
                        //rotation += (int)PdfPageRotateAngle.RotateAngle180;

                        page.Rotation = (PdfPageRotateAngle)rotation;

                        doc.SaveToFile(@"output.pdf");
                    }
                    PdfDocument doc1 = new PdfDocument();
                    doc1.LoadFromFile(@"output.pdf");

                    if (printerName != "")
                        doc1.PrintDocument.PrinterSettings.PrinterName = printerName;
                    doc1.PrintDocument.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("打印出错:"+ex.Message);
            }
        }

        /// <summary>
        /// 
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
        public void AutoPNGJNPrint(string Grade, string HeatNo, string RollingNo, string BudleNo, string Size,
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
                    BottomData = RollingNo + BudleNo;//轧钢号+捆号
                    //公司名→牌号→规格→轧制号捆号→重量→生产日期→炉号→标准号→许可证号
                    DataMatrixStr = SoftConfig.CompanyName + ";" + Grade + ";" + Size + ";" + BottomData + ";" + Weight + ";" + Date + ";" + HeatNo + ";" + Standard + ";" + Licence;
                    LocalReport report = new LocalReport();
                    DStatus = SoftConfig.DStatus;//热轧
                    ProductName = SoftConfig.ProductName;//钢筋混凝
                    report.ReportPath = @".\Report\Report3.rdlc";
                    //Image img = Encode_Code_128(DataMatrixStr);
                    Image img = Encode_Code_39(BottomData);
                    Bitmap imgBit = new Bitmap(img);
                    byte[] imgBytes = BitmapToBytes(imgBit);


                    //Image img2 = Encode_DM(DataMatrixStr, 5, 5);
                    Image img2 = GetDimensionalCode(DataMatrixStr);
                    Bitmap imgBit2 = new Bitmap(img2);
                    byte[] imgBytes2 = BitmapToBytes(imgBit2);
                    ReportParameter ReportParam = new ReportParameter("ReportParameter1", Convert.ToBase64String(imgBytes));//二维码
                    ReportParameter ReportParam2 = new ReportParameter("ReportParameter2", Convert.ToBase64String(imgBytes2));//条形码
                    ReportParameter ReportParam3 = new ReportParameter("ReportParameter3", "3");
                    ReportParameter ReportParam4 = new ReportParameter("ReportParameter4", Grade);
                    ReportParameter ReportParam5 = new ReportParameter("ReportParameter5", HeatNo);
                    ReportParameter ReportParam6 = new ReportParameter("ReportParameter6", RollingNo);
                    ReportParameter ReportParam7 = new ReportParameter("ReportParameter7", BudleNo);
                    ReportParameter ReportParam8 = new ReportParameter("ReportParameter8", Size);
                    ReportParameter ReportParam9 = new ReportParameter("ReportParameter9", Weight);
                    ReportParameter ReportParam10 = new ReportParameter("ReportParameter10", Standard);
                    ReportParameter ReportParam11 = new ReportParameter("ReportParameter11", Date);
                    ReportParameter ReportParam12 = new ReportParameter("ReportParameter12", DStatus);
                    ReportParameter ReportParam13 = new ReportParameter("ReportParameter13", ProductName);
                    ReportParameter ReportParam14 = new ReportParameter("ReportParameter14", Licence);
                    ReportParameter ReportParam15 = new ReportParameter("ReportParameter15", BottomData);
                    ReportParameter ReportParam16 = new ReportParameter("ReportParameter16", "16");
                    report.SetParameters(new ReportParameter[] { ReportParam, ReportParam2,
                    ReportParam3, ReportParam4, ReportParam5, ReportParam6, ReportParam7, ReportParam8,
                    ReportParam9, ReportParam10, ReportParam11, ReportParam12, ReportParam13, ReportParam14,
                    ReportParam15, ReportParam16 });

                    report.Refresh();

                    string deviceInfo = "<DeviceInfo>" +
                        "  <OutputFormat>PNG</OutputFormat>" +
                        "  <PageWidth>11m</PageWidth>" +
                        "  <PageHeight>7m</PageHeight>" +
                        "  <MarginTop>0.1cm</MarginTop>" +
                        "  <MarginLeft>0.1cm</MarginLeft>" +
                        "  <MarginRight>0.1cm</MarginRight>" +
                        "  <MarginBottom>0.1cm</MarginBottom>" +
                        "</DeviceInfo>";

                    byte[] byteImage = report.Render("Image", deviceInfo);
                    Guid MEG = Guid.NewGuid();
                    string Uid=MEG.ToString();
                    FileStream fs = new FileStream(MEG.ToString() + ".png", FileMode.Create);
                    fs.Write(byteImage, 0, byteImage.Length);
                    fs.Close();
                    printImage = Image.FromFile(MEG.ToString() + ".png");
                    if (checkBox1.Checked)
                    {
                        string str = comboBox1.Text;
                        switch (str)
                        {
                            case "90":
                                {
                                    printImage= RotateImg2(printImage,90);
                                }
                                break;
                            case "180":
                                {
                                    printImage = RotateImg2(printImage, 180);
                                }
                                break;
                            case "270":
                                {
                                    printImage = RotateImg2(printImage, 270);
                                }
                                break;
                            default:
                                {
                                    break;
                                }
                        }
                    }
                    pd.Print();
                    Thread.Sleep(100);
                    pd.Print();//钢厂要求每次都打印两张
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("打印出错:" + ex.Message);
            }
        }
        public static Bitmap Encode_Code_39(string content)
        {
            Code39Writer ean8w = new Code39Writer();
            ByteMatrix byteMatrix = ean8w.encode(content, BarcodeFormat.CODE_39, 300, 38);
            Bitmap bitmap = ByteMatrixToBitmap(byteMatrix);
            return bitmap;
        }
        public static Bitmap ByteMatrixToBitmap(ByteMatrix matrix)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            Bitmap bmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bmap.SetPixel(x, y, matrix.get_Renamed(x, y) != -1 ? ColorTranslator.FromHtml("0xFF000000") : ColorTranslator.FromHtml("0xFFFFFFFF"));
                }
            }
            return bmap;
        }
        private byte[] BitmapToBytes(Bitmap Bitmap)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();
                Bitmap.Save(ms, ImageFormat.Gif);
                byte[] byteImage = new Byte[ms.Length];
                byteImage = ms.ToArray();
                return byteImage;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
            }
        }

        /// <summary>
        /// 根据链接获取二维码
        /// </summary>
        /// <param name="link">链接</param>
        /// <returns>返回二维码图片</returns>
        private Bitmap GetDimensionalCode(string link)
        {
            Bitmap bmp = null;
            try
            {
                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                qrCodeEncoder.QRCodeScale = 4;
                qrCodeEncoder.QRCodeVersion = 7;
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                bmp = qrCodeEncoder.Encode(link);
            }
            catch (Exception ex)
            {

            }
            return bmp;
        }

        private  static Bitmap Encode_Code_128(string content)
        {
            Code128 _Code = new Code128();
            _Code.ValueFont = new Font("宋体", 20);
            return _Code.GetCodeImage(content, Code128.Encode.Code128A);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //AutoJNPrint("HRB400E", "21495036", "C17000136", "7014",
            //            "8.0X00", "1965", "GB/T 1499.2-2018", "20210301",
            //            "0", "0", "XK05-001-00042");
            //TestPrint();
            try
            {
                if (!string.IsNullOrEmpty(txt_BudleNo.Text.ToString()) &&
                    !string.IsNullOrEmpty(txt_HeatNo.Text.ToString()) &&
                    !string.IsNullOrEmpty(txt_RollingNo.Text.ToString()) &&
                    !string.IsNullOrEmpty(txt_Weight.Text.ToString())) 
                {
                    AutoJNPrint("HRB400E", txt_HeatNo.Text.ToString(), txt_RollingNo.Text.ToString(), txt_BudleNo.Text.ToString(),
                       "8.0X00", txt_Weight.Text.ToString(), "GB/T 1499.2-2018", DateTime.Now.ToString("yyyyMMdd"),
                       "0", "0", "XK05-001-00042");
                }
                else
                {
                    MessageBox.Show("请输入正确的格式!");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 获取轧制号
        /// </summary>
        /// <returns>轧制号</returns>
        static  string line = "C";//南钢产线代码
        static  string Product = "1";//南钢给晋南生产线的代码
        static  string Order = "0001";
        
        static int Rolling = 1;
        public  string GetRollingNo()
        {
            string Result = string.Empty;
            try
            {
                string Year = DateTime.Now.Year.ToString().Substring(3, 1);
                if (Rolling > 99)
                {
                    Rolling = 1;
                    int _order = Convert.ToInt32(Order) + 1;
                    Order = _order.ToString().PadLeft(4,'0');
                }
                else
                {
                    Rolling += 1;
                }
                Result = line + Year + Product + Order + Rolling.ToString().PadLeft(2,'0');
                SoftConfig.RollPlanNo = Result;
            }
            catch (Exception ex)
            {
                Result = null;
            }
            return Result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>炉号</returns>
        public   string GetHeatNo()
        {
            string Result = string.Empty;
            try
            {
                string Data = string.Empty;
                string Year = DateTime.Now.Year.ToString().Substring(2, 2);
                string NG = "6";
                if (Convert.ToInt32(SoftConfig.HeatNo) > 99999)
                    SoftConfig.HeatNo = "97001";
                SoftConfig.HeatNo = (Convert.ToInt32(SoftConfig.HeatNo) + 1).ToString();//每使用一次加一个
                string Heat = SoftConfig.HeatNo;
                Result = Year + NG + Heat;
            }
            catch (Exception ex)
            {
                Result = "00000000";
            }
            return Result;
        }
        /// <summary>
        /// 获取捆号
        /// </summary>
        /// <param name="Plan计划号"></param>
        /// <returns>捆号</returns>
        public   string GetBudleNo(/*string Plan*/)
        {
            string Result = string.Empty;
            try
            {
                //if (Plan != SoftConfig.LastPlanNum)
                //{
                //    SoftConfig.BudleNo = "7001";
                //    Result = SoftConfig.BudleNo;
                //}
                //else
                //{
                Result = (Convert.ToInt32(SoftConfig.BudleNo) + 1).ToString();
                SoftConfig.BudleNo = Result;//原来数量上加一捆
                //}
                //SoftConfig.LastPlanNum = Plan;
            }
            catch (Exception ex)
            {
                Result = null;
            }
            return Result;
        }
        Image printImage;
        private PrintDocument pd = new PrintDocument();
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Rectangle m = e.MarginBounds;
            if ((double)printImage.Width / (double)printImage.Height > (double)m.Width / (double)m.Height) // image is wider
            {
                m.Height = (int)((double)printImage.Height / (double)printImage.Width * (double)m.Width);
            }
            else
            {
                m.Width = (int)((double)printImage.Width / (double)printImage.Height * (double)m.Height);
            }
            //e.Graphics.DrawImage(printImage, 10, 10, 300, 150);//80*40
            int x =Convert.ToInt32( txt_x.Text);
            int y = Convert.ToInt32(txt_y.Text);
            int k = Convert.ToInt32(txt_chang.Text);
            int l = Convert.ToInt32(txt_kuan.Text);
            e.Graphics.DrawImage(printImage, x, y, k, l);//110*70
            e.HasMorePages = false;
            //e.Graphics.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_BudleNo.Text.ToString()) &&
                    !string.IsNullOrEmpty(txt_HeatNo.Text.ToString()) &&
                    !string.IsNullOrEmpty(txt_RollingNo.Text.ToString()) &&
                    !string.IsNullOrEmpty(txt_Weight.Text.ToString()))
                {
                    AutoPNGJNPrint("HRB400E", txt_HeatNo.Text.ToString(), txt_RollingNo.Text.ToString(), txt_BudleNo.Text.ToString(),
                       "8.0X00", txt_Weight.Text.ToString(), "GB/T 1499.2-2018", DateTime.Now.ToString("yyyyMMdd"),
                       "0", "0", "XK05-001-00042");
                }
                else
                {
                    MessageBox.Show("请输入正确的格式!");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 第二种方法
        /// </summary>
        /// <param name="b"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Image RotateImg2(Image b, float angle)
        {
            angle = angle % 360;            //弧度转换
            double radian = angle * Math.PI / 180.0;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);
            //原图的宽和高
            int w = b.Width;
            int h = b.Height;
            int W = (int)(Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin)));
            int H = (int)(Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos)));
            //目标位图
            Image dsImage = new Bitmap(W, H);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dsImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //计算偏移量
            Point Offset = new Point((W - w) / 2, (H - h) / 2);
            //构造图像显示区域：让图像的中心与窗口的中心点一致
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, w, h);
            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(360 - angle);
            //恢复图像在水平和垂直方向的平移
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(b, rect);
            //重至绘图的所有变换
            g.ResetTransform();
            g.Save();
            g.Dispose();
            //dsImage.Save("yuancd.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return dsImage;
        }
    }
}
