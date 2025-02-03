using FlexyChains_Library.Interfaces;
using FlexyChains_Library.ProtectionProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FlexyChains_Library
{
    public class NodeConnectionStrings : NodeBase, INodeManipulator
    {
        public IProtectionProvider ProtectionProvider { get;  }

        public NodeConnectionStrings(
            string nodeName,
            string elementName
            ) : base(nodeName, elementName)
        {
            ProtectionProvider = new RsaProtector();
        }

        public void DecryptNode()
        {
            try
            {

                _node = ProtectionProvider.Desencrypt(_node);
                _items = _node.SelectNodes(_itemName);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        
        public XElement EditItem(XElement item)
        {
            throw new NotImplementedException();
        }

        

        

        
    }
}
