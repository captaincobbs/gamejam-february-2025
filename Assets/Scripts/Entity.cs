using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [HideInInspector] public bool beenForciblyMoved;

    public abstract void OnMove();
    public abstract void OnDeath();
}
