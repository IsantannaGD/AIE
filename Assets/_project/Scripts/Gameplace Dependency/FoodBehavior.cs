using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehavior : EntityBase, IInteractableObject
{
    [SerializeField] private ICharacter _currentPlayer;
    [SerializeField] private GameSpaceType _placeSpawned;

    public override void OnEntitySpawn(GameSpaceType spaceSpawn)
    {
        _placeSpawned = spaceSpawn;
        GameManager.OnRegisterEntity?.Invoke(this, _spaceSpawned);
    }

    public void InteractionCallback(ICharacter playerTouched)
    {
        _currentPlayer = playerTouched;
        EatFood();
    }

    private void EatFood()
    {
        GameManager.Instance.OnFoodEaten?.Invoke(_placeSpawned);
        _currentPlayer.BodyGrow();
        Destroy(this.gameObject);
    }

}
