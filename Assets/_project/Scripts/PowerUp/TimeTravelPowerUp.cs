using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTravelPowerUp : EntityBase, IInteractableObject
{
    public override void OnEntitySpawn(int gameSet)
    {
        _gameSetID = gameSet;
        GameManager.OnRegisterEntity?.Invoke(this);
    }

    public void InteractionCallback(ICharacter playerTouched)
    {
        playerTouched.PickPowerUp(_entityType);
        Destroy(this.gameObject);
    }

    protected override void OnSaveStateCallback()
    { }
}
