using FMODUnity;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Level.Props
{
    public class Portal : MonoBehaviour
    {
        [Header("State")]
        public bool Enabled;
        [Tooltip("Portals with the same ID will teleport to each other")]
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

        static Dictionary<PortalID, List<Portal>> Portals = new();

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            portalCollider = GetComponent<BoxCollider2D>();
        }

        void Toggle()
        {

        }

        void OnTriggerEnter2D(Collider2D other)
        {
        }

        void OnTurn()
        {

        }

        void Teleport()
        {
            if (Enabled)
            {
                Vector2 colliderCenter = portalCollider.bounds.center;
                Vector2 colliderSize = portalCollider.bounds.size;

                foreach (Collider2D collider in Physics2D.OverlapBoxAll(colliderCenter, colliderSize, 0f, LayerMask.GetMask("Entity")))
                {
                    if (collider != null && collider.TryGetComponent<Entity>(out var entity))
                    {

                    }
                }
            }
        }
    }
}