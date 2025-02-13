using FlexyChains_Library;
using System;
using System.Xml;

namespace FlexyChains
{
    /// <summary>
    /// Handles the menu interactions for the FlexyChains console application.
    /// </summary>
    internal class MenuHandler
    {
        /// <summary>
        /// Prints the title of the menu.
        /// </summary>
        /// <param name="title">The title to print.</param>
        /// <param name="clear">If set to <c>true</c>, clears the console before printing the title.</param>
        internal static void PrintTitle(string title, bool clear = false)
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

        /// <summary>
        /// Prints a separator line in the console.
        /// </summary>
        /// <param name="color">The color of the separator line.</param>
        /// <param name="slashesNum">The number of slashes in the separator line.</param>
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

        /// <summary>
        /// Prints an error message in the console.
        /// </summary>
        /// <param name="errorMessage">The error message to print.</param>
        /// <param name="clear">If set to <c>true</c>, clears the console before printing the error message.</param>
        internal static void PrintError(string errorMessage, bool clear = false)
        {
            if (clear)
            {
                Console.Clear();
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR -> {errorMessage}");
        }

        /// <summary>
        /// Prompts the user to set the file path for the web.config file.
        /// </summary>
        /// <returns>The file path entered by the user.</returns>
        internal static string SetFilePath()
        {
            PrintTitle("FlexyChains");

            Console.ResetColor();
            Console.WriteLine("Write or paste absolute path to web.config:");
            Console.WriteLine("(Type 'quit' to exit)");
            string path = Console.ReadLine();

            if (path.ToLower() == "quit") Environment.Exit(0);

            return path;
        }

        /// <summary>
        /// Prompts the user to select the type of manipulator.
        /// </summary>
        /// <returns>The index of the selected manipulator type.</returns>
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

        /// <summary>
        /// Displays the content of the node in the console.
        /// </summary>
        /// <param name="parentNodeString">The string representation of the parent node.</param>
        /// <param name="nodesList">The list of child nodes.</param>
        /// <param name="IsNodeEdited">If set to <c>true</c>, indicates that the node has been edited.</param>
        /// <returns>The user's selection.</returns>
        internal static string DisplayNodeContent(string parentNodeString, XmlNodeList nodesList = null, bool IsNodeEdited = false)
        {
            try
            {
                PrintTitle("SHOWING NODE CONTENT");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("[1] Parent: ");
                Console.ResetColor();
                Console.WriteLine(parentNodeString);
                PrintSeparator(ConsoleColor.Magenta);

                if (nodesList != null)
                {
                    for (int i = 0; i < nodesList.Count; i++)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write($"[{i + 2}] ");//<= 1 Parent node
                        Console.ResetColor();
                        Console.WriteLine(nodesList[i].OuterXml);
                        PrintSeparator(ConsoleColor.Magenta);
                    }
                }

                PrintSeparator(ConsoleColor.Magenta, 30);

                if (IsNodeEdited)
                {
                    Console.WriteLine("[s]: Save changes to file");
                    Console.WriteLine("[x]: Discard changes");
                }
                else
                {
                    Console.WriteLine("[x]: Back");
                }

                Console.WriteLine("[q]: Quit");

                Console.Write("[Node number to edit] : ");
                return Console.ReadLine();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Displays the save node menu in the console.
        /// </summary>
        /// <param name="nodeValue">The value of the node.</param>
        /// <returns>The user's selection.</returns>
        internal static string SaveNodeMenu(string nodeValue)
        {
            PrintSeparator(ConsoleColor.Green, 40);
            Console.WriteLine($"New value:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(nodeValue);
            PrintSeparator();
            Console.WriteLine("Changes received but not saved until file is updated");
            Console.WriteLine("[n]: Edit another node");
            Console.WriteLine("[s]: Save changes to file");
            Console.WriteLine("[x]: Discard changes and go back");
            return Console.ReadLine();
        }

        /// <summary>
        /// Prompts the user to edit the content of the node.
        /// </summary>
        /// <param name="error">The error message to display, if any.</param>
        /// <returns>The edited node content.</returns>
        internal static string EditNodeContent(string error = null)
        {
            if (error != null)
                PrintError(error);

            PrintTitle("NODE EDITION", true);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Node content copied to clipboard.");
            Console.WriteLine("Paste it here (Ctrl + V), edit at will and press Enter:");
            Console.ResetColor();
            PrintSeparator(ConsoleColor.Magenta);

            return Console.ReadLine();
        }
    }
}