﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.PowerApps.TestEngine.Config;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.PowerApps.TestEngine.Tests.Config
{
    public class TestStateTests
    {
        private Mock<ITestConfigParser> MockTestConfigParser;

        public TestStateTests()
        {
            MockTestConfigParser = new Mock<ITestConfigParser>(MockBehavior.Strict);
        }

        private TestPlanDefinition GenerateTestPlanDefinition()
        {
            return new TestPlanDefinition()
            {
                Test = new TestDefinition()
                {
                    Name = "Test Name",
                    Persona = "User1",
                    AppLogicalName = Guid.NewGuid().ToString(),
                    TestSteps = "= 1+1"
                },
                TestSettings = new TestSettings()
                {
                    BrowserConfigurations = new List<BrowserConfiguration>()
                    {
                        new BrowserConfiguration()
                        {
                            Browser = "Chromium",
                        },
                        new BrowserConfiguration()
                        {
                            Browser = "Safari"
                        }
                    }
                },
                EnvironmentVariables = new EnvironmentVariables()
                {
                    Users = new List<UserConfiguration>()
                    {
                        new UserConfiguration()
                        {
                            PersonaName = "User1",
                            EmailKey = "User1Email",
                            PasswordKey = "User1Password"
                        }
                    }
                }
            };
        }

        [Fact]
        public void TestStateSuccessTest()
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";

            var testPlanDefinition = GenerateTestPlanDefinition();

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            state.ParseAndSetTestState(testConfigFile);

            var testDefinitions = state.GetTestDefinitions();
            Assert.NotNull(testDefinitions);
            Assert.Single(testDefinitions);
            var testDefinition = testDefinitions[0];
            Assert.Equal(testPlanDefinition.Test, testDefinition);

            var testSettings = state.GetTestSettings();
            Assert.NotNull(testSettings);
            Assert.Equal(testPlanDefinition.TestSettings, testSettings);

            var userConfiguration = state.GetUserConfiguration("User1");
            Assert.Equal(testPlanDefinition.EnvironmentVariables.Users[0], userConfiguration);

            Assert.Throws<InvalidOperationException>(() => state.GetUserConfiguration("NonExistentUser"));

            var environmentId = Guid.NewGuid().ToString();
            var tenantId = Guid.NewGuid().ToString();
            var cloud = "Prod";
            var outputDirectory = Guid.NewGuid().ToString();

            state.SetEnvironment(environmentId);
            Assert.Equal(environmentId, state.GetEnvironment());
            state.SetTenant(tenantId);
            Assert.Equal(tenantId, state.GetTenant());
            state.SetCloud(cloud);
            Assert.Equal(cloud, state.GetCloud());
            state.SetOutputDirectory(outputDirectory);
            Assert.Equal(outputDirectory, state.GetOutputDirectory());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ParseAndSetTestStateThrowsOnInvalidTestConfigFile(string? testConfigPath)
        {
            var state = new TestState(MockTestConfigParser.Object);
            Assert.Throws<ArgumentNullException>(() => state.ParseAndSetTestState(testConfigPath));
        }

        [Fact]
        public void ParseAndSetTestStateThrowsOnNoTestDefinition()
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.Test = null;

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ParseAndSetTestStateThrowsOnNoNameInTestDefinition(string testName)
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.Test.Name = testName;

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ParseAndSetTestStateThrowsOnNoPersonaInTestDefinition(string persona)
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.Test.Persona = persona;

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ParseAndSetTestStateThrowsOnNoAppLogicalNameInTestDefinition(string appLogicalName)
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.Test.AppLogicalName = appLogicalName;

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ParseAndSetTestStateThrowsOnNoTestStepsInTestDefinition(string testSteps)
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.Test.TestSteps = testSteps;

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Fact]
        public void ParseAndSetTestStateThrowsOnNoTestSettings()
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.TestSettings = null;

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Fact]
        public void ParseAndSetTestStateThrowsOnNullBrowserConfigurationInTestSettings()
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.TestSettings.BrowserConfigurations = null;

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Fact]
        public void ParseAndSetTestStateThrowsOnNoBrowserConfigurationInTestSettings()
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.TestSettings.BrowserConfigurations = new List<BrowserConfiguration>();

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ParseAndSetTestStateThrowsOnNoBrowserInBrowserConfigurationInTestSettings(string? browser)
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.TestSettings.BrowserConfigurations.Add(new BrowserConfiguration() { Browser = browser });

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Theory]
        [InlineData(null, 800)]
        [InlineData(800, null)]
        public void ParseAndSetTestStateThrowsOnInvalidScreenConfigInBrowserConfigurationInTestSettings(int? screenWidth, int? screenHeight)
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.TestSettings.BrowserConfigurations.Add(new BrowserConfiguration() { 
                Browser = "Chromium",
                ScreenWidth = screenWidth,
                ScreenHeight = screenHeight
            });

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Fact]
        public void ParseAndSetTestStateThrowsOnNoEnvironmentVariables()
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.EnvironmentVariables = null;

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Fact]
        public void ParseAndSetTestStateThrowsOnNullUserConfigurationInEnvironmentVariables()
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.EnvironmentVariables.Users = null;

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Fact]
        public void ParseAndSetTestStateThrowsOnNoUserConfigurationInEnvironmentVariables()
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.EnvironmentVariables.Users = new List<UserConfiguration>();

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ParseAndSetTestStateThrowsOnNoPersonaNameInUserConfigurationInEnvironmentVariables(string personaName)
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.EnvironmentVariables.Users[0].PersonaName = personaName;

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ParseAndSetTestStateThrowsOnNoEmailKeyInUserConfigurationInEnvironmentVariables(string emailKey)
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.EnvironmentVariables.Users[0].EmailKey = emailKey;

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ParseAndSetTestStateThrowsOnNoPasswordKeyInUserConfigurationInEnvironmentVariables(string passwordKey)
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.EnvironmentVariables.Users[0].PasswordKey = passwordKey;

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Fact]
        public void ParseAndSetTestStateThrowsOnTestDefinitionUserNotDefined()
        {
            var state = new TestState(MockTestConfigParser.Object);
            var testConfigFile = "testPlan.fx.yaml";
            var testPlanDefinition = GenerateTestPlanDefinition();
            testPlanDefinition.Test.Persona = Guid.NewGuid().ToString();

            MockTestConfigParser.Setup(x => x.ParseTestConfig(It.IsAny<string>())).Returns(testPlanDefinition);
            Assert.Throws<InvalidOperationException>(() => state.ParseAndSetTestState(testConfigFile));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetEnvironmentThrowsOnNullInput(string environment)
        {
            var state = new TestState(MockTestConfigParser.Object);
            Assert.Throws<ArgumentNullException>(() => state.SetEnvironment(environment));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetCloudThrowsOnNullInput(string cloud)
        {
            var state = new TestState(MockTestConfigParser.Object);
            Assert.Throws<ArgumentNullException>(() => state.SetCloud(cloud));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetTenantThrowsOnNullInput(string tenant)
        {
            var state = new TestState(MockTestConfigParser.Object);
            Assert.Throws<ArgumentNullException>(() => state.SetTenant(tenant));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetOutputDirectoryThrowsOnNullInput(string outputDirectory)
        {
            var state = new TestState(MockTestConfigParser.Object);
            Assert.Throws<ArgumentNullException>(() => state.SetOutputDirectory(outputDirectory));
        }
    }
}