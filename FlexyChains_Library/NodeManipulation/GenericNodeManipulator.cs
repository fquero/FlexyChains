using FlexyChains_Library.ProtectionProviders;
using System;
using System.Xml;

namespace FlexyChains_Library
{
    /// <summary>
    /// Provides generic node manipulation functionality.
    /// </summary>
    public class GenericNodeManipulator : NodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericNodeManipulator"/> class.
        /// </summary>
        /// <param name="nodeName">The name of the node.</param>
        /// <param name="elementName">The name of the element.</param>
        public GenericNodeManipulator(
            string nodeName,
            string elementName
            ) : base(nodeName, elementName)
        {
            ProtectionProvider = new RsaProtector();
        }

        /// <summary>
        /// Decrypts the node.
        /// </summary>
        public override void DecryptNode()
        {
            try
            {
                //1. Get the decrypted node (and if it's from another doc, import it).
                XmlNode decryptedNode = ProtectionProvider.Desencrypt(ParentNode);
                if (decryptedNode.OwnerDocument != XmlDocument)
                {
                    decryptedNode = XmlDocument.ImportNode(decryptedNode, true);
                }

                //2. Replace ParentNode content without replacing the instance.
                //  a) Delete current attributes and child nodes
                ParentNode.RemoveAll(); // Quita hijos y atributos

                //  b) Copy attributes from decrypted node
                foreach (XmlAttribute attr in decryptedNode.Attributes)
                {
                    // Add attribute to original node
                    ((XmlElement)ParentNode).SetAttribute(attr.Name, attr.Value);
                }

                //  c) copy each child from decrypted node
                foreach (XmlNode child in decryptedNode.ChildNodes)
                {
                    ParentNode.AppendChild(child.CloneNode(deep: true));
                }

                //Update the child nodes list.
                SetChildNodes();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Sets the child nodes.
        /// </summary>
        protected override void SetChildNodes()
        {
            if (_childNodeName != null)
            {
                try
                {
                    ChildNodesList = ParentNode.SelectNodes(_childNodeName);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Updates the full child node.
        /// </summary>
        /// <param name="newNodeContent">The new node content.</param>
        /// <param name="oldNode">The old node.</param>
        protected override void UpdateFullChildNode(string newNodeContent, XmlNode oldNode)
        {
            //Parse new content to get full elmenet instead of using innerXml
            XmlDocumentFragment fragment = XmlDocument.CreateDocumentFragment();
            fragment.InnerXml = newNodeContent;

            if (fragment.ChildNodes.Count == 1 && fragment.FirstChild.NodeType == XmlNodeType.Element)
            {
                // Asume new content is a full element
                XmlNode newNode = fragment.FirstChild;

                XmlNode parent = oldNode.ParentNode;
                if (parent == null)
                    throw new InvalidOperationException("oldNode has no parent and cannot be replaced");

                // Replace original node with new one (importing if necessary)
                parent.ReplaceChild(XmlDocument.ImportNode(newNode, true), oldNode);
            }
            else
            {
                // If fragment is not a single element, update inner XML directly.
                oldNode.InnerXml = newNodeContent;
            }
        }

        /// <summary>
        /// Updates the parent tag node.
        /// </summary>
        /// <param name="openingTagWithAttributes">The opening tag with attributes.</param>
        /// <param name="oldNode">The old node.</param>
        protected override void UpdateParentTagNode(string openingTagWithAttributes, XmlNode oldNode)
        {
            if (oldNode == null)
                throw new ArgumentNullException(nameof(oldNode));
            if (oldNode.OwnerDocument == null)
                throw new InvalidOperationException("El nodo antiguo no está asociado a un documento.");
            if (oldNode.NodeType != XmlNodeType.Element)
                throw new InvalidOperationException("oldNode debe ser un elemento para actualizar sus atributos.");

            // 1. Parse the opening tag to extract the name and attributes.
            //    assuming openingTagWithAttributes contains something like:
            //    "<smtp deliveryMethod="Network" from="mymail@domain.com">"

            // convert it into a “balanced” XML to be able to use InnerXml:
            //   "<smtp ...> </smtp>"
            string openingTag = openingTagWithAttributes.Trim();

            // Obtain tag name after '<'
            var match = System.Text.RegularExpressions.Regex.Match(
                openingTag,
                @"^<\s*(?<tagName>[\w:_-]+)"
            );
            if (!match.Success)
                throw new InvalidOperationException("Tag name couldn't be extracted from opening tag ");

            string newTagName = match.Groups["tagName"].Value;

            // Verifica que el nombre no cambie
            if (!oldNode.Name.Equals(newTagName, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Node name changed from '{oldNode.Name}' to '{newTagName}', and it is not allowed."
                );
            }

            // Complete the tag with the closing '>' if it's missing
            if (!openingTag.EndsWith(">"))
            {
                openingTag += ">";
            }
            // Example "<smtp ...></smtp>"
            string balancedXml = $"{openingTag}</{newTagName}>";

            // 2. Parse into a DocumentFragment
            XmlDocument doc = oldNode.OwnerDocument;
            XmlDocumentFragment fragment = doc.CreateDocumentFragment();
            fragment.InnerXml = balancedXml;

            // 3. Verify that the fragment contains a single element
            if (!(fragment.FirstChild is XmlElement parsedElement))
                throw new InvalidOperationException("Fragment could not be parsed like single element.");

            // 4. overwrite attributes in the old node
            XmlElement oldElem = (XmlElement)oldNode;
            oldElem.Attributes.RemoveAll(); // elimina atributos antiguos

            // Copy attributes from parsed element to old element
            foreach (XmlAttribute attr in parsedElement.Attributes)
            {
                // Import to ensure it belongs to the same document
                XmlAttribute importedAttr = (XmlAttribute)doc.ImportNode(attr, true);
                oldElem.Attributes.Append(importedAttr);
            }

            // 5. Don't touch oldNode children => they remain.

            //6. Update ParentNodeToString
            SetParentNodeToString();
        }

        /// <summary>
        /// Encrypts the node.
        /// </summary>
        public override void EncryptNode()
        {
            try
            {
                if (ProtectionProvider == null)
                    throw new InvalidOperationException("ProtectionProvider is not set");
                if (IsNodeEncrypted())
                    throw new InvalidOperationException("Node is already encrypted");

                ProtectionProvider.Encrypt(ParentNode);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Couldn't encrypt: {ex.Message}", ex);

            }

        }
    }
}