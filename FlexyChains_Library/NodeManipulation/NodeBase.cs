using FlexyChains_Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FlexyChains_Library
{
    public abstract class NodeBase : INodeManipulator
    {
        public abstract void DecryptNode();
        public XmlDocument XmlDocument { get; private set; }
        public XmlNode ParentNode { get; protected set; }
        public XmlNode OriginalParentNode { get; protected set; }
        public XmlNodeList ChildNodesList { get; protected set; }
        public string ParentNodeToString { get; private set; }
        public bool IsInitialParentNodeEncrypted { get; private set; }
        public bool IsNodeModified { get; protected set; }
        public IProtectionProvider ProtectionProvider { get; protected set; }

        private string _parentNodeName;
        protected string _childNodeName;

        protected NodeBase(string parentNodeName, string childNodeName = null)
        {
            _parentNodeName = parentNodeName;
            _childNodeName = childNodeName;
        }

        public void AddDocument(XmlDocument document)
        {
            XmlDocument = document;
            OriginalParentNode = XmlDocument.SelectSingleNode(_parentNodeName); //Keep original node
            ParentNode = XmlDocument.SelectSingleNode(_parentNodeName); // Keep a node copy to work with

            if (ParentNode == null)
            {
                throw new InvalidOperationException($"Can't find node {_parentNodeName}");
            }

            if (IsNodeEncrypted())
            {
                IsInitialParentNodeEncrypted = true;
                DecryptNode();
            }
            else
            {
                IsInitialParentNodeEncrypted = false;
            }

            SetParentNodeToString();
        }

        public XmlNode GetNode() => ParentNode;

        public XmlNodeList GetChildNodes()
        {
            if (IsNodeEncrypted())
                throw new InvalidOperationException("Node is encrypted! Desencrypt it first");

            if (ChildNodesList.Count == 0)
                throw new InvalidOperationException("No items found");

            return ParentNode.SelectNodes(_childNodeName);
        }

        public bool IsNodeEncrypted() => ParentNode.Attributes["configProtectionProvider"] != null;


        public void UpdateNodeContent(string newNodeContent, XmlNode oldNode, bool isChild = true)
        {
            if (oldNode == null || XmlDocument == null)
                throw new ArgumentNullException("oldNode or document cannot be null");

            if (oldNode.OwnerDocument != XmlDocument)
                throw new InvalidOperationException("oldNode belongs to a different document");

            if (isChild)
            {
                UpdateFullChildNode(newNodeContent, oldNode);
            }
            else
            {
                UpdateParentTagNode(newNodeContent, oldNode);
            }

            IsNodeModified = true;
        }

        private void UpdateFullChildNode(string newNodeContent, XmlNode oldNode)
        {
            //Parse new content to get full elmenet instead of using innerXml
            XmlDocumentFragment fragment = XmlDocument.CreateDocumentFragment();
            fragment.InnerXml = newNodeContent;

            if (fragment.ChildNodes.Count == 1 && fragment.FirstChild.NodeType == XmlNodeType.Element)
            {
                // Asume new content is a full element
                XmlNode newNode = fragment.FirstChild;

                XmlNode parent = oldNode.ParentNode;
                if (parent == null)
                    throw new InvalidOperationException("oldNode has no parent and cannot be replaced");

                // Replace original node with new one (importing if necessary)
                parent.ReplaceChild(XmlDocument.ImportNode(newNode, true), oldNode);
            }
            //else
            //{
            //    // If fragment is not a single element, update inner XML directly.
            //    oldNode.InnerXml = newNodeContent;
            //}
        }

        private void UpdateParentTagNode(string openingTagWithAttributes, XmlNode oldNode)
        {
            if (oldNode == null)
                throw new ArgumentNullException(nameof(oldNode));
            if (oldNode.OwnerDocument == null)
                throw new InvalidOperationException("El nodo antiguo no está asociado a un documento.");
            if (oldNode.NodeType != XmlNodeType.Element)
                throw new InvalidOperationException("oldNode debe ser un elemento para actualizar sus atributos.");

            // 1. Parsear la etiqueta de apertura para extraer el nombre y atributos.
            //    Asumimos que openingTagWithAttributes contiene algo como:
            //    "<smtp deliveryMethod="Network" from="mymail@domain.com">"

            // Conviértelo en un XML “balanceado” para poder usar InnerXml:
            //   "<smtp ...> </smtp>"
            string openingTag = openingTagWithAttributes.Trim();

            // Obtener nombre de la etiqueta tras '<'
            var match = System.Text.RegularExpressions.Regex.Match(
                openingTag,
                @"^<\s*(?<tagName>[\w:_-]+)"  // Ajustar según tu caso
            );
            if (!match.Success)
                throw new InvalidOperationException("No se pudo extraer el nombre de la etiqueta de apertura.");

            string newTagName = match.Groups["tagName"].Value;

            // Verifica que el nombre no cambie
            if (!oldNode.Name.Equals(newTagName, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"El nombre del nodo cambió de '{oldNode.Name}' a '{newTagName}', y no está permitido."
                );
            }

            // Completa la etiqueta con el cierre para parsearlo:
            if (!openingTag.EndsWith(">"))
            {
                openingTag += ">";
            }
            // "<smtp ...></smtp>"
            string balancedXml = $"{openingTag}</{newTagName}>";

            // 2. Parsear en un DocumentFragment
            XmlDocument doc = oldNode.OwnerDocument;
            XmlDocumentFragment fragment = doc.CreateDocumentFragment();
            fragment.InnerXml = balancedXml;

            // 3. Esperar un único elemento
            if (!(fragment.FirstChild is XmlElement parsedElement))
                throw new InvalidOperationException("No se pudo parsear como un único elemento.");

            // 4. Sobrescribir atributos en caliente
            XmlElement oldElem = (XmlElement)oldNode;
            oldElem.Attributes.RemoveAll(); // elimina atributos antiguos

            // Copiar atributos “nuevos”
            foreach (XmlAttribute attr in parsedElement.Attributes)
            {
                // Importar para asegurarnos de que pertenezca al mismo documento
                XmlAttribute importedAttr = (XmlAttribute)doc.ImportNode(attr, true);
                oldElem.Attributes.Append(importedAttr);
            }

            // 5. No tocamos los hijos de oldNode => se mantienen.
            // oldNode = oldElem; // No hace falta reasignar; oldNode sigue apuntando al mismo objeto.

            //6. Actualizamos tag
            SetParentNodeToString();
        }

        protected void SetParentNodeToString()
        {
            ParentNodeToString = $"<{ParentNode.Name} {string.Join(" ", ParentNode.Attributes.Cast<XmlAttribute>().Select(a => $"{a.Name}=\"{a.Value}\""))}>";
        }

    }


    }
