using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BIW.DataConversionLibrary
{
    public class XmlHelper
    {
        XmlDocument xmlDoc;
        string filename;
        public XmlHelper()
        {
        }
        public XmlHelper(string file)
        {
            Open(file);
        }
        ///<summary>
        /// 创建一个XML文档
        ///</summary>
        ///<param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        ///<param name="rootNodeName">XML文档根节点名称(须指定一个根节点名称)</param>
        ///<param name="version">XML文档版本号(必须为:"1.0")</param>
        ///<param name="encoding">XML文档编码方式</param>
        ///<param name="standalone">该值必须是"yes"或"no",如果为null,Save方法不在XML声明上写出独立属性</param>
        ///<returns>成功返回true,失败返回false</returns>
        public bool Create(string xmlFileName, string rootNodeName, string version, string encoding, string standalone)
        {
            bool isSuccess = false;
            try
            {
                xmlDoc = new XmlDocument();
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration(version, encoding, standalone);
                XmlNode root = xmlDoc.CreateElement(rootNodeName);
                xmlDoc.AppendChild(xmlDeclaration);
                xmlDoc.AppendChild(root);
                filename = xmlFileName;
                xmlDoc.Save(xmlFileName);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                throw ex; //这里可以定义你自己的异常处理
            }
            return isSuccess;
        }


        public void Open(string file)
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(file);
            filename = file;
        }

        public void Save()
        {
            xmlDoc.Save(filename);
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时返回该属性值，否则返回串联值</param>
        /// <returns>string</returns>
        /**************************************************
        * 使用示列:
        * XmlHelper.Read(path, "/Node", "")
        * XmlHelper.Read(path, "/Node/Element[@Attribute='Name']", "Attribute")
        ************************************************/
        public string Read(string node, string attribute)
        {
            string value = "";
            try
            {
                XmlNode xn = xmlDoc.SelectSingleNode(node);
                value = (attribute.Equals("") ? xn.InnerText : xn.Attributes[attribute].Value);
            }
            catch { }
            return value;
        }

        public XmlNodeList GetNodeList(string xpath)
        {
            try
            {
                return xmlDoc.SelectSingleNode(xpath).ChildNodes;
            }
            catch /*(Exception ex)*/
            {
                return null;
                //throw ex; //这里可以定义你自己的异常处理
            }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="element">元素名，非空时插入新元素，否则在该元素中插入属性</param>
        /// <param name="attribute">属性名，非空时插入该元素属性值，否则插入元素值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
        * 使用示列:
        * XmlHelper.Insert(path, "/Node", "Element", "", "Value")
        * XmlHelper.Insert(path, "/Node", "Element", "Attribute", "Value")
        * XmlHelper.Insert(path, "/Node", "", "Attribute", "Value")
        ************************************************/
        public void Insert(string node, string element, string attribute, string value)
        {
            try
            {
                XmlNode xn = xmlDoc.SelectSingleNode(node);
                if (element.Equals(""))
                {
                    if (!attribute.Equals(""))
                    {
                        XmlElement xe = (XmlElement)xn;
                        xe.SetAttribute(attribute, value);
                    }
                }
                else
                {
                    if (value.Equals(""))
                    {
                        xn.AppendChild(xmlDoc.CreateElement(element));
                    }
                    else
                    {
                        XmlElement xe = xmlDoc.CreateElement(element);
                        if (attribute.Equals(""))
                            xe.InnerText = value;
                        else
                            xe.SetAttribute(attribute, value);
                        xn.AppendChild(xe);
                    }
                }
            }
            catch { }
        }
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时修改该节点属性值，否则修改节点值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
        * 使用示列:
        * XmlHelper.Insert("/Node", "", "Value")
        * XmlHelper.Insert( "/Node", "Attribute", "Value")
        ************************************************/
        public void Update(string node, string attribute, string value)
        {
            try
            {
                XmlNode xn = xmlDoc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xe.InnerText = value;
                else
                    xe.SetAttribute(attribute, value);
            }
            catch { }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时删除该节点属性值，否则删除节点值</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        /**************************************************
        * 使用示列:
        * XmlHelper.Delete(path, "/Node", "")
        * XmlHelper.Delete(path, "/Node", "Attribute")
        ************************************************/
        public void Delete(string node, string attribute)
        {
            try
            {
                XmlNode xn = xmlDoc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xn.ParentNode.RemoveChild(xn);
                else
                    xe.RemoveAttribute(attribute);
            }
            catch { }
        }

        public static string GetLangValue(string key, int LanguageType)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"DLL\Language.xml";
            XmlHelper xml = new XmlHelper(path);
            string str;
            switch (LanguageType)
            {
                case 0:
                    str = "/Language/Chinese/";
                    break;
                case 1:
                    str = "/Language/English/";
                    break;
                default:
                    str = "/Language/Chinese/";
                    break;
            }
            return xml.Read(str + key, "");
        }
    }
}
