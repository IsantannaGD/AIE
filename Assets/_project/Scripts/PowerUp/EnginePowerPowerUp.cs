using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnginePowerPowerUp : EntityBase, IInteractableObject
{
    public override void OnEntitySpawn(int gameSet)
    {
        _gameSetID = gameSet;
        GameManager.OnRegisterEntity?.Invoke(this);
    }

    public void InteractionCallback(ICharacter playerTouched)
    {
        GameManager.Instance.OnFoodEaten?.Invoke(_gameSetID);
        playerTouched.PickPowerUp(_entityType);
        Destroy(this.gameObject);
    }
}
