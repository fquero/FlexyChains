using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FlexyChains_Library
{
    public abstract class NodeBase
    {
        protected XmlDocument _document;
        protected XmlNode _node;
        protected XmlNodeList _items;
        protected string _nodeName;
        protected string _itemName;
        
        protected NodeBase(string nodeName, string itemName = null)
        {
            _nodeName = nodeName;
            _itemName = itemName;
        }

        public void AddDocument(XmlDocument document)
        {
            _document = document;
            _node = _document.SelectSingleNode(_nodeName);

            if (_node == null)
            {
                throw new InvalidOperationException($"Can't find node {_nodeName}");
            }

            if (_itemName != null)
            {
                try
                {
                    _items = _node.SelectNodes(_itemName);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }
                
        public XmlNode GetNode() => _node;
       
        public XmlNodeList GetItems()
        {
            if(IsNodeEncrypted())
            {
                throw new InvalidOperationException("Node is encrypted! Desencrypt it first");
            }


            if (_items == null || _items.Count == 0)
            {
                throw new InvalidOperationException("No items found");
            }
            return _items;
        }

        public bool IsNodeEncrypted() => _node.Attributes["configProtectionProvider"] != null;


    }
}
