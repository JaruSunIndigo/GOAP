﻿using System.Linq;
using CrashKonijn.Goap.Core.Interfaces;

namespace CrashKonijn.Goap.Classes.Validators
{
    public class ActionTargetValidator : IValidator<IAgentTypeConfig>
    {
        public void Validate(IAgentTypeConfig agentTypeConfig, IValidationResults results)
        {
            var missing = agentTypeConfig.Actions.Where(x => x.Target == null).ToArray();
            
            if (!missing.Any())
                return;
            
            results.AddWarning($"Actions without Target: {string.Join(", ", missing.Select(x => x.Name))}");
        }
    }
}