using FMODUnity;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Level.Props
{
    [ExecuteInEditMode]
    public class Portal : MonoBehaviour
    {
        [Header("State")]
        public bool Enabled;
        [Tooltip("Portals with the same ID will teleport to each other, if there are more than 2 then entities will be teleported between them in a cycle")]
        public PortalID PortalID;
        [Tooltip("The type of entity that can enter the portal")]
        public EntityFilterType EntityFilter;
        [Tooltip("Whether the portal can be toggled on/off via a trigger")]
        public bool ToggledOnTrigger;
        [Tooltip("The ID of the trigger to invoke when the button is pressed")]
        public uint TriggerID;

        [Header("Sprite")]
        [SerializeField] private Sprite whenEnabled;
        [SerializeField] private Sprite whenDisabled;

        [Header("Sound Events")]
        [SerializeField] private EventReference onTeleport;
        [SerializeField] private EventReference onToggle;

        //References
        SpriteRenderer spriteRenderer;
        BoxCollider2D portalCollider;
        LevelManager LevelManager
        {
            get => LevelManager.Instance;
        }

        static readonly Dictionary<PortalID, List<Portal>> Portals = new();

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateSprite();
            portalCollider = GetComponent<BoxCollider2D>();

            if (!Portals.ContainsKey(PortalID))
            {
                Portals[PortalID] = new();
            }

            Portals[PortalID].Add(this);

            if (LevelManager != null)
            {
                LevelManager.OnTurnEnd += OnTurn;

                if (ToggledOnTrigger)
                {
                    LevelManager.SubscribeTrigger(
                        TriggerID,
                        Toggle,
                        new(SoundEventType.PortalToggle, onToggle)
                    );
                }
            }
        }

        void Toggle()
        {
            Enabled = !Enabled;
            UpdateSprite();
            AudioManager.Instance.PlayOneShot(onToggle, $"Portal.{nameof(onToggle)}");
        }

        void OnTurn()
        {
            if (Enabled)
            {
                Vector2 colliderCenter = portalCollider.bounds.center;
                Vector2 colliderSize = portalCollider.bounds.size;

                foreach (Collider2D collider in Physics2D.OverlapBoxAll(colliderCenter, colliderSize, 0f, LayerMask.GetMask("Entity")))
                {
                    if (collider != null && collider.TryGetComponent(out Entity entity) && !entity.alreadyTeleported)
                    {
                        Teleport(entity);
                    }
                }
            }
        }

        void Teleport(Entity entity)
        {
            Portal nextPortal = FindNextPortal();

            if (nextPortal != null && !entity.alreadyTeleported)
            {
                if (Entity.TryGetEntityAtPosition(entity.transform.position, EntityFilter, out Entity filteredEntity) && filteredEntity != null)
                {
                    entity.alreadyTeleported = true;
                    entity.transform.position = nextPortal.transform.position;
                    AudioManager.Instance.PlayOneShot(onTeleport, $"Portal.{nameof(onTeleport)}");
                }
            }
        }

        Portal FindNextPortal()
        {
            if (Portals.TryGetValue(PortalID, out List<Portal> portals))
            {
                int currentIndex = portals.IndexOf(this);
                int portalCount = portals.Count;
                int nextIndex = (currentIndex + 1) % portalCount;

                while (nextIndex != currentIndex)
                {
                    Portal nextPortal = portals[nextIndex];

                    if (nextPortal != this && nextPortal.Enabled)
                    {
                        return nextPortal;
                    }

                    nextIndex = (nextIndex + 1) % portalCount;
                }
            }
            return null;
        }

        void UpdateSprite()
        {
            spriteRenderer.sprite = Enabled ? whenEnabled : whenDisabled;
        }

        #region Editor Scripts
        public void OnValidate()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            EditorApplication.delayCall += UpdateSprite;
        }
        #endregion
    }
}