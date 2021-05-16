//#define MulDB

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using MySql.Data.MySqlClient;
using LogR;
using DataStorage;
using BIW.PasswordSecurity;

namespace DataStorage
{
    public class NewDBLib
    {
        static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        static string connString = ConfigurationManager.ConnectionStrings["LocalDB"].ConnectionString;
        static string CurveConnString = string.Empty;
        static string ResultConnString = string.Empty;
        static bool isInit = false;

        static NewDBLib()
        {
            if (config.AppSettings.Settings["DBType"] != null)
                DataStorageLib.DBServerType = config.AppSettings.Settings["DBType"].Value;
#if MulDB
            if (config.AppSettings.Settings["DBTypeC"] != null)
                DataStorageLib.DBServerTypeCurve = config.AppSettings.Settings["DBTypeC"].Value;
            if (config.AppSettings.Settings["DBTypeD"] != null)
                DataStorageLib.DBServerTypeData = config.AppSettings.Settings["DBTypeD"].Value;
#endif
        }

        public NewDBLib()
        {
            if (!isInit)
            {
                string connStringDES = DesEncryption.DecryptDES(connString);
                connString = connStringDES == null ? connString : connStringDES;
#if MulDB
                DataTable dt = DBSelect("select * from TNSysConfig", DataStorageLib.DBServerType);
                if (dt.Rows.Count > 0)
                {
                    CurveConnString = dt.Select("Parameter='CurveDB'")[0]["Value"].ToString();
                    string CurveConnStringDES = DesEncryption.DecryptDES(CurveConnString);
                    CurveConnString = CurveConnStringDES == null ? CurveConnString : CurveConnStringDES;
                    ResultConnString = dt.Select("Parameter='DataDB'")[0]["Value"].ToString();
                    string ResultConnStringDES = DesEncryption.DecryptDES(ResultConnString);
                    ResultConnString = ResultConnStringDES == null ? ResultConnString : ResultConnStringDES;
                }
#endif
                isInit = true;
            }
        }

        public DataTable DBSelect(string sql, string DBType = "SQL", string linkType = "Local",
            List<KeyValuePair<string, object>> kvpLst = null)
        {
            DataTable dt = new DataTable();
            if (DBType == "SQL")
            {
                List<SqlParameter> sparam = new List<SqlParameter>();
                if (kvpLst != null)
                    foreach (KeyValuePair<string, object> kvp in kvpLst)
                        sparam.Add(new SqlParameter(kvp.Key, kvp.Value));

                if (linkType == "Curve")
                    dt = SQLSelect(sql, CurveConnString, sparam);
                else if (linkType == "Data")
                    dt = SQLSelect(sql, ResultConnString, sparam);
                else
                    dt = SQLSelect(sql, connString, sparam);
            }
            else
            {
                List<MySqlParameter> sparam = new List<MySqlParameter>();
                if (kvpLst != null)
                    foreach (KeyValuePair<string, object> kvp in kvpLst)
                        sparam.Add(new MySqlParameter(kvp.Key, kvp.Value));

                if (linkType == "Curve")
                    dt = MySQLSelect(sql, CurveConnString, sparam);
                else if (linkType == "Data")
                    dt = MySQLSelect(sql, ResultConnString, sparam);
                else
                    dt = MySQLSelect(sql, connString, sparam);
            }
            return dt;
        }

        public int DBNonQuery(string sql, string DBType = "SQL", string linkType = "Local",
            List<KeyValuePair<string, object>> kvpLst = null)
        {
            int exR = -1;
            if (DBType == "SQL")
            {
                List<SqlParameter> sparam = new List<SqlParameter>();
                if (kvpLst != null)
                    foreach (KeyValuePair<string, object> kvp in kvpLst)
                        sparam.Add(new SqlParameter(kvp.Key, kvp.Value));

                if (linkType == "Curve")
                    exR = SQLNonQuery(sql, CurveConnString, sparam);
                else if (linkType == "Data")
                    exR = SQLNonQuery(sql, ResultConnString, sparam);
                else
                    exR = SQLNonQuery(sql, connString, sparam);
            }
            else
            {
                List<MySqlParameter> sparam = new List<MySqlParameter>();
                if (kvpLst != null)
                    foreach (KeyValuePair<string, object> kvp in kvpLst)
                        sparam.Add(new MySqlParameter(kvp.Key, kvp.Value));

                if (linkType == "Curve")
                    exR = MySQLNonQuery(sql, CurveConnString, sparam);
                else if (linkType == "Data")
                    exR = MySQLNonQuery(sql, ResultConnString, sparam);
                else
                    exR = MySQLNonQuery(sql, connString, sparam);
            }
            return exR;
        }

        DataTable SQLSelect(string sql, string connStr, List<SqlParameter> sparam = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand(sql, con);
                    if (sparam != null)
                        cmd.Parameters.AddRange(sparam.ToArray());
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SQLSelect");
            }
            return dt;
        }

        int SQLNonQuery(string sql, string connStr, List<SqlParameter> sparam = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sql, con);
                    if (sparam != null)
                        cmd.Parameters.AddRange(sparam.ToArray());
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "SQLNonQuery");
            }
            return -1;
        }

        DataTable MySQLSelect(string sql, string connStr, List<MySqlParameter> sparam = null)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    if (sparam != null)
                        cmd.Parameters.AddRange(sparam.ToArray());
                    MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                    sda.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "MySQLSelect");
            }
            return dt;
        }

        int MySQLNonQuery(string sql, string connStr, List<MySqlParameter> sparam = null)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    if (sparam != null)
                        cmd.Parameters.AddRange(sparam.ToArray());
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "MySQLNonQuery");
            }
            return -1;
        }
    }
}
