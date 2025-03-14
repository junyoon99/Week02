using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    GameObject player;
    GameObject enemy_1;
    private void Awake()
    {
        player = Resources.Load<GameObject>("Prefabs/Player");
        for (int i = 1; i <= 1; i++) 
        {
            enemy_1 = Resources.Load<GameObject>("Prefabs/Enemies/Enemy_" + i.ToString());
        }
        
    }
    void Start()
    {
        player = Instantiate(player);
        GameObject spawnedEnemy = Instantiate(enemy_1, new Vector3(2, 0, 0), Quaternion.Euler(Vector3.zero));
        spawnedEnemy.TryGetComponent<EnemyBase>(out EnemyBase enemyBase);
        enemyBase.player = player;
    }
    void Update()
    {
        
    }
}
