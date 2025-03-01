using FMODUnity;
using UnityEngine;

namespace Assets.Scripts.Level.Props
{
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
        SpriteRenderer buttonRenderer;
        ButtonTimer timer;

        LevelManager LevelManager
        {
            get => LevelManager.Instance;
        }

        private void Start()
        {
            buttonRenderer = GetComponent<SpriteRenderer>();

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
            if (!Pressed)
            {
                AudioManager.Instance.PlayOneShot(onPress, $"ButtonWall.{nameof(onPress)}");
                buttonRenderer.sprite = whenPressed;
                LevelManager.InvokeTrigger(TriggerID);

                if (ButtonType == ButtonType.Timer)
                {
                    StartTimer(TimerDuration);
                }
            }
            else
            {
                AudioManager.Instance.PlayOneShot(onUnpress, $"ButtonWall.{nameof(onUnpress)}");
                buttonRenderer.sprite = whenUnpressed;
                LevelManager.InvokeTrigger(TriggerID);
            }

            Pressed = !Pressed;
        }

        private void TimerTick()
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
            InTimer = true;
            TurnsRemaining = turns - 1;
            LevelManager.OnTurnEnd += TimerTick;
            ToggleTimer(true);
        }

        public void ToggleTimer(bool enabled)
        {
            timer.gameObject.SetActive(enabled);
        }
    }

}