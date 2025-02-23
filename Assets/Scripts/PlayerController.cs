using SuperTiled2Unity;
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

    private bool isTurnProcessing = false;

    public delegate void TurnHandler();
    public static event TurnHandler OnTurnEnd;

    void Start()
    {
        MovePoint.parent = null;
        CurrentOxygen = InitialOxygen;
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
        if (!Physics2D.OverlapCircle(MovePoint.position + direction, .2f, CollisionLayer))
        {
            MovePoint.position += direction;
            StartCoroutine(HandleTurn());
        }
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