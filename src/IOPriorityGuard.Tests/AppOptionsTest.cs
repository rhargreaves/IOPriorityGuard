using System.Diagnostics;
using System.Linq;
using CommandLine;
using IOPriorityGuard.Model;
using NUnit.Framework;

namespace IOPriorityGuard.Tests
{
    [TestFixture]
    public class AppOptionsTest
    {
        [SetUp]
        public void SetUp()
        {
            _options = new AppOptions();
        }

        private AppOptions _options;

        [Test]
        public void GetUsage_gets_non_empty_text()
        {
            // Act
            string usage = _options.GetUsage();

            // Assert
            Assert.IsNotEmpty(usage);
        }

        [Test]
        public void Parser_correctly_parses_base_priority_option()
        {
            // Arrange
            string[] args =
            {
                "-c",
                "Normal"
            };

            // Act
            Parser.Default.ParseArguments(args, _options);

            // Assert
            Assert.AreEqual(ProcessPriorityClass.Normal, _options.ProcessPriority);
        }

        [Test]
        public void Parser_correctly_parses_monitor_option()
        {
            // Arrange
            string[] args =
            {
                "-m"
            };

            // Act
            Parser.Default.ParseArguments(args, _options);

            // Assert
            Assert.True(_options.Monitor);
        }

        [Test]
        public void Parser_correctly_parses_priority_option()
        {
            // Arrange
            string[] args =
            {
                "-d",
                "Normal"
            };

            // Act
            Parser.Default.ParseArguments(args, _options);

            // Assert
            Assert.AreEqual(DiskIoPriorityClass.Normal, _options.DiskIoPriority);
        }

        [Test]
        public void Parser_correctly_parses_process_names_option()
        {
            // Arrange
            string[] args =
            {
                "-p",
                "exe1,exe2"
            };

            // Act
            Parser.Default.ParseArguments(args, _options);

            // Assert
            Assert.AreEqual(new[] {"exe1", "exe2"}, _options.ProcessNames);
        }

        [Test]
        public void Parser_returns_false_when_arguments_do_not_valid()
        {
            // Arrange\
            var args = new string[0];

            // Act
            bool result = Parser.Default.ParseArguments(args, _options);

            // Assert
            Assert.False(result);
        }
    }
}