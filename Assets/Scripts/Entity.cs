using Assets.Scripts;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public abstract class Entity : Killable
{
    [HideInInspector] public bool beenForciblyMoved;

    [ProPlayButton]
    public abstract void Move();
    [ProPlayButton]
    public override abstract void Death();
}
