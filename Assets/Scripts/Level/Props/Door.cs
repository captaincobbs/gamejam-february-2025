using FMODUnity;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Level.Props
{
    [ExecuteInEditMode]
    public class Door : MonoBehaviour
    {
        [SerializeField] public bool EnterToWin;

        [Header("State")]
        [SerializeField] public bool IsOpen;
        [SerializeField] public bool ToggledByTrigger;
        [SerializeField] public uint ToggleTriggerID;

        [Header("Sprite")]
        [SerializeField] private Sprite whenOpen;
        [SerializeField] private Sprite whenClosed;

        [Header("Sound Events")]
        [SerializeField] private EventReference onOpen;
        [SerializeField] private EventReference onClose;

        // References
        LevelManager LevelManager
        {
            get => LevelManager.Instance;
        }
        private SpriteRenderer spriteRenderer;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateSprite();

            if (IsOpen)
            {
                Open();
            }
            else
            {
                Close();
            }

            if (LevelManager != null && ToggledByTrigger)
            {
                LevelManager.SubscribeTrigger(
                    ToggleTriggerID,
                    Toggle
                );
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (EnterToWin)
                {
                    LevelManager.WinLevel();
                }
            }
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
            UpdateSprite();
            if (IsOpen)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        void Open()
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            AudioManager.Instance.PlayOneShot(onOpen, $"Door.{nameof(onOpen)}");
        }

        void Close()
        {
            gameObject.layer = LayerMask.NameToLayer("Static");
            AudioManager.Instance.PlayOneShot(onClose, $"Door.{nameof(onClose)}");
        }

        private void UpdateSprite()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = IsOpen ? whenOpen : whenClosed;
            }
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