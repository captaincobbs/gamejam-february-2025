using Assets.Scripts;
using Assets.Scripts.Level.Props;
using FMODUnity;
using UnityEngine;

public class WallButton : Interactable
{
    public ButtonType ButtonType;
    public bool Pressed;
    public uint TriggerID;
    private int turnsRemaining;
    private bool inTimer = false;

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
    }

    public override void Interact()
    {
        Toggle();
    }

    public void Toggle()
    {
        if (!inTimer)
        {
            if (!Pressed)
            {
                AudioManager.Instance.PlayOneShot(onPress);
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
                AudioManager.Instance.PlayOneShot(onUnpress);
                buttonRenderer.sprite = whenUnpressed;
                LevelManager.InvokeTrigger(TriggerID);
            }

            Pressed = !Pressed;
        }
    }

    private void TimerTick()
    {
        if (turnsRemaining > 0)
        {
            turnsRemaining--;
            UpdateTimer();
            AudioManager.Instance.PlayOneShot(onTick);
        }
        else
        {
            inTimer = false;
            Toggle();
            HideTimer();
        }
    }

    public void StartTimer(int turns)
    {
        turnsRemaining = turns;
        inTimer = true;
        LevelManager.OnTurnEnd += TimerTick;
        ShowTimer();
    }

    public void ShowTimer()
    {

    }

    public void HideTimer()
    {

    }

    public void UpdateTimer()
    {

    }
}
