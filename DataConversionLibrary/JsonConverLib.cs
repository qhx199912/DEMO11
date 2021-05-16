using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace BIW.DataConversionLibrary
{
    public class JsonConverLib
    {
        /// <summary>
        /// json转实体
        /// </summary>
        public T JsonDeserialize<T>(string jsonString)
        {
            //string str = file;
            Regex r = new Regex(@"([A-Za-z]{1,}\s{1}){1,}[A-Za-z]{1,}""\:");
            MatchCollection Matches = r.Matches(jsonString);
            foreach (Match NextMatch in Matches)
            {
                jsonString = jsonString.Replace(NextMatch.Value, NextMatch.Value.Replace(" ", ""));
            }
            jsonString = jsonString.Replace("\r", "");

            var ser = new DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var obj = (T)ser.ReadObject(ms);
            return obj;
        }

        #region Model
        [DataContract]
        public class Model0
        {
            [DataMember]
            public string format { get; set; }
            [DataMember]
            public int noofchannels { get; set; }
            [DataMember]
            public List<Model1> channels { get; set; }
        }

        [DataContract]
        public class Model1
        {
            [DataMember]
            public string date { get; set; }
            [DataMember]
            public string channel { get; set; }
            [DataMember]
            public int nr { get; set; }
            [DataMember]
            public string nodeid { get; set; }
            [DataMember]
            public string cellid { get; set; }
            [DataMember]
            public string toolserial { get; set; }
            [DataMember]
            public string redundancytransducerserial { get; set; }
            [DataMember]
            public float nominaltorque { get; set; }
            [DataMember]
            public string torqueunit { get; set; }
            [DataMember]
            public float MCEfactor { get; set; }
            [DataMember]
            public int cycle { get; set; }
            [DataMember]
            public string idcode { get; set; }
            [DataMember]
            public int prgnr { get; set; }
            [DataMember]
            public string prgname { get; set; }
            [DataMember]
            public string prgdate { get; set; }
            [DataMember]
            public string result { get; set; }
            [DataMember]
            public string hardware { get; set; }
            [DataMember]
            public float totaltime { get; set; }
            [DataMember]
            public string lastcmd { get; set; }
            [DataMember]
            public int batchnr { get; set; }
            [DataMember]
            public int batchdirectionok { get; set; }
            [DataMember]
            public int batchdirectionnok { get; set; }
            [DataMember]
            public int batchcanceled { get; set; }
            [DataMember]
            public int batchmaxOK { get; set; }
            [DataMember]
            public int batchOK { get; set; }
            [DataMember]
            public int batchmaxNOK { get; set; }
            [DataMember]
            public int batchNOK { get; set; }
            [DataMember]
            public List<Model2> tighteningsteps { get; set; }
        }

        [DataContract]
        public class Model2
        {
            [DataMember]
            public int row { get; set; }
            [DataMember]
            public string column { get; set; }
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public int category { get; set; }
            [DataMember]
            public string result { get; set; }
            [DataMember]
            public string lastcmd { get; set; }
            [DataMember]
            public float speed { get; set; }
            [DataMember]
            public float torque { get; set; }
            [DataMember]
            public float angle { get; set; }
            [DataMember]
            public float time { get; set; }
            [DataMember]
            public float duration { get; set; }
            [DataMember]
            public float anglethresholdnom { get; set; }
            [DataMember]
            public float anglethresholdact { get; set; }
            [DataMember]
            public float torquethresholdact { get; set; }
            [DataMember]
            public List<Model3> tighteningfunctions { get; set; }
            [DataMember]
            public Model4 graph { get; set; }
        }

        [DataContract]
        public class Model3
        {
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public float nom { get; set; }
            [DataMember]
            public float act { get; set; }
        }

        [DataContract]
        public class Model4
        {
            [DataMember]
            public float[] anglevalues { get; set; }
            [DataMember]
            public float[] torquevalues { get; set; }
            [DataMember]
            public float[] timevalues { get; set; }
            [DataMember]
            public float[] gradientvalues { get; set; }
            [DataMember]
            public float[] torqueRedvalues { get; set; }
            [DataMember]
            public float[] angleRedvalues { get; set; }
        }
        #endregion
    }
}
