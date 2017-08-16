using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Reporting.WinForms;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Printing;
using ByteMatrix = com.google.zxing.common.ByteMatrix;
using EAN8Writer = com.google.zxing.oned.EAN8Writer;
using Code39Writer = com.google.zxing.oned.Code39Writer;
using MultiFormatWriter = com.google.zxing.MultiFormatWriter;
using com.google.zxing;
using DataMatrix.net;
using S7;
using Spire.Pdf;
using LogR;
using BIW.DataConversionLibrary;
using DataStorage;
using System.Configuration;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.IO.Ports;
using System.Net.Sockets;

namespace IDCodePrinter
{
    delegate void printTagDelegate(string packSN);
    delegate bool PLCConnDelegate();
    public partial class IDCodePrinter : Form
    {
        PLC plc = null;
        bool runFlag = false;
        SerialPort SPort = new SerialPort();
        public IDCodePrinter()
        {
            InitializeComponent();
            Logger.Info("IDCodePrinter Start" + Application.ProductVersion);

            Init();

            //NewDBLib dblib = new NewDBLib();
            //DataTable dt = dblib.DBSelect("select top 1 * from [CurveTFCB].[Data].[TighteningData]");
            //dt = null;
        }

        void Init()
        {
            Thread t;
            runFlag = true;

            try
            {
                plc = new PLC(CPU_Type.S7300, ConfigurationManager.AppSettings["S7IP"].ToString(), 0, 2);
                if (plc.Open() == ErrorCode.NoError)
                {
                    t = new Thread(S7Thread);
                    t.IsBackground = true;
                    t.Start();
                }
                else
                    Logger.Info("plc InitError>>" + plc.lastErrorCode.ToString());
                /////////////////////////////////////////////////
                SPort.PortName = ConfigurationManager.AppSettings["ScannerPort"].ToString();
                SPort.BaudRate = int.Parse(ConfigurationManager.AppSettings["ScannerBaudRate"].ToString());
                //SPort.Parity = Parity.None;
                //SPort.StopBits = StopBits.One;
                SPort.ReadTimeout = 2000;
                SPort.WriteTimeout = 2000;
                SPort.Open();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "初始化异常");
            }
        }

        private bool con_plc()
        {
            if (InvokeRequired)
                Invoke(new PLCConnDelegate(con_plc));
            else
            {
                if (!plc.IsConnected)
                {
                    plc.Open();
                    Logger.Info("重新连接PLC");
                }
                if (plc.IsConnected)
                {
                    label6.Text = "已连接";
                    label6.BackColor = Color.Green;
                }
                else
                {
                    label6.Text = "已断开";
                    label6.BackColor = Color.Red;
                }
            }
            return plc.IsConnected;
        }

        private bool con_scanner()
        {
            if (!SPort.IsOpen)
            {
                SPort.Open();
                Logger.Info("重新连接扫码枪");
            }
            return SPort.IsOpen;
        }

        string packSN = "";
        JObject json = null;
        JObject json2 = null;
        byte type = 0;
        byte[] readBuff;
        string DataMatrixStr;
        string[] packSNArr;//请勿清空

        bool lastFlag1 = false;
        bool lastFlag2 = false;
        bool lastFlag3 = false;
        bool lastFlag4 = false;
        bool lastFlag5 = false;
        bool lastFlag6 = false;
        void S7Thread()
        {
            while (runFlag)
            {
                try
                {
                    if (!con_plc() /*|| plc.WriteBytes(DataType.DataBlock, 800, 10, new byte[] { 0x01 }) != ErrorCode.NoError*/)
                    {
                        Logger.Info(plc.lastErrorCode.ToString());
                        Thread.Sleep(2000);
                        continue;
                    }

                    readBuff = plc.ReadBytes(DataType.DataBlock, 160, 0, 14);
                    //Logger.Info("1-" + readBuff[1] + "|" +
                    //    "3-" + readBuff[3] + "|" +
                    //    "9-" + readBuff[9] + "|" +
                    //    "11-" + readBuff[11] + "|" +
                    //    "5-" + readBuff[5] + "|" +
                    //    "7-" + readBuff[7]);
                    if (readBuff != null && readBuff.Length == 14)
                    {
                        step1(readBuff[1] == 1);
                        step2(readBuff[3] == 1);
                        step3(readBuff[9] == 1);
                        step4F(readBuff[11] == 1);
                        step5(readBuff[5] == 1);
                        step6(readBuff[7] == 1);
                    }
                }
                catch(Exception ex)
                {
                    reSetPram();
                    Logger.Debug(ex, "S7Thread");
                    Thread.Sleep(2000);
                }
                Thread.Sleep(500);
            }
        }

