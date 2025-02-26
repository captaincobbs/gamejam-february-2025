using com.cyborgAssets.inspectorButtonPro;
using FMODUnity;
using UnityEngine;

public class WoodenBox : Entity
{
    [Header("Sound Events")]
    [SerializeField] private EventReference onMove;
    [SerializeField] private EventReference onDeath;
    [ProButton]
    public override void Move()
    {
        AudioManager.Instance.PlayOneShot(onMove);
    }
    [ProButton]
    public override void Death()
    {
        AudioManager.Instance.PlayOneShot(onDeath);
    }
}
