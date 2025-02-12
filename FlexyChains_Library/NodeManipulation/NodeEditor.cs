using System;
using System.Xml;

namespace FlexyChains_Library.NodeManipulation
{
    /// <summary>
    /// Provides methods for editing XML nodes.
    /// </summary>
    public class NodeEditor
    {
        /// <summary>
        /// Validates if the received content is valid XML.
        /// </summary>
        /// <param name="xmlContent">The XML content to validate.</param>
        /// <param name="isChild">Indicates whether the content is a child node.</param>
        /// <returns><c>true</c> if the content is valid XML; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Creates a new node correctly within the original document.
        /// </summary>
        /// <param name="newNodeContent">The new node content.</param>
        /// <param name="document">The XML document.</param>
        /// <returns>The created node.</returns>
        private static XmlNode CreateNewNode(string newNodeContent, XmlDocument document)
        {
            XmlDocument tempDoc = new XmlDocument();
            tempDoc.LoadXml(newNodeContent);
            return tempDoc.DocumentElement;
        }
    }
}