using FMODUnity;
using UnityEngine;

namespace Assets.Scripts.Level.Props
{
    public abstract class Entity : MonoBehaviour
    {
        [Header("Entity")]
        public bool CanBeWalkedOn = false;
        public bool CanBePushed = true;
        public bool CanBeKilled = true;
        [HideInInspector] public bool alreadyPushed = false;
        [HideInInspector] public bool alreadyTeleported = false;

        [Header("Sound Events")]
        [SerializeField] protected EventReference onKill;
        [SerializeField] protected EventReference onMove;

        public abstract void Move();
        public abstract void Kill();
    }
}