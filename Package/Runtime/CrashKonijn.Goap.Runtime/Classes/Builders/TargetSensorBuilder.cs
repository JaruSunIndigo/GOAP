﻿using System;
using CrashKonijn.Goap.Core;

namespace CrashKonijn.Goap.Runtime
{
    public class TargetSensorBuilder
    {
        private readonly TargetKeyBuilder targetKeyBuilder;
        private readonly TargetSensorConfig config;

        public TargetSensorBuilder(Type type, TargetKeyBuilder targetKeyBuilder)
        {
            this.targetKeyBuilder = targetKeyBuilder;
            this.config = new TargetSensorConfig()
            {
                Name = type.Name,
                ClassType = type.AssemblyQualifiedName,
            };
        }

        public TargetSensorBuilder SetTarget<TTarget>()
            where TTarget : ITargetKey
        {
            this.config.Key = this.targetKeyBuilder.GetKey<TTarget>();

            return this;
        }

        public ITargetSensorConfig Build()
        {
            return this.config;
        }

        public static TargetSensorBuilder Create<TTargetSensor>(TargetKeyBuilder targetKeyBuilder) where TTargetSensor : ITargetSensor
        {
            return new TargetSensorBuilder(typeof(TTargetSensor), targetKeyBuilder);
        }
    }
}
