using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    [SerializeField] uint oxygenRefilled = 5;
    [SerializeField] uint refillUpTo = 5;

    LevelManager levelManager
    {
        get => LevelManager.Instance;
    }

    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        levelManager.RefillOxygen(oxygenRefilled, refillUpTo);
    }
}
