using FlexyChains_Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FlexyChains_Library
{
    /// <summary>
    /// Represents a manipulation option.
    /// </summary>
    public struct ManipulationOption
    {
        public string Name { get; set; }
        public Type ManipulatorType { get; set; }
        public object[] Arguments { get; set; }
    }

    /// <summary>
    /// Factory class for creating node manipulators.
    /// </summary>
    public static class NodeManipulatorFactory
    {
        /// <summary>
        /// Gets the manipulation index.
        /// </summary>
        /// <returns>A dictionary of manipulation options.</returns>
        public static Dictionary<int, ManipulationOption> GetManipulationIndex()
        {
            return new Dictionary<int, ManipulationOption>()
            {
                {
                    1,
                    new ManipulationOption
                    {
                        Name =  "connectionStrings",
                        ManipulatorType = typeof(GenericNodeManipulator),
                        Arguments = new object[] { "//connectionStrings", "add" }
                    }
                },
                {
                    2,
                    new ManipulationOption
                    {
                        Name =  "mailSettings",
                        ManipulatorType = typeof(GenericNodeManipulator),
                        Arguments = new object[] { "//smtp", "network" }
                    }
                }
            };
        }

        /// <summary>
        /// Creates an instance of the specified node manipulator type.
        /// </summary>
        /// <param name="manipulatorType">The type of the node manipulator.</param>
        /// <param name="additionalArgs">The additional arguments for the constructor.</param>
        /// <returns>An instance of the specified node manipulator type.</returns>
        /// <exception cref="ArgumentException">Thrown when the specified type does not implement <see cref="INodeManipulator"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no suitable constructor is found.</exception>
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

        /// <summary>
        /// Finds a suitable constructor for the specified arguments.
        /// </summary>
        /// <param name="constructors">The array of constructors.</param>
        /// <param name="additionalArgs">The additional arguments for the constructor.</param>
        /// <returns>The suitable constructor, or <c>null</c> if no suitable constructor is found.</returns>
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