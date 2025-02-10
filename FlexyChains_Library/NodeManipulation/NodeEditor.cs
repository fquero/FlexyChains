using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlexyChains_Library.NodeManipulation
{
    public class NodeEditor
    {

        /// <summary>
        /// Validates if received content is valid XML
        /// </summary>
        /// <param name="xmlContent"></param>
        /// <returns></returns>
        public static bool IsValidXML(string xmlContent, bool isChild)
        {
            //Adds closing tag if it's not present because is parent node
            if (!isChild)
            {
                string tagName = xmlContent.Split(new[] { ' ', '>' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim('<');
                if (!xmlContent.Contains("</"))
                {
                    xmlContent += $"</{tagName}>";
                }
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }

        
        // Método auxiliar para crear un nodo correctamente dentro del documento original
        private static XmlNode CreateNewNode(string newNodeContent, XmlDocument document)
        {
            XmlDocument tempDoc = new XmlDocument();
            tempDoc.LoadXml(newNodeContent);
            return tempDoc.DocumentElement;
        }


    }
}
