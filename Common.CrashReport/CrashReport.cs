using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using BIW.DataConversionLibrary;

namespace BIW.Common.CrashReport
{
    internal partial class CrashReport : Form
    {
        private readonly ReportCrash _reportCrash;

        private readonly ComponentResourceManager _resources = new ComponentResourceManager(typeof (CrashReport));

        private ProgressDialog _progressDialog;

        #region Form Events

        public CrashReport(ReportCrash reportCrashObject)
        {
            InitializeComponent();
            _reportCrash = reportCrashObject;
            Text = string.Format(_resources.GetString("TitleText"), _reportCrash.ApplicationTitle,
                _reportCrash.ApplicationVersion);
            saveFileDialog.FileName = string.Format(_resources.GetString("ReportFileName"),
                _reportCrash.ApplicationTitle, _reportCrash.ApplicationVersion);
            saveFileDialog.Filter = @"HTML files(*.html)|*.html";
            if (File.Exists(_reportCrash.ScreenShot))
            {
                checkBoxIncludeScreenshot.Checked = _reportCrash.IncludeScreenshot;
                pictureBoxScreenshot.ImageLocation = _reportCrash.ScreenShot;
                pictureBoxScreenshot.Show();
            }
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void CrashReportLoad(object sender, EventArgs e)
        {
            textBoxException.Text = _reportCrash.Exception.GetType().ToString();
            textBoxApplicationName.Text = _reportCrash.ApplicationTitle;
            textBoxApplicationVersion.Text = _reportCrash.ApplicationVersion;
            textBoxExceptionMessage.Text = _reportCrash.Exception.Message;
            textBoxMessage.Text = _reportCrash.Exception.Message;
            textBoxTime.Text = (_reportCrash.TimeStamp=DateTime.Now).ToString(CultureInfo.InvariantCulture);
            textBoxSource.Text = _reportCrash.Exception.Source;
            textBoxStackTrace.Text = string.Format("{0}\n{1}", _reportCrash.Exception.InnerException,
                _reportCrash.Exception.StackTrace);
            
        }

        private void CrashReport_Shown(object sender, EventArgs e)
        {
            Activate();
            textBoxEmail.Select();
        }

        private void CrashReport_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (File.Exists(_reportCrash.ScreenShot))
            {
                try
                {
                    File.Delete(_reportCrash.ScreenShot);
                }
                catch (Exception exception)
                {
                    Debug.Write(exception.Message);
                }
            }
        }

        #endregion

        #region Control Events

