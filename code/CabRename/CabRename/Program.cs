using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace CabRename
{
    class Program
    {
        static void Main(string[] args)
        {
            string xml = File.ReadAllText("_setup.xml");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNodeList list = doc.SelectNodes(@"/wap-provisioningdoc/characteristic[@type='FileOperation']/*/characteristic[@translation='install']");
            Directory.CreateDirectory("Extracted");
            foreach (XmlNode node in list)
            {
                string dest = node.Attributes["type"].Value;
                string source = node.FirstChild.FirstChild.Attributes["value"].Value;

                if (dest.EndsWith("lnk")) continue;

                File.Copy(source, Path.Combine("Extracted", dest), true);
            }

            //foreach (Match match in Regex.Matches(xml, @"<characteristic type=""([^""]+)"" translation=""install"">\n<characteristic type=""Extract"">\n<parm name=""Source"" value=""([^""]+)"" />"))
            //{
            //    File.Move(match.Groups[2].Value, match.Groups[1].Value);
            //}
        }
    }
}
