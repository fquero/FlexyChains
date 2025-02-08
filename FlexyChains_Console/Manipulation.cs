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

namespace FlexyChains_Console
{
    internal class Manipulation
    {
        private FileManager _fileManager;
        private INodeManipulator _manipulator;

        private XmlNode _parentNode;
        private string _parentNodeToString;
        private List<XmlNode> _nodesList;


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
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("-------------------------");
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
                NodeManipulatorFactory.GetManipulationIndex().TryGetValue(selectedOptionIndex, out ManipulationOption selectedOption);
                _manipulator = NodeManipulatorFactory.Create(selectedOption.ManipulatorType, selectedOption.Arguments);
                _manipulator.AddDocument(_fileManager.Document);
                StartManipulation();
            }
            catch (Exception ex)
            {
                Console.Clear();
                throw new Exception(ex.Message);
            }
            finally
            {
                Start();
            }
            

        }

        

        private void StartManipulation()
        {

            if (_manipulator.IsNodeEncrypted())
            {
                _manipulator.DecryptNode();
            }
            _parentNode = _manipulator.GetNode();

            _parentNodeToString = $"<{_parentNode.Name} {string.Join(" ", _parentNode.Attributes.Cast<XmlAttribute>().Select(a => $"{a.Name}=\"{a.Value}\""))}>";
            //CreateNodesList();
            ShowNodeContent();

        }

        private void ShowNodeContent()
        {

            try
            {

                while (true)
                {
                    string selection = MenuHandler.DisplayNodeContent(_parentNodeToString, _manipulator.GetItems());

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

                    if(!int.TryParse(selection, out int selectedNodeIndex))
                    {
                        //Console.WriteLine($"Invalid option selected: {selectedNodeIndex}");
                        throw new Exception($"Invalid option selected: {selectedNodeIndex}");
                    }

                    //if (selectedNodeIndex != 1 && !_nodesList.Contains(_nodesList[selectedNodeIndex - 2]))
                    if (selectedNodeIndex != 1 && _manipulator.GetItems()[selectedNodeIndex - 2] == null)
                    {
                        //Console.WriteLine($"Invalid option selected: {selectedNodeIndex}");
                        throw new Exception($"Invalid option selected: {selectedNodeIndex}");
                    }

                    EditNode(_manipulator.GetItems()[selectedNodeIndex - 2]);
                    break;

                }

            }
            catch (Exception ex)
            {
                
                Console.Clear();
                MenuHandler.PrintError(ex.Message);
                //Console.WriteLine(ex.Message);                
                ShowNodeContent();
                throw new Exception($"{ex.Message}");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>Original nodelist or edited if there have been changes</returns>
        //private List<XmlNode> GetNodeList() => _editedNodesList ?? _nodesList;
        //private List<XmlNode> GetNodeList() => _nodesList;
        
        //private List<XmlNode> GetNodeList() => _manipulator.GetItems().Cast<XmlNode>().ToList();

        //private void CreateNodesList()
        //{
        //    _nodesList = new List<XmlNode>();

        //    if (_parentNode.ChildNodes.Count == 0)
        //        return;

        //    _nodesList = _manipulator.GetItems().Cast<XmlNode>().ToList();
            
        //}

        private void EditNode(XmlNode node, string error = null)
        {
            Clipboard.SetText(node.OuterXml); //Copy node content to clipboard

            string input = MenuHandler.EditNodeContent(error);

            // If user does not type, we keep initial value
            string newValue = string.IsNullOrWhiteSpace(input) ? node.OuterXml : input;

            string option = MenuHandler.SaveNodeMenu(newValue);

            if(option.Equals("x",StringComparison.OrdinalIgnoreCase))
            {
                ShowNodeContent();
            }
            
            int.TryParse(option, out int selection);
            switch (selection)
            {
                case 1:
                    SaveNode(node, newValue);
                    _manipulator.AddDocument(_fileManager.Document);//Refresh nodes in INodeManipulator
                    ShowNodeContent();
                    break;
                case 2:
                    SaveChanges();
                    break;
                default:
                    Console.WriteLine("Invalid selection");
                    //EditNode(node);
                    break;
            }

        }

        private void SaveNode(XmlNode node, string newValue)
        {
            if (!NodeEditor.IsValidXML(newValue))
            {
                EditNode(node, "Received value is not valid XML. Discarded");
            }
            else
            {
                NodeEditor.UpdateNodeContent(newValue, node, _fileManager.Document);
                _manipulator.AddDocument(_fileManager.Document);
            }
                
        }
        

        //private void NodeEditionMenu()
        //{
        //    try
        //    {
        //        string selection = MenuHandler.DisplayNodeContent(_parentNodeToString, _nodesList);

        //        switch (selection.ToLower())
        //        {
        //            case "x":

        //                break;
        //        }

        //        int.TryParse(Console.ReadLine(), out int selectedNodeIndex);

        //    }
        //    catch (Exception ex)
        //    {
        //        MenuHandler.PrintError(ex.Message);
        //    }



        //    //int.TryParse(selection, out int intSelection);

        //    //if (intSelection == 1)
        //    //{
        //    //    Console.Clear();
        //    //    EditNode(_parentNode);
        //    //}


        //    //if (_nodesDictionary.TryGetValue(intSelection, out XmlNode editingNode))
        //    //{
        //    //    Console.Clear();
        //    //    EditNode(editingNode);
        //    //}
        //    //else
        //    //{
        //    //    Console.WriteLine("Invalid selection");
        //    //    NodeEditionMenu();
        //    //}
        //}


        //void EditNode(XmlNode node)
        //{
        //    // Copiar automáticamente el contenido al portapapeles
        //    Clipboard.SetText(node.OuterXml);

        //    Console.WriteLine("El contenido XML ha sido copiado al portapapeles.");
        //    Console.WriteLine("Péguelo aquí (Ctrl + V o Cmd + V), edítelo si lo desea y presione Enter:");

        //    // Leer la entrada del usuario
        //    string input = Console.ReadLine();

        //    // Si el usuario no escribe nada, se mantiene el valor original
        //    string newValue = string.IsNullOrWhiteSpace(input) ? node.OuterXml : input;

        //    Console.WriteLine($"Valor final: {newValue}");
        //    Console.WriteLine("------------------------------");
        //    Console.WriteLine("Changes received but not saved  until file is updated");
        //    Console.WriteLine("[1]: Edit another node");
        //    Console.WriteLine("[2]: Save file and exit");

        //    int.TryParse(Console.ReadLine(), out int selection);
        //    switch (selection)
        //    {
        //        case 1:
        //            NodeEditionMenu();
        //            break;
        //        case 2:
        //            SaveChanges();
        //            break;
        //        default:
        //            Console.WriteLine("Invalid selection");
        //            EditNode(node);
        //            break;
        //    }

        //}

        //private void EditOrBackMenu()
        //{
        //    Console.WriteLine("[1]: Edit");
        //    Console.WriteLine("[2]: Back");
        //    int.TryParse(Console.ReadLine(), out int selection);

        //    Console.Clear();

        //    switch (selection)
        //    {
        //        case 1:
        //            NodeEditionMenu();
        //            break;
        //        case 2:
        //            ManipulatorSelection();
        //            break;
        //        default:
        //            EditOrBackMenu();
        //            break;
        //    }
        //}


        private void SaveChanges()
        {
            Console.Clear();
            Console.WriteLine("@Todo: saving changes.....");
        }





    }
}
