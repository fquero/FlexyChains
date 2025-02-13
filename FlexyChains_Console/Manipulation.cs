using FlexyChains_Library.Interfaces;
using FlexyChains_Library;
using System;
using System.Xml;
using System.Windows.Forms;
using FlexyChains_Library.NodeManipulation;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace FlexyChains
{
    /// <summary>
    /// Handles the manipulation of XML nodes in the FlexyChains console application.
    /// </summary>
    internal class Manipulation
    {
        private FileManager _fileManager;
        private INodeManipulator _manipulator;

        /// <summary>
        /// Initializes a new instance of the <see cref="Manipulation"/> class and starts the manipulation process.
        /// </summary>
        public Manipulation() => Start();

        /// <summary>
        /// Starts the manipulation process.
        /// </summary>
        public void Start()
        {
            try
            {
                string path = MenuHandler.SetFilePath();

                if (!Regex.IsMatch(path, @"web.config$"))
                {
                    Console.Clear();
                    MenuHandler.PrintError("Invalid input: path must resolve to a web.config file.");
                    Start();
                    return;
                }

                //Select file
                _fileManager = new FileManager(path);
                ManipulatorSelection();
            }
            catch (Exception ex)
            {
                MenuHandler.PrintError(ex.Message, true);
                Start();
            }
        }

        /// <summary>
        /// Prompts the user to select a manipulator and performs the manipulation.
        /// </summary>
        public void ManipulatorSelection()
        {
            int selectedOptionIndex = MenuHandler.SelectManipulatorType();

            if (selectedOptionIndex == 0)
            {
                Console.Clear();
                Start();
                return;
            }

            try
            {
                Console.Clear();
                if (!NodeManipulatorFactory.GetManipulationIndex().TryGetValue(selectedOptionIndex, out ManipulationOption selectedOption))
                {
                    throw new Exception("Invalid option selected");
                }

                _manipulator = NodeManipulatorFactory.Create(selectedOption.ManipulatorType, selectedOption.Arguments);
                _manipulator.AddDocument(_fileManager.Document);
                ShowNodeContent();
            }
            catch (Exception ex)
            {
                MenuHandler.PrintError(ex.Message, true);
                Console.WriteLine("Run as administrator to get access to RSA keys");
                Console.WriteLine("Restarting...");
                Start();
            }
        }

        /// <summary>
        /// Displays the content of the node and prompts the user to edit it.
        /// </summary>
        private void ShowNodeContent()
        {
            try
            {
                while (true)
                {
                    string selection = MenuHandler.DisplayNodeContent(_fileManager.FilePath, _manipulator.ParentNodeToString, _manipulator.GetChildNodes(), _manipulator.IsNodeModified);

                    if (selection.Equals("x", StringComparison.OrdinalIgnoreCase)) //faster and optimized comparison
                    {
                        ManipulatorSelection();
                        break;
                    }

                    if (selection.Equals("q", StringComparison.OrdinalIgnoreCase)) //faster and optimized comparison
                    {
                        Environment.Exit(0);
                        break;
                    }

                    if (selection.Equals("s", StringComparison.OrdinalIgnoreCase)) //faster and optimized comparison
                    {
                        SaveChanges();
                        break;
                    }

                    if (!int.TryParse(selection, out int selectedNodeIndex))
                    {
                        throw new Exception($"Invalid option selected: {selectedNodeIndex}");
                    }

                    if (selectedNodeIndex != 1 && _manipulator.GetChildNodes()[selectedNodeIndex - 2] == null)
                    {
                        throw new Exception($"Invalid option selected: {selectedNodeIndex}");
                    }

                    if (selectedNodeIndex == 1)
                    {
                        //Parent
                        EditNode(_manipulator.ParentNode, false);
                    }
                    else
                    {
                        //Child
                        EditNode(_manipulator.GetChildNodes()[selectedNodeIndex - 2], true);
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                MenuHandler.PrintError(ex.Message, true);
                Start();
            }
        }

        /// <summary>
        /// Prompts the user to edit the content of the specified node.
        /// </summary>
        /// <param name="node">The node to edit.</param>
        /// <param name="isChild">If set to <c>true</c>, indicates that the node is a child node.</param>
        /// <param name="error">The error message to display, if any.</param>
        private void EditNode(XmlNode node, bool isChild, string error = null)
        {
            if (isChild)
            {
                Clipboard.SetText(node.OuterXml); //Copy node content to clipboard
            }
            else
            {
                Clipboard.SetText(_manipulator.ParentNodeToString); //Copy node content to clipboard
            }

            string input = MenuHandler.EditNodeContent(error);

            // If user does not type, we keep initial value
            string newValue = string.IsNullOrWhiteSpace(input) ? node.OuterXml : input;

            //For security, clipboard is cleared
            Clipboard.Clear();

            string option = MenuHandler.SaveNodeMenu(newValue);

            Console.Clear();
            switch (option)
            {
                case "n":
                    SaveNode(node, newValue, isChild);
                    ShowNodeContent();
                    break;
                case "s":
                    SaveNode(node, newValue, isChild);
                    SaveChanges();
                    break;
                case "x":
                    ShowNodeContent();
                    break;
                default:
                    Console.WriteLine("Invalid selection");
                    break;
            }
        }

        /// <summary>
        /// Saves the content of the specified node.
        /// </summary>
        /// <param name="node">The node to save.</param>
        /// <param name="newValue">The new value of the node.</param>
        /// <param name="isChild">If set to <c>true</c>, indicates that the node is a child node.</param>
        private void SaveNode(XmlNode node, string newValue, bool isChild)
        {
            if (!NodeEditor.IsValidXML(newValue, isChild))
            {
                throw new FormatException("Received value is not valid XML. Discarded");
            }

            try
            {
                _manipulator.UpdateNodeContent(newValue, node, isChild);
            }
            catch (Exception ex)
            {
                Console.Clear();
                MenuHandler.PrintError($"{ex.Message} : {ex}", true);
                ShowNodeContent();
            }
        }

        /// <summary>
        /// Saves the changes to the XML document and encrypts the node if necessary.
        /// </summary>
        private void SaveChanges()
        {
            if (_manipulator.IsNodeEncrypted())
            {
                Console.WriteLine("El nodo ya está encriptado. No se realizará la operación.");
                return;
            }

            try
            {
                // Encriptar el nodo
                _manipulator.EncryptNode();
                //Actualizar el archivo
                _fileManager.SaveChanges(_manipulator.XmlDocument);

                MenuHandler.PrintTitle("File updated and encypted :)", true);
                Console.WriteLine(_fileManager.FilePath);
                Console.WriteLine("Press any key to restart");
                Console.ReadKey();
                Console.Clear();
                Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante la encriptación: {ex.Message}");
            }
        }
    }
}