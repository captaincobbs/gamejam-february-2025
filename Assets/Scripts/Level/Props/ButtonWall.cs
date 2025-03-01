using Assets.Scripts;
using Assets.Scripts.Level.Props;
using FMODUnity;
using System.Threading;
using UnityEngine;

public class ButtonWall : MonoBehaviour, IInteractable
{
    public ButtonType ButtonType;
    public bool Pressed;
    public uint TriggerID;
    public int TurnsRemaining
    {
        get => timer?.TurnsRemaining ?? 0;
        set => timer.TurnsRemaining = value;
    }
    private bool InTimer = false;

    [Header("Sprite")]
    public Vector2 timerOffset = Vector2.zero;
    [SerializeField]private Sprite whenPressed;
    [SerializeField]private Sprite whenUnpressed;

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

            switch (ButtonType)
            {
                case (ButtonType.Trigger10):
                    StartTimer(10);
                    break;
                case (ButtonType.Trigger5):
                    StartTimer(5);
                    break;
                case (ButtonType.Trigger1):
                    StartTimer(1);
                    break;
            }
        }
        else
        {
            if (onUnpress.IsNull)
            {
                Debug.LogWarning("OnUnpress is null");
            }

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
            timer.TurnsRemaining = TurnsRemaining;
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
