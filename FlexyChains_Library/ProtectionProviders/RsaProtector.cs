using FlexyChains_Library.Interfaces;
using System;
using System.Configuration;
using System.Web.Configuration;
using System.Xml;

namespace FlexyChains_Library.ProtectionProviders
{
    public class RsaProtector : IProtectionProvider
    {
        private readonly RsaProtectedConfigurationProvider _provider;

        public RsaProtector()
        {
            // Get provider from configuration
            _provider = (RsaProtectedConfigurationProvider)ProtectedConfiguration.Providers["RsaProtectedConfigurationProvider"];
            if (_provider == null)
            {
                throw new Exception("Couldn't get provider: RsaProtectedConfigurationProvider.");
            }
        }

        
        public XmlNode Desencrypt(XmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }
                

            try
            {
                // XmlNamespaceManager to manage namespace
                XmlNamespaceManager nsManager = new XmlNamespaceManager(new NameTable());
                nsManager.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");

                // Search EncryptedData node in namespace
                XmlNode encryptedNode = node.SelectSingleNode("enc:EncryptedData", nsManager) ?? throw new Exception("EncryptedData node not found");

                // Node desencryption
                XmlNode decryptedNode = _provider.Decrypt(encryptedNode);

                return decryptedNode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't desencrypt: {ex.Message}", ex);
            }
        }


        public void Encrypt(XmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            try
            {
                Console.WriteLine("Nodo antes de encriptar:");
                Console.WriteLine(node.OuterXml);

                if (_provider == null)
                {
                    throw new Exception("El proveedor de encriptación no está inicializado.");
                }

                // Aplicar la encriptación directamente al nodo
                XmlNode encryptedNode = _provider.Encrypt(node);
                if (encryptedNode == null)
                {
                    throw new Exception("El proveedor de encriptación devolvió un nodo nulo.");
                }

                // Crear un nuevo nodo que preserve el nombre original
                XmlDocument doc = node.OwnerDocument;
                XmlElement newParentNode = doc.CreateElement(node.Name);
                newParentNode.SetAttribute("configProtectionProvider", "RsaProtectedConfigurationProvider");

                // Importar el nodo encriptado dentro del nuevo nodo
                XmlNode importedEncryptedNode = doc.ImportNode(encryptedNode, true);
                newParentNode.AppendChild(importedEncryptedNode);

                // Reemplazar el nodo original con el nuevo nodo estructurado
                XmlNode parent = node.ParentNode;
                if (parent != null)
                {
                    parent.ReplaceChild(newParentNode, node);
                }
                else
                {
                    throw new Exception("El nodo original no tiene un nodo padre válido.");
                }

                Console.WriteLine("Nodo después de encriptar:");
                Console.WriteLine(newParentNode.OuterXml);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la encriptación: {ex.Message}");
                throw;
            }
        }





    }
}
