using FlexyChains_Library.Interfaces;
using System;
using System.Linq;
using System.Xml;

namespace FlexyChains_Library
{
    /// <summary>
    /// Abstract base class for node manipulation.
    /// </summary>
    public abstract class NodeManipulatorBase : INodeManipulator
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

        /// <summary>
        /// Sets the string representation of the parent node.
        /// </summary>
        protected void SetParentNodeToString()
        {
            ParentNodeToString = $"<{ParentNode.Name} {string.Join(" ", ParentNode.Attributes.Cast<XmlAttribute>().Select(a => $"{a.Name}=\"{a.Value}\""))}>";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeManipulatorBase"/> class.
        /// </summary>
        /// <param name="parentNodeName">The name of the parent node.</param>
        /// <param name="childNodeName">The name of the child node.</param>
        protected NodeManipulatorBase(string parentNodeName, string childNodeName = null)
        {
            _parentNodeName = parentNodeName;
            _childNodeName = childNodeName;
        }

        /// <summary>
        /// Adds the XML document to the node manipulator.
        /// </summary>
        /// <param name="document">The XML document.</param>
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

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <returns>The parent node.</returns>
        public XmlNode GetNode() => ParentNode;

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        /// <returns>The child nodes.</returns>
        public XmlNodeList GetChildNodes()
        {
            if (IsNodeEncrypted())
                throw new InvalidOperationException("Node is encrypted! Desencrypt it first");

            if (ChildNodesList.Count == 0)
                throw new InvalidOperationException("No items found");

            return ParentNode.SelectNodes(_childNodeName);
        }

        /// <summary>
        /// Determines whether the node is encrypted.
        /// </summary>
        /// <returns><c>true</c> if the node is encrypted; otherwise, <c>false</c>.</returns>
        public bool IsNodeEncrypted() => ParentNode.Attributes["configProtectionProvider"] != null;

        /// <summary>
        /// Updates the content of the node.
        /// </summary>
        /// <param name="newNodeContent">The new node content.</param>
        /// <param name="oldNode">The old node.</param>
        /// <param name="isChild">if set to <c>true</c> [is child].</param>
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