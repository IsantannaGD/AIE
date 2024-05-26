using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehavior : EntityBase, IInteractableObject
{
    [SerializeField] private ICharacter _currentPlayer;
    [SerializeField] private GameSpaceType _placeSpawned;

    public void OnSpawn(GameSpaceType placeSpawned)
    {
        _placeSpawned = placeSpawned;
    }

    public void InteractionCallback(ICharacter playerTouched)
    {
        _currentPlayer = playerTouched;
        EatFood();
    }

    private void EatFood()
    {
        GameManager.OnFoodEaten?.Invoke(_placeSpawned);
        _currentPlayer.BodyGrow();
        Destroy(this.gameObject);
    }
}
