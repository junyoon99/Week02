using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    int score = 0;
    GameObject[] leftEnemies;
    private void Awake()
    {
    }
    public bool isClear = false;
    private void Update()
    {
        leftEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (leftEnemies.Length == 0) 
        {
            isClear = true;
        }   
    }
}
