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

namespace IDCodePrinter
{
    public partial class IDCodePrinter : Form
    {
        public IDCodePrinter()
        {
            InitializeComponent();

            //try
            //{
            //    pictureBox2.Image = Encode_DM("# 3Q0.915.590.F#131355100#", 5, 10);
            //    pictureBox1.Image = Encode_EAN_13("12345678");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
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
            DateTime datetime = DateTime.Now;
            LocalReport report = new LocalReport();
            report.ReportPath = @".\Report\Report1.rdlc";

            Image img = Encode_Code_39(textBox1.Text + textBox3.Text);//Encode_EAN_8(textBox2.Text);
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
            //doc.PageScaling = PdfPrintPageScaling.ActualSize;
            doc.PrintDocument.Print();
        }

        private List<Stream> m_streams;
        private Stream CreateStream(string name, string fileNameExtension,
            Encoding encoding, string mimeType, bool willSeek)
        {
            m_streams = new List<Stream>();
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        private int m_currentPageIndex;
        private void Print()
        {
            m_currentPageIndex = 0;
            if (m_streams == null || m_streams.Count == 0)
                return;
            PrintDocument printDoc = new PrintDocument();
            //printDoc.PrinterSettings.PrinterName = "Microsoft Print to PDF";//打印机名称 默认打印机不设
            if (!printDoc.PrinterSettings.IsValid)
            {
                MessageBox.Show("Can't find printer");
                return;
            }
            //printDoc.DefaultPageSettings.PaperSize.Height = 590;
            //printDoc.DefaultPageSettings.PaperSize.Width = 472;
            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            printDoc.Print();
        }

        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            m_streams[m_currentPageIndex].Position = 0;
            Metafile pageImage = new Metafile(m_streams[m_currentPageIndex]);

            ev.PageSettings.Landscape = false;//指定是否横向打印
            Rectangle destRect = new Rectangle(0, 0, pageImage.Width, pageImage.Height);//设置打印区域大小
            ev.Graphics.DrawImage(pageImage, destRect);
            //ev.Graphics.DrawImage(pageImage, 0, 0);
            m_streams[m_currentPageIndex].Close();
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

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
