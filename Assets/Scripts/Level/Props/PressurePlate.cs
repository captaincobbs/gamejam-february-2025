using FMODUnity;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Level.Props
{
    [ExecuteInEditMode]
    public class PressurePlate : MonoBehaviour
    {
        [Header("State")]
        [Tooltip("Whether the pressure plate holds its state or loses it after a few turns")]
        public ButtonType ButtonType;
        [Tooltip("The type of entity that can trigger the pressure plate")]
        public EntityFilterType EntityFilter;
        [Tooltip("Whether the button is currently pressed")]
        public bool Pressed;
        [Tooltip("The ID of the trigger to invoke when the pressure plate is pressed")]
        public uint TriggerID;
        [Tooltip("The duration of the timer in turns")]
        public int TimerDuration;
        public int TurnsRemaining
        {
            get => timer?.TurnsRemaining ?? 0;
            set => timer.TurnsRemaining = value;
        }
        [HideInInspector] public bool InTimer = false;
        [HideInInspector] public bool BeingPressed;

        [Header("Sprite")]
        [SerializeField] private Sprite whenPressed;
        [SerializeField] private Sprite whenUnpressed;

        [Header("Sound Events")]
        [SerializeField] private EventReference onPress;
        [SerializeField] private EventReference onUnpress;
        [SerializeField] private EventReference onTick;

        // References
        SpriteRenderer spriteRenderer;
        ButtonTimer timer;

        LevelManager LevelManager
        {
            get => LevelManager.Instance;
        }

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Pressed ? whenPressed : whenUnpressed;
            Transform timerTransform = transform.GetChild(0);

            if (timerTransform.TryGetComponent(out ButtonTimer childTimer))
            {
                timer = childTimer;
            }
            else
            {
                Debug.LogWarning("Pressure Plate is missing an associated timer");
            }
        }

        public void Toggle()
        {
            if (!Pressed)
            {
                AudioManager.Instance.PlayOneShot(onPress, $"PressurePlate.{nameof(onPress)}");
                spriteRenderer.sprite = whenPressed;
                LevelManager.InvokeTrigger(TriggerID);
            }
            else
            {
                AudioManager.Instance.PlayOneShot(onUnpress, $"PressurePlate.{nameof(onUnpress)}");
                spriteRenderer.sprite = whenUnpressed;
                LevelManager.InvokeTrigger(TriggerID);
            }

            Pressed = !Pressed;
        }

        void TimerTick()
        {
            if (TurnsRemaining > 0)
            {
                TurnsRemaining--;
                AudioManager.Instance.PlayOneShot(onTick, $"PressurePlate.{nameof(onTick)}");
            }
            else
            {
                EndTimer();
                Toggle();
            }
        }

        public void StartTimer(int turns)
        {
            InTimer = true;
            TurnsRemaining = turns - 1;
            LevelManager.OnTurnEnd += TimerTick;
            ToggleTimer(true);
        }

        public void EndTimer()
        {
            InTimer = false;
            LevelManager.OnTurnEnd -= TimerTick;
            ToggleTimer(false);
        }

        public void ToggleTimer(bool enabled)
        {
            timer.gameObject.SetActive(enabled);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !BeingPressed)
            {
                if (EntityFilter == EntityFilterType.Player || EntityFilter == EntityFilterType.Any)
                {
                    Press();
                }
            }
            else
            {
                if (EntityFilter == EntityFilterType.Entity || EntityFilter == EntityFilterType.Any)
                {
                    Press();
                }
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Player _))
            {
                if (EntityFilter == EntityFilterType.Player || EntityFilter == EntityFilterType.Any)
                {
                    Unpress();
                }
            }
            else
            {
                if (EntityFilter == EntityFilterType.Entity || EntityFilter == EntityFilterType.Any)
                {
                    Unpress();
                }
            }
        }

        void Press()
        {
            if (!BeingPressed)
            {
                BeingPressed = true;

                if (!Pressed)
                {
                    Toggle();
                }

                if (InTimer && ButtonType == ButtonType.Timer)
                {
                    EndTimer();
                }
            }
        }

        void Unpress()
        {
            if (BeingPressed)
            {
                BeingPressed = false;

                if (ButtonType == ButtonType.Timer)
                {
                    StartTimer(TimerDuration);
                }
                else
                {
                    Toggle();
                }
            }
        }

        void UpdateSprite()
        {
            spriteRenderer.sprite = Pressed ? whenPressed : whenUnpressed;
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