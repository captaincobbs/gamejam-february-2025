using FMODUnity;
using UnityEngine;

public class Player : Entity
{
    [Header("Sound Events")]
    [SerializeField] private EventReference onMove;
    [SerializeField] private EventReference onDeath;
    [SerializeField] private EventReference onOxygenUsed;
    [SerializeField] private EventReference onOxygenRefilled;
    [SerializeField] private EventReference onLift;
    [SerializeField] private EventReference onDrop;
    [SerializeField] private EventReference onInteract;
    [SerializeField] private EventReference onWait;
    [SerializeField] private EventReference onSlide;

    [HideInInspector] public Vector2 Direction = Vector2.down;

    public void AdvanceAnimation()
    {

    }

    public override void Move()
    {
        AudioManager.Instance.PlayOneShot(onMove);
    }

    public override void Death()
    {
        AudioManager.Instance.PlayOneShot(onDeath);
    }

    public void OxygenUsed()
    {
        AudioManager.Instance.PlayOneShot(onOxygenUsed);
    }

    public void OxygenRefilled()
    {
        AudioManager.Instance.PlayOneShot(onOxygenRefilled);
    }

    public void Lift()
    {
        AudioManager.Instance.PlayOneShot(onLift);
    }

    public void Drop()
    {
        AudioManager.Instance.PlayOneShot(onDrop);
    }

    public void Interact()
    {
        AudioManager.Instance.PlayOneShot(onInteract);
    }

    public void Wait()
    {
        AudioManager.Instance.PlayOneShot(onWait);
    }

    public void Slide()
    {
        AudioManager.Instance.PlayOneShot(onSlide);
    }
}