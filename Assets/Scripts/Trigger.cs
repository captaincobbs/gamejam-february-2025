using FMODUnity;
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class Trigger
    {
        public Action Action;
        public Dictionary<SoundEventType, EventReference> EventReferences = new();
    }

    public enum SoundEventType
    {
        ConveyorAdvance,
        ConveyorReverse,
        ConveyorEnable,
        DoorEnable,
        MachineEnable,
        MachineCreate,
        LaserEnable,
        CollectorEnable,
        LifeSupportEnable,
        RoomDepressurize
    }

    public class TriggerEvent
    {
        public SoundEventType Type { get; }
        public EventReference SoundEvent { get; }

        public TriggerEvent(SoundEventType type, EventReference @event)
        {
            Type = type;
            SoundEvent = @event;
        }
    }
}
