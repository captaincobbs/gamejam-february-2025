using System.Collections.Generic;
using Assets.Scripts;
using FMODUnity;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Direction")]
    [Tooltip("Don't make a vertical-looking conveyor belt go horizontally (or vice versa) or the animation will break")]
    public MovementDirection Direction = MovementDirection.Left;
    [Tooltip("Whether this conveyor belt can be flipped via a trigger")]
    public bool ReversedByTrigger = false;
    [Tooltip("The ID of the trigger that will flip this conveyor belt")]
    public uint ReverseTriggerID = 0;

    [Header("State")]
    [Tooltip("Whether the conveyor belt will push or not")]
    public bool Enabled = true;
    [Tooltip("Whether this conveyor belt can be toggled on/off via a trigger")]
    public bool ToggledByTrigger = false;
    [Tooltip("The ID of the trigger that will toggle this conveyor belt")]
    public uint ToggleTriggerID = 0;

    [Header("Sound Events")]
    [SerializeField] private EventReference onAdvance;
    [SerializeField] private EventReference onReverse;
    [SerializeField] private EventReference onToggle;

    [Header("Sprite")]
    [Tooltip("An order list of sprites that the conveyor belt will cycle through during turns")]
    public List<Sprite> Sprites = new();
    int currentFrame;
    SpriteRenderer spriteRenderer;

    // Private
    LevelManager LevelManager
    {
        get => LevelManager.Instance;
    }

    BoxCollider2D conveyorCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        conveyorCollider = GetComponent<BoxCollider2D>();

        if (LevelManager != null)
        {
            LevelManager.OnTurnEnd += Advance;
            if (ToggledByTrigger)
            {
                LevelManager.SubscribeTrigger(
                    ToggleTriggerID,
                    Toggle,
                    new(SoundEventType.ConveyorEnable, onToggle)
                );
            }

            if (ReversedByTrigger)
            {
                LevelManager.SubscribeTrigger(
                    ReverseTriggerID,
                    Reverse,
                    new(SoundEventType.ConveyorReverse, onReverse)
                );
            }

            if (Enabled)
            {
                if (!LevelManager.OnTurnEvents.ContainsKey(SoundEventType.ConveyorAdvance))
                {
                    LevelManager.OnTurnEvents.Add(SoundEventType.ConveyorAdvance, onAdvance);
                }
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

    void Toggle()
    {
        Enabled = !Enabled;

        if (!Enabled)
        {
            if (!LevelManager.OnTurnEvents.ContainsKey(SoundEventType.ConveyorAdvance))
            {
                LevelManager.OnTurnEvents.Add(SoundEventType.ConveyorAdvance, onAdvance);
            }
        }
    }

    void Reverse()
    {
        if (Direction == MovementDirection.Left)
        {
            Direction = MovementDirection.Right;
        }
        else if (Direction == MovementDirection.Right)
        {
            Direction = MovementDirection.Left;
        }
        else if (Direction == MovementDirection.Up)
        {
            Direction = MovementDirection.Down;
        }
        else
        {
            Direction = MovementDirection.Up;
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
                if (collider != null && collider.TryGetComponent<Entity>(out var entity))
                {
                    Vector2 moveDirection = GetDirectionalValue();

                    if (!entity.alreadyPushed)
                    {
                        LevelManager.MoveEntity(entity, new(moveDirection.x, moveDirection.y, 0f), true);
                    }
                }
            }
        }
    }

    void ProgressAnimation()
    {
        // If going positively, move animation forwards
        if (Direction == MovementDirection.Right || Direction == MovementDirection.Down)
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
        if (Direction == MovementDirection.Left)
        {
            return Vector2.left;
        }
        else if (Direction == MovementDirection.Right)
        {
            return Vector2.right;
        }
        else if (Direction == MovementDirection.Up)
        {
            return Vector2.up;
        }
        else if (Direction == MovementDirection.Down)
        {
            return Vector2.down;
        }

        return Vector2.zero;
    }

    public bool AbsorbsMovement(Vector3 direction)
    {
        if (Enabled)
        {
            switch (Direction)
            {
                case (MovementDirection.Left):
                    return direction == Vector3.right;
                case (MovementDirection.Right):
                    return direction == Vector3.left;
                case (MovementDirection.Up):
                    return direction == Vector3.down;
                case (MovementDirection.Down):
                    return direction == Vector3.up;
            }
        }

        return false;
    }

    #region Static Members
    public static ConveyorBelt GetAtPosition(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position, LayerMask.GetMask("Conveyor"));

        if (hit != null)
        {
            return hit.GetComponent<ConveyorBelt>();
        }

        return null;
    }
    #endregion
}
