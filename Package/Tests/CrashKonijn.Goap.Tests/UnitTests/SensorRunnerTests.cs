﻿using System.Collections.Generic;
using CrashKonijn.Goap.Behaviours;
using CrashKonijn.Goap.Classes.Runners;
using CrashKonijn.Goap.Configs;
using CrashKonijn.Goap.Configs.Interfaces;
using CrashKonijn.Goap.Interfaces;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace CrashKonijn.Goap.UnitTests
{
    public class SensorRunnerTests
    {
        // Global
        [Test]
        public void SenseGlobal_WithPositiveWorldSense_IsPresentInStates()
        {
            // Arrange
            var key = new WorldKey("test");
            
            var sensor = Substitute.For<IGlobalWorldSensor>();
            sensor.Sense().Returns(true);
            sensor.Key.Returns(key);
            
            var worldSensors = new IWorldSensor[] { sensor };
            var targetSensors = new ITargetSensor[] { };
            
            var runner = new SensorRunner(worldSensors, targetSensors);

            // Act
            var data = runner.SenseGlobal();

            // Assert
            data.States.Should().Contain(key);
        }
        
        [Test]
        public void SenseGlobal_WithNegativeWorldSense_IsNotPresentInStates()
        {
            // Arrange
            var key = new WorldKey("test");
            
            var sensor = Substitute.For<IGlobalWorldSensor>();
            sensor.Sense().Returns(false);
            sensor.Key.Returns(key);
            
            var worldSensors = new IWorldSensor[] { sensor };
            var targetSensors = new ITargetSensor[] { };
            
            var runner = new SensorRunner(worldSensors, targetSensors);

            // Act
            var data = runner.SenseGlobal();

            // Assert
            data.States.Should().NotContain(key);
        }
        
        [Test]
        public void SenseGlobal_WithPositiveTargetSense_IsPresentInTargets()
        {
            // Arrange
            var target = Substitute.For<ITarget>();
            var key = new TargetKey("test");
            
            var sensor = Substitute.For<IGlobalTargetSensor>();
            sensor.Sense().Returns(target);
            sensor.Key.Returns(key);
            
            var worldSensors = new IWorldSensor[] {  };
            var targetSensors = new ITargetSensor[] { sensor };
            
            var runner = new SensorRunner(worldSensors, targetSensors);

            // Act
            var data = runner.SenseGlobal();

            // Assert
            data.Targets.Should().ContainKey(key);
            data.Targets.Should().ContainValue(target);
        }
        
        [Test]
        public void SenseGlobal_WithNegativeTargetSense_IsPresentInTargets()
        {
            // Arrange
            var target = Substitute.For<ITarget>();
            var key = new TargetKey("test");
            
            var sensor = Substitute.For<IGlobalTargetSensor>();
            sensor.Sense().ReturnsNull();
            sensor.Key.Returns(key);
            
            var worldSensors = new IWorldSensor[] {  };
            var targetSensors = new ITargetSensor[] { sensor };
            
            var runner = new SensorRunner(worldSensors, targetSensors);

            // Act
            var data = runner.SenseGlobal();

            // Assert
            data.Targets.Should().ContainKey(key);
            data.Targets.Should().NotContainValue(target);
        }
        
        // Local
        [Test]
        public void SenseLocal_WithPositiveWorldSense_IsPresentInStates()
        {
            // Arrange
            var key = new WorldKey("test");
            
            var sensor = Substitute.For<ILocalWorldSensor>();
            sensor.Sense(Arg.Any<IMonoAgent>()).Returns(true);
            sensor.Key.Returns(key);
            
            var worldSensors = new IWorldSensor[] { sensor };
            var targetSensors = new ITargetSensor[] { };
            
            var runner = new SensorRunner(worldSensors, targetSensors);

            // Act
            var data = runner.SenseLocal(new GlobalWorldData
            {
                Targets = new Dictionary<ITargetKey, ITarget>(),
                States = new HashSet<IWorldKey>()
            }, Substitute.For<IMonoAgent>());

            // Assert
            data.States.Should().Contain(key);
        }
        
        [Test]
        public void SenseLocal_WithNegativeWorldSense_IsNotPresentInStates()
        {
            // Arrange
            var key = new WorldKey("test");
            
            var sensor = Substitute.For<ILocalWorldSensor>();
            sensor.Sense(Arg.Any<IMonoAgent>()).Returns(false);
            sensor.Key.Returns(key);
            
            var worldSensors = new IWorldSensor[] { sensor };
            var targetSensors = new ITargetSensor[] { };
            
            var runner = new SensorRunner(worldSensors, targetSensors);

            // Act
            var data = runner.SenseLocal(new GlobalWorldData
            {
                Targets = new Dictionary<ITargetKey, ITarget>(),
                States = new HashSet<IWorldKey>()
            }, Substitute.For<IMonoAgent>());

            // Assert
            data.States.Should().NotContain(key);
        }
        
        [Test]
        public void SenseLocal_WithPositiveTargetSense_IsPresentInTargets()
        {
            // Arrange
            var target = Substitute.For<ITarget>();
            var key = new TargetKey("test");
            
            var sensor = Substitute.For<ILocalTargetSensor>();
            sensor.Sense(Arg.Any<IMonoAgent>()).Returns(target);
            sensor.Key.Returns(key);
            
            var worldSensors = new IWorldSensor[] {  };
            var targetSensors = new ITargetSensor[] { sensor };
            
            var runner = new SensorRunner(worldSensors, targetSensors);

            // Act
            var data = runner.SenseLocal(new GlobalWorldData
            {
                Targets = new Dictionary<ITargetKey, ITarget>(),
                States = new HashSet<IWorldKey>()
            }, Substitute.For<IMonoAgent>());

            // Assert
            data.Targets.Should().ContainKey(key);
            data.Targets.Should().ContainValue(target);
        }
        
        [Test]
        public void SenseLocal_WithNegativeTargetSense_IsPresentInTargets()
        {
            // Arrange
            var target = Substitute.For<ITarget>();
            var key = new TargetKey("test");
            
            var sensor = Substitute.For<ILocalTargetSensor>();
            sensor.Sense(Arg.Any<IMonoAgent>()).ReturnsNull();
            sensor.Key.Returns(key);
            
            var worldSensors = new IWorldSensor[] {  };
            var targetSensors = new ITargetSensor[] { sensor };
            
            var runner = new SensorRunner(worldSensors, targetSensors);

            // Act
            var data = runner.SenseLocal(new GlobalWorldData
            {
                Targets = new Dictionary<ITargetKey, ITarget>(),
                States = new HashSet<IWorldKey>()
            }, Substitute.For<IMonoAgent>());

            // Assert
            data.Targets.Should().ContainKey(key);
            data.Targets.Should().NotContainValue(target);
        }
    }
}