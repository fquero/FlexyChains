using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace FlexyChains_Library.Tests
{
    public class FileManagerTests
    {
        private readonly ITestOutputHelper _testConsoleHelper;
        private string _assetsPath;

        /// <summary>
        /// Helps with tests :)
        /// Use Example: _testConsoleHelper.WriteLine("Your testing message");
        /// </summary>
        /// <param name="outputHelper"></param>
        public FileManagerTests(ITestOutputHelper outputHelper)
        {
            _testConsoleHelper = outputHelper;
        }

        [Fact]
        public void Can_Access_And_Read_The_Given_File()
        {
            _assetsPath = GetAssetsPath();
            Assert.NotNull(_assetsPath);

            string filePath = Path.Combine(_assetsPath, "WebUnencrypted.config");
            Assert.True(File.Exists(filePath), $"Not found: {filePath}");

            FileManager fileManager = new FileManager(filePath);

            //Assert.False(string.IsNullOrWhiteSpace(fileManager.FileContent), "File content is empty or unreadable.");

        }

        /// <summary>
        /// Search the assets folder from de build directory so the tests files can be found
        /// </summary>
        /// <returns>Assets dir path from build directory</returns>
        private string GetAssetsPath()
        {
            // Get folder where test is being executed
            string currentPath = AppContext.BaseDirectory;
            
            // Going up 2 levels from build folder
            string projectRoot = Directory.GetParent(currentPath)?.Parent?.FullName;
            
            if (string.IsNullOrEmpty(projectRoot))
                throw new DirectoryNotFoundException("Can't find project root folder.");

            string assetsPath = Path.Combine(projectRoot, "assets");

            return assetsPath;
        }
    }

    
}
