using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static int score;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);  // �ߺ� ����
        }
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "LobbyScene")
        {
            score = 0;
        }
        else 
        {
            score++;
        }
    }
    void Update()
    {
        
    }
}
