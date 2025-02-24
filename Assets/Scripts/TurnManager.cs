using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

public class TurnManager : MonoBehaviour
{
    private static TurnManager instance;
    public static TurnManager Instance
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

    [Header("Oxygen")]
    public bool UseOxygen = true;
    public uint InitialOxygen = 5;
    [HideInNormalInspector] public uint CurrentOxygen;

    [Header("Kinematics")]
    public float DelayBetweenMovement = 0.15f;

    [Header("Associations")]
    public Player player;

    [Header("UI")]
    public OxygenDisplay OxygenDisplay;

    // Events
    public event Action OnTurnEnd;
    public Dictionary<int, Action> OnTrigger = new();

    // Turn Processing
    private bool isTurnProcessing = false;
    private bool canPlayerMove = true;
    private int turnNumber = 0;
    void Start()
    {
        if (UseOxygen)
        {
            CurrentOxygen = InitialOxygen;
            OxygenDisplay?.SetDisplayLevel(CurrentOxygen);
        }
        else
        {
            OxygenDisplay?.SetDisplay(false);
        }
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

        entity.transform.position += direction;

        ConveyorBelt conveyorBelt = HitsConveyorBelt(entity);
        if (conveyorBelt != null && !entity.beenForciblyMoved)
        {
            entity.beenForciblyMoved = true;
            bool output = MoveEntity(entity, conveyorBelt.GetDirectionalValue(), true);
            entity.beenForciblyMoved = false;
            return output;
        }

        return true;
    }

    bool IsBlocked(Entity entity, Vector3 direction)
    {
        Vector3 targetPosition = entity.transform.position + direction;

        Collider2D hit = Physics2D.OverlapBox(targetPosition, Vector2.one, 0f, LayerMask.GetMask("Static"));
        return hit != null;
    }

    ConveyorBelt HitsConveyorBelt(Entity entity)
    {
        Collider2D hit = Physics2D.OverlapPoint(entity.transform.position, LayerMask.GetMask("Conveyor"));

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
        yield return new WaitForSeconds(DelayBetweenMovement);
        isTurnProcessing = false;
    }

    void GameOver()
    {
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
}
