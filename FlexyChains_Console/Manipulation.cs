using FlexyChains_Library.Interfaces;
using FlexyChains_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlexyChains_Console
{
    internal class Manipulation
    {
       private FileManager _fileManager;
       private INodeManipulator _manipulator;

        public Manipulation()
        {
            InitialPrompt();
        }
        public void NodeManipulatorSelector()
        {
            Console.WriteLine("Select Manipulation Type:");
            foreach (var option in NodeManipulatorFactory.GetManipulationIndex())
            {
                Console.WriteLine($"[{option.Key}]: {option.Value.Name}");
            }
            Console.WriteLine("[0]: Cancel");

            int.TryParse(Console.ReadLine(), out int selectedOptionIndex);

            if (selectedOptionIndex == 0)
            {
                InitialPrompt();
                return;
            }

            NodeManipulatorFactory.GetManipulationIndex().TryGetValue(selectedOptionIndex, out ManipulationOption selectedOption);

            _manipulator = NodeManipulatorFactory.Create(selectedOption.ManipulatorType, selectedOption.Arguments);
            _manipulator.AddDocument(_fileManager.Document);

            StartManipulation();

        }

        public void InitialPrompt()
        {
            try
            {
                Console.WriteLine("Write FilePath to web.config:");
                Console.WriteLine("(Type 'quit' to exit)");
                string path = Console.ReadLine();

                if(path == "quit") Environment.Exit(0);
                
                //Select file
                _fileManager = new FileManager(path);
                NodeManipulatorSelector();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("-------------------------");
                InitialPrompt();
            }

        }


        private void StartManipulation()
        {
            
            if (_manipulator.IsNodeEncrypted())
            {
                Console.WriteLine("Node is encrypted. Do you want to desencrypt and read it? (y/n)");
                string election = (string)(Console.ReadLine());
                if ((string)election == "y")
                {
                    _manipulator.DecryptNode();
                    ShowItems();
                    return;
                }
                else
                {
                    InitialPrompt();
                }
            }
            else
            {
                ShowItems();
            }
        }

        public void ShowItems()
        {
            try
            {
                XmlNode parentNode = _manipulator.GetNode();
                Console.WriteLine($"Parent node: <{parentNode.Name} {string.Join(" ", parentNode.Attributes.Cast<XmlAttribute>().Select(a => $"{a.Name}=\"{a.Value}\""))}>");
                Console.WriteLine("--------------------------------------------");

                foreach (XmlNode node2 in _manipulator.GetItems())
                {
                    Console.WriteLine(node2.OuterXml);
                    Console.WriteLine("--------------------------------------------");
                }
                NodeManipulatorSelector();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
