using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartBehavior : EntityBase, IDangerousEncounter
{
    [SerializeField] private float _intangibleTime;
    public override void OnEntitySpawn(GameSpaceType spaceSpawn)
    {
        _spaceSpawned = spaceSpawn;
        GameManager.OnRegisterEntity?.Invoke(this, _spaceSpawned);
        StartCoroutine(SpawningRoutine());
    }

    public void DangerousInteractionCallback(ICharacter playerTouched)
    {
       playerTouched.ReceiveDamage();
    }

    private IEnumerator SpawningRoutine()
    {
        Collider2D col = GetComponent<Collider2D>();

        yield return new WaitForSeconds(_intangibleTime);

        col.enabled = true;
    }


}
