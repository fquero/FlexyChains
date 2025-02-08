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

        //public static void UpdateNodeContent(string newNodeContent, XmlNode oldNode, XmlDocument document)
        //{
        //    //Create new XmlNode
        //    XmlDocument doc = new XmlDocument();
        //    doc.LoadXml(newNodeContent);
        //    XmlNode newNode = doc.DocumentElement;

        //    // Import new node to document
        //    XmlNode importedNode = document.ImportNode(newNode, true);

        //    // Replace old node with new one
        //    if (oldNode.ParentNode != null)
        //    {
        //        oldNode.ParentNode.ReplaceChild(importedNode, oldNode);
        //    }


        //}

        public static void UpdateNodeContent(string newNodeContent, XmlNode oldNode, XmlDocument document)
        {
            if (oldNode == null || document == null)
                throw new ArgumentNullException("oldNode or document cannot be null");

            if (oldNode.ParentNode == null)
                throw new InvalidOperationException("The oldNode has no parent and cannot be replaced.");

            // 🔹 Asegurar que el nuevo nodo se cree dentro del mismo documento
            XmlDocument tempDoc = new XmlDocument();
            tempDoc.LoadXml(newNodeContent);
            XmlNode newNode = tempDoc.DocumentElement;

            // 🔹 Importar el nodo al documento original
            XmlNode importedNode = document.ImportNode(newNode, true);

            // 🔹 Verificar que `oldNode` realmente sigue en el documento antes de reemplazarlo
            if (!ReferenceEquals(oldNode.OwnerDocument, document))
                throw new InvalidOperationException("The oldNode does not belong to the target XmlDocument.");

            // 🔹 Reemplazar el nodo
            oldNode.ParentNode.ReplaceChild(importedNode, oldNode);
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
