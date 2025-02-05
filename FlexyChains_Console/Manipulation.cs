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

namespace FlexyChains_Console
{
    internal class Manipulation
    {
        private FileManager _fileManager;
        private INodeManipulator _manipulator;

        private XmlNode _parentNode;
        private String _parentNodeToString;
        private Dictionary<int, XmlNode> _nodesDictionary;

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
            CreateNodesDictionary();

            ShowNodeContent();

        }

        private void ShowNodeContent()
        {
            try
            {
                while(true)
                {
                    int option = MenuHandler.DisplayNodeContent(_parentNodeToString, _nodesDictionary);

                    try
                    {
                        if (option == 1)
                        {
                            NodeEditionMenu();
                            break;
                        }
                        else if (option == 2)
                        {
                            ManipulatorSelection();
                            break;
                        }
                        else
                        {
                            MenuHandler.PrintError("Invalid option selected");
                        }
                    }
                    catch (Exception ex)
                    {
                        MenuHandler.PrintError(ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CreateNodesDictionary()
        {
            if (_parentNode.ChildNodes.Count == 0)
                return;

            int nodeIndex = 2;//1 is the parent node
            _nodesDictionary = new Dictionary<int, XmlNode>();
            foreach (XmlNode node in _manipulator.GetItems())
            {
                _nodesDictionary.Add(nodeIndex, node);
                nodeIndex++;
            }
        }

        private void NodeEditionMenu()
        {
            //Console.WriteLine("Select the parent or child node to edit");
            //Console.WriteLine($"[1]: Parent: {_parentNodeToString}");
            //foreach (var option in _nodesDictionary)
            //{
            //    Console.WriteLine($"[{option.Key}]: {option.Value.OuterXml.Substring(0, 60)} ...");
            //}
            //Console.WriteLine("[x]: back");

            //string selection = Console.ReadLine();

            //if (selection == "x")
            //    EditOrBackMenu();
            try
            {
                MenuHandler.DisplayNodeContent(_parentNodeToString, _nodesDictionary);
            }
            catch (Exception ex)
            {
                MenuHandler.PrintError(ex.Message);
            }
            

            
            //int.TryParse(selection, out int intSelection);

            //if (intSelection == 1)
            //{
            //    Console.Clear();
            //    EditNode(_parentNode);
            //}


            //if (_nodesDictionary.TryGetValue(intSelection, out XmlNode editingNode))
            //{
            //    Console.Clear();
            //    EditNode(editingNode);
            //}
            //else
            //{
            //    Console.WriteLine("Invalid selection");
            //    NodeEditionMenu();
            //}
        }


        void EditNode(XmlNode node)
        {
            // Copiar automáticamente el contenido al portapapeles
            Clipboard.SetText(node.OuterXml);

            Console.WriteLine("El contenido XML ha sido copiado al portapapeles.");
            Console.WriteLine("Péguelo aquí (Ctrl + V o Cmd + V), edítelo si lo desea y presione Enter:");

            // Leer la entrada del usuario
            string input = Console.ReadLine();

            // Si el usuario no escribe nada, se mantiene el valor original
            string newValue = string.IsNullOrWhiteSpace(input) ? node.OuterXml : input;

            Console.WriteLine($"Valor final: {newValue}");
            Console.WriteLine("------------------------------");
            Console.WriteLine("Changes received but not saved  until file is updated");
            Console.WriteLine("[1]: Edit another node");
            Console.WriteLine("[2]: Save file and exit");

            int.TryParse(Console.ReadLine(), out int selection);
            switch (selection)
            {
                case 1:
                    NodeEditionMenu();
                    break;
                case 2:
                    SaveChanges();
                    break;
                default:
                    Console.WriteLine("Invalid selection");
                    EditNode(node);
                    break;
            }

        }

        private void EditOrBackMenu()
        {
            Console.WriteLine("[1]: Edit");
            Console.WriteLine("[2]: Back");
            int.TryParse(Console.ReadLine(), out int selection);

            Console.Clear();

            switch (selection)
            {
                case 1:
                    NodeEditionMenu();
                    break;
                case 2:
                    ManipulatorSelection();
                    break;
                default:
                    EditOrBackMenu();
                    break;
            }
        }


        private void SaveChanges()
        {
            Console.Clear();
            Console.WriteLine("@Todo: saving changes.....");
        }





    }
}
