using FlexyChains_Library.Interfaces;
using System;
using System.Configuration;
using System.Xml;

namespace FlexyChains_Library.ProtectionProviders
{
    /// <summary>
    /// Provides RSA encryption and decryption for XML nodes.
    /// </summary>
    public class RsaProtector : IProtectionProvider
    {
        private readonly RsaProtectedConfigurationProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaProtector"/> class.
        /// </summary>
        public RsaProtector()
        {
            // Get provider from configuration
            _provider = (RsaProtectedConfigurationProvider)ProtectedConfiguration.Providers["RsaProtectedConfigurationProvider"];
            if (_provider == null)
            {
                throw new Exception("Couldn't get provider: RsaProtectedConfigurationProvider.");
            }
        }

        /// <summary>
        /// Desencrypts the specified node.
        /// </summary>
        /// <param name="node">The node to desencrypt.</param>
        /// <returns>The desencrypted node.</returns>
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

        /// <summary>
        /// Encrypts the specified node.
        /// </summary>
        /// <param name="node">The node to encrypt.</param>
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
                XmlNode encryptedNode = _provider.Encrypt(node) ?? throw new Exception("El proveedor de encriptación devolvió un nodo nulo.");

                // Modificar el nodo original en lugar de crear uno nuevo
                node.RemoveAll(); // Elimina todos los hijos actuales

                if (node is XmlElement element)
                {
                    element.SetAttribute("configProtectionProvider", "RsaProtectedConfigurationProvider");
                }

                XmlNode importedEncryptedNode = node.OwnerDocument.ImportNode(encryptedNode, true);
                node.AppendChild(importedEncryptedNode);

                //Console.WriteLine("Nodo después de encriptar:");
                //Console.WriteLine(node.OuterXml);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la encriptación: {ex.Message}");
                throw;
            }
        }
    }
}