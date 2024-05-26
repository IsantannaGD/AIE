using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartBehavior : EntityBase, IDangerousEncounter
{
    [SerializeField] private float _intangibleTime;
    public void OnSpawnBodyPart()
    {
        StartCoroutine(SpawningRoutine());
    }

    public void DangerousInteractionCallback(ICharacter playerTouched)
    {
        throw new System.NotImplementedException();
    }

    public void DangerousInteractionCallback()
    {
        GameManager.OnGameOver?.Invoke();
    }

    private IEnumerator SpawningRoutine()
    {
        Collider2D col = GetComponent<Collider2D>();

        yield return new WaitForSeconds(_intangibleTime);

        col.enabled = true;
    }

    
}
