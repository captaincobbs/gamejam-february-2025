using FMODUnity;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] bool isEntrance;
    [Header("State")]
    [SerializeField] bool startOpen;
    [SerializeField] bool toggledByTrigger;
    [SerializeField] uint triggerID;
    [Header("Sound Events")]
    [SerializeField] private EventReference onOpen;
    [SerializeField] private EventReference onClose;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Open()
    {

    }

    public void Close()
    {

    }
}
