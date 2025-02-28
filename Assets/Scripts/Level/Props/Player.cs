using Assets.Scripts;
using FMODUnity;
using UnityEngine;

public class Player : Entity
{
    [Header("Sound Events")]
    [SerializeField] private EventReference onMove;
    [SerializeField] private EventReference onDeath;
    [SerializeField] private EventReference onOxygenUsed;
    [SerializeField] private EventReference onLift;
    [SerializeField] private EventReference onDrop;
    [SerializeField] private EventReference onInteract;
    [SerializeField] private EventReference onWait;
    [SerializeField] private EventReference onSlide;

    [HideInInspector] public MovementDirection Direction = MovementDirection.Down;

    public void AdvanceAnimation()
    {

    }

    public override void Move()
    {
        AudioManager.Instance.PlayOneShot(onMove, $"Player.{nameof(onMove)}");
    }

    public override void Death()
    {
        AudioManager.Instance.PlayOneShot(onDeath, $"Player.{nameof(onLift)}");
    }

    public void OxygenUsed()
    {
        AudioManager.Instance.PlayOneShot(onOxygenUsed, $"Player.{nameof(onLift)}");
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

    public void Slide()
    {
        AudioManager.Instance.PlayOneShot(onSlide, $"Player.{nameof(onSlide)}");
    }
}