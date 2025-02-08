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

            if(IsNodeEncrypted())
            {
                IsInitialParentNodeEncrypted = true;
                DecryptNode();
            } else
            {
                IsInitialParentNodeEncrypted = false;
            }

            ParentNodeToString = $"<{ParentNode.Name} {string.Join(" ", ParentNode.Attributes.Cast<XmlAttribute>().Select(a => $"{a.Name}=\"{a.Value}\""))}>";

            if (_childNodeName != null)
            {
                try
                {
                    ChildNodesList = ParentNode.SelectNodes(_childNodeName);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }
                
        public XmlNode GetNode() => ParentNode;
       
        public XmlNodeList GetChildNodes()
        {
            if(IsNodeEncrypted())
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
            else
            {
                // If fragment is not a single element, update inner XML directly.
                oldNode.InnerXml = newNodeContent;
            }

            IsNodeModified = true;
        }
    


    }
}
