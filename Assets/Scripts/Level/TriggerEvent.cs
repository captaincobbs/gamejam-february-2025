using FMODUnity;

namespace Assets.Scripts.Level
{
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
