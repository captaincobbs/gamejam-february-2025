using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Threading.Tasks.Sources;
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
    public override void OnMove()
    {
        AudioManager.PlayOneShot(onMove);
    }

    public override void OnDeath()
    {
        AudioManager.PlayOneShot(onDeath);
    }

    public void OnOxygenUsed()
    {
        AudioManager.PlayOneShot(onOxygenUsed);
    }

    public void OnOxygenRefilled()
    {
        AudioManager.PlayOneShot(onOxygenRefilled);
    }

    public void OnLift()
    {
        AudioManager.PlayOneShot(onLift);
    }

    public void OnDrop()
    {
        AudioManager.PlayOneShot(onDrop);
    }

    public void OnInteract()
    {
        AudioManager.PlayOneShot(onInteract);
    }

    public void OnWait()
    {
        AudioManager.PlayOneShot(onWait);
    }

    public void OnSlide()
    {
        AudioManager.PlayOneShot(onSlide);
    }
}