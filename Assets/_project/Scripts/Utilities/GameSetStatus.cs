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
    public Dictionary<EntityType, Vector2> TimeTravelRecorder;

    public GameSetStatus(int id)
    {
        GameSetID = id;
    }
}
