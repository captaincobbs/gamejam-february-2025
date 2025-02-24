using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;

public class TurnManager : MonoBehaviour
{
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

    private bool isTurnProcessing = false;
    private bool canPlayerMove = true;

    void Start()
    {
        CurrentOxygen = InitialOxygen;
        OxygenDisplay?.SetOxygenDisplayLevel(CurrentOxygen);
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

    public bool MoveEntity(Entity entity, Vector3 direction)
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
        return true;
    }

    bool IsBlocked(Entity entity, Vector3 direction)
    {
        Vector3 targetPosition = entity.transform.position + direction;

        Collider2D hit = Physics2D.OverlapBox(targetPosition, Vector2.one, 0f, LayerMask.GetMask("Static"));
        return hit != null;
    }

    Entity GetEntityInFront(Entity entity, Vector3 direction)
    {
        Vector3 targetPosition = entity.transform.position + direction;
        Collider2D hit = Physics2D.OverlapPoint(targetPosition, LayerMask.GetMask("Entity"));

        if (hit != null)
        {
            return hit.GetComponent<Entity>();
        }

        return null;
    }

    IEnumerator ProcessEndTurn()
    {
        isTurnProcessing = true;
        if (UseOxygen)
        {
            if (CurrentOxygen > 0)
            {
                CurrentOxygen--;
                OxygenDisplay?.SetOxygenDisplayLevel(CurrentOxygen);
            }
            else
            {
                GameOver();
            }
        }

        Debug.Log("Turn End");
        yield return new WaitForSeconds(DelayBetweenMovement);
        isTurnProcessing = false;
    }

    void GameOver()
    {
    }
}
