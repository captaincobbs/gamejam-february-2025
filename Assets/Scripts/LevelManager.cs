using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Oxygen")]
    [Tooltip("Whether oxygen will be used in this level, also toggles the visibility of the oxygen display")]
    public bool UseOxygen = true;
    [Tooltip("The starting oxygen value when the player spawns")]
    public uint InitialOxygen = 5;
    [Tooltip("The player's current oxygen value")]
    [HideInNormalInspector] public uint CurrentOxygen;

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
    public Dictionary<int, Action> OnTrigger = new();

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
            if (CurrentOxygen > 0)
            {
                CurrentOxygen--;
                OxygenDisplay?.SetDisplayLevel(CurrentOxygen);
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

    void GameOver()
    {

    }

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
        if (instance == this)
        {
            instance = null;
        }
    }
    #endregion
}
