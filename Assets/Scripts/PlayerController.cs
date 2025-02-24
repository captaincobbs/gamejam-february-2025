using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Oxygen")]
    public int InitialOxygen = 5;
    public bool UseOxygen = true;
    [HideInNormalInspector]public int CurrentOxygen;

    [Header("Kinetics")]
    public float MoveSpeed = 5f;
    public Transform MovePoint;
    public LayerMask CollisionLayer;
    public Animator Animator;

    [Header("Sound")]
    public string MoveEvent = "event:/PlayerMove";
    private EventInstance moveInstance;

    private bool isTurnProcessing = false;

    public delegate void TurnHandler();
    public static event TurnHandler OnTurnEnd;

    void Start()
    {
        MovePoint.parent = null;
        CurrentOxygen = InitialOxygen;
        moveInstance = RuntimeManager.CreateInstance(MoveEvent);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, MovePoint.position, MoveSpeed * Time.deltaTime);

        if (!isTurnProcessing && Vector3.Distance(transform.position, MovePoint.position) <= 0.05f)
        {
            float horizontalDelta = Input.GetAxisRaw("Horizontal");
            float verticalDelta = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(horizontalDelta) == 1f)
            {
                TryMove(new Vector3(horizontalDelta, 0f, 0f));
            }
            else if (Mathf.Abs(verticalDelta) == 1f)
            {
                TryMove(new Vector3(0f, verticalDelta, 0f));
            }
        }
    }

    void TryMove(Vector3 direction)
    {
        if (!Physics2D.OverlapCircle(MovePoint.position + direction, 0.2f, CollisionLayer))
        {
            if (IsBoxAtTile(MovePoint.position + direction))
            {
                Vector3 nextTile = MovePoint.position + (direction * 2);

                if (!Physics2D.OverlapCircle(nextTile, 0.2f, CollisionLayer))
                {
                    PushBox(MovePoint.position + direction, direction);
                }
                else
                {
                    Debug.Log("Cannot push box");
                }
            }
            else
            {
                MovePoint.position += direction;
                PlayMoveSound();
                StartCoroutine(HandleTurn());
            }
        }
    }

    bool IsBoxAtTile(Vector3 position)
    {
        return Physics2D.OverlapCircle(position, 0.2f, LayerMask.GetMask("Box"));
    }

    void PushBox(Vector3 boxTile, Vector3 direction)
    {
        GameObject boxObject = GetBoxAtTile(boxTile);
        if (boxObject != null)
        {
            Vector3 nextTile = boxTile + direction;

            if (IsTileFree(nextTile))
            {
                PushBox(nextTile, direction);
                boxObject.transform.position += direction;

                // Play sound and animation for moving the box
                //PlayBoxMoveSound(boxObject);
                //PlayBoxMoveAnimation(boxObject);
            }
            else
            {
                Debug.Log("Cannot push box further.");
            }
        }
    }

    bool IsTileFree(Vector3 position)
    {
        return !Physics2D.OverlapCircle(position, 0.2f, CollisionLayer);
    }

    GameObject GetBoxAtTile(Vector3 position)
    {
        Collider2D boxCollider = Physics2D.OverlapCircle(position, 0.2f, LayerMask.GetMask("Box"));
        return boxCollider ? boxCollider.gameObject : null;
    }

    void PlayMoveSound()
    {
        moveInstance.start();
    }

    IEnumerator HandleTurn()
    {
        isTurnProcessing = true;
        if (UseOxygen)
        {
            if (CurrentOxygen <= 0)
            {
                Debug.Log("Out of Oxygen");
                // Do death processing
            }
            else
            {
                CurrentOxygen--;
                Debug.Log($"Oxygen: {CurrentOxygen}");
            }
        }

        OnTurnEnd?.Invoke();
        yield return new WaitForSeconds(0.5f);
        isTurnProcessing = false;
    }
}