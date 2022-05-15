using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnedBullet;

    [SerializeReference]
    private GameObject bulletPrefab;

    private Vector3 spawnPosition;

    private void Start()
    {
        spawnPosition = spawnedBullet.transform.position;
    }

    private void Update()
    {
        if(spawnedBullet == null)
        {
            spawnedBullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            spawnedBullet.layer = 6;
        }
    }


}
