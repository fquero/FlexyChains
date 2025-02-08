using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FlexyChains_Library.Interfaces
{
    public interface INodeManipulator
    {
        XmlDocument XmlDocument { get; }
        XmlNode ParentNode { get; }
        XmlNodeList ChildNodesList { get; }
        string ParentNodeToString { get; }
        bool IsInitialParentNodeEncrypted { get; }
        IProtectionProvider ProtectionProvider { get; }

        void AddDocument(XmlDocument document);
        XmlNode GetNode();
        bool IsNodeEncrypted();
        XmlNodeList GetItems();

        void DecryptNode();
        
    }
}
