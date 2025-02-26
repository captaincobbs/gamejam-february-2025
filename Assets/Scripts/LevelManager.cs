using FMODUnity;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Editor;

public class LevelManager : MonoBehaviour
{
    [Header("Oxygen")]
    [Tooltip("Whether oxygen will be used in this level, also toggles the visibility of the oxygen display")]
    public bool UseOxygen = true;
    [Tooltip("The starting oxygen value when the player spawns")]
    public uint InitialOxygen = 5;
    [Tooltip("The maximum allowed oxygen value")]
    public uint MaximumOxygen = 10;
    [Tooltip("The minimum allowed oxygen value, going below this will kill the player")]
    public uint MinimumOxygen = 0;
    [Tooltip("The player's current oxygen value")]

    private uint currentOxygen;
    [HideInNormalInspector] public uint CurrentOxygen
    {
        get => currentOxygen;
        set
        {
            currentOxygen = value;
            AudioManager.Instance.SetParameterWithValue("parameter:/Player/Player_Material", value);
        }
    }

    [Header("Kinematics")]
    [Tooltip("The delay between when button presses are read (this will hopefully eventually be replaced with something better)")]
    public float DelayBetweenMovement = 0.15f;

    [Header("Associations")]
    [Tooltip("A reference to the player object in this level")]
    public Player player;

    [Header("UI")]
    [Tooltip("A reference to the OxygenDisplay object in this level")]
    public OxygenDisplay OxygenDisplay;

    [Header("Sound Events")]
    [SerializeField] private EventReference onLoad;
    [SerializeField] private EventReference onUndo;
    [SerializeField] private EventReference onTurn;
    [SerializeField] private EventReference onRestart;
    [SerializeField] private EventReference onUnload;

    // Events
    public event Action OnTurnEnd;
    public Dictionary<uint, Action> Triggers = new();
    public Dictionary<uint, List<EventReference>> SoundTriggers = new();

    // Turn Processing
    private bool isTurnProcessing = false;
    private bool canPlayerMove = true;
    private int turnNumber = 0;
    private List<Entity> entities = new();

    void Start()
    {
        AudioManager.Instance.PlayOneShot(onLoad);

        if (UseOxygen)
        {
            CurrentOxygen = InitialOxygen;
            OxygenDisplay?.SetDisplayLevel(CurrentOxygen);
        }
        else
        {
            OxygenDisplay?.SetDisplay(false);
        }

        if (player == null)
        {
            Debug.LogError("No Player object was associated with the TurnManager");
        }

        entities = FindObjectsByType<Entity>(FindObjectsSortMode.InstanceID).ToList();
    }

    void Update()
    {
        if (!isTurnProcessing && canPlayerMove)
        {
            ProcessPlayerInput();
        }
    }

    void ProcessPlayerInput()
    {
        Vector2 direction = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool turnPassed = false;

        if (direction.x == -1)
        {
            turnPassed = MoveEntity(player, Vector3.left);
        }
        else if (direction.x == 1)
        {
            turnPassed = MoveEntity(player, Vector3.right);
        }
        else if (direction.y == 1)
        {
            turnPassed = MoveEntity(player, Vector3.up);
        }
        else if (direction.y == -1)
        {
            turnPassed = MoveEntity(player, Vector3.down);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            turnPassed = true;
            player.OnWait();
        }

        if (turnPassed)
        {
            StartCoroutine(ProcessEndTurn());
        }
    }

    public bool MoveEntity(Entity entity, Vector3 direction, bool pushed = false)
    {
        if (IsBlocked(entity, direction))
        {
            return false;
        }

        Entity entityInFront = GetEntityInFront(entity, direction);

        if (entityInFront != null)
        {
            if (!MoveEntity(entityInFront, direction))
            {
                return false;
            }
        }

        entity.alreadyPushed = entity.alreadyPushed || pushed;
        entity.transform.position += direction;
        entity.Move();
        Physics2D.SyncTransforms();

        ConveyorBelt conveyorBelt = HitsConveyorBelt(entity.transform.position);
        if (conveyorBelt != null && !entity.alreadyPushed)
        {
            entity.alreadyPushed = true;
            return MoveEntity(entity, conveyorBelt.GetDirectionalValue(), true);
        }

        return true;
    }

    bool IsBlocked(Entity entity, Vector3 direction)
    {
        Vector3 targetPosition = entity.transform.position + direction;

        Collider2D hit = Physics2D.OverlapPoint(targetPosition, LayerMask.GetMask("Static"));
        bool output = hit != null;

        ConveyorBelt conveyorBelt = HitsConveyorBelt(entity.transform.position + direction);
        if (conveyorBelt != null && ConveyorBeltCancelsMovement(direction, conveyorBelt))
        {
            //entity.alreadyPushed = true;

            // Is Also blocked if the conveyor belt prevents movement
            output = true;
        }

        return output;
    }

