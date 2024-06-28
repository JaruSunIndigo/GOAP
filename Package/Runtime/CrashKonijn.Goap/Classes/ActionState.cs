﻿using CrashKonijn.Goap.Core.Interfaces;

namespace CrashKonijn.Goap.Classes
{
    public class ActionState : IActionState
    {
        public bool HasPerformed => this.RunState != null;
        public IAction Action { get; private set; }
        public IActionRunState RunState { get; set; }
        public IActionData Data { get; private set; }
        
        public void SetAction(IAction action, IActionData data)
        {
            this.Action = action;
            this.RunState = null;
            this.Data = data;
        }

        public void Reset()
        {
            this.Action = null;
            this.RunState = null;
            this.Data = null;
        }
    }
}