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
        if (onMove.IsNull)
        {
            Debug.LogWarning("OnMove is null");
        }

        AudioManager.Instance.PlayOneShot(onMove);
    }

    public override void Death()
    {
        if (onDeath.IsNull)
        {
            Debug.LogWarning("OnDeath is null");
        }

        AudioManager.Instance.PlayOneShot(onDeath);
    }

    public void OxygenUsed()
    {
        if (onOxygenUsed.IsNull)
        {
            Debug.LogWarning("OnOxygenUsed is null");
        }

        AudioManager.Instance.PlayOneShot(onOxygenUsed);
    }

    public void Lift()
    {
        if (onLift.IsNull)
        {
            Debug.LogWarning("OnLift is null");
        }

        AudioManager.Instance.PlayOneShot(onLift);
    }

    public void Drop()
    {
        if (onDrop.IsNull)
        {
            Debug.LogWarning("OnDrop is null");
        }

        AudioManager.Instance.PlayOneShot(onDrop);
    }

    public void Interact()
    {
        if (onInteract.IsNull)
        {
            Debug.LogWarning("OnInteract is null");
        }

        AudioManager.Instance.PlayOneShot(onInteract);
    }

    public void Wait()
    {
        if (onWait.IsNull)
        {
            Debug.LogWarning("OnWait is null");
        }

        AudioManager.Instance.PlayOneShot(onWait);
    }

    public void Slide()
    {
        if (onSlide.IsNull)
        {
            Debug.LogWarning("OnSlide is null");
        }

        AudioManager.Instance.PlayOneShot(onSlide);
    }
}