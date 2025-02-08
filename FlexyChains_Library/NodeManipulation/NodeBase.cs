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
        public XmlNodeList ChildNodesList { get; protected set; }
        public string ParentNodeToString { get; private set; }
        public bool IsInitialParentNodeEncrypted { get; private set; }
        public IProtectionProvider ProtectionProvider { get; protected set; }

        protected string _parentNodeName;
        protected string _childNodeName;

        protected NodeBase(string parentNodeName, string childNodeName = null)
        {
            _parentNodeName = parentNodeName;
            _childNodeName = childNodeName;
        }

        public void AddDocument(XmlDocument document)
        {
            XmlDocument = document;
            ParentNode = XmlDocument.SelectSingleNode(_parentNodeName);

            if (ParentNode == null)
            {
                throw new InvalidOperationException($"Can't find node {_parentNodeName}");
            }

            ParentNodeToString = $"<{ParentNode.Name} {string.Join(" ", ParentNode.Attributes.Cast<XmlAttribute>().Select(a => $"{a.Name}=\"{a.Value}\""))}>";
            
            if(IsNodeEncrypted())
            {
                IsInitialParentNodeEncrypted = true;
                
            }

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
       
        public XmlNodeList GetItems()
        {
            if(IsNodeEncrypted())
            {
                throw new InvalidOperationException("Node is encrypted! Desencrypt it first");
            }


            if (ChildNodesList == null || ChildNodesList.Count == 0)
            {
                throw new InvalidOperationException("No items found");
            }
            return ChildNodesList;
        }

        public bool IsNodeEncrypted() => ParentNode.Attributes["configProtectionProvider"] != null;


    }
}
