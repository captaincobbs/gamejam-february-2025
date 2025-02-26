using Assets.Scripts;
using com.cyborgAssets.inspectorButtonPro;
using FMODUnity;
using UnityEngine;

public class MetalBox : Killable
{
    [SerializeField] private EventReference onDeath;

    [ProPlayButton]
    public override void Death()
    {
        AudioManager.Instance.PlayOneShot(onDeath);
    }
}
