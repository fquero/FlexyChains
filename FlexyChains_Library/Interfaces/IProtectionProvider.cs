using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FlexyChains_Library.Interfaces
{
    public interface IProtectionProvider
    {
        XmlNode Desencrypt(XmlNode node);
        bool Encrypt(XElement node);
    }
}
