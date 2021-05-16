using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Odbc;
using System.Data;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.Data.SqlClient;

using System.IO;
using MySql.Data.MySqlClient;
using System.Configuration;
using LogR;

namespace DataStorage
{
    /// <summary>
    /// SQL数据存储类
    /// </summary>
    public class DataStorageLib
    {
        ///本地数据库写入
        public static SqlConnection ocn1 = new SqlConnection();
        public static SqlCommand ocm1 = new SqlCommand();
        //本地查询
        public static SqlConnection ocn2 = new SqlConnection();
        public static SqlCommand ocm2 = new SqlCommand();
        public static SqlDataAdapter sda2 = new SqlDataAdapter();
        //本地查询2
        public static SqlConnection ocn3 = new SqlConnection();
        public static SqlCommand ocm3 = new SqlCommand();
        public static SqlDataAdapter sda3 = new SqlDataAdapter();
        //结果数据库
        public static SqlConnection ocnResult = new SqlConnection();
        public static SqlCommand ocmResult = new SqlCommand();
        public static SqlDataAdapter sdaResult = new SqlDataAdapter();
        public static bool UseResult = false;

        ///本地数据库写入
        public static MySqlConnection ocn1MySql = new MySqlConnection();
        public static MySqlCommand ocm1MySql = new MySqlCommand();
        //本地查询
        public static MySqlConnection ocn2MySql = new MySqlConnection();
        public static MySqlCommand ocm2MySql = new MySqlCommand();
        public static MySqlDataAdapter sda2MySql = new MySqlDataAdapter();
        //本地查询2
        public static MySqlConnection ocn3MySql = new MySqlConnection();
        public static MySqlCommand ocm3MySql = new MySqlCommand();
        public static MySqlDataAdapter sda3MySql = new MySqlDataAdapter();
        //结果数据库
        public static MySqlConnection ocnResultMySql = new MySqlConnection();
        public static MySqlCommand ocmResultMySql = new MySqlCommand();
        public static MySqlDataAdapter sdaResultMySql = new MySqlDataAdapter();

        //public static bool insertDisabled = false;
        public static string DBServerType = "SQL";
        public static string DBServerTypeCurve = "SQL";
        public static string DBServerTypeData = "SQL";

        static Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        /// <summary>
        /// 初始化数据库 +Shen
        /// </summary>
        public static bool initSQLServer(string SQLConStr, string ResultConStr = "")
        {
            if(config.AppSettings.Settings["DBType"]  != null)
                DBServerType = config.AppSettings.Settings["DBType"].Value;
            if (config.AppSettings.Settings["DBTypeC"] != null)
                DBServerTypeCurve = config.AppSettings.Settings["DBTypeC"].Value;
            if (config.AppSettings.Settings["DBTypeD"] != null)
                DBServerTypeData = config.AppSettings.Settings["DBTypeD"].Value;

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (DBServerType == "SQL")
                    {
                        ocn1.ConnectionString = SQLConStr;
                        ocn1.Open();
                        ocm1.Connection = ocn1;
                        ocm1.CommandType = CommandType.Text;

                        ocn2.ConnectionString = SQLConStr;
                        ocn2.Open();
                        ocm2.Connection = ocn2;
                        ocm2.CommandType = CommandType.Text;

                        ocn3.ConnectionString = SQLConStr;
                        ocn3.Open();
                        ocm3.Connection = ocn3;
                        ocm3.CommandType = CommandType.Text;
                    }
                    else
                    {
                        ocn1MySql.ConnectionString = SQLConStr;
                        ocn1MySql.Open();
                        ocm1MySql.Connection = ocn1MySql;
                        ocm1MySql.CommandType = CommandType.Text;

                        ocn2MySql.ConnectionString = SQLConStr;
                        ocn2MySql.Open();
                        ocm2MySql.Connection = ocn2MySql;
                        ocm2MySql.CommandType = CommandType.Text;

                        ocn3MySql.ConnectionString = SQLConStr;
                        ocn3MySql.Open();
                        ocm3MySql.Connection = ocn3MySql;
                        ocm3MySql.CommandType = CommandType.Text;
                    }

                    if (DBServerTypeData == "SQL")
                    {
                        if (!string.IsNullOrEmpty(ResultConStr))
                        {
                            ocnResult.ConnectionString = ResultConStr;
                            ocnResult.Open();
                            ocmResult.Connection = ocnResult;
                            ocmResult.CommandType = CommandType.Text;
                            UseResult = true;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ResultConStr))
                        {
                            ocnResultMySql.ConnectionString = ResultConStr;
                            ocnResultMySql.Open();
                            ocmResultMySql.Connection = ocnResultMySql;
                            ocmResultMySql.CommandType = CommandType.Text;
                            UseResult = true;
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    LogR.Logger.Error(ex, "initSQLServer");
                }

                Thread.Sleep(1000);
            }
            MessageBox.Show("数据库初始化失败");
            return false;
        }

