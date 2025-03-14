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
        Debug.Log("�� ����!");
        timeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        StartCoroutine(Wait(duration));
    }

    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Debug.Log("�ð��� �ٽ� �����δ�!");
        Time.timeScale = timeScale;
        waiting = false;
    }
}