    public bool ConveyorBeltCancelsMovement(Vector3 direction, ConveyorBelt conveyorBelt)
    {
        switch (conveyorBelt.Direction)
        {
            case (ConveyorBelt.ConveyorDirection.Left):
                return direction == Vector3.right;
            case (ConveyorBelt.ConveyorDirection.Right):
                return direction == Vector3.left;
            case (ConveyorBelt.ConveyorDirection.Up):
                return direction == Vector3.down;
            case (ConveyorBelt.ConveyorDirection.Down):
                return direction == Vector3.up;
        }

        return false;
    }

    ConveyorBelt HitsConveyorBelt(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position, LayerMask.GetMask("Conveyor"));

        if (hit != null)
        {
            return hit.GetComponent<ConveyorBelt>();
        }

        return null;
    }

    Entity GetEntityInFront(Entity entity, Vector3 direction)
    {
        Vector3 targetPosition = entity.transform.position + direction;
        Collider2D hit = Physics2D.OverlapPoint(targetPosition, LayerMask.GetMask("Entity"));

        if (hit != null)
        {
            Entity hitEntity = hit.GetComponent<Entity>();

            if (hitEntity == entity)
            {
                return null; // Prevent stack overflow
            }

            return hitEntity;
        }

        return null;
    }

    IEnumerator ProcessEndTurn()
    {
        isTurnProcessing = true;

        OnTurnEnd?.Invoke();
        AudioManager.Instance.PlayOneShot(onTurn);

        if (UseOxygen)
        {
            if (CurrentOxygen > MinimumOxygen)
            {
                CurrentOxygen--;
                player.OnOxygenUsed();
                OxygenDisplay.SetDisplayLevel(CurrentOxygen);
            }
            else
            {

                GameOver();
            }
        }

        turnNumber++;
        Debug.Log($"Turn {turnNumber}");

        foreach (Entity entity in entities)
        {
            entity.alreadyPushed = false;
        }

        yield return new WaitForSeconds(DelayBetweenMovement);
        isTurnProcessing = false;
    }

    public void KillEntity(Entity entity)
    {
        entity.Death();
    }

    public void RefillOxygen(uint amount, uint refillUpTo)
    {
        CurrentOxygen = Math.Min(Math.Max(MaximumOxygen, CurrentOxygen + amount), refillUpTo);
        OxygenDisplay.SetDisplayLevel(CurrentOxygen);
        player.OnOxygenRefilled();
    }

    void GameOver()
    {

    }

    #region Trigger Management
    public void SubscribeTrigger(uint triggerID, Action action, EventReference? soundEvent = null)
    {
        if (!Triggers.ContainsKey(triggerID))
        {
            Triggers[triggerID] = action;
        }
        else
        {
            Triggers[triggerID] += action;
        }

        if (soundEvent != null)
        {
            if (!SoundTriggers.ContainsKey(triggerID))
            {
                SoundTriggers[triggerID] = new() { soundEvent.Value };
            }
            else
            {
                SoundTriggers[triggerID].Add(soundEvent.Value);
            }
        }
    }

    public void UnsubscribeTrigger(uint triggerID, Action action)
    {
        if (Triggers.ContainsKey(triggerID))
        {
            Triggers[triggerID] -= action;

            // If no more listeners then remove the trigger
            if (Triggers[triggerID] == null)
            {
                Triggers.Remove(triggerID);
            }
        }
    }

    public void InvokeTrigger(uint triggerID)
    {
        if (Triggers.TryGetValue(triggerID, out Action action))
        {
            Debug.Log($"Trigger: {triggerID}");
            action?.Invoke();

            if (SoundTriggers.TryGetValue(triggerID, out List<EventReference> events) && events.Count > 0)
            {
                foreach (EventReference soundEvent in events)
                {
                    AudioManager.Instance.PlayOneShot(soundEvent);
                }
            }
        }
    }
    #endregion

    #region Singleton
    private static LevelManager instance;
    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("This Scene does not have a TurnManager");
            }

            return instance;
        }
    }

    // Prevent duplicate instances
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Destroy static instance when scene is unloaded
    private void OnDestroy()
    {
        AudioManager.Instance.PlayOneShot(onUnload);

        if (instance == this)
        {
            instance = null;
        }
    }
    #endregion
}
