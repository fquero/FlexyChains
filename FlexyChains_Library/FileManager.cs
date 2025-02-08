﻿using FlexyChains_Library.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FlexyChains_Library
{
    public class FileManager
    {
        private readonly string _filePath;
        public XmlDocument Document { get; private set; }
        public FileManager(string filePath)
        {
            _filePath = filePath;

            try
            {
                Document = new XmlDocument();
                Document.Load(_filePath);
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

        
        
        
    }
}
