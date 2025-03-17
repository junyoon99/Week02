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
            Destroy(gameObject);  // 중복 방지
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
