using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteringRamPowerUp : EntityBase, IInteractableObject
{
    public override void OnEntitySpawn(GameSpaceType spaceSpawn)
    {
        _spaceSpawned = spaceSpawn;
        GameManager.OnRegisterEntity?.Invoke(this, _spaceSpawned);
    }

    public void InteractionCallback(ICharacter playerTouched)
    {
        
    }
}
