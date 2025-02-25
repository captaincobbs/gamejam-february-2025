using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Direction")]
    [Tooltip("Don't make a vertical-looking conveyor belt go horizontally (or vice versa) or the animation will break")]
    public ConveyorDirection Direction = ConveyorDirection.Left;
    [Tooltip("Whether this conveyor belt can be flipped via a trigger")]
    public bool FlippedByTrigger = false;
    [Tooltip("The ID of the trigger that will flip this conveyor belt")]
    public int FlipTriggerID = 0;

    [Header("State")]
    [Tooltip("Whether the conveyor belt will push or not")]
    public bool Enabled = true;
    [Tooltip("Whether this conveyor belt can be toggled on/off via a trigger")]
    public bool ToggledByTrigger = false;
    [Tooltip("The ID of the trigger that will toggle this conveyor belt")]
    public int ToggleTriggerID = 0;

    [Header("Sprite")]
    [Tooltip("An order list of sprites that the conveyor belt will cycle through during turns")]
    public List<Sprite> Sprites = new();
    int currentFrame;
    SpriteRenderer spriteRenderer;

    // Private
    TurnManager TurnManager
    {
        get => TurnManager.Instance;
    }

    BoxCollider2D conveyorCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        conveyorCollider = GetComponent<BoxCollider2D>();



        if (TurnManager != null)
        {
            TurnManager.OnTurnEnd += Advance;
            if (ToggledByTrigger)
            {
                TurnManager.OnTrigger[ToggleTriggerID] += Toggle;
            }

            if (FlippedByTrigger)
            {
                TurnManager.OnTrigger[ToggleTriggerID] += FlipDirection;
            }
        }
        else
        {
            Debug.LogError("Conveyor Belt can't find a parent TurnManager");
        }

        if (Sprites == null || Sprites.Count == 0)
        {
            Debug.LogError("Conveyor Belt was not provided a Sprite");
        }
    }

    void Toggle() { Enabled = !Enabled; }

    void FlipDirection()
    {
        if (Direction == ConveyorDirection.Left)
        {
            Direction = ConveyorDirection.Right;
        }
        else if (Direction == ConveyorDirection.Right)
        {
            Direction = ConveyorDirection.Left;
        }
        else if (Direction == ConveyorDirection.Up)
        {
            Direction = ConveyorDirection.Down;
        }
        else
        {
            Direction = ConveyorDirection.Up;
        }
    }

    void Advance()
    {
        if (Enabled)
        {
            ProgressAnimation();

            Vector2 colliderCenter = conveyorCollider.bounds.center;
            Vector2 colliderSize = conveyorCollider.bounds.size;

            foreach (Collider2D collider in Physics2D.OverlapBoxAll(colliderCenter, colliderSize, 0f, LayerMask.GetMask("Entity")))
            {
                if (collider != null)
                {
                    Entity entity = collider.GetComponent<Entity>();

                    if (entity != null)
                    {
                        Vector2 moveDirection = GetDirectionalValue();

                        TurnManager.MoveEntity(entity, new(moveDirection.x, moveDirection.y, 0f), true);
                    }
                }
            }
        }
    }

    void ProgressAnimation()
    {
        // If going positively, increment normally
        if (Direction == ConveyorDirection.Right || Direction == ConveyorDirection.Down)
        {
            currentFrame = (currentFrame + 1) % (Sprites.Count);
        }
        // Otherwise, go backwards
        else
        {
            currentFrame = (currentFrame - 1 + Sprites.Count) % Sprites.Count;
        }
        spriteRenderer.sprite = Sprites[currentFrame];
    }

    public Vector2 GetDirectionalValue()
    {
        if (Direction == ConveyorDirection.Left)
        {
            return Vector2.left;
        }
        else if (Direction == ConveyorDirection.Right)
        {
            return Vector2.right;
        }
        else if (Direction == ConveyorDirection.Up)
        {
            return Vector2.up;
        }
        else
        {
            return Vector2.down;
        }
    }

    public enum ConveyorDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}