        /// <summary>
        /// 插入本地数据库
        /// </summary>
        public static int insertSQLServer(string SQLstr)
        {
            try
            {
                if (DBServerType == "SQL")
                {
                    ocm1.CommandText = SQLstr;
                    return ocm1.ExecuteNonQuery();
                }
                else
                {
                    ocm1MySql.CommandText = SQLstr;
                    return ocm1MySql.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "insertSQLServer ->" + SQLstr);
            }

            return -1;
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        public static void SQLExecuteNonQuery(string SQLstr)
        {
            if (DBServerType == "SQL")
            {
                ocm1.CommandText = SQLstr;
                ocm1.ExecuteNonQuery();
            }
            else
            {
                ocm1MySql.CommandText = SQLstr;
                ocm1MySql.ExecuteNonQuery();
            }
        }

        public static DataTable selectSQLServer(string SQLt)
        {
            DataTable dt = new DataTable();
            try
            {
                if (DBServerType == "SQL")
                {
                    ocm2.CommandText = SQLt;
                    sda2.SelectCommand = ocm2;
                    sda2.Fill(dt);
                }
                else
                {
                    ocm2MySql.CommandText = SQLt;
                    sda2MySql.SelectCommand = ocm2MySql;
                    sda2MySql.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                LogR.Logger.Error(ex, "selectSQLSever RtDT");
            }
            
            return dt;
        }

        public static DataTable selectSQLServer2(string SQLt)
        {
            DataTable dt = new DataTable();
            try
            {
                if (DBServerType == "SQL")
                {
                    ocm3.CommandText = SQLt;
                    sda3.SelectCommand = ocm3;
                    sda3.Fill(dt);
                }
                else
                {
                    ocm3MySql.CommandText = SQLt;
                    sda3MySql.SelectCommand = ocm3MySql;
                    sda3MySql.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                LogR.Logger.Error(ex, "selectSQLSever2 RtDT");
            }

            return dt;
        }

        public static DataTable selectSQLServerResult(string SQLt)
        {
            DataTable dt = new DataTable();
            try
            {
                if (DBServerType == "SQL")
                {
                    ocmResult.CommandText = SQLt;
                    sdaResult.SelectCommand = ocmResult;
                    sdaResult.Fill(dt);
                }
                else
                {
                    ocmResultMySql.CommandText = SQLt;
                    sdaResultMySql.SelectCommand = ocmResultMySql;
                    sdaResultMySql.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                LogR.Logger.Error(ex, "selectSQLSeverResult RtDT");
            }

            return dt;
        }

        public static void SaveDataLog(string xml, int CurveType, string IP, string PRG)
        {
            try
            {
                //if (ConfigurationManager.AppSettings["SaveXML"].ToString() != "True")
                //    return;

                string Dir;
                if (CurveType == 1)
                    Dir = @"\ErrorXMLCurve\" + IP + "\\";
                else if (CurveType == 2)
                    Dir = @"\ErrorXML\" + IP + "\\";
                else if (CurveType == 3)
                    Dir = @"\ErrorXMLPLC\" + IP + "\\";
                else if (CurveType == 4)
                    Dir = @"\XMLDataPart\" + IP + "\\";
                else
                    Dir = @"\DataLog\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" + IP + "\\";

                string path = Application.StartupPath + Dir;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                StreamWriter sw;

                if (CurveType == 0)
                {
                    sw = new StreamWriter(path + DateTime.Now.ToString("yyyy-MM-dd-HH") + ".txt", true);
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd_HHmmss_fff") + " : " + xml);
                    sw.Close();
                }
                else
                {
                    sw = new StreamWriter(path + DateTime.Now.ToString("yyyy-MM-dd_HHmmss_fff") + ".txt");
                    sw.Write(xml);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                LogR.Logger.Error(ex, "SaveDataLog->" + IP);
            }
        }

        public static void AutoDeleteLog()
        {
            while (true)
            {
                try
                {
                    string path = Application.StartupPath + @"\DataLog\";
                    DateTime d = DateTime.Now.AddDays(-30);

                    if (Directory.Exists(path))
                    {
                        DirectoryInfo di = new DirectoryInfo(path);
                        DirectoryInfo[] diArr = di.GetDirectories();

                        foreach (DirectoryInfo ndi in diArr)
                        {
                            if (DateTime.Parse(ndi.Name) < d)
                            {
                                ndi.Delete(true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "AutoDeleteLog");
                }

                Thread.Sleep(21600 * 1000);//6小时执行一次
            }
        }

        public static string byteArr2Str(byte[] BinData)
        {
            string BinDataStr = "";
            try
            {
                foreach (byte b in BinData)
                    BinDataStr += b.ToString("X2");
            }
            catch { }
            return BinDataStr;
        }

        public static void CloseAllConn()
        {
            try
            {
                if (DBServerTypeData == "SQL")
                {
                    ocn1.Close();
                    ocn2.Close();
                }
                else
                {
                    ocn1MySql.Close();
                    ocn2MySql.Close();
                }
            }
            catch (Exception ex)
            {
                LogR.Logger.Error(ex, "CloseAllConn");
            }
        }
    }
}
