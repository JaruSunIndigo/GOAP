﻿using System;
using System.Linq;
using CrashKonijn.Goap.Core.Interfaces;

namespace CrashKonijn.Goap.Behaviours
{
    public abstract class GoalBase : IGoal
    {
        private IGoalConfig config;
        public IGoalConfig Config => this.config;
        
        public Guid Guid { get; } = Guid.NewGuid();
        public IEffect[] Effects { get; } = {};
        public ICondition[] Conditions => this.config.Conditions.ToArray();

        public void SetConfig(IGoalConfig config)
        {
            this.config = config;
        }

        public virtual int GetCost(IWorldData data)
        {
            return this.config.BaseCost;
        }
    }
}