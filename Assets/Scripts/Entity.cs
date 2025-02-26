using Assets.Scripts;
using UnityEngine;

public abstract class Entity : Killable
{
    [HideInInspector] public bool alreadyPushed = false;

    public abstract void Move();
    public override abstract void Death();
}
