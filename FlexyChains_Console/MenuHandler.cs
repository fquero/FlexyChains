using FlexyChains_Library;
using System;
using System.Collections.Generic;
using System.Xml;

namespace FlexyChains_Console
{
    internal class MenuHandler
    {
        private static void PrintTitle(string title, bool clear = false)
        {
            if (clear)
            {
                Console.Clear();
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("");
            Console.WriteLine(title);
            PrintSeparator(ConsoleColor.Blue, 70);
            Console.ResetColor();
        }

        private static void PrintSeparator(ConsoleColor color = ConsoleColor.Blue, int slashesNum = 5)
        {
            string slashes = "";
            while (slashes.Length < slashesNum)
            {
                slashes += "-";
            }
            Console.ForegroundColor = color;
            Console.WriteLine(slashes);
            Console.ResetColor();

        }

        internal static void PrintError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
        }

        internal static string SetFilePath()
        {
            PrintTitle("FlexyChains");

            Console.ResetColor();
            Console.WriteLine("Write FilePath to web.config:");
            Console.WriteLine("(Type 'quit' to exit)");
            string path = Console.ReadLine();

            if (path.ToLower() == "quit") Environment.Exit(0);

            return path;
        }

        internal static int SelectManipulatorType()
        {

            PrintTitle("SELECT MANIPULATION TYPE", true);

            foreach (var option in NodeManipulatorFactory.GetManipulationIndex())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"[{option.Key}] ");
                Console.ResetColor();
                Console.WriteLine(option.Value.Name);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[0] ");
            Console.ResetColor();
            Console.WriteLine("Cancel");

            
            int.TryParse(Console.ReadLine(), out int selectedOptionIndex);

            return selectedOptionIndex;
        }

        internal static string DisplayNodeContent(string parentNodeString, XmlNodeList nodesList = null)
        {
            try
            {
                PrintTitle("SHOWING NODE CONTENT", true);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("[1] Parent: ");
                Console.ResetColor();
                Console.WriteLine(parentNodeString);
                PrintSeparator(ConsoleColor.Magenta);
                
                
                if (nodesList != null)
                {
                    for(int i = 0; i < nodesList.Count; i++) 
                    {
                        Console.ForegroundColor= ConsoleColor.Magenta;
                        Console.Write($"[{i+2}] ");//<= 1 Parent node
                        Console.ResetColor();
                        Console.WriteLine(nodesList[i].OuterXml);
                        PrintSeparator(ConsoleColor.Magenta);
                    }
                }

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Type node number to edit it");
                Console.ResetColor();
                PrintSeparator(ConsoleColor.Magenta, 30);
                Console.WriteLine("[x]: Back");
                Console.WriteLine("[q]: Quit");
                
                return Console.ReadLine();

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        internal static string SaveNodeMenu(string nodeValue)
        {
            PrintSeparator(ConsoleColor.Green,40);
            Console.WriteLine($"New value:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(nodeValue);
            PrintSeparator();
            Console.WriteLine("Changes received but not saved  until file is updated");
            Console.WriteLine("[1]: Save and edit another node");
            Console.WriteLine("[2]: Save and update web.config");
            Console.WriteLine("[x]: Discard changes");
            return Console.ReadLine();
        }

        internal static string EditNodeContent(string error = null)
        {
            if(error != null)
                PrintError(error);

            PrintTitle("NODE EDITION",true);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Node content copied to clipboard.");
            Console.WriteLine("Paste it here (Ctrl + V), edit at will and press Enter:");
            Console.ResetColor();
            PrintSeparator(ConsoleColor.Magenta);

            return Console.ReadLine();
        }


        private static void EditOrBackMenu()
        {
            
                Console.WriteLine("");
                Console.WriteLine("[1]: Edit");
                Console.WriteLine("[2]: Back");
            
        }



        //internal static int SelectItemToEdit(string parentNodeString)
        //{
        //    PrintTitle("Select the parent or child node to edit", true);
        //    Console.WriteLine($"[1]: Parent: {parentNodeString}");
        //    foreach (var option in _nodesDictionary)
        //    {
        //        Console.WriteLine($"[{option.Key}]: {option.Value.OuterXml.Substring(0, 60)} ...");
        //    }
        //    Console.WriteLine("[x]: back");

        //    string selection = Console.ReadLine();

        //}
    }

    
}
