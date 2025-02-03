using FlexyChains_Library.Interfaces;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlexyChains_Library
{
    public struct ManipulationOption
    {
        public string Name { get; set; }
        public Type ManipulatorType { get; set; }

        public object[] Arguments { get; set; }
    }

    public static class NodeManipulatorFactory
    {

        public static Dictionary<int, ManipulationOption> GetManipulationIndex()
        {
            return new Dictionary<int, ManipulationOption>()
            {
                { 
                    1,
                    new ManipulationOption
                    { 
                        Name =  "connectionStrings",
                        ManipulatorType = typeof(NodeConnectionStrings),
                        Arguments = new object[] { "//connectionStrings", "add"}
                     } 
                },
                { 
                    2,
                    new ManipulationOption
                    { 
                        Name =  "mailSettings",
                        ManipulatorType = typeof(NodeConnectionStrings),
                        Arguments = new object[] { "//smtp", "network"}
                     } 
                }

            };

        }

        //public static INodeManipulator NewNodeManipulator(Type manipulatorType, XmlDocument document)
        //{
        //    if(!typeof(INodeManipulator).IsAssignableFrom(manipulatorType))
        //    {
        //        throw new ArgumentException("Received Type does not implement INodeManipulator", nameof(manipulatorType));
        //    }

        //    var manipulator = (INodeManipulator)Activator.CreateInstance(manipulatorType) ?? throw new ArgumentException($"Could not create {manipulatorType.FullName} instance.");
        //    manipulator.AddDocument(document);
        //    return manipulator;            

        //}

        public static INodeManipulator Create(Type manipulatorType, params object[] additionalArgs)
        {
            if (!typeof(INodeManipulator).IsAssignableFrom(manipulatorType))
            {
                throw new ArgumentException($"Received Type does not implement {nameof(INodeManipulator)}", nameof(manipulatorType));
            }

            // Find constructor
            var constructor = FindConstructor(manipulatorType.GetConstructors(), additionalArgs) ?? throw new InvalidOperationException($"No suitable constructor found for {manipulatorType.FullName} with provided arguments.");

            // Create instance with correct arguments
            var manipulator = (INodeManipulator)constructor.Invoke(additionalArgs);

            return manipulator;
        }

        private static ConstructorInfo FindConstructor(ConstructorInfo[] constructors, params object[] additionalArgs)
        {
            if (additionalArgs == null) additionalArgs = Array.Empty<object>();

            return constructors.FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters();
                    if (parameters.Length != additionalArgs.Length) return false;

                    return parameters.Select(p => p.ParameterType)
                                     .SequenceEqual(additionalArgs.Select(a => a?.GetType() ?? typeof(object)));
                });
        }



    }
}