        void reSetPram()
        {
            packSN = "";
            json = null;
            json2 = null;
            type = 0;
            readBuff = null;
            DataMatrixStr = "";
        }

        /// <summary>
        /// 1-获取产品型号
        /// </summary>
        void step1(bool flag)
        {
            if (lastFlag1 == flag)
                return;

            if (!flag)
                plc.WriteBytes(DataType.DataBlock, 160, 100, new byte[] { 0x00, 0x00 });
            else
            {
                if (json == null)
                {
                    try
                    {
                        PostDataAPI postDataAPI = new PostDataAPI();
                        string getStr = postDataAPI.HttpPost("http://192.168.20.250:51566/getstatus/packstatus", "{ \"StationID\" : \"A490\" }");
                        Logger.Info("step1->" + getStr);
                        json2 = json = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(getStr);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "step1 Data从服务器请求超时");
                        plc.WriteBytes(DataType.DataBlock, 160, 100, new byte[] { 0x00, 101 });
                    }
                }

                if (json["AGVSN"].ToString() == "-1")
                    type = 113;
                else if (json["Pack1SN"].ToString() == "" && json["Pack2SN"].ToString() == "")
                    type = 112;
                else if (json.ToString() != "" && json["Type"].ToString() != "0")
                {
                    type = byte.Parse(json["Type"].ToString());
                }

                plc.WriteBytes(DataType.DataBlock, 160, 100, new byte[] { 0x00, type });

                Logger.Info("step1");
            }
            lastFlag1 = flag;
        }
        /// <summary>
        /// 2-获取抓取位置号
        /// </summary>
        void step2(bool flag)
        {
            if (lastFlag2 == flag)
                return;

            if (!flag)
                plc.WriteBytes(DataType.DataBlock, 160, 102, new byte[] { 0x00, 0x00 });
            else
            {
                if (json2["Pack1SN"].ToString() != "")
                {
                    if (plc.WriteBytes(DataType.DataBlock, 160, 102, new byte[] { 0x00, 0x01 }) == ErrorCode.NoError)
                    {
                        packSN = json2["Pack1SN"].ToString();
                        json2["Pack1SN"] = "";
                    }
                }
                else if (json2["Pack2SN"].ToString() != "")
                {
                    if (plc.WriteBytes(DataType.DataBlock, 160, 102, new byte[] { 0x00, 0x02 }) == ErrorCode.NoError)
                    {
                        packSN = json2["Pack2SN"].ToString();
                        json2["Pack2SN"] = "";
                    }
                }

                Logger.Info("step2");
            }
            lastFlag2 = flag;
        }
        /// <summary>
        /// 5-请求条码打印
        /// </summary>
        void step5(bool flag)
        {
            if (lastFlag5 == flag)
                return;

            if (!flag)
                plc.WriteBytes(DataType.DataBlock, 160, 104, new byte[] { 0x00, 0x00 });
            else
            {
                string printPackSN = "";

                bool isOK = false;
                if (json["Pack1SN"].ToString() == packSN)
                    isOK = json["Pack1Status"].ToString() == "1" ? true : false;
                if (json["Pack2SN"].ToString() == packSN)
                    isOK = json["Pack2Status"].ToString() == "1" ? true : false;

                if (packSNArr.Length == 2)
                {
                    if (packSNArr[0] == "PHEV")
                        printPackSN = "SVWAP" + packSNArr[1];
                    else if (packSNArr[0] == "BEV")
                        printPackSN = "SVWAB" + packSNArr[1];
                }

                if (printPackSN != "")
                {
                    try
                    {
                        printTag(printPackSN);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "打印机故障");
                        plc.WriteBytes(DataType.DataBlock, 160, 104, new byte[] { 0x00, 180 });

                    }
                    //Thread.Sleep(3000);
                    plc.WriteBytes(DataType.DataBlock, 160, 104, new byte[] { 0x00, 0x01 });
                }
                else
                    DataMatrixStr = "";

                Logger.Info("step5");
            }
            lastFlag5 = flag;
        }
        /// <summary>
        /// 6-请求条码扫描
        /// </summary>
        void step6(bool flag)
        {
            if (lastFlag6 == flag)
                return;

            if (!flag)
                plc.WriteBytes(DataType.DataBlock, 160, 106, new byte[] { 0x00, 0x00 });
            else
            {
                try
                {
                    //扫描二维码 比对是否与打印一致
                    if (con_scanner())
                    {
                        //byte[] SPSendBuff = new byte[100];
                        byte[] SPReadBuff = new byte[100];
                        SPort.Write(new byte[] { 0x2B, 0x0D }, 0, 2);
                        Thread.Sleep(100);
                        int RBuffLen = SPort.Read(SPReadBuff, 0, SPort.BytesToRead);
                        byte[] DMCode = new byte[RBuffLen];
                        Array.Copy(SPReadBuff, DMCode, RBuffLen);
                        string DMStr = ASCIIEncoding.ASCII.GetString(DMCode).Trim();
                        Thread.Sleep(100);
                        SPort.Write(new byte[] { 0x2C, 0x0D }, 0, 2);

                        Logger.Info(DMStr + "->" + DateTime.Now);

                        if (DMStr == DataMatrixStr && DataMatrixStr != "")
                        {
                            plc.WriteBytes(DataType.DataBlock, 160, 106, new byte[] { 0x00, 0x01 });
                            reSetPram();
                        }
                        if (DMStr == "")
                            plc.WriteBytes(DataType.DataBlock, 160, 106, new byte[] { 0x00, 171 });
                    }

                    Logger.Info("step6");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "条码扫描异常");
                    plc.WriteBytes(DataType.DataBlock, 160, 106, new byte[] { 0x00, 170 });
                }
            }
            lastFlag6 = flag;
        }
        /// <summary>
        /// 3-数据解绑
        /// </summary>
        void step3(bool flag)
        {
            if (lastFlag3 == flag)
                return;

            if (!flag)
                plc.WriteBytes(DataType.DataBlock, 160, 108, new byte[] { 0x00, 0x00 });
            else
            {
                //调用接口 数据解绑  ->等站完成统一解绑
                //PostDataAPI postDataAPI = new PostDataAPI();

                //string getStr = postDataAPI.HttpPost("http://192.168.20.249:9997/AGVS/GetStationAGV", "{ \"StationID\" : \"A490\" }");
                //JObject getjson = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(getStr);

                //JObject send = new JObject();
                //send.Add("AGVSN", getjson["AGVID"].ToString());
                //send.Add("PackSN", packSN);
                //getStr = postDataAPI.HttpPost("http://192.168.20.250:51566/ubinding/packandagv", send.ToString());

                packSNArr = packSN.Split('_');//用于打印的电池包编号

                plc.WriteBytes(DataType.DataBlock, 160, 108, new byte[] { 0x00, 0x01 });

                Logger.Info("step3");

                step4T();//发送站完成信号
            }
            lastFlag3 = flag;
        }
        /// <summary>
        /// 4-站完成置1
        /// </summary>
        void step4T()
        {
            if (json2["Pack1SN"].ToString() == "" && json2["Pack2SN"].ToString() == "")
            {
                //统一解绑
                PostDataAPI postDataAPI = new PostDataAPI();
                string getStr = postDataAPI.HttpPost("http://192.168.20.249:9997/AGVS/GetStationAGV", "{ \"StationID\" : \"A490\" }");
                JObject getjson = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(getStr);
                JObject send = new JObject();
                send.Add("AGVSN", getjson["AGVID"].ToString());
                send.Add("PackSN", packSN);
                getStr = postDataAPI.HttpPost("http://192.168.20.250:51566/ubinding/packandagv", send.ToString());
                //getjson = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(getStr);

                //写站完成 AGV放行
                postDataAPI = new PostDataAPI();
                send = new JObject();
                send.Add("StationID", "A490");
                send.Add("Pack1SN", json["Pack1SN"].ToString());
                send.Add("Pack1Status", json["Pack1Status"].ToString());
                send.Add("Pack2SN", json["Pack2SN"].ToString());
                send.Add("Pack2Status", json["Pack2Status"].ToString());
                send.Add("IsReturnRepair", false);
                send.Add("Time", DateTime.Now.ToString("yyyy-MM-dd HH:ss:mm"));
                getStr = postDataAPI.HttpPost("http://192.168.20.250:51566/upload/stationstate", send.ToString());
                //JObject getjson = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(getStr);

                plc.WriteBytes(DataType.DataBlock, 160, 110, new byte[] { 0x00, 0x01 });
            }

            Logger.Info("step4T");
        }
        /// <summary>
        /// 4-站完成置0
        /// </summary>
        void step4F(bool flag)
        {
            if (flag && lastFlag4 != flag)
            {
                plc.WriteBytes(DataType.DataBlock, 160, 110, new byte[] { 0x00, 0x00 });
                Logger.Info("step4F");
            }

            lastFlag4 = flag;
        }

        public static Bitmap Encode_DM(string content, int moduleSize = 5, int margin = 5)
        {
            DmtxImageEncoderOptions opt = new DmtxImageEncoderOptions();
            opt.ModuleSize = moduleSize;
            opt.MarginSize = margin;
            //opt.SizeIdx = DmtxSymbolSize.DmtxSymbol40x40;
            //opt.Scheme = DmtxScheme.DmtxSchemeText;

            DmtxImageEncoder encoder = new DmtxImageEncoder();

            Bitmap bm = encoder.EncodeImage(content, opt);
            return bm;
        }

        public static Bitmap Encode_EAN_8(string content)
        {
            EAN8Writer ean8w = new EAN8Writer();
            ByteMatrix byteMatrix = ean8w.encode(content, BarcodeFormat.EAN_8, 300, 200);
            Bitmap bitmap = ByteMatrixToBitmap(byteMatrix);

            return bitmap;
        }

        public static Bitmap Encode_Code_39(string content)
        {
            Code39Writer ean8w = new Code39Writer();
            
            ByteMatrix byteMatrix = ean8w.encode(content, BarcodeFormat.CODE_39, 500, 38);
            Bitmap bitmap = ByteMatrixToBitmap(byteMatrix);

            return bitmap;
        }

        public static Bitmap Encode_Code_128(string content)
        {
            Code128 _Code = new Code128();
            //_Code.ValueFont = new Font("宋体", 20);
            return _Code.GetCodeImage(content, Code128.Encode.Code128A);
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

        private void IDCodePrinter_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //DateTime datetime = DateTime.Now;
            //LocalReport report = new LocalReport();
            //report.ReportPath = @".\Report\Report1.rdlc";

            //Image img = Encode_Code_39(textBox1.Text + textBox3.Text);//Encode_EAN_8(textBox2.Text);
            //Bitmap imgBit = new Bitmap(img);
            //byte[] imgBytes = BitmapToBytes(imgBit);

            //Image img2 = Encode_DM(textBox4.Text + textBox2.Text + "_______#___#" + datetime.ToString("ddMMyyyyHHmmss") + "#", 5, 10);
            //Bitmap imgBit2 = new Bitmap(img2);
            //byte[] imgBytes2 = BitmapToBytes(imgBit2);

            //ReportParameter ReportParam = new ReportParameter("ReportParameter1", Convert.ToBase64String(imgBytes));
            //ReportParameter ReportParam2 = new ReportParameter("ReportParameter2", Convert.ToBase64String(imgBytes2));
            //ReportParameter ReportParam3 = new ReportParameter("ReportParameter3", textBox4.Text);
            //ReportParameter ReportParam4 = new ReportParameter("ReportParameter4", datetime.ToString("ddMMyyyy"));
            //ReportParameter ReportParam5 = new ReportParameter("ReportParameter5", textBox1.Text);
            //ReportParameter ReportParam6 = new ReportParameter("ReportParameter6", textBox3.Text);
            //ReportParameter ReportParam7 = new ReportParameter("ReportParameter7", textBox2.Text);
            //report.SetParameters(new ReportParameter[] { ReportParam, ReportParam2,
            //        ReportParam3, ReportParam4, ReportParam5, ReportParam6, ReportParam7 });

            //report.Refresh();

            //string deviceInfo = "<DeviceInfo>" +
            //    "  <OutputFormat>EMF</OutputFormat>" +
            //    "  <PageWidth>9.5cm</PageWidth>" +
            //    "  <PageHeight>9.5cm</PageHeight>" +
            //    "  <MarginTop>0.1cm</MarginTop>" +
            //    "  <MarginLeft>0.1cm</MarginLeft>" +
            //    "  <MarginRight>0.1cm</MarginRight>" +
            //    "  <MarginBottom>0.1cm</MarginBottom>" +
            //    "</DeviceInfo>";
            //Warning[] warnings;
            ////report.Render("Image", deviceInfo, CreateStream, out warnings);//生成数据流
            ////Print();//执行打印

            //string[] streamids;
            //string mimeType;
            //string encoding;
            //string extension;

            //byte[] bytes = report.Render(
            //   "PDF", deviceInfo, out mimeType, out encoding, out extension,
            //   out streamids, out warnings);

            //FileStream fs = new FileStream(@"output.pdf", FileMode.Create);
            //fs.Write(bytes, 0, bytes.Length);
            //fs.Close();

            //PdfDocument doc = new PdfDocument();
            //doc.LoadFromFile(@"output.pdf");
            ////doc.PDFStandard.SaveToXPS("output.xps");
            ////doc.PageScaling = PdfPrintPageScaling.ActualSize;
            //doc.PrintDocument.Print();

            printTag(textBox2.Text);
        }

        void printTag(string packSN)
        {
            if (InvokeRequired)
                Invoke(new printTagDelegate(printTag), new object[] { packSN });
            else
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Connect(ConfigurationManager.AppSettings["PrinterIP"].ToString(), 9100);
                s.Send(ASCIIEncoding.ASCII.GetBytes("~JAOA"));//清空打印机队列
                s.Close();

                label7.Text = packSN;
                //NewDBLib dblib = new NewDBLib();
                //DataTable dt;
                DateTime datetime = DateTime.Now;
                LocalReport report = new LocalReport();
                report.ReportPath = @".\Report\Report1.rdlc";

                Image img = Encode_Code_39(textBox1.Text + textBox3.Text);//Encode_EAN_8(packType);
                Bitmap imgBit = new Bitmap(img);
                byte[] imgBytes = BitmapToBytes(imgBit);

                DataMatrixStr = textBox4.Text + packSN + "_______#___#" + datetime.ToString("ddMMyyyyHHmmss") + "#";
                Image img2 = Encode_DM(DataMatrixStr, 5, 10);
                Bitmap imgBit2 = new Bitmap(img2);
                byte[] imgBytes2 = BitmapToBytes(imgBit2);

                ReportParameter ReportParam = new ReportParameter("ReportParameter1", Convert.ToBase64String(imgBytes));
                ReportParameter ReportParam2 = new ReportParameter("ReportParameter2", Convert.ToBase64String(imgBytes2));
                ReportParameter ReportParam3 = new ReportParameter("ReportParameter3", textBox4.Text);
                ReportParameter ReportParam4 = new ReportParameter("ReportParameter4", datetime.ToString("ddMMyyyy"));
                ReportParameter ReportParam5 = new ReportParameter("ReportParameter5", textBox1.Text);
                ReportParameter ReportParam6 = new ReportParameter("ReportParameter6", textBox3.Text);
                ReportParameter ReportParam7 = new ReportParameter("ReportParameter7", packSN);
                report.SetParameters(new ReportParameter[] { ReportParam, ReportParam2,
                    ReportParam3, ReportParam4, ReportParam5, ReportParam6, ReportParam7 });

                report.Refresh();

                string deviceInfo = "<DeviceInfo>" +
                    "  <OutputFormat>EMF</OutputFormat>" +
                    "  <PageWidth>9.5cm</PageWidth>" +
                    "  <PageHeight>9.5cm</PageHeight>" +
                    "  <MarginTop>0.1cm</MarginTop>" +
                    "  <MarginLeft>0.1cm</MarginLeft>" +
                    "  <MarginRight>0.1cm</MarginRight>" +
                    "  <MarginBottom>0.1cm</MarginBottom>" +
                    "</DeviceInfo>";
                Warning[] warnings;
                //report.Render("Image", deviceInfo, CreateStream, out warnings);//生成数据流
                //Print();//执行打印

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

                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(@"output.pdf");
                //doc.PDFStandard.SaveToXPS("output.xps");
                //doc.PageScaling = PdfPrintPageScaling.ActualSize;
                doc.PrintDocument.Print();
            }
        }

        //private List<Stream> m_streams;
        //private Stream CreateStream(string name, string fileNameExtension,
        //    Encoding encoding, string mimeType, bool willSeek)
        //{
        //    m_streams = new List<Stream>();
        //    Stream stream = new MemoryStream();
        //    m_streams.Add(stream);
        //    return stream;
        //}

        //private int m_currentPageIndex;
        //private void Print()
        //{
        //    m_currentPageIndex = 0;
        //    if (m_streams == null || m_streams.Count == 0)
        //        return;
        //    PrintDocument printDoc = new PrintDocument();
        //    //printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";//打印机名称 默认打印机不设
        //    if (!printDoc.PrinterSettings.IsValid)
        //    {
        //        MessageBox.Show("Can't find printer");
        //        return;
        //    }
        //    //printDoc.DefaultPageSettings.PaperSize.Height = 590;
        //    //printDoc.DefaultPageSettings.PaperSize.Width = 472;
        //    printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
        //    printDoc.Print();
        //}

        //private void PrintPage(object sender, PrintPageEventArgs ev)
        //{
        //    m_streams[m_currentPageIndex].Position = 0;
        //    Metafile pageImage = new Metafile(m_streams[m_currentPageIndex]);

        //    //ev.Graphics.PageUnit = GraphicsUnit.Millimeter;//设置图片长度单位
        //    ev.PageSettings.Landscape = false;//指定是否横向打印
        //    //1cm 0.3937008in
        //    //9cm 3.543in 乘100 -> 354
        //    //9.5cm 3.740in 乘100 -> 374
        //    Rectangle destRect = new Rectangle(0, 0, 374, 374);//设置打印区域大小
        //    ev.Graphics.DrawImage(pageImage, destRect);
        //    //ev.Graphics.DrawImage(pageImage, 0, 0);
        //    m_streams[m_currentPageIndex].Close();
        //    m_currentPageIndex++;
        //    ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        //}

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime datetime = DateTime.Now;
                //foreach (string ptname in PrinterSettings.InstalledPrinters)//获取打印机列表
                //{
                //    comboBox1.Items.Add(ptname);
                //    comboBox1.SelectedIndex = 0;
                //}
                //reportViewer1.LocalReport.ReportEmbeddedResource = "IDCodePrinter.Report.Report1.rdlc";
                reportViewer1.LocalReport.ReportPath = @".\Report\Report1.rdlc";

                Image img = Encode_Code_39(textBox1.Text + textBox3.Text); //Encode_EAN_8(textBox2.Text);
                Bitmap imgBit = new Bitmap(img);
                byte[] imgBytes = BitmapToBytes(imgBit);

                Image img2 = Encode_DM(textBox4.Text + textBox2.Text + "_______#___#" + datetime.ToString("ddMMyyyyHHmmss") + "#", 5, 10);
                Bitmap imgBit2 = new Bitmap(img2);
                byte[] imgBytes2 = BitmapToBytes(imgBit2);

                ReportParameter ReportParam = new ReportParameter("ReportParameter1", Convert.ToBase64String(imgBytes));
                ReportParameter ReportParam2 = new ReportParameter("ReportParameter2", Convert.ToBase64String(imgBytes2));
                ReportParameter ReportParam3 = new ReportParameter("ReportParameter3", textBox4.Text);
                ReportParameter ReportParam4 = new ReportParameter("ReportParameter4", datetime.ToString("ddMMyyyy"));
                ReportParameter ReportParam5 = new ReportParameter("ReportParameter5", textBox1.Text);
                ReportParameter ReportParam6 = new ReportParameter("ReportParameter6", textBox3.Text);
                ReportParameter ReportParam7 = new ReportParameter("ReportParameter7", textBox2.Text);

                reportViewer1.LocalReport.SetParameters(new ReportParameter[] { ReportParam, ReportParam2,
                    ReportParam3, ReportParam4, ReportParam5, ReportParam6, ReportParam7 });
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
