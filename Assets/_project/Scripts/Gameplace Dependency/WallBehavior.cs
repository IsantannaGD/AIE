using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehavior : MonoBehaviour, IDangerousEncounter
{
    private void Start()
    {
        GameManager.OnRegisterLimits?.Invoke(transform.position);
    }

    public void DangerousInteractionCallback(ICharacter playerTouched)
    {
        playerTouched.ReceiveDamage();
    }
}
