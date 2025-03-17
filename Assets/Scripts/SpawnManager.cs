using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
	GameObject player;
	public List<GameObject> enemies;
	GameObject cam;
	GameObject Elevator;

	private void Awake()
	{
		enemies = new List<GameObject>();
		player = Resources.Load<GameObject>("Prefabs/Player");
		// 몹 수 늘릴 때마다 수정 필요
		for (int i = 1; i <= 3; i++) 
		{
			enemies.Add(Resources.Load<GameObject>("Prefabs/Enemies/Enemy_" + i.ToString()));
		}
		cam = Resources.Load<GameObject>("Prefabs/MainCamera");
        Elevator = Resources.Load<GameObject>("Prefabs/ElevatorParent");
    }
	void Start()
	{
        //엘베와 플레이어 소환
        player = Instantiate(player);
        Elevator = Instantiate(Elevator, new Vector2(0,30), Quaternion.identity).transform.GetChild(0).gameObject;
		Elevator.GetComponent<Elevato>().isStart = true;
		player.transform.SetParent(Elevator.transform);
		player.transform.localPosition = new Vector3(0, 0, 0);

        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "LobbyScene")
        {
            // 몹 소환
            int score = GameManager.score;
			int bigEnemyStack = 0;
            while (score > 0)
            {
                int randomEnemy = score - 1;
                randomEnemy = Mathf.Clamp(randomEnemy, 0, 2);
                randomEnemy = Random.Range(0, randomEnemy + 1);
                score -= randomEnemy + 1;

                if (randomEnemy == 2) 
				{
					bigEnemyStack++;
					if (bigEnemyStack < 2)
					{
						continue;
					}
					else 
					{
						bigEnemyStack = 0;
					}
				}
                Instantiate(enemies[randomEnemy], RandomPos(), Quaternion.identity);
            }
        }

        cam = Instantiate(cam);
        cam.GetComponent<CameraScript>().player = player;
    }

	Vector2 RandomPos() 
	{
		return new Vector2(Random.Range(-20, 20), Random.Range(-14, 15));
	}
}