        private void ButtonSendReportClick(object sender, EventArgs e)
        {
            var fromAddress = !string.IsNullOrEmpty(_reportCrash.FromEmail) ? new MailAddress(_reportCrash.FromEmail) : null;


            const string r0_255 = @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])";
            var regexEmail = new Regex(@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                       + @"((" + r0_255 + @"\." + r0_255 + @"\." + r0_255 + @"\." + r0_255 + @"){1}|"
                                       + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$");
            var subject = "";

            if (string.IsNullOrEmpty(textBoxEmail.Text.Trim()))
            {
                if (_reportCrash.EmailRequired)
                {
                    errorProviderEmail.SetError(textBoxEmail, _resources.GetString("EmailRequiredError"));
                    return;
                }
            }
            else
            {
                errorProviderEmail.SetError(textBoxEmail, "");
                if (!regexEmail.IsMatch(textBoxEmail.Text.Trim()))
                {
                    if (_reportCrash.EmailRequired)
                    {
                        errorProviderEmail.SetError(textBoxEmail, _resources.GetString("InvalidEmailAddressError"));
                        return;
                    }
                }
                else
                {
                    errorProviderEmail.SetError(textBoxEmail, "");
                    //fromAddress = new MailAddress(textBoxEmail.Text.Trim());
                    subject = string.Format("{0} {1} Crash Report by {2}", _reportCrash.ApplicationTitle,
                        _reportCrash.ApplicationVersion, textBoxEmail.Text.Trim());
                }
            }

            string json = GetReportJsonString();

            //JsonConverLib jcl = new JsonConverLib();
            //JsonConverLib.ErrorReporter pi1 = jcl.JsonDeserialize<JsonConverLib.ErrorReporter>(json);
            //string SQLStr = string.Format("INSERT INTO [PBDCSystem].[dbo].[CrashReport]([appname],[version],[os],[framework],[timestamp],[report]) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')",
            //                               pi1.appname, pi1.version, pi1.os, pi1.framework, pi1.timestamp, pi1.report);

            //string ss = HelperMethods.GetStringFromBase64("PCFET0NUWVBFIGh0bWwgUFVCTElDICItLy9XM0MvL0RURCBYSFRNTCAxLjAgVHJhbnNpdGlvbmFsLy9FTiIgImh0dHA6Ly93d3cudzMub3JnL1RSL3hodG1sMS9EVEQveGh0bWwxLXRyYW5zaXRpb25hbC5kdGQiPg0KICAgICAgICAgICAgICAgICAgICA8aHRtbCB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMTk5OS94aHRtbCI+DQogICAgICAgICAgICAgICAgICAgIDxoZWFkPg0KICAgICAgICAgICAgICAgICAgICA8bWV0YSBodHRwLWVxdWl2PSJDb250ZW50LVR5cGUiIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD11dGYtOCIgLz4NCiAgICAgICAgICAgICAgICAgICAgPHRpdGxlPkNyYXNoUmVwb3J0ZXJUZXN0LkRlbW8gMS41LjEuMCBDcmFzaCBSZXBvcnQ8L3RpdGxlPg0KICAgICAgICAgICAgICAgICAgICA8c3R5bGUgdHlwZT0idGV4dC9jc3MiPg0KICAgICAgICAgICAgICAgICAgICAubWVzc2FnZSB7DQogICAgICAgICAgICAgICAgICAgIHBhZGRpbmctdG9wOjVweDsNCiAgICAgICAgICAgICAgICAgICAgcGFkZGluZy1ib3R0b206NXB4Ow0KICAgICAgICAgICAgICAgICAgICBwYWRkaW5nLXJpZ2h0OjIwcHg7DQogICAgICAgICAgICAgICAgICAgIHBhZGRpbmctbGVmdDoyMHB4Ow0KICAgICAgICAgICAgICAgICAgICBmb250LWZhbWlseTpTYW5zLXNlcmlmOw0KICAgICAgICAgICAgICAgICAgICB9DQogICAgICAgICAgICAgICAgICAgIC5jb250ZW50DQogICAgICAgICAgICAgICAgICAgIHsNCiAgICAgICAgICAgICAgICAgICAgYm9yZGVyLXN0eWxlOmRhc2hlZDsNCiAgICAgICAgICAgICAgICAgICAgYm9yZGVyLXdpZHRoOjFweDsNCiAgICAgICAgICAgICAgICAgICAgfQ0KICAgICAgICAgICAgICAgICAgICAudGl0bGUNCiAgICAgICAgICAgICAgICAgICAgew0KICAgICAgICAgICAgICAgICAgICBwYWRkaW5nLXRvcDoxcHg7DQogICAgICAgICAgICAgICAgICAgIHBhZGRpbmctYm90dG9tOjFweDsNCiAgICAgICAgICAgICAgICAgICAgcGFkZGluZy1yaWdodDoxMHB4Ow0KICAgICAgICAgICAgICAgICAgICBwYWRkaW5nLWxlZnQ6MTBweDsNCiAgICAgICAgICAgICAgICAgICAgZm9udC1mYW1pbHk6QXJpYWw7DQogICAgICAgICAgICAgICAgICAgIH0NCiAgICAgICAgICAgICAgICAgICAgPC9zdHlsZT4NCiAgICAgICAgICAgICAgICAgICAgPC9oZWFkPg0KICAgICAgICAgICAgICAgICAgICA8Ym9keT4NCiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0idGl0bGUiIHN0eWxlPSJiYWNrZ3JvdW5kLWNvbG9yOiAjRkZDQzk5Ij4NCiAgICAgICAgICAgICAgICAgICAgPGgyPkNyYXNoUmVwb3J0ZXJUZXN0LkRlbW8gMS41LjEuMCBDcmFzaCBSZXBvcnQgLS0tIDIvMTgvMjAxNyAxMTo0NDoyMiBBTTwvaDI+DQogICAgICAgICAgICAgICAgICAgIDwvZGl2Pg0KICAgICAgICAgICAgICAgICAgICA8YnIvPg0KICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJjb250ZW50Ij4NCiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0idGl0bGUiIHN0eWxlPSJiYWNrZ3JvdW5kLWNvbG9yOiAjNjZDQ0ZGOyI+DQogICAgICAgICAgICAgICAgICAgIDxoMz5XaW5kb3dzIFZlcnNpb248L2gzPg0KICAgICAgICAgICAgICAgICAgICA8L2Rpdj4NCiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0ibWVzc2FnZSI+DQogICAgICAgICAgICAgICAgICAgIDxwPk1pY3Jvc29mdCBXaW5kb3dzIDcgVWx0aW1hdGUgNjQtYml0IChPUyBCdWlsZCA3NjAxKTwvcD4NCiAgICAgICAgICAgICAgICAgICAgPC9kaXY+DQogICAgICAgICAgICAgICAgICAgIDwvZGl2Pg0KICAgICAgICAgICAgICAgICAgICA8YnIvPg0KICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJjb250ZW50Ij4NCiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0idGl0bGUiIHN0eWxlPSJiYWNrZ3JvdW5kLWNvbG9yOiAjNjZDQ0ZGOyI+DQogICAgICAgICAgICAgICAgICAgIDxoMz5DTFIgVmVyc2lvbjwvaDM+DQogICAgICAgICAgICAgICAgICAgIDwvZGl2Pg0KICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJtZXNzYWdlIj4NCiAgICAgICAgICAgICAgICAgICAgPHA+NC4wLjMwMzE5LjQyMDAwPC9wPg0KICAgICAgICAgICAgICAgICAgICA8L2Rpdj4NCiAgICAgICAgICAgICAgICAgICAgPC9kaXY+DQogICAgICAgICAgICAgICAgICAgIDxici8+ICAgIA0KICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJjb250ZW50Ij4NCiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0idGl0bGUiIHN0eWxlPSJiYWNrZ3JvdW5kLWNvbG9yOiAjNjZDQ0ZGOyI+DQogICAgICAgICAgICAgICAgICAgIDxoMz5FeGNlcHRpb248L2gzPg0KICAgICAgICAgICAgICAgICAgICA8L2Rpdj4NCiAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0ibWVzc2FnZSI+DQogICAgICAgICAgICAgICAgICAgIDxici8+DQogICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJjb250ZW50Ij4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9InRpdGxlIiBzdHlsZT0iYmFja2dyb3VuZC1jb2xvcjogIzY2Q0NGRjsiPg0KICAgICAgICAgICAgICAgICAgICAgICAgPGgzPkV4Y2VwdGlvbiBUeXBlPC9oMz4NCiAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2Pg0KICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0ibWVzc2FnZSI+DQogICAgICAgICAgICAgICAgICAgICAgICA8cD5TeXN0ZW0uSU8uRmlsZU5vdEZvdW5kRXhjZXB0aW9uPC9wPg0KICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+DQogICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj48YnIvPg0KICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0iY29udGVudCI+DQogICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJ0aXRsZSIgc3R5bGU9ImJhY2tncm91bmQtY29sb3I6ICM2NkNDRkY7Ij4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxoMz5FcnJvciBNZXNzYWdlPC9oMz4NCiAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2Pg0KICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0ibWVzc2FnZSI+DQogICAgICAgICAgICAgICAgICAgICAgICA8cD5GaWxlIE5vdCBmb3VuZCB3aGVuIHRyeWluZyB0byB3cml0ZSBhcmd1bWVudCBleGNlcHRpb24gdG8gdGhlIGZpbGU8L3A+DQogICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj4NCiAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2Pjxici8+DQogICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJjb250ZW50Ij4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9InRpdGxlIiBzdHlsZT0iYmFja2dyb3VuZC1jb2xvcjogIzY2Q0NGRjsiPg0KICAgICAgICAgICAgICAgICAgICAgICAgPGgzPlNvdXJjZTwvaDM+DQogICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9Im1lc3NhZ2UiPg0KICAgICAgICAgICAgICAgICAgICAgICAgPHA+Q3Jhc2hSZXBvcnRlclRlc3QuRGVtbzwvcD4NCiAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2Pg0KICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+PGJyLz4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9ImNvbnRlbnQiPg0KICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0idGl0bGUiIHN0eWxlPSJiYWNrZ3JvdW5kLWNvbG9yOiAjNjZDQ0ZGOyI+DQogICAgICAgICAgICAgICAgICAgICAgICA8aDM+U3RhY2sgVHJhY2U8L2gzPg0KICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+DQogICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJtZXNzYWdlIj4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxwPiAgIGF0IENyYXNoUmVwb3J0ZXJUZXN0LkZvcm1NYWluLlRocm93RXhjZXB0aW9uKCkgaW4gZDpcQ09ERVxDcmFzaFJlcG9ydGVyXENyYXNoUmVwb3J0ZXJcQ3Jhc2hSZXBvcnRlclRlc3RcRm9ybU1haW4uY3M6bGluZSAzODxici8+ICAgYXQgQ3Jhc2hSZXBvcnRlclRlc3QuRm9ybU1haW4uQnV0dG9uVGVzdENsaWNrKE9iamVjdCBzZW5kZXIsIEV2ZW50QXJncyBlKSBpbiBkOlxDT0RFXENyYXNoUmVwb3J0ZXJcQ3Jhc2hSZXBvcnRlclxDcmFzaFJlcG9ydGVyVGVzdFxGb3JtTWFpbi5jczpsaW5lIDE3PGJyLz4gICBhdCBTeXN0ZW0uV2luZG93cy5Gb3Jtcy5Db250cm9sLk9uQ2xpY2soRXZlbnRBcmdzIGUpPGJyLz4gICBhdCBTeXN0ZW0uV2luZG93cy5Gb3Jtcy5CdXR0b24uT25DbGljayhFdmVudEFyZ3MgZSk8YnIvPiAgIGF0IFN5c3RlbS5XaW5kb3dzLkZvcm1zLkJ1dHRvbi5Pbk1vdXNlVXAoTW91c2VFdmVudEFyZ3MgbWV2ZW50KTxici8+ICAgYXQgU3lzdGVtLldpbmRvd3MuRm9ybXMuQ29udHJvbC5XbU1vdXNlVXAoTWVzc2FnZSZhbXA7IG0sIE1vdXNlQnV0dG9ucyBidXR0b24sIEludDMyIGNsaWNrcyk8YnIvPiAgIGF0IFN5c3RlbS5XaW5kb3dzLkZvcm1zLkNvbnRyb2wuV25kUHJvYyhNZXNzYWdlJmFtcDsgbSk8YnIvPiAgIGF0IFN5c3RlbS5XaW5kb3dzLkZvcm1zLkJ1dHRvbkJhc2UuV25kUHJvYyhNZXNzYWdlJmFtcDsgbSk8YnIvPiAgIGF0IFN5c3RlbS5XaW5kb3dzLkZvcm1zLkJ1dHRvbi5XbmRQcm9jKE1lc3NhZ2UmYW1wOyBtKTxici8+ICAgYXQgU3lzdGVtLldpbmRvd3MuRm9ybXMuQ29udHJvbC5Db250cm9sTmF0aXZlV2luZG93Lk9uTWVzc2FnZShNZXNzYWdlJmFtcDsgbSk8YnIvPiAgIGF0IFN5c3RlbS5XaW5kb3dzLkZvcm1zLkNvbnRyb2wuQ29udHJvbE5hdGl2ZVdpbmRvdy5XbmRQcm9jKE1lc3NhZ2UmYW1wOyBtKTxici8+ICAgYXQgU3lzdGVtLldpbmRvd3MuRm9ybXMuTmF0aXZlV2luZG93LkNhbGxiYWNrKEludFB0ciBoV25kLCBJbnQzMiBtc2csIEludFB0ciB3cGFyYW0sIEludFB0ciBscGFyYW0pPC9wPg0KICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+DQogICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj48YnIvPg0KICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0iY29udGVudCI+DQogICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJ0aXRsZSIgc3R5bGU9ImJhY2tncm91bmQtY29sb3I6ICM2NkNDRkY7Ij4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxoMz5Jbm5lciBFeGNlcHRpb248L2gzPg0KICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+DQogICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJtZXNzYWdlIj4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxici8+DQogICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJjb250ZW50Ij4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9InRpdGxlIiBzdHlsZT0iYmFja2dyb3VuZC1jb2xvcjogIzY2Q0NGRjsiPg0KICAgICAgICAgICAgICAgICAgICAgICAgPGgzPkV4Y2VwdGlvbiBUeXBlPC9oMz4NCiAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2Pg0KICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0ibWVzc2FnZSI+DQogICAgICAgICAgICAgICAgICAgICAgICA8cD5TeXN0ZW0uQXJndW1lbnRFeGNlcHRpb248L3A+DQogICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj4NCiAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2Pjxici8+DQogICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJjb250ZW50Ij4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9InRpdGxlIiBzdHlsZT0iYmFja2dyb3VuZC1jb2xvcjogIzY2Q0NGRjsiPg0KICAgICAgICAgICAgICAgICAgICAgICAgPGgzPkVycm9yIE1lc3NhZ2U8L2gzPg0KICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+DQogICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJtZXNzYWdlIj4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxwPuWAvOS4jeWcqOmihOacn+eahOiMg+WbtOWGheOAgjwvcD4NCiAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2Pg0KICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+PGJyLz4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9ImNvbnRlbnQiPg0KICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0idGl0bGUiIHN0eWxlPSJiYWNrZ3JvdW5kLWNvbG9yOiAjNjZDQ0ZGOyI+DQogICAgICAgICAgICAgICAgICAgICAgICA8aDM+U291cmNlPC9oMz4NCiAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2Pg0KICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0ibWVzc2FnZSI+DQogICAgICAgICAgICAgICAgICAgICAgICA8cD5DcmFzaFJlcG9ydGVyVGVzdC5EZW1vPC9wPg0KICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+DQogICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj48YnIvPg0KICAgICAgICAgICAgICAgICAgICAgICAgPGRpdiBjbGFzcz0iY29udGVudCI+DQogICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJ0aXRsZSIgc3R5bGU9ImJhY2tncm91bmQtY29sb3I6ICM2NkNDRkY7Ij4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxoMz5TdGFjayBUcmFjZTwvaDM+DQogICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj4NCiAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9Im1lc3NhZ2UiPg0KICAgICAgICAgICAgICAgICAgICAgICAgPHA+ICAgYXQgQ3Jhc2hSZXBvcnRlclRlc3QuRm9ybU1haW4uVGhyb3dFeGNlcHRpb24oKSBpbiBkOlxDT0RFXENyYXNoUmVwb3J0ZXJcQ3Jhc2hSZXBvcnRlclxDcmFzaFJlcG9ydGVyVGVzdFxGb3JtTWFpbi5jczpsaW5lIDMwPC9wPg0KICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+DQogICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj48YnIvPg0KICAgICAgICAgICAgICAgICAgICAgICAgPC9kaXY+DQogICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj48YnIvPg0KICAgICAgICAgICAgICAgICAgICA8L2Rpdj4NCiAgICAgICAgICAgICAgICAgICAgPC9kaXY+PGJyLz4NCiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJjb250ZW50Ij4NCiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8ZGl2IGNsYXNzPSJ0aXRsZSIgc3R5bGU9ImJhY2tncm91bmQtY29sb3I6ICM2NkZGOTk7Ij4NCiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8aDM+U2NyZWVuIFNob3Q8L2gzPg0KICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvZGl2Pg0KICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxkaXYgY2xhc3M9Im1lc3NhZ2UiPg0KICAgICAgICAgICAgICAgICAgICAgICAgICAgIDxpbWcgc3JjPSJkYXRhOmltYWdlL3BuZztiYXNlNjQsaVZCT1J3MEtHZ29BQUFBTlNVaEVVZ0FBQVZ3QUFBRG1DQVlBQUFDWk1EMkxBQUFBQVhOU1IwSUFyczRjNlFBQUFBUm5RVTFCQUFDeGp3djhZUVVBQUFBSmNFaFpjd0FBRHNJQUFBN0NBUlVvU29BQUFCUGtTVVJCVkhoZTdkejlyeHpWZWNEeC9TUDZGMVRpcFRLSmRQMFNGYW1sRW9Ra0ZRWnFXdFUyR0FJQlIyMVF3bHNOQ012bU9uVnRpeGpNaFI5Qy9BSkZkcWtKS0x5NFRoUEhtSkxZVmR5bVVJaERqRjk0Y1YzcVlEc1l2NEo5N2RONVpuZDJ6am56ek03czllN2hucjNmSzMza2UyZlBuRGt6dS9QZDRSY2FVNmRPTlFDQS9rdUR5dzgvL1BERFQzOS9uT0N1L3ZHYkFJQStrQitDQ3dBQnlJOFQzQjlzZkJNQTBBZWZuZldDdS9MSGJ3RUErdURvbVhNRUZ3QkNLQVIzelU5MkFBRDY0SGVudmVBK3RlazNBSUErZU8vVVdZSUxBQ0VVZ3Z2MHozNExBQlBXOTBhZU5UZis2ZFhtaWovNHc5cGt2T3lueldmYmNYelVEZTdhTGU4QXdJUjE3WVhUekthVnE4enhYLytxTmhrdisybnoyYlovY3FhNzRDNi9mY2hjdnRUYXR2WUJjOEgwVmM0WXg5S1o1b0xiTnhXMk5aUjlDbk1YeG0weWN5Yk5OSCtYdmRaby9lNVlaUzZmOUlCWjdtOXZ6eVZ6TkpKOUxkWXgybXZRMXAyUmMwN1djWGt5ajdOZUFOR1RKOVlqLzduVjdIL3FzZHBrdk95bnpXY3JCUGVaVjNkM3NObmNPR21XdWRmYWR1LzBocmxpbVQzR2RlLzBJWFBqT3UvM1piT1N5SzB1akYyUnhNNEpvY282L3JyNTVvTDA3OVhtQ25Wc1l0SjhzeUlibng1M2ZuSU8rWnJTT1dRdHlXc1gzTDQ1WFVOMlBuSnVzaTBkWjh2MlNhK0hOUmVBNkVrNFAvN2xhK2FESHl5dlRjYkxmdHA4dGxkKzd3VjMvV3Q3VlNQZm51ekZiTEw1K2pOcnpKZWRiUzJYTERBanN0OHpDOHlGMmUrdkpXT3ozeCthYlJwWHJ5a2NZK3lzdVV0L3oyd3hYNzhrVy90c2M3K3NVZGFTck9uQ2IyOUp6L1BMRDFsanIxNlFVTTdSMXROekFmQjVrbkFlM3ZxS2VmZXh4YW5sZDl4dC92S3Z2NW55ZjgvR3lIalpUNXZQOXVMQnorb0ZWN2d4Mm12dVQwSWtrYkxIMk52U1NHY3hrc2hxc1VvNDQvM3RoZjBrbHY2eEpaNzJHRXNydVBuNGJvSmJSZ3M1Z0VFZzRYeG41U1BtSjFkY2twSzRaai8rNzlrWUdUK200UDV3NjdzbFhqVTNYekxiUExDKytkVDZXUGF2dkxZOGlkVjNYalUvdExkdGZiSVp3V3VlYk8wNzJkeTh2alZYTXI2NXZmVjN5MlBmU1dLM1BCK1R6dW1NZGVlUjhjVjVrdU8yMTJDVGZSdkovTmtjc3I3VytjZ2NyZU0xMTlBY0s4RnVyOGRSZGd3QXNmT0R1K0RhYTV6UXlvLzhMZHY5NEdyejJmN3B3S2R1Y0ovZjlyN3UyWVhtb211ZWF2NTd5VUx6dVAzYXc5ZWJpKzU0elJuLytCMlRrL0hKOW13ZitkY2EzN0QvYnBGOTdLZlRkRTRaYTIyVEo5eGJuclgzZThwYzZieXVjTmI3bXJrbENlNHR6OHArMTV2NTJkcGE1eUJydVBMaDV0anM5K0s2RnBvci9Xc0FZQ0Q0d2ZXajY4ZFdaTUhWNXJNOXNiOW1jUDNvQ0R0TWZrRG5KMUY2M0E5dHBrTndzem5iRVhmR1pySE14L3VoNzZRNS9xazh1QkxOR3NGdDdpL0hUZ0pkK0IzQUlKRnc3bG96WWpiOStlVFV3aGt6MHNqdTJiT25UZjZXN2RrWUdUK200TDc0eTMyZFBUOXNMdnJDc1BsK2U5dlB6YTFma0FCUE1iYytiNDNMeHM1NDJyeTQ0b1pDckIydCtSYk9jTGRmZE5mUGxYM3o0OGo0cjZ5UTM1ODJYM0hHV0t5MUxwd2grOHA2azMvdlN1YVZ0VmxybE9OOS82NHByVG4zdFg5ZmVKZk1JZnZkWUJZNmN6Vi9CekE0Skp4NzF6NWhYdjJyUDB2WnNmVi96OGJJZU5sUG04LzI4TDVUYm5CZitvLzluZjFvMkZ5Y3hXekdXdlBFM1ZQU2YxOGFTUUtXeE8wSmY2eThabThUTXJhd2ZhdTVMUW5hZzlhWWkrL2VtczR2LytaanBwamJmdVNQWDJ1K21oMWJqdGxlaDdWZGZtOWs0MlhmNWpua2N6Zko4YjQ2WXYwK0kxdXJ1NzcydXB6akFZaWRoUFA5NS83UmJQdkd0YW52M25CakdsZmgvNTZOa2ZHeW56YWY3YnZ2bmF3WDNBZVRwMG43cVZFQzA5eVdSOGovdTZ2ZyttUFQ0SzYxQWl1czRDcWhTK09mcmRGZWgyalAzNHh0RmxwWnN4MWRQN2o1bDBnZTZYUnVtUy83Vnp0SEFGR1NjTzcvbCtmTWY5MXphMjB5Zmt6QjNmQ3JEenQ3WVpHNStJdUx6UEE5eVU3SnZ5dTkxNGV2UzRLVWJaZXgxNjF6WGsrTnpERU5iL3Z3ZFZQTjNCZVMzMldmVmpTL2RrKytmenB2dW4yT0dVNytYaW5IVDEvYlp1WitzZldhTStjNjg3VnNucEhrNytTWUY5K3pMaDJiL3QwZUovc254NzRuV1ZNNnZyV081TFZzVGVteEN2UG4yOTM1QU1STXdubmdsWTNtTi85d2IyMHlYdmJUNXJQZHY4Y0w3c2JYUHdTQUNlc3YvdWlQemVhVnE4eWhmLyszMm1TODdLZk5aN3RqOXdtQ0N3Q1pKNTdja1A2UGFPU0p0UzRaTC90cDg5a0t3ZjNYTnc0QUFQcmdiM1o2d2YzcGZ4OEFBUFRCTjM1N25PQUNRQWkxZzN2a3lCRUFRQTFhUXdYQkJZQWUweG9xQ0M0QTlKaldVRUZ3QWFESHRJWUtnZ3NBUGFZMVZCQmNBT2d4cmFHQzRBSkFqMmtORlFRWEFIcE1hNmdndUFEUVkxcERCY0VGZ0I3VEdpb0lMZ0QwbU5aUVFYQUJvTWUwaGdxQ0N3QTlwalZVRUZ3QTZER3RvYUx2d2YzbTR4dk41WCsvd2Z6Smd5OEJRQlNrV2RJdXJXbDFhQTBWZlEzdXJJZGVNSCs3NWhmbXZZUEh6T216NXdBZ0N0SXNhWmMwVEd0YkZhMmhvcS9Cdld6NFpmUEI0ZVBtMUpsejVxT1Rad0VnQ3RJc2FaYzBUR3RiRmEyaG9xL0JsVWZ6ejVKdml3UEpDUUJBVEtSZDBqQ3RiVlcwaG9yK0IzZjBuUG53eEZrQWlJcTBpK0FDUUFBRUZ3QUNJYmdBRUFqQkJZQkFvZ3p1cDhtaTl4OGZCWUNvU0xzSUxnQUVRSEFCSUJDQ0N3Q0JFRndBQ0lUZ0FrQWdVUWIzVkxMb0Q0Nk5Ba0JVcEYwRUZ3QUNJTGdBRUFqQkJZQkFDQzRBQkJKdGNOODlPZ29BVVNHNEFCQUl3UVdBUUFndUFBUkNjQUVnRUlJTEFJRkVHZHlUWjg2WlBaK01Ba0JVcEYwRUZ3QUNJTGdBRUFqQkJZQkFDQzRBQkJKbGNFOGtpMzdueUNnQVJFWGFSWEFCSUFDQ0MvVElwcVdYbWthajBYVHpodnkxNSthMnR3OHQzZW5zVThlcW0xdHpKbVkrcDQ5QkhBaHV4T3diMFJmOHhyU2lvcHRyVm1uN1JXT0RtYW1lMTZYbWdkZWJZOHFDNjd4UFgzckViR3JQV1U5UGdxdThQNlZ6dmY2SUdmTEdqdVdMSW1VZmR3em5QbWdJYm9TY0c3dkUrUWEzZVl3OEpwVUdPTGlkdnRqcUJIZGNQT0ZxNzQrOVJvdjIrU0s0dlVGd0krUGYvTm9OS0dQR0h0eWQ1b0V2WmZPUE1iZ0RjMlBaMTBLL0hxdHVyaEhjODlTMzRLcGZndjQ1TnhIYzNvZ3l1QjkvZHM1cytmRHN4TFBPdm1rdU5iZHZVOGFjdDEzbTlxbGpPSWE5dHFrcnpGcHRUR1RXTHJLZjlPYWFwY29ZbXpOK3prWjF6RmdzblpPdG9XR21yOVBIVkxMZW4wbFQ4M1VXNXR1MndreEtYN3MwR1pjZmQ5S2lYZTY0dXRyekpYcDRUV0lsN1NLNFViQkRPTFlid0EySWNJTmFmRDFYZWJ3dWdtc0hKSjAzMjdlMVg5WHIyanh0eWsxZGR6NVg5OWU3TExpVkliYWpaTWxpcUFiWDM2Y3FadGI3TTMxUmVRVHp0YzQxMC8zclpvMXp4MmFVTDJpQzZ5QzRzWEJ1c0c2ZmJqZWE2ZTE5aTdLYitITUo3cHk1K1hscHdWVmVyem9mLy9wVXo2Y1l3L1VlUzNEdHRmazZCZGZacitKNnAremdyck8vVE93bmQydDdzazdudWpudmY3M1BVNHJnT2dodUxPeWdsZDBram55TWZ1UFV1T202Q2J1elBvVjFzNVZHUmdsdThYWHZmTXZDWmtXbzZualovbzdTNjEydTIrQTYyLzFyblJ5L05MaGpXSnU5ajN3RzdHTzNQeFBXR0RtTy9ya3AyMTd5ZVNLNERvSWJpOUtickNxNDl0T0llM1BhTjEzK1ZCSTZ1TVZqZEh6ZGVmTDBZMk9mYTc1ZjFmRlVZNGhhZDhIVjE2cXgxKzgrblhwUGs1MTR3WFd1WStHTHJubSs5bkh6c0hiNWVTSzRqaWlEKy90UHo1bk4venM2c1d4N3hBck5wZVpiMjZyR3pEVkxaTnZhaWhDMlRCcmUyWnBucC9uV2xHeDd5WEUwOW5HbVBHS2Uxc2EwTExFRE9HZERkNjkzUEk2OTlvYTVhbTF6ZTlYeFZNNTFxM2Nkbmg2Mnc1b2ZSOTJ1dlZjbDdQVlBtcUlmbzVKMVBzMzMybitmTjVpcm5OZTk0MmFmajY0L1Q4VjVKekpwRjhHTlF2N0JMZjN3UmhoYzdUdzZ2dDd4T05YQnJYL1R1OWM3bTZ1VDRNR3QyTTlSQ0s2N3JxdUdzL1hrN3puQjdUMkNHeEg3QmxCdk51MG03dUxHYmhybndlMTRQbllrSzhKUlErWDE5blFWM0pLMWF1eDFTUGlkZGRWOXlsV0M2MTdMRnV1OVU2OWIxNThuMkFodVZOeW5yc0lIWHIwWjNLZSt3ZzJhN0hPVkV5RjNmTzFBaFFwdWgvVTVZYXNLUngyRklCVURzMlJPSHN2dWd1dXVxekIzY2oyMUovVG10dTZmdnRYZ0p0dzFsTCtXYisvMjgyU3R0ZTZYd3dBanVOSHhvMXZHdW9FTDRYRDVFWElDVVRLbXdBNXVDUzBnMnJ4VnIxZWRqeCt2eXZrNnFUelcySU5iOVY2V0I5ZC9qMnI4bDBoSmNOMzN6WjJuOUxwMThYbHkxMW44d3Bwb0NHNnNPbnpvOWFoNFR5WnQrazFnMzJ5aU1sUWhnOXZpcjdGc2ZOMzVPdEcraEpyeTY5ZDljRHZObmNmUFhuLytOT3UvbnhVeEt3dXVIWDN2djB3Nlg3ZTZueWVlY0cxUkJ2ZHdzdWlmL3M4WkFJaUt0SXZnQWtBQUJCY0FBaUc0QUJBSXdRV0FRS0lNN3FGazBSdjNuUWFBcUVpN0NDNEFCRUJ3QVNBUWdnc0FnUkJjQUFpRTRBSkFJSEVHOTlSWjgvTDdud0ZBVktSZEJCY0FBaUM0QUJBSXdRV0FRQWd1QUFRU1pYQVBKb3QrNGQxUEFTQXEwaTZDQ3dBQkVGd0FDSVRnQWtBZ0JCY0FBaUc0QUJCSWxNSDk2T1JaOC95ZVV3QVFGV2tYd1FXQUFBZ3VBQVJDY0FFZ0VJSUxBSUZFR2R6ZkpZdCtkdmRKQUlpS3RDdXE0RjYyNkdYejZ3K1BtajFIenBoL2Z1Y0VBRVJCbWlYdGtvWnBiYXVpTlZUME5ialhmKzlGYzl1cVg1Z2QvM2ZNSERneENnQlJrR1pKdTZSaFd0dXFhQTBWZlEydW1QblFDK2F5NFpmVFIzTUFpSUUwUzlxbE5hME9yYUdpNzhFRmdJbEdhNmdndUFEUVkxcERCY0VGZ0I3VEdpb0lMZ0QwbU5aUVFYQUJvTWUwaGdxQ0N3QTlwalZVRUZ3QTZER3RvWUxnQWtDUGFRMFZCQmNBZWt4cnFPaDdjQjk5OUZGejMzMzNtVHZ2dkJNQW9pRE5rblpwVGF0RGE2am9hM0NYTGx0bVJrWkd6SUVEQjh6bzZDZ0FSRUdhSmUyU2htbHRxNkkxVlBRMXVQUG16VE1mZmZTUk9YMzZ0RGw2OUNnQVJFR2FKZTJTaG1sdHE2STFWUFExdVBKb0x0OFcyZ2tCd0hnbTdaS0dhVzJyb2pWVTlEMjRaODZjTVo5ODhna0FSRVhhUlhBQklBQ0NDd0NCRUZ3QUNJVGdBa0FnMFFaWGV3MEF4ak9DQ3dDQkVGd0FDSVRnQWtBZ0JCY0FBaUc0QUJCSXRNSDkrT09QQVNBcUJCY0FBaUc0QUJBSXdRV0FRQWd1QUFRU1pYRGwvNXgrK1BCaEFJaUt0SXZnQWtBQUJCY0FBaUc0QUJBSXdRV0FRQWd1QUFRU2JYQVBIVG9FQUZFaHVBQVFDTUVGZ0VBSUxnQUVRbkFCSUpCb2czdnc0RUVBaUFyQkJZQkFDTzRFdGY2bWhtazBtcVl0M3E2T0dkZlczOVJlZjJQYVlyTmRHNE1tNjFwRitWNFBFSUk3QU94NFZzbHV1QWtYWEh1ODZpYXpYdHR2QURpZkQ3NmNQbGNFZHdBUVhJSXJ0aStlbHB6SE5MTjR1L2NhVDdqakJzRWRRSFZpT3FHRE8zQlBlZHZONG1tdGM5T0NpM0VqeXVDZVBIblM3TjI3RnlYV3pNNXV2b2FadkdETG1NZU1hMnRtdDlmZm1MekFiTkhHMkxvZEg1VXRac0hrMXJrMUpwc0ZXN1F4R0Era1hRUjN3SXdsdUZzV1RHNy9yZTFYbURNTG1CY3ZmNTZ5QU5qejVXYWJOZDY0cGpWbXRqTXVtWE5CZjRMcnJzdGRqL1BhN0RYT2ZudTNMRENUMi92bFpxK3h4aVNxcjQ4ZHorYisvcld5MzV2aWZNVnh6aGgvM1FuMXZhZ1lwMzFtdEgzZ0lyZ0R5TDh4cXNhVXNmZDE1cHc5TzQ5TE8xNStGRjFPZU96NEZYZ0JLZ21abzRmQjlZUFh2Z2IyT3J6OU8xM0wvTHpyWGgvMytHWFVtSHJVTVU0VU82L0pmeSs2L2N5Z2lPQU9vTzZEbXovSk9kdXRzSlRlYksweCtqSHRlRmhQaXhJLzcybklqb0srZi9uMnpnRnQ2Umo1aEwwZUovTE42T1RuNTBiSURaNzNaWkVjTXd0cC9ldmpCOWVhMHprSCsrbmIzc2RiUTBJUHJuY2M2L3lkOGFXZmdlclBESW9JN2dEU2IrNmFZNXpZbE54VWhadmFmbEt5UStEZXZNNVRyczgrYm5iemw2d2xWZnVKdGFXYjRDYWNMd0RyaWQ2OW52WjVGME9uajZ1NlBtNEkzZU81citsUHhUV0QyK25hbHB4WHQ1OFpGRVViM04yN2Q2UEU2bG41alRFMGYzTjNZemJQTjBPdDdZM0dMTE5hR2QrWXRUb2ZMMWJQeWwvcklEL09aak4vU0IrVEdwcHZOdnZ6WnRzeTlqcjkxelNkNWxJcGEreTBCdXRhRlhSMWZkemp6bHJ0em1XL0QvbHI5ajVEWnY1bWQ1L044NGZhKzdUZnU0N1hRMTlEdDU4WkZCSGNBZFR2NEJibTdDb29xODBzNVRWSEZnQjdYai95bjBkdy9aZ0VEMjdaYXdRM0ZnUjNBQVVQYmpjM1hGbEV0WUIyaWtLM0FlMXl2QjJwb1NFbFdDbjd5Nk1ZdXJhdWd0UXB1R1hIRzBOd082NUpQdzdCUFg4RWR3QUZENjRYQ2UxcGRGYTJUMGx3N2ZueklMcFB3L2x4dmFma1hnZlhIcHRlQS9kNGRnU2RkZnV4U2VacGp1M2krdmhqclRtZGNEcm40Tzdqdno5cWNEdnNVM2FjYmo4ektDSzRBNmh6SEN2R2pDbTQvbjVGN1gwcXhxV3NtOXk1K2N0MEc5d1N6VGpxY1hYWFlVZkZpNytuSGVlNjE2Y1FYRTNGVTJ4TE5xY2UzRVRsZStIR2srQ2VQNEk3Z0Q2WDRLYktZdUhkaFA2TkxzRzB0L2tCOVdNcHIzZnp4Q3BxQmRkYnZ6T3ZGMWJ2S1ZYL1l2RERXT2Y2dUdObXJmYURYb3h0eG42UFJHVndTL1lUMm52YzdXY0dSZEVHZDlldVhjQUErcGtUM0ptcnRUR0lGY0VGeGhXQ084Z0lMakN1RU54QlJuQ0JjWVhnRGpLQ0M0d3JCSGVRUlJuY0V5ZE9tSjA3ZHdKQVZLUmRCQmNBQWlDNEFCQUl3UVdBUUFndUFBUkNjQUVna0dpRCsvYmJid05BVkFndUFBUkNjQUVnRUlJTEFJRVFYQUFJSk1yZ0hqOSszT3pZc1FNQW9pTHRJcmdBRUFEQkJZQkFDQzRBQkVKd0FTQVFnZ3NBZ1VRYjNMZmVlZ3NBb2tKd0FTQVFnZ3NBZ1JCY0FBaUU0QUpBSUZFRzk5aXhZK2JOTjk4RWdLaEl1NklLN3J4NTg4eStmZnZNb1VPSHpCdHZ2QUVBVVpCbVNidWtZVnJicW1nTkZYME43ckpseTh5S0ZTdk0vdjM3MDI4TEFJaUJORXZhSlEzVDJsWkZhNmpvYTNERmtpVkwwbThKZVRRSGdCaElzNlJkV3RQcTBCb3EraDVjQUpob3RJWUtnZ3NBUGFZMVZCQmNBT2d4cmFHQzRBSkFqMmtORlFRWEFIcE1hNmdndUFEUVkxcERCY0VGZ0I3VEdpb0lMZ0QwbU5aUVFYQUJvTWUwaGdxQ0N3QTlwalZVRUZ3QTZER3RvWUxnQWtDUGFRMFZCQmNBZWt4cnFLZ2RYQURBK1NHNEFCQUl3UVdBUUFndUFBUkNjQUVnRUlJTEFJRTR3WlUvQUFEOTB3NHVBS0RmcHByL0Iva2liSlVTOTlTUEFBQUFBRWxGVGtTdVFtQ0MiLz4NCiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj4NCiAgICAgICAgICAgICAgICAgICAgICAgICAgICA8L2Rpdj48L2JvZHk+PC9odG1sPg==");

            if (HttpPost2("http://submit.biw.com.cn/CrashReporter", json) == "Post Success")
                MessageBox.Show("异常报告，发送成功", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("异常报告，发送失败", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //_progressDialog = new ProgressDialog();
            //_progressDialog.ShowDialog();
        }

      

        private void ButtonSaveClick(object sender, EventArgs e)
        {
            saveFileDialog.ShowDialog();
        }

        private void SaveFileDialogFileOk(object sender, CancelEventArgs e)
        {
            File.WriteAllText(saveFileDialog.FileName, HtmlReport());
        }

        private void LinkLabelViewLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(_reportCrash.ScreenShot);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(
                    _resources.GetString("ErrorCapturingImageMessage"),
                    _resources.GetString("ErrorCapturingImageCaption"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    _resources.GetString("NoImageShownMessage"),
                    _resources.GetString("NoImageShownCaption"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region HTML Report Generator

        private string HtmlReport()
        {
            string report =
                string.Format(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                    <html xmlns=""http://www.w3.org/1999/xhtml"">
                    <head>
                    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
                    <title>{0} {1} Crash Report</title>
                    <style type=""text/css"">
                    .message {{
                    padding-top:5px;
                    padding-bottom:5px;
                    padding-right:20px;
                    padding-left:20px;
                    font-family:Sans-serif;
                    }}
                    .content
                    {{
                    border-style:dashed;
                    border-width:1px;
                    }}
                    .title
                    {{
                    padding-top:1px;
                    padding-bottom:1px;
                    padding-right:10px;
                    padding-left:10px;
                    font-family:Arial;
                    }}
                    </style>
                    </head>
                    <body>
                    <div class=""title"" style=""background-color: #FFCC99"">
                    <h2>{0} {1} Crash Report --- {5}</h2>
                    </div>
                    <br/>
                    <div class=""content"">
                    <div class=""title"" style=""background-color: #66CCFF;"">
                    <h3>Windows Version</h3>
                    </div>
                    <div class=""message"">
                    <p>{2}</p>
                    </div>
                    </div>
                    <br/>
                    <div class=""content"">
                    <div class=""title"" style=""background-color: #66CCFF;"">
                    <h3>CLR Version</h3>
                    </div>
                    <div class=""message"">
                    <p>{3}</p>
                    </div>
                    </div>
                    <br/>    
                    <div class=""content"">
                    <div class=""title"" style=""background-color: #66CCFF;"">
                    <h3>Exception</h3>
                    </div>
                    <div class=""message"">
                    {4}
                    </div>
                    </div>", HttpUtility.HtmlEncode(_reportCrash.ApplicationTitle),
                    HttpUtility.HtmlEncode(_reportCrash.ApplicationVersion),
                    HttpUtility.HtmlEncode(HelperMethods.GetWindowsVersion()),
                    HttpUtility.HtmlEncode(Environment.Version.ToString()),
                    CreateReport(_reportCrash.Exception),
                    HttpUtility.HtmlEncode(_reportCrash.TimeStamp.ToString()));
            if (File.Exists(_reportCrash.ScreenShot) && checkBoxIncludeScreenshot.Checked)
            {
              string imgBase64= HelperMethods.GetBase64FromImage(_reportCrash.ScreenShot);

                if (imgBase64 != null)
                {
                    report += string.Format(@"<br/>
                            <div class=""content"">
                            <div class=""title"" style=""background-color: #66FF99;"">
                            <h3>Screen Shot</h3>
                            </div>
                            <div class=""message"">
                            <img src=""data:image/png;base64,{0}""/>
                            </div>
                            </div>", imgBase64);
                }
            }
            if (!String.IsNullOrEmpty(textBoxEmail.Text.Trim()))
            {
                report += string.Format(@"<br/>
                            <div class=""content"">
                            <div class=""title"" style=""background-color: #66FF99;"">
                            <h3>User MailAddress</h3>
                            </div>
                            <div class=""message"">
                            <p>{0}</p>
                            </div>
                            </div>", HttpUtility.HtmlEncode(textBoxEmail.Text.Trim()));
            }
            if (!String.IsNullOrEmpty(textBoxUserMessage.Text.Trim()))
            {
                report += string.Format(@"<br/>
                            <div class=""content"">
                            <div class=""title"" style=""background-color: #66FF99;"">
                            <h3>User Comment</h3>
                            </div>
                            <div class=""message"">
                            <p>{0}</p>
                            </div>
                            </div>", HttpUtility.HtmlEncode(textBoxUserMessage.Text.Trim()));
            }
            if (!String.IsNullOrEmpty(_reportCrash.DeveloperMessage.Trim()))
            {
                report += string.Format(@"<br/>
                            <div class=""content"">
                            <div class=""title"" style=""background-color: #66FF99;"">
                            <h3>Developer Message</h3>
                            </div>
                            <div class=""message"">
                            <p>{0}</p>
                            </div>
                            </div>", HttpUtility.HtmlEncode(_reportCrash.DeveloperMessage.Trim()));
            }
            report += "</body></html>";
            return report;
        }

        private string CreateReport(Exception exception)
        {
            string report = string.Format(@"<br/>
                        <div class=""content"">
                        <div class=""title"" style=""background-color: #66CCFF;"">
                        <h3>Exception Type</h3>
                        </div>
                        <div class=""message"">
                        <p>{0}</p>
                        </div>
                        </div><br/>
                        <div class=""content"">
                        <div class=""title"" style=""background-color: #66CCFF;"">
                        <h3>Error Message</h3>
                        </div>
                        <div class=""message"">
                        <p>{1}</p>
                        </div>
                        </div><br/>
                        <div class=""content"">
                        <div class=""title"" style=""background-color: #66CCFF;"">
                        <h3>Source</h3>
                        </div>
                        <div class=""message"">
                        <p>{2}</p>
                        </div>
                        </div><br/>
                        <div class=""content"">
                        <div class=""title"" style=""background-color: #66CCFF;"">
                        <h3>Stack Trace</h3>
                        </div>
                        <div class=""message"">
                        <p>{3}</p>
                        </div>
                        </div>", HttpUtility.HtmlEncode(exception.GetType().ToString()),
                HttpUtility.HtmlEncode(exception.Message),
                HttpUtility.HtmlEncode(exception.Source ?? "No source"),
                HttpUtility.HtmlEncode(exception.StackTrace ?? "No stack trace").Replace("\r\n", "<br/>"));
            if (exception.InnerException != null)
            {
                report += string.Format(@"<br/>
                        <div class=""content"">
                        <div class=""title"" style=""background-color: #66CCFF;"">
                        <h3>Inner Exception</h3>
                        </div>
                        <div class=""message"">
                        {0}
                        </div>
                        </div>", CreateReport(exception.InnerException));
            }
            report += "<br/>";
            return report;
        }

        #endregion

        #region http post
        private bool HttpPost(string Url, string postDataStr)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                Stream myRequestStream = request.GetRequestStream();
                StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
                myStreamWriter.Write(postDataStr);
                myStreamWriter.Close();
                return true;
            }
            catch
            {
                return false;
            }

        }

        CookieContainer cookie = new CookieContainer();
        /// <summary>
        /// 推送数据2
        /// </summary>
        public string HttpPost2(string Url, string postDataStr)
        {
            string retString = null;
            try
            {
                postDataStr = postDataStr.Replace("+", "[jhjhjhjh[");//防止加号被替换成空格
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                request.CookieContainer = cookie;
                Stream myRequestStream = request.GetRequestStream();
                StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
                myStreamWriter.Write(postDataStr);
                myStreamWriter.Close();

                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    response = (HttpWebResponse)ex.Response;
                    //LogR.Logger.Error(ex, "request.GetResponse");
                    //throw ex;
                }
                response.Cookies = cookie.GetCookies(response.ResponseUri);
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
            }
            catch (Exception ex)
            {
                //LogR.Logger.Error(ex, "HttpPost");
                throw ex;
            }
            return retString;
        }

        private string GetReportJsonString()
        {
            return @"{""appname"":""" + _reportCrash.ApplicationTitle + @""",""version"":"""
                    + _reportCrash.ApplicationVersion + @""",""timestamp"":"""
                    + _reportCrash.TimeStamp.ToString() + @""",""os"":"""
                    + HelperMethods.GetWindowsVersion() + @""",""framework"":"""
                    + Environment.Version.ToString() + @""",""report"":"""
                    + HelperMethods.GetBase64FormString(HtmlReport()) + @"""}";                                                                                          
        }
        #endregion

        private bool SendReport(string report)
        {
            return HttpPost("URL", report);
        }

        private void ReportSuccess()
        {
            _progressDialog.Close();
            MessageBox.Show(
                string.Format(_resources.GetString("MessageSentMessage"),
                    _reportCrash.ApplicationTitle, _reportCrash.ApplicationVersion),
                _resources.GetString("MessageSentCaption"), MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ReportFailure(Exception exception)
        {
            _progressDialog.Close();
            MessageBox.Show(exception.Message, exception.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}