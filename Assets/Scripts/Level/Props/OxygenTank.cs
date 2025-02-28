using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    [SerializeField] uint oxygenRefilled = 5;
    [SerializeField] uint refillUpTo = 5;

    LevelManager LevelManager
    {
        get => LevelManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D _)
    {
        LevelManager.RefillOxygen(oxygenRefilled, refillUpTo);
    }
}
