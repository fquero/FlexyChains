using FlexyChains_Library.Interfaces;
using FlexyChains_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;
using System.Windows.Forms;
using FlexyChains_Library.NodeManipulation;

namespace FlexyChains
{
    internal class Manipulation
    {
        private FileManager _fileManager;
        private INodeManipulator _manipulator;



        public Manipulation() => Start();

        public void Start()
        {
            try
            {
                Clipboard.SetText("C:\\Users\\franp\\source\\repos\\FlexyChains\\FlexyChains_Library.Tests\\assets\\my\\web.config");

                string path = MenuHandler.SetFilePath();

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


        public void ManipulatorSelection()
        {
            int selectedOptionIndex = MenuHandler.SelectManipulatorType();

            if (selectedOptionIndex == 0)
            {
                Start();
                return;
            }

            try
            {
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

        private void ShowNodeContent()
        {
            try
            {

                while (true)
                {
                    string selection = MenuHandler.DisplayNodeContent(_manipulator.ParentNodeToString, _manipulator.GetChildNodes(), _manipulator.IsNodeModified);

                    if (selection.Equals("x", StringComparison.OrdinalIgnoreCase)) //faster and optimized comparison
                    {
                        ManipulatorSelection();
                        break;
                    }

                    if(selection.Equals("q", StringComparison.OrdinalIgnoreCase)) //faster and optimized comparison
                    {
                        Environment.Exit(0);
                        break;
                    }
                    
                    if(selection.Equals("s", StringComparison.OrdinalIgnoreCase)) //faster and optimized comparison
                    {
                        SaveChanges();
                        break;
                    }

                    if(!int.TryParse(selection, out int selectedNodeIndex))
                    {
                        //Console.WriteLine($"Invalid option selected: {selectedNodeIndex}");
                        throw new Exception($"Invalid option selected: {selectedNodeIndex}");
                    }

                    //if (selectedNodeIndex != 1 && !_nodesList.Contains(_nodesList[selectedNodeIndex - 2]))
                    if (selectedNodeIndex != 1 && _manipulator.GetChildNodes()[selectedNodeIndex - 2] == null)
                    {
                        //Console.WriteLine($"Invalid option selected: {selectedNodeIndex}");
                        throw new Exception($"Invalid option selected: {selectedNodeIndex}");
                    }

                    if(selectedNodeIndex == 1)
                    {
                        //Parent
                        EditNode(_manipulator.ParentNode, false);
                    } else
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


        private void EditNode(XmlNode node, bool isChild, string error = null)
        {
            if (isChild)
            {
                Clipboard.SetText(node.OuterXml); //Copy node content to clipboard
            } else {

                Clipboard.SetText(_manipulator.ParentNodeToString); //Copy node content to clipboard
            }

            string input = MenuHandler.EditNodeContent(error);

            // If user does not type, we keep initial value
            string newValue = string.IsNullOrWhiteSpace(input) ? node.OuterXml : input;

            string option = MenuHandler.SaveNodeMenu(newValue);
            
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
                    //EditNode(node);
                    break;
            }

        }

        private void SaveNode(XmlNode node, string newValue, bool isChild)
        {
            if (!NodeEditor.IsValidXML(newValue, isChild))
            {
                throw new FormatException("Received value is not valid XML. Discarded");
            }
            
            try
            {
                //NodeEditor.UpdateNodeContent(newValue, node, _manipulator.XmlDocument);
                _manipulator.UpdateNodeContent(newValue, node, isChild);


            } catch (Exception ex)
            {
                MenuHandler.PrintError($"{ex.Message} : {ex}", true);
                ShowNodeContent();
            }
                
            
                
        }
        

        private void SaveChanges()
        {
            Console.Clear();
            Console.WriteLine("@Todo: saving changes to file.....");

            Console.WriteLine(_manipulator.ParentNode.OuterXml);
            //foreach(XmlNode childi in _manipulator.ChildNodesList)
            //{
            //    Console.WriteLine(childi.OuterXml);
            //}
        }





    }
}
