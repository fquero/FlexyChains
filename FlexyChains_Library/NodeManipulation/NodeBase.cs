using FlexyChains_Library.Interfaces;
using System;
using System.Linq;
using System.Xml;

namespace FlexyChains_Library
{
    public abstract class NodeBase : INodeManipulator
    {
        public abstract void DecryptNode();
        public XmlDocument XmlDocument { get; private set; }
        public XmlNode ParentNode { get; protected set; }
        public XmlNodeList ChildNodesList { get; protected set; }
        public string ParentNodeToString { get; private set; }
        public bool IsInitialParentNodeEncrypted { get; private set; }
        public bool IsNodeModified { get; protected set; }
        public IProtectionProvider ProtectionProvider { get; protected set; }

        private readonly string _parentNodeName;
        protected string _childNodeName;

        protected abstract void SetChildNodes();
        protected abstract void UpdateFullChildNode(string newNodeContent, XmlNode oldNode);
        protected abstract void UpdateParentTagNode(string openingTagWithAttributes, XmlNode oldNode);

        public abstract void EncryptNode();

        protected void SetParentNodeToString()
        {
            ParentNodeToString = $"<{ParentNode.Name} {string.Join(" ", ParentNode.Attributes.Cast<XmlAttribute>().Select(a => $"{a.Name}=\"{a.Value}\""))}>";
        }

        protected NodeBase(string parentNodeName, string childNodeName = null)
        {
            _parentNodeName = parentNodeName;
            _childNodeName = childNodeName;
        }

        public void AddDocument(XmlDocument document)
        {
            XmlDocument = document;
            ParentNode = XmlDocument.SelectSingleNode(_parentNodeName); // Keep a node copy to work with

            if (ParentNode == null)
            {
                throw new InvalidOperationException($"Can't find node {_parentNodeName}");
            }

            if (IsNodeEncrypted())
            {
                IsInitialParentNodeEncrypted = true;
                DecryptNode(); //Decrypts and add child nodes
            }
            else
            {
                IsInitialParentNodeEncrypted = false;
                SetChildNodes(); //Just add child nodes
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

        

        


    }
}
