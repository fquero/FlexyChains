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

        bool IsNodeModified { get; }
        IProtectionProvider ProtectionProvider { get; }

        void AddDocument(XmlDocument document);
        XmlNode GetNode();
        bool IsNodeEncrypted();
        XmlNodeList GetChildNodes();

        void DecryptNode();

        void UpdateNodeContent(string newNodeContent, XmlNode oldNode, bool isChild = true);

        void EncryptNode();

    }
}
