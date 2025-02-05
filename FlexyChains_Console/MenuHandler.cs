using FlexyChains_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        internal static int DisplayNodeContent(string parentNodeString, Dictionary<int, XmlNode> nodesDictionary = null)
        {
            try
            {
                PrintTitle("SHOWING NODE CONTENT", true);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("Parent: ");
                Console.ResetColor();
                Console.WriteLine(parentNodeString);
                PrintSeparator(ConsoleColor.Magenta);
                
                
                if (nodesDictionary != null)
                {
                    for(int i = 2; i < nodesDictionary.Count+2; i++) 
                    {
                        Console.WriteLine(nodesDictionary[i].OuterXml);
                        PrintSeparator(ConsoleColor.Magenta);
                    }
                }

                Console.WriteLine("");
                Console.WriteLine("[1]: Edit");
                Console.WriteLine("[2]: Back");
                
                int.TryParse(Console.ReadLine(), out int selection);
                return selection;


            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
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
