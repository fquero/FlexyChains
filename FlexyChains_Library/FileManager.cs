using System;
using System.IO;
using System.Xml;

namespace FlexyChains_Library
{
    /// <summary>
    /// Manages file operations for XML documents.
    /// </summary>
    public class FileManager
    {
        public string FilePath { get; private set; }
        public XmlDocument Document { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public FileManager(string filePath)
        {
            FilePath = filePath;

            try
            {
                Document = new XmlDocument();
                Document.Load(FilePath);
            }
            catch (IOException ex)
            {
                throw new IOException($"Error reading file: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new FormatException("Input file is not a valid XML file", ex);
            }
        }

        /// <summary>
        /// Saves the changes to the XML document.
        /// </summary>
        /// <param name="xmlModifiedDocument">The modified XML document.</param>
        /// <returns><c>true</c> if the changes were saved successfully; otherwise, <c>false</c>.</returns>
        public bool SaveChanges(XmlDocument xmlModifiedDocument)
        {
            try
            {
                xmlModifiedDocument.Save(FilePath);
                return true;
            }
            catch (IOException ex)
            {
                throw new IOException($"Error saving file: {ex.Message}", ex);
            }
        }
    }
}