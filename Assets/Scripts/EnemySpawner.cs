using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private Transform spawnPointsParent;

    private IEnumerator Start()
    {
        foreach(Transform child in spawnPointsParent)
        {
            Instantiate(enemyPrefab, child.position, Quaternion.identity);
            yield return null;
        }
    }
}
