using FlexyChains_Library.Interfaces;
using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using System.Xml;
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

        [Fact]
        public void Can_Copy_And_Encrypt_Sample_Web_Config_File()
        {
            //Copy the sample file to the test folder.
            string filePath = Copy_Sample_File();
            Assert.True(File.Exists(filePath), $"Not found: {filePath}");

            //Map the directory where the copied web.config is located.
            string configDirectory = Path.GetDirectoryName(filePath);
            WebConfigurationFileMap fileMap = new WebConfigurationFileMap();
            fileMap.VirtualDirectories.Add("/", new VirtualDirectoryMapping(configDirectory, true));

            //open the configuration of the copied web.config.
            Configuration config = WebConfigurationManager.OpenMappedWebConfiguration(fileMap, "/");

            // -------------------------------
            // Encrypt the connectionStrings section
            // -------------------------------
            ConfigurationSection connectionSection = config.GetSection("connectionStrings");
            if (connectionSection != null && !connectionSection.SectionInformation.IsProtected)
            {
                connectionSection.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                connectionSection.SectionInformation.ForceSave = true;
            }

            // -------------------------------
            // Encrypt mailSettings section
            // -------------------------------
            ConfigurationSection mailSettingsSection = config.GetSection("mailSettings");
            if (mailSettingsSection != null && !mailSettingsSection.SectionInformation.IsProtected)
            {
                mailSettingsSection.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                mailSettingsSection.SectionInformation.ForceSave = true;
            }

            // Save changes in web.config.
            config.Save(ConfigurationSaveMode.Modified);

            // -------------------------------
            // Verify that the web.config file has been encrypted.
            // Find the "configProtectionProvider" attribute that indicates the section is encrypted.
            string configContent = File.ReadAllText(filePath);
            Assert.Contains("configProtectionProvider", configContent);

            // -------------------------------
            //Verify using the configuration API that each section is protected.
            Configuration configReloaded = WebConfigurationManager.OpenMappedWebConfiguration(fileMap, "/");

            // connectionStrings
            var connectionSectionReloaded = configReloaded.GetSection("connectionStrings");
            Assert.True(connectionSectionReloaded.SectionInformation.IsProtected, "La sección connectionStrings no está encriptada");

            // mailSettings (if exists)
            var mailSettingsSectionReloaded = configReloaded.GetSection("mailSettings");
            if (mailSettingsSectionReloaded != null)
            {
                Assert.True(mailSettingsSectionReloaded.SectionInformation.IsProtected, "La sección mailSettings no está encriptada");
            }
        }


        [Fact]
        public void Can_Decrypt_Modify_And_Encrypt_Sample_Web_Config_File()
        {
            FileManager fileManager = new FileManager(Path.Combine(GetAssetsPath(), "filelab\\web.config"));

            //1. connectionStrings (1)
            string testConnString = "<add name=\"ConfConnectionString\" connectionString=\"Data Source=TEST_SERVER\\INSTANCE;Initial Catalog=TEST_DATABASE_IC;Persist Security Info=True;User ID=sa;Password=F3S-aZ123456;TrustServerCertificate=true\" providerName=\"System.Data.SqlClient\" />";
            NodeManipulatorFactory.GetManipulationIndex().TryGetValue(1, out ManipulationOption selectedOption);
            INodeManipulator connectionsNodeManipulator = NodeManipulatorFactory.Create(selectedOption.ManipulatorType, selectedOption.Arguments);
            connectionsNodeManipulator.AddDocument(fileManager.Document);
            XmlNode testConnStringNode = connectionsNodeManipulator.GetChildNodes()[1]; //Get the first connection string node
            connectionsNodeManipulator.UpdateNodeContent(
                testConnString
                , testConnStringNode
                , true
            );
            connectionsNodeManipulator.EncryptNode();
                 //1.1 Encrypt data
                 Assert.True(fileManager.SaveChanges(connectionsNodeManipulator.XmlDocument), "Filemanager could not save file");

                //1.2 Decrypt data
                connectionsNodeManipulator.DecryptNode();
                XmlNode verifyConnStringNode = connectionsNodeManipulator.GetChildNodes()[1]; //Get the first connection string node
                Assert.Equal(testConnString, verifyConnStringNode.OuterXml);

            //2. mailSettings (2)
            string testMailSettings = "<smtp deliveryMethod=\"Network\" from=\"test@domain.test\">";
            NodeManipulatorFactory.GetManipulationIndex().TryGetValue(2, out ManipulationOption selectedOption2);
            INodeManipulator mailSettingsNodeManipulator = NodeManipulatorFactory.Create(selectedOption2.ManipulatorType, selectedOption2.Arguments);
            mailSettingsNodeManipulator.AddDocument(fileManager.Document);
            XmlNode testEmailSettingsNode = mailSettingsNodeManipulator.ParentNode;
            mailSettingsNodeManipulator.UpdateNodeContent(
                testMailSettings
                , testEmailSettingsNode
                , false
            );
            mailSettingsNodeManipulator.EncryptNode();
            //2.1 Encrypt data
            Assert.True(fileManager.SaveChanges(mailSettingsNodeManipulator.XmlDocument), "Filemanager could not save file");
            //2.2 Decrypt data
            mailSettingsNodeManipulator.DecryptNode();
            XmlNode verifyEmailSettingsNode = mailSettingsNodeManipulator.ParentNode;
            Assert.Equal(testMailSettings, mailSettingsNodeManipulator.ParentNodeToString);


        }

        [Fact]
        public void Can_Modify_Not_Encrypted_File_And_Encrypt_Sample_Web_Config_File()
        {
            //Generate new clean copy
            Copy_Sample_File();

            FileManager fileManager = new FileManager(Path.Combine(GetAssetsPath(), "filelab\\web.config"));

            //1. connectionStrings (1)
            string testConnString = "<add name=\"ConfConnectionString\" connectionString=\"Data Source=CLEAN_TEST_SERVER\\INSTANCE;Initial Catalog=CLEAN_TEST_DATABASE_IC;Persist Security Info=True;User ID=sa;Password=F3S-aZ123456;TrustServerCertificate=true\" providerName=\"System.Data.SqlClient\" />";
            NodeManipulatorFactory.GetManipulationIndex().TryGetValue(1, out ManipulationOption selectedOption);
            INodeManipulator connectionsNodeManipulator = NodeManipulatorFactory.Create(selectedOption.ManipulatorType, selectedOption.Arguments);
            connectionsNodeManipulator.AddDocument(fileManager.Document);
            XmlNode testConnStringNode = connectionsNodeManipulator.GetChildNodes()[1]; //Get the first connection string node
            connectionsNodeManipulator.UpdateNodeContent(
                testConnString
                , testConnStringNode
                , true
            );
            connectionsNodeManipulator.EncryptNode();
            //1.1 Encrypt data
            Assert.True(connectionsNodeManipulator.IsNodeEncrypted(), "connectionsString not encrypted");
            Assert.True(fileManager.SaveChanges(connectionsNodeManipulator.XmlDocument), "Filemanager could not save file");

            ////1.2 Decrypt data
            connectionsNodeManipulator.DecryptNode();
            XmlNode verifyConnStringNode = connectionsNodeManipulator.GetChildNodes()[1]; //Get the first connection string node
            Assert.Equal(testConnString, verifyConnStringNode.OuterXml);

            ////2. mailSettings (2)
            string testMailSettings = "<smtp deliveryMethod=\"Network\" from=\"clean.test@domain.test\">";
            NodeManipulatorFactory.GetManipulationIndex().TryGetValue(2, out ManipulationOption selectedOption2);
            INodeManipulator mailSettingsNodeManipulator = NodeManipulatorFactory.Create(selectedOption2.ManipulatorType, selectedOption2.Arguments);
            mailSettingsNodeManipulator.AddDocument(fileManager.Document);
            XmlNode testEmailSettingsNode = mailSettingsNodeManipulator.ParentNode;
            mailSettingsNodeManipulator.UpdateNodeContent(
                testMailSettings
                , testEmailSettingsNode
                , false
            );
            mailSettingsNodeManipulator.EncryptNode();
            ////2.1 Encrypt data
            Assert.True(mailSettingsNodeManipulator.IsNodeEncrypted(), "mailsSettings not encrypted");
            Assert.True(fileManager.SaveChanges(mailSettingsNodeManipulator.XmlDocument), "Filemanager could not save file");
            //2.2 Decrypt data
            mailSettingsNodeManipulator.DecryptNode();
            Assert.Equal(testMailSettings, mailSettingsNodeManipulator.ParentNodeToString);


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

        private string Copy_Sample_File()
        {
            string originalFilePath = Path.Combine(GetAssetsPath(), "filelab\\web_CLEAN_EXAMPLE.config");
            string copyFilePath = Path.Combine(GetAssetsPath(), "filelab\\web.config");
            try
            {
                File.Copy(originalFilePath, copyFilePath, true);
                return copyFilePath;
            }
            catch (IOException ex)
            {
                throw new IOException($"Error copying file: {ex.Message}", ex);
            }
            
        }
    }

    
}
