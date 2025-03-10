﻿using System;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using CrashKonijn.Goap.UnitTests.Classes;
using NSubstitute;
using NUnit.Framework;
using Unity.Collections;

namespace CrashKonijn.Goap.UnitTests
{
    public class SensorRunnerTests
    {
        [SetUp]
        public void Init()
        {
            // Unity sometimes thinks that a temporary job is leaking memory
            // This is not the case, so we ignore the message
            // This can trigger in any test, even the ones that don't use the Job system
            NativeLeakDetection.Mode = NativeLeakDetectionMode.Disabled;
        }
        
        [Test]
        public void SenseGlobal_GlobalSensor_CallsSense()
        {
            // Arrange
            var sensor = Substitute.For<IGlobalWorldSensor>();

            var runner = new SensorRunner(new IWorldSensor[] { sensor }, new ITargetSensor[] { }, new IMultiSensor[] { }, Substitute.For<IGlobalWorldData>());

            // Act
            runner.SenseGlobal();

            // Assert
            sensor.Received().Sense(Arg.Any<IWorldData>());
        }

        [Test]
        public void SenseGlobal_LocalSensor_DoesntCallsSense()
        {
            // Arrange
            var sensor = Substitute.For<ILocalWorldSensor>();

            var runner = new SensorRunner(new IWorldSensor[] { sensor }, new ITargetSensor[] { }, new IMultiSensor[] { }, Substitute.For<IGlobalWorldData>());

            // Act
            runner.SenseGlobal();

            // Assert
            sensor.DidNotReceive().Sense(Arg.Any<IWorldData>(), Arg.Any<IMonoAgent>(), Arg.Any<IComponentReference>());
        }

        [Test]
        public void SenseGlobal_MultiSensor_CallsSense()
        {
            // Arrange
            var sensor = Substitute.For<IMultiSensor>();
            sensor.LocalSensors.Returns(new Dictionary<Type, ILocalSensor>());
            sensor.GlobalSensors.Returns(new Dictionary<Type, IGlobalSensor>());

            var runner = new SensorRunner(new IWorldSensor[] { }, new ITargetSensor[] { }, new IMultiSensor[] { sensor }, Substitute.For<IGlobalWorldData>());

            // Act
            runner.SenseGlobal();

            // Assert
            sensor.Received().Sense(Arg.Any<IWorldData>());
        }

        [Test]
        public void SenseLocal_GlobalSensor_DoesntCallsSense()
        {
            // Arrange
            var sensor = Substitute.For<IGlobalWorldSensor>();

            var runner = new SensorRunner(new IWorldSensor[] { sensor }, new ITargetSensor[] { }, new IMultiSensor[] { }, Substitute.For<IGlobalWorldData>());

            var agent = Substitute.For<IMonoGoapActionProvider>();
            agent.WorldData.Returns(new LocalWorldData());

            // Act
            runner.SenseLocal(agent);

            // Assert
            sensor.DidNotReceive().Sense(Arg.Any<IWorldData>());
        }

        [Test]
        public void SenseLocal_LocalSensor_CallsSense()
        {
            // Arrange
            var sensor = Substitute.For<ILocalWorldSensor>();

            var runner = new SensorRunner(new IWorldSensor[] { sensor }, new ITargetSensor[] { }, new IMultiSensor[] { }, Substitute.For<IGlobalWorldData>());

            var agent = Substitute.For<IMonoGoapActionProvider>();
            agent.WorldData.Returns(new LocalWorldData());

            // Act
            runner.SenseLocal(agent);

            // Assert
            sensor.Received().Sense(Arg.Any<IWorldData>(), agent.Receiver, Arg.Any<IComponentReference>());
        }

        [Test]
        public void SenseLocal_MultiSensor_CallsSense()
        {
            // Arrange
            var sensor = Substitute.For<IMultiSensor>();
            sensor.LocalSensors.Returns(new Dictionary<Type, ILocalSensor>());
            sensor.GlobalSensors.Returns(new Dictionary<Type, IGlobalSensor>());

            var runner = new SensorRunner(new IWorldSensor[] { }, new ITargetSensor[] { }, new IMultiSensor[] { sensor }, Substitute.For<IGlobalWorldData>());

            var agent = Substitute.For<IMonoGoapActionProvider>();
            agent.WorldData.Returns(new LocalWorldData());

            // Act
            runner.SenseLocal(agent);

            // Assert
            sensor.Received().Sense(Arg.Any<IWorldData>(), agent.Receiver, Arg.Any<IComponentReference>());
        }

        [Test]
        public void SenseLocal_WithAgent_OnlyRunsMatchingKey()
        {
            // Arrange
            var key = new TestKey1();

            var matchedSensor = Substitute.For<ILocalWorldSensor>();
            matchedSensor.Key.Returns(key);

            var unMatchedSensor = Substitute.For<ILocalWorldSensor>();
            unMatchedSensor.Key.Returns(new TestKey2());

            var condition = Substitute.For<ICondition>();
            condition.WorldKey.Returns(key);

            var action = Substitute.For<IGoapAction>();
            action.Conditions.Returns(new[] { condition });

            var runner = new SensorRunner(new IWorldSensor[] { matchedSensor, unMatchedSensor }, new ITargetSensor[] { }, new IMultiSensor[] { }, Substitute.For<IGlobalWorldData>());

            // Act
            runner.SenseLocal(Substitute.For<IMonoGoapActionProvider>(), action);

            // Assert
            matchedSensor.Received().Sense(Arg.Any<IWorldData>(), Arg.Any<IActionReceiver>(), Arg.Any<IComponentReference>());
            unMatchedSensor.DidNotReceive().Sense(Arg.Any<IWorldData>(), Arg.Any<IActionReceiver>(), Arg.Any<IComponentReference>());
        }
    }
}
