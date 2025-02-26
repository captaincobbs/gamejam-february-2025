using Assets.Scripts;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public abstract class Entity : Killable
{
    [HideInInspector] public bool beenForciblyMoved;

    [ProButton]
    public abstract void Move();
    [ProButton]
    public override abstract void Death();
}
