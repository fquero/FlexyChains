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
        public static bool IsValidXML(string xmlContent)
        {
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
