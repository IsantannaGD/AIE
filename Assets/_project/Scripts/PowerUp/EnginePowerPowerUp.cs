using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnginePowerPowerUp : EntityBase, IInteractableObject
{
    public override void OnEntitySpawn(GameSpaceType spaceSpawn)
    {
        _spaceSpawned = spaceSpawn;
        GameManager.OnRegisterEntity?.Invoke(this, _spaceSpawned);
    }

    public void InteractionCallback(ICharacter playerTouched)
    {
        GameManager.Instance.OnFoodEaten?.Invoke(_spaceSpawned);
        playerTouched.PickPowerUp(_entityType);
        Destroy(this.gameObject);
    }
}
