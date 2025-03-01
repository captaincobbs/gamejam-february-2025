using FMODUnity;
using UnityEngine;

namespace Assets.Scripts.Level.Props
{
    public class Door : MonoBehaviour
    {
        [SerializeField] public bool IsEntrance;

        [Header("State")]
        [SerializeField] public bool IsOpen;
        [SerializeField] bool toggledByTrigger;
        [SerializeField] uint triggerID;

        [Header("Sprite")]
        [SerializeField] private Sprite whenOpen;
        [SerializeField] private Sprite whenClosed;

        [Header("Sound Events")]
        [SerializeField] private EventReference onOpen;
        [SerializeField] private EventReference onClose;

        // References
        private SpriteRenderer spriteRenderer;
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = IsOpen ? whenOpen : whenClosed;
        }

        public void Open()
        {
            AudioManager.Instance.PlayOneShot(onOpen, $"Door.{nameof(onOpen)}");
        }

        public void Close()
        {
            AudioManager.Instance.PlayOneShot(onClose, $"Door.{nameof(onClose)}");
        }
    }
}