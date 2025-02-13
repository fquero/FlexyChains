using System.Xml;

namespace FlexyChains_Library.Interfaces
{
    /// <summary>
    /// Interface for protection providers.
    /// </summary>
    public interface IProtectionProvider
    {
        /// <summary>
        /// Desencrypts the specified node.
        /// </summary>
        /// <param name="node">The node to desencrypt.</param>
        /// <returns>The desencrypted node.</returns>
        XmlNode Desencrypt(XmlNode node);

        /// <summary>
        /// Encrypts the specified node.
        /// </summary>
        /// <param name="node">The node to encrypt.</param>
        void Encrypt(XmlNode node);
    }
}