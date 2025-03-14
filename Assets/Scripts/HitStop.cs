using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }
    public bool waiting;
    public float timeScale;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
    }
    public void Stop(float duration)
    {
        if (waiting)
        {
            return;
        }
        Debug.Log("더 월드!");
        timeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        StartCoroutine(Wait(duration));
    }

    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Debug.Log("시간은 다시 움직인다!");
        Time.timeScale = timeScale;
        waiting = false;
    }
}
