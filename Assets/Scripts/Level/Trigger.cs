using FMODUnity;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Level
{
    public class Trigger
    {
        public Action Action;
        public Dictionary<SoundEventType, EventReference> EventReferences = new();
    }
}
