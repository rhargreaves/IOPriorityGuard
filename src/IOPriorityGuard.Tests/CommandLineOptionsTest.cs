using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace IOPriorityGuard.Tests
{
    [TestFixture]
    public class CommandLineOptionsTest
    {
        private CommandLineOptions _options;

        [SetUp]
        public void SetUp()
        {
            _options = new CommandLineOptions();
        }

        [Test]
        public void GetUsage_gets_non_empty_text()
        {
            // Act
            var usage = _options.GetUsage();

            // Assert
            Assert.IsNotEmpty(usage);
        }
    }
}
