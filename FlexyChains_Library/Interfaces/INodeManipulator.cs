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
        IProtectionProvider ProtectionProvider { get; }

        void AddDocument(XmlDocument document);
        XmlNode GetNode();
        bool IsNodeEncrypted();
        XmlNodeList GetItems();

        void DecryptNode();
        XElement EditItem(XElement item);
    }
}
