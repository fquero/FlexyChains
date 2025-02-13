using System.Xml;

namespace FlexyChains_Library.Interfaces
{
    /// <summary>
    /// Interface for node manipulators.
    /// </summary>
    public interface INodeManipulator
    {
        XmlDocument XmlDocument { get; }
        XmlNode ParentNode { get; }
        XmlNodeList ChildNodesList { get; }
        string ParentNodeToString { get; }
        bool IsInitialParentNodeEncrypted { get; }

        bool IsNodeModified { get; }
        IProtectionProvider ProtectionProvider { get; }

        /// <summary>
        /// Adds the XML document to the node manipulator.
        /// </summary>
        /// <param name="document">The XML document.</param>
        void AddDocument(XmlDocument document);

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <returns>The parent node.</returns>
        XmlNode GetNode();

        /// <summary>
        /// Determines whether the node is encrypted.
        /// </summary>
        /// <returns><c>true</c> if the node is encrypted; otherwise, <c>false</c>.</returns>
        bool IsNodeEncrypted();

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        /// <returns>The child nodes.</returns>
        XmlNodeList GetChildNodes();

        /// <summary>
        /// Decrypts the node.
        /// </summary>
        void DecryptNode();

        /// <summary>
        /// Updates the content of the node.
        /// </summary>
        /// <param name="newNodeContent">The new node content.</param>
        /// <param name="oldNode">The old node.</param>
        /// <param name="isChild">if set to <c>true</c> [is child].</param>
        void UpdateNodeContent(string newNodeContent, XmlNode oldNode, bool isChild = true);

        /// <summary>
        /// Encrypts the node.
        /// </summary>
        void EncryptNode();
    }
}