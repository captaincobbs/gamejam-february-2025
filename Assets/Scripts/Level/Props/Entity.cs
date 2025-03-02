using FMODUnity;
using System.Collections.Generic;
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

        #region Static Members
        public static bool TryGetEntityAtPosition(Vector3 position, EntityFilterType filterType, out Entity entity)
        {
            entity = null;
            Collider2D collider = Physics2D.OverlapPoint(position, LayerMask.GetMask("Entity"));

            if (collider != null && collider.TryGetComponent(out Entity collisionEntity))
            {
                entity = collisionEntity;

                switch (filterType)
                {
                    case (EntityFilterType.Any):
                        return true;
                    case EntityFilterType.Player:
                        if (entity.CompareTag("Player"))
                        {
                            return true;
                        }
                        break;
                    case EntityFilterType.Entity:
                        if (!entity.CompareTag("Player"))
                        {
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        public static bool GetPlayerFromEntity(Entity entity, out Player player)
        {
            player = null;
            if (entity.CompareTag("Player"))
            {
                return entity.TryGetComponent(out player);
            }

            return false;
        }
        #endregion
    }
}