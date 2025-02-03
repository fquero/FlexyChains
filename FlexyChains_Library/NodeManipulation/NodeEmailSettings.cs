using FlexyChains_Library.Interfaces;
using FlexyChains_Library.ProtectionProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FlexyChains_Library.NodeManipulation
{
    internal class NodeEmailSettings : NodeBase, INodeManipulator
    {
        public NodeEmailSettings(string nodeName, string itemName = null) : base(nodeName, itemName)
        {
        }

        public IProtectionProvider ProtectionProvider => new RsaProtector();


        public void DecryptNode()
        {
            throw new NotImplementedException();
        }

        public XElement EditItem(XElement item)
        {
            throw new NotImplementedException();
        }
    }
}
