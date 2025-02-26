using Assets.Scripts;
using com.cyborgAssets.inspectorButtonPro;
using FMODUnity;
using UnityEngine;

public class MetalBox : Killable
{
    [SerializeField] private EventReference onDeath;

    [ProButton]
    public override void Death()
    {

    }
}
