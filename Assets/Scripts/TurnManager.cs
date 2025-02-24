using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;

public class TurnManager : MonoBehaviour
{
    [Header("Oxygen")]
    public bool UseOxygen = true;
    public int InitialOxygen = 5;
    [HideInNormalInspector] public int CurrentOxygen;

    [Header("Associations")]
    public Player player;

    private bool isTurnProcessing = false;
    private bool canPlayerMove = true;

    void Start()
    {
        CurrentOxygen = InitialOxygen;
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
            turnPassed = MoveEntity(player, Vector2.left);
        }
        else if (direction.x == 1)
        {
            turnPassed = MoveEntity(player, Vector2.right);
        }
        else if (direction.y == 1)
        {
            turnPassed = MoveEntity(player, Vector2.up);
        }
        else if (direction.y == -1)
        {
            turnPassed = MoveEntity(player, Vector2.down);
        }

        if (turnPassed)
        {
            EndTurn();
        }
    }

    public bool MoveEntity(Entity entity, Vector2 direction)
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

        entity.transform.position += (Vector3)direction;
        return true;
    }

    bool IsBlocked(Entity entity, Vector2 direction)
    {
        Vector3 targetPosition = entity.transform.position + (Vector3)direction;

        Collider2D hit = Physics2D.OverlapBox(targetPosition, Vector2.one, 0f, LayerMask.GetMask("Static"));
        return hit != null;
    }

    Entity GetEntityInFront(Entity entity, Vector2 direction)
    {
        Vector3 targetPosition = entity.transform.position + (Vector3)direction;
        Collider2D hit = Physics2D.OverlapBox(targetPosition, Vector2.one, 0f, LayerMask.GetMask("Entity"));

        if (hit != null)
        {
            return hit.GetComponent<Entity>();
        }

        return null;
    }

    void EndTurn()
    {
        if (UseOxygen)
        {
            CurrentOxygen--;

            if (CurrentOxygen <= 0)
            {
                GameOver();
                return;
            }
            else
            {
                Debug.Log($"Oxygen Remaining: {CurrentOxygen}");
            }
        }

        Debug.Log("Turn End");
    }

    void GameOver()
    {
        Debug.Log("Game Over: Out of oxygen");
    }
}
