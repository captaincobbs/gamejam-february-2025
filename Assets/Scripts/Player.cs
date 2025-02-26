using com.cyborgAssets.inspectorButtonPro;
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

    public override void Move()
    {
        AudioManager.Instance.PlayOneShot(onMove);
    }

    public override void Death()
    {
        AudioManager.Instance.PlayOneShot(onDeath);
    }

    public void OnOxygenUsed()
    {
        AudioManager.Instance.PlayOneShot(onOxygenUsed);
    }

    public void OnOxygenRefilled()
    {
        AudioManager.Instance.PlayOneShot(onOxygenRefilled);
    }

    public void OnLift()
    {
        AudioManager.Instance.PlayOneShot(onLift);
    }

    public void OnDrop()
    {
        AudioManager.Instance.PlayOneShot(onDrop);
    }

    public void OnInteract()
    {
        AudioManager.Instance.PlayOneShot(onInteract);
    }

    public void OnWait()
    {
        AudioManager.Instance.PlayOneShot(onWait);
    }

    public void OnSlide()
    {
        AudioManager.Instance.PlayOneShot(onSlide);
    }
}