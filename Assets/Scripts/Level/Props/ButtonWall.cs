using FMODUnity;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Level.Props
{
    [ExecuteInEditMode]
    public class ButtonWall : MonoBehaviour, IInteractable
    {
        [Header("State")]
        [Tooltip("Whether the button holds its state or loses it after a few turns")]
        public ButtonType ButtonType;
        [Tooltip("Whether the button is currently pressed")]
        public bool Pressed;
        [Tooltip("The ID of the trigger to invoke when the button is pressed")]
        public uint TriggerID;
        [Tooltip("The duration of the timer in turns")]
        public int TimerDuration;
        public int TurnsRemaining
        {
            get => timer?.TurnsRemaining ?? 0;
            set => timer.TurnsRemaining = value;
        }
        public bool InTimer = false;

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
            UpdateSprite();

            Transform timerTransform = transform.GetChild(0);
            if (timerTransform.TryGetComponent(out ButtonTimer childTimer))
            {
                timer = childTimer;
            }
            else
            {
                Debug.LogWarning("Button is missing an associated timer");
            }
        }

        public void Interact()
        {
            Toggle();
        }

        public void Toggle()
        {
            if (!InTimer)
            {
                if (!Pressed)
                {
                    if (ButtonType == ButtonType.Timer)
                    {
                        StartTimer(TimerDuration);
                    }

                    AudioManager.Instance.PlayOneShot(onPress, $"ButtonWall.{nameof(onPress)}");
                    LevelManager.InvokeTrigger(TriggerID);
                }
                else
                {
                    AudioManager.Instance.PlayOneShot(onUnpress, $"ButtonWall.{nameof(onUnpress)}");
                    LevelManager.InvokeTrigger(TriggerID);
                }

                Pressed = !Pressed;
                UpdateSprite();
            }
            else
            {
                AudioManager.Instance.PlayOneShot(onPress, $"ButtonWall.{nameof(onPress)}");
                StartTimer(TimerDuration);
            }
        }

        void TimerTick()
        {
            if (TurnsRemaining > 0)
            {
                TurnsRemaining--;
                AudioManager.Instance.PlayOneShot(onTick, $"ButtonWall.{nameof(onTick)}");
            }
            else
            {
                InTimer = false;
                LevelManager.OnTurnEnd -= TimerTick;
                Toggle();
                ToggleTimer(false);
            }
        }

        public void StartTimer(int turns)
        {
            if (!InTimer)
            {
                LevelManager.OnTurnEnd += TimerTick;
            }
            InTimer = true;
            TurnsRemaining = turns - 1;
            ToggleTimer(true);
        }

        public void ToggleTimer(bool enabled)
        {
            timer.gameObject.SetActive(enabled);
        }

        void UpdateSprite()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = Pressed ? whenPressed : whenUnpressed;
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