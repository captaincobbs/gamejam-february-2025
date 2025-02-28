using FMODUnity;
using UnityEngine;

public class WoodenBox : Entity
{
    [Header("Sound Events")]
    [SerializeField] private EventReference onMove;
    [SerializeField] private EventReference onDeath;
    public override void Move()
    {
        AudioManager.Instance.PlayOneShot(onMove, $"WoodenBox.{nameof(onMove)}");
    }

    public override void Death()
    {
        AudioManager.Instance.PlayOneShot(onDeath, $"WoodenBox.{nameof(onDeath)}");
    }
}
