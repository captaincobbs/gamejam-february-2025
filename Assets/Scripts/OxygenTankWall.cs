using Assets.Scripts;
using UnityEngine;

public class OxygenTankWall : Interactable
{
    [SerializeField] uint oxygenRefilled = 5;
    [SerializeField] uint refillUpTo = 5;

    LevelManager LevelManager
    {
        get => LevelManager.Instance;
    }

    public override void Interact()
    {
        // +1 because oxygen is used immediately afterwards
        LevelManager.RefillOxygen(oxygenRefilled + 1, refillUpTo + 1);
    }
}
