using Assets.Scripts.Level.Props;
using Assets.Scripts.Level.Tile;
using Assets.Scripts.Level.UI;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Level
{
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
        [HideInNormalInspector]
        public uint CurrentOxygen
        {
            get => currentOxygen;
            set
            {
                currentOxygen = value;
                AudioManager.Instance.SetParameterWithValue("parameter:/Player/Player_OxygenLevel", value);
            }
        }

        private FloorMaterial playerFloorMaterial = FloorMaterial.Error;
        [HideInInspector]
        public FloorMaterial PlayerFloorMaterial
        {
            get => playerFloorMaterial;
            set
            {
                playerFloorMaterial = value;
                AudioManager.Instance.SetParameterWithLabel("parameter:/Player/Player_Material", Enum.GetName(typeof(FloorMaterial), playerFloorMaterial));
            }
        }

        [Header("Kinematics")]
        [Tooltip("The delay between when button presses are read (this will hopefully eventually be replaced with something better)")]
        public float DelayBetweenMovement = 0.15f;

        [Header("Associations")]
        [Tooltip("A reference to the player object in this level")]
        public Player player;
        public Tilemap floorTilemap;

        [Header("UI")]
        [Tooltip("A reference to the OxygenDisplay object in this level")]
        public OxygenDisplay OxygenDisplay;
        public PlayerFaceDisplay PlayerFaceDisplay;

        [Header("Sound Events")]
        [SerializeField] private EventReference onLoad;
        [SerializeField] private EventReference onUndo;
        [SerializeField] private EventReference onTurn;
        [SerializeField] private EventReference onRestart;
        [SerializeField] private EventReference onUnload;

        // Events
        public event Action AsTurnEnd;
        public event Action OnTurnEnd;
        private Dictionary<uint, Trigger> Triggers = new();

        // Turn Processing
        private bool isTurnProcessing = false;
        private bool canPlayerMove = true;
        private int turnNumber = 0;
        private List<Entity> entities = new();

        void Start()
        {
            AudioManager.Instance.PlayOneShot(onLoad, nameof(onLoad));
            CurrentOxygen = InitialOxygen;

            if (UseOxygen)
            {
                if (OxygenDisplay != null)
                {
                    OxygenDisplay.SetDisplayLevel(CurrentOxygen);
                    PlayerFaceDisplay.SetDisplayLevel(CurrentOxygen);
                }
            }
            else
            {
                if (OxygenDisplay != null)
                {
                    OxygenDisplay.SetDisplay(false);
                    PlayerFaceDisplay.SetDisplay(false);
                }
            }

            if (player == null)
            {
                Debug.LogError("No Player object was associated with the TurnManager");
            }
            else
            {
                PlayerFloorMaterial = CheckPlayerFloor();
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

        IEnumerator ProcessEndTurn()
        {
            isTurnProcessing = true;
            AsTurnEnd?.Invoke(); // Reserved exclusively for things that should happen before movement
            OnTurnEnd?.Invoke();
            if (onTurn.IsNull)
            {
                Debug.LogWarning("OnTurn is null");
            }

            AudioManager.Instance.PlayOneShot(onTurn, nameof(onTurn));

            if (UseOxygen)
            {
                if (CurrentOxygen > MinimumOxygen)
                {
                    CurrentOxygen--;
                    player.OxygenUsed();
                    OxygenDisplay.SetDisplayLevel(CurrentOxygen);
                    PlayerFaceDisplay.SetDisplayLevel(CurrentOxygen);
                }
                else
                {
                    LoseLevel();
                }
            }

            turnNumber++;
            Debug.Log($"Turn {turnNumber}");

            foreach (Entity entity in entities)
            {
                entity.alreadyPushed = false;
                entity.alreadyTeleported = false;
            }

            yield return new WaitForSeconds(DelayBetweenMovement);
            isTurnProcessing = false;
        }

        #region Player
        void ProcessPlayerInput()
        {
            Vector2 rawDirection = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            bool moved = false;
            bool turnPassed = false;
            MovementDirection? actualDirection = null;

            if (rawDirection.x == -1)
            {
                moved = MoveEntity(player, Vector3.left);
                actualDirection = MovementDirection.Left;
            }
            else if (rawDirection.x == 1)
            {
                moved = MoveEntity(player, Vector3.right);
                actualDirection = MovementDirection.Right;
            }
            else if (rawDirection.y == -1)
            {
                moved = MoveEntity(player, Vector3.down);
                actualDirection = MovementDirection.Down;
            }
            else if (rawDirection.y == 1)
            {
                moved = MoveEntity(player, Vector3.up);
                actualDirection = MovementDirection.Up;
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                turnPassed = true;
                player.Wait();
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                // Interacting does not take a turn
                //turnPassed = true;
                InteractWith(player.Direction);
                player.Interact();
            }

            if (moved)
            {
                player.Direction = actualDirection.Value;
                player.AdvanceAnimation();
                PlayerFloorMaterial = CheckPlayerFloor();
            }

            if (turnPassed || moved)
            {
                StartCoroutine(ProcessEndTurn());
            }
        }

        public void RefillOxygen(uint amount, uint refillUpTo)
        {
            CurrentOxygen = Math.Min(Math.Max(MaximumOxygen, CurrentOxygen + amount), refillUpTo);
            OxygenDisplay.SetDisplayLevel(CurrentOxygen);
            PlayerFaceDisplay.SetDisplayLevel(CurrentOxygen);
        }

        public void InteractWith(MovementDirection direction)
        {
            Vector3 offset = Vector3.zero;

            switch (direction)
            {
                case MovementDirection.Up:
                    offset = Vector3.up;
                    break;
                case MovementDirection.Down:
                    offset = Vector3.down;
                    break;
                case MovementDirection.Left:
                    offset = Vector3.left;
                    break;
                case MovementDirection.Right:
                    offset = Vector3.right;
                    break;
            }

            Collider2D hit = Physics2D.OverlapPoint(player.transform.position + offset, LayerMask.GetMask("Interaction"));
            if (hit != null)
            {
                if (hit.TryGetComponent(out IInteractable hitInteractable))
                {
                    hitInteractable.Interact();
                }
            }
        }

        void LoseLevel()
        {
            KillEntity(player);
        }

        void WinLevel()
        {

        }

        FloorMaterial CheckPlayerFloor() => GetFloorMaterialAtPosition(player.transform.position);
        #endregion

        #region Entity
        public bool MoveEntity(Entity entity, Vector3 direction, bool pushed = false, bool canPush = true)
        {
            if (IsBlocked(entity, direction))
            {
                return false;
            }

            bool isEntity = TryGetEntityAt(entity.transform.position + direction, entity, out Entity entityInFront);

            if (isEntity)
            {
                if (entityInFront.CanBePushed)
                {
                    if (!MoveEntity(entityInFront, direction))
                    {
                        return false;
                    }
                }
                else if (!entityInFront.CanBeWalkedOn || !entityInFront.CanBePushed)
                {
                    return false;
                }
            }

            entity.alreadyPushed = entity.alreadyPushed || pushed;
            entity.transform.position += direction;
            entity.Move();
            Physics2D.SyncTransforms();

            if (GetFloorMaterialAtPosition(entity.transform.position) == FloorMaterial.Ice)
            {
                MoveEntity(entity, direction, false, false);
            }

            return true;
        }

        public void KillEntity(Entity entity)
        {
            if (entity.CanBeKilled)
            {
                entity.Kill();
            }
        }

        public bool TryGetEntityAt(Vector3 position, Entity exclude, out Entity entity)
        {
            return TryGetEntityAt(position, new List<Entity> { exclude }, out entity);
        }

        public bool TryGetEntityAt(Vector3 position, List<Entity> excludes, out Entity entity)
        {
            Collider2D hit = Physics2D.OverlapPoint(position, LayerMask.GetMask("Entity"));
            entity = null;

            if (hit != null)
            {
                Entity hitEntity = hit.GetComponent<Entity>();

                if (excludes.Contains(hitEntity))
                {
                    return false; // Prevent stack overflow self-reference
                }

                entity = hitEntity;
                return true;
            }

            return false;
        }

        public FloorMaterial GetFloorMaterialAtPosition(Vector3 position)
        {
            Collider2D hit = Physics2D.OverlapPoint(position, LayerMask.GetMask("Floor"));
            if (hit != null && hit.TryGetComponent(out FloorTile floorTile))
            {
                return floorTile.Material;
            }
            return FloorMaterial.Error;
        }

        bool IsBlocked(Entity entity, Vector3 direction)
        {
            Vector3 targetPosition = entity.transform.position + direction;

            Collider2D hit = Physics2D.OverlapPoint(targetPosition, LayerMask.GetMask("Static"));
            return hit != null;
        }
        #endregion

        #region Trigger Management
        public void SubscribeTrigger(uint triggerID, Action action, TriggerEvent @event)
        {
            if (!Triggers.ContainsKey(triggerID))
            {
                Triggers[triggerID] = new()
                {
                    Action = action,
                };

                if (@event != null)
                {
                    Triggers[triggerID].EventReferences.Add(@event.Type, @event.SoundEvent);
                }
            }
            else
            {
                Triggers[triggerID].Action += action;

                if (@event != null && Triggers[triggerID].EventReferences.ContainsKey(@event.Type))
                {
                    Triggers[triggerID].EventReferences.Add(@event.Type, @event.SoundEvent);
                }
            }
        }

        public void UnsubscribeTrigger(uint triggerID, Action action)
        {
            if (Triggers.ContainsKey(triggerID))
            {
                Triggers[triggerID].Action = null;
                Triggers[triggerID].EventReferences.Clear();
                Triggers.Remove(triggerID);
            }
        }

        public void InvokeTrigger(uint triggerID)
        {
            if (Triggers.TryGetValue(triggerID, out Trigger trigger))
            {
                Debug.Log($"Trigger: {triggerID}");
                trigger.Action?.Invoke();

                foreach (EventReference soundEvent in trigger.EventReferences.Values)
                {
                    AudioManager.Instance.PlayOneShot(soundEvent, $"TriggerInvoke");
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
            if (onUnload.IsNull)
            {
                Debug.LogWarning("OnUnload is null");
            }

            AudioManager.Instance.PlayOneShot(onUnload, nameof(onUnload));

            if (instance == this)
            {
                instance = null;
            }
        }
        #endregion
    }
}