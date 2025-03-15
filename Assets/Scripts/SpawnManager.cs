using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    GameObject player;
    public List<GameObject> enemies;
    private void Awake()
    {
        enemies = new List<GameObject>();
        player = Resources.Load<GameObject>("Prefabs/Player");
        // 몹 수 늘릴 때마다 수정 필요
        for (int i = 1; i <= 3; i++) 
        {
            enemies.Add(Resources.Load<GameObject>("Prefabs/Enemies/Enemy_" + i.ToString()));
        }
        
    }
    void Start()
    {
        player = Instantiate(player);

        GameObject spawnedEnemy1 = Instantiate(enemies[0], new Vector3(2, 0, 0), Quaternion.Euler(Vector3.zero));
        spawnedEnemy1.TryGetComponent<EnemyBase>(out EnemyBase enemyBase);
        enemyBase.player = player;

        GameObject spawnedEnemy2 = Instantiate(enemies[1], new Vector3(10, 0, 0), Quaternion.Euler(Vector3.zero));
        spawnedEnemy2.TryGetComponent<EnemyBase>(out EnemyBase enemyBase2);
        enemyBase2.player = player;

        GameObject spawnedEnemy3 = Instantiate(enemies[2], new Vector3(10, 0, 0), Quaternion.Euler(Vector3.zero));
        spawnedEnemy3.TryGetComponent<EnemyBase>(out EnemyBase enemyBase3);
        enemyBase3.player = player;
    }
}
