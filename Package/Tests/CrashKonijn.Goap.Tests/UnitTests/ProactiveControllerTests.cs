﻿using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using NSubstitute;
using NUnit.Framework;
using Unity.Collections;

namespace CrashKonijn.Goap.UnitTests
{
    [TestFixture]
    public class ProactiveControllerTests
    {
        private IGoap goap;
        private ProactiveController proactiveController;
        private IGoapEvents events;
        private Dictionary<IAgentType, IAgentTypeJobRunner> typeRunners;
        private IMonoGoapActionProvider actionProvider;
        private IAgentType agentType;
        private IAgentTypeJobRunner runner;

        [SetUp]
        public void SetUp()
        {
            this.agentType = Substitute.For<IAgentType>();
            this.agentType.SensorRunner.Returns(Substitute.For<ISensorRunner>());

            this.actionProvider = Substitute.For<IMonoGoapActionProvider>();
            this.actionProvider.AgentType.Returns(this.agentType);

            this.events = Substitute.For<IGoapEvents>();
            this.runner = Substitute.For<IAgentTypeJobRunner>();

            this.typeRunners = new Dictionary<IAgentType, IAgentTypeJobRunner>()
            {
                { this.agentType, this.runner },
            };

            this.goap = Substitute.For<IGoap>();
            this.goap.Agents.Returns(new List<IMonoGoapActionProvider>());
            this.goap.AgentTypeRunners.Returns(this.typeRunners);
            this.goap.Events.Returns(this.events);

            this.proactiveController = new ProactiveController();
            
            // Unity sometimes thinks that a temporary job is leaking memory
            // This is not the case, so we ignore the message
            // This can trigger in any test, even the ones that don't use the Job system
            NativeLeakDetection.Mode = NativeLeakDetectionMode.Disabled;
        }

        [Test]
        public void OnUpdate_ResolveActionCalledWhenResolveTimeExpired()
        {
            // Arrange
            this.actionProvider.Receiver.Timers.Resolve.IsRunningFor(this.proactiveController.ResolveTime).Returns(true);

            this.goap.Agents.Returns(new List<IMonoGoapActionProvider>() { this.actionProvider });
            this.proactiveController.Initialize(this.goap);

            // Act
            this.proactiveController.OnUpdate();

            // Assert
            this.actionProvider.Received().ResolveAction();
        }

        [Test]
        public void OnUpdate_ResolveActionNotCalledWhenResolveTimeNotExpired()
        {
            // Arrange
            this.actionProvider = Substitute.For<IMonoGoapActionProvider>();
            this.actionProvider.Receiver.Timers.Resolve.IsRunningFor(this.proactiveController.ResolveTime).Returns(false);

            this.goap.Agents.Returns(new List<IMonoGoapActionProvider>() { this.actionProvider });
            this.proactiveController.Initialize(this.goap);

            // Act
            this.proactiveController.OnUpdate();

            // Assert
            this.actionProvider.DidNotReceive().ResolveAction();
        }

        [Test]
        public void OnUpdate_RunCalledOnAgentTypeRunners()
        {
            // Arrange
            this.proactiveController.Initialize(this.goap);

            // Act
            this.proactiveController.OnUpdate();

            // Assert
            this.runner.Received().Run(Arg.Any<IMonoGoapActionProvider[]>());
        }

        [Test]
        public void OnLateUpdate_CompleteCalledOnAgentTypeRunners()
        {
            // Arrange
            this.proactiveController.Initialize(this.goap);

            // Act
            this.proactiveController.OnLateUpdate();

            // Assert
            this.runner.Received().Complete();
        }
    }
}
