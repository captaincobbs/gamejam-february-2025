using FMODUnity;
using UnityEngine;

public class WallButton : MonoBehaviour
{
    public ButtonType buttonType;
    public uint triggerID;
    [Header("Sound Events")]
    [SerializeField] private EventReference onPress;
    [SerializeField] private EventReference onUnpress;
    [SerializeField] private EventReference onTick;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public enum ButtonType
    {
        Toggle,
        Trigger1,
        Trigger5,
        Trigger10
    }
}
