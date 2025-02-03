using FlexyChains_Library;
using FlexyChains_Library.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlexyChains_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Manipulation manipulation = new Manipulation();
                
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            
        }

        //private static INodeManipulator ManipulationSelector(XmlDocument document)
        //{
        //   Console.WriteLine("Select Manipulation Type:");
        //   foreach(var option in NodeManipulatorFactory.GetManipulationIndex())
        //    {
        //        Console.WriteLine($"[{option.Key}]: {option.Value.Name}");
        //    }
            
        //    int.TryParse(Console.ReadLine(), out int selectedOptionIndex);

        //    NodeManipulatorFactory.GetManipulationIndex().TryGetValue(selectedOptionIndex, out ManipulationOption selectedOption);

        //    INodeManipulator manipulator = NodeManipulatorFactory.Create(selectedOption.ManipulatorType, selectedOption.Arguments);
        //    manipulator.AddDocument(document);
        //    return manipulator;

        //}

        //private static void InitialPrompt()
        //{
        //    try
        //    {
        //        Console.WriteLine("FilePath:");
        //        string path = Console.ReadLine();
                
        //        //Select file
        //        FileManager file = new FileManager(path);

        //        //Select NodeManipulator
        //        INodeManipulator manipulator = ManipulationSelector(file.Document);

        //        if (manipulator.IsNodeEncrypted())
        //        {
        //            Console.WriteLine("Node is encrypted. Do you want to desencrypt it? (y/n)");
        //            string election = (string)(Console.ReadLine());
        //            if ((string)election == "y")
        //            {
        //                manipulator.DecryptNode();
        //                ShowItems(manipulator);
        //                return;
        //            }
        //            else
        //            {
        //                InitialPrompt();
        //            }
        //        }
        //        else
        //        {
        //            ShowItems(manipulator);
        //        }

                
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //        InitialPrompt();
        //    }

        //}

        //private static void ShowItems(INodeManipulator manipulator)
        //{
        //    try
        //    {
        //        XmlNode parentNode = manipulator.GetNode();
        //        Console.WriteLine($"Parent node: <{parentNode.Name} {string.Join(" ", parentNode.Attributes.Cast<XmlAttribute>().Select(a => $"{a.Name}=\"{a.Value}\""))}>");
        //        Console.WriteLine("--------------------------------------------");

        //        foreach (XmlNode node2 in manipulator.GetItems())
        //        {
        //            Console.WriteLine(node2.OuterXml);
        //            Console.WriteLine("--------------------------------------------");
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
    }


}