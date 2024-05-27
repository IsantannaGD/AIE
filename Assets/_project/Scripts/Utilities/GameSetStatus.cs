using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSetStatus
{
    public readonly int GameSetID;
    public Player Player;
    public EnemyController EnemyController;
    public EntityBase Food;
    public char LeftInput;
    public char RightInput;

    public GameSetStatus(int id, char leftInput, char rightInput)
    {
        GameSetID = id;
        LeftInput = leftInput;
        RightInput = rightInput;
    }
}
