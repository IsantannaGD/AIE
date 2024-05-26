using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAssemblyFunc : MonoBehaviour
{
    [SerializeField] private bool _isVertical;
    [SerializeField] private float _cellSize = 0.3f;
    [SerializeField] private string _prefix;

    [SerializeField] private List<WallBehavior> _allWalls = new List<WallBehavior>();

    public List<WallBehavior> AllWalls => _allWalls;

    [ContextMenu("Organize")]
    private void OrganizeWall()
    {
        Transform parent = transform;
        Vector3 startPosition = parent.GetChild(0).transform.position;

        for (int i = 1; i < parent.childCount; i++)
        {
            Transform cPos = parent.GetChild(i).transform;
            WallBehavior block = cPos.GetComponent<WallBehavior>();

            if (_isVertical)
            {
                cPos.position = new Vector2(startPosition.x, startPosition.y - _cellSize * i);
            }
            else
            {
                cPos.position  = new Vector2(startPosition.x + _cellSize * i, startPosition.y);
            }

            cPos.name = $"[Block] {_prefix} wall {i + 1}";
            _allWalls.Add(block);
        }
    }
}
