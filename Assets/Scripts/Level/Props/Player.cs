using FMODUnity;
using UnityEngine;

namespace Assets.Scripts.Level.Props
{
    public class Player : Entity
    {
        [SerializeField] private EventReference onOxygenUsed;
        [SerializeField] private EventReference onLift;
        [SerializeField] private EventReference onDrop;
        [SerializeField] private EventReference onInteract;
        [SerializeField] private EventReference onWait;

        [HideInInspector] public MovementDirection Direction = MovementDirection.Down;

        public void AdvanceAnimation()
        {

        }

        public override void Move()
        {
            AudioManager.Instance.PlayOneShot(onMove, $"Player.{nameof(onMove)}");
        }

        public override void Kill()
        {
            AudioManager.Instance.PlayOneShot(onKill, $"Player.{nameof(onKill)}");
        }

        public void OxygenUsed()
        {
            AudioManager.Instance.PlayOneShot(onOxygenUsed, $"Player.{nameof(onOxygenUsed)}");
        }

        public void Lift()
        {
            AudioManager.Instance.PlayOneShot(onLift, $"Player.{nameof(onLift)}");
        }

        public void Drop()
        {
            AudioManager.Instance.PlayOneShot(onDrop, $"Player.{nameof(onDrop)}");
        }

        public void Interact()
        {
            AudioManager.Instance.PlayOneShot(onInteract, $"Player.{nameof(onInteract)}");
        }

        public void Wait()
        {
            AudioManager.Instance.PlayOneShot(onWait, $"Player.{nameof(onWait)}");
        }
    }
}