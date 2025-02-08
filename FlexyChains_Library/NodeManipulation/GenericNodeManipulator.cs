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
    public class GenericNodeManipulator : NodeBase
    {
        
        public GenericNodeManipulator(
            string nodeName,
            string elementName
            ) : base(nodeName, elementName)
        {
            ProtectionProvider = new RsaProtector();
        }

        public override void DecryptNode()
        {
            try
            {

                ParentNode = ProtectionProvider.Desencrypt(ParentNode);
                ChildNodesList = ParentNode.SelectNodes(_childNodeName);

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
