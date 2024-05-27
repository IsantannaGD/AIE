using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehavior : EntityBase, IInteractableObject
{
    [SerializeField] private ICharacter _currentPlayer;

    public override void OnEntitySpawn(int gameSet)
    {
        _gameSetID = gameSet;
        GameManager.OnRegisterEntity?.Invoke(this);
    }

    public void InteractionCallback(ICharacter playerTouched)
    {
        _currentPlayer = playerTouched;
        EatFood();
    }

    private void EatFood()
    {
        GameManager.Instance.OnFoodEaten?.Invoke(_gameSetID);
        _currentPlayer.BodyGrow();
        Destroy(this.gameObject);
    }

}
