using UnityEngine;

public class AfterImage : MonoBehaviour
{
    SpriteRenderer sprite;
    Color color;
    float deleteTIme = 2f;
    float currTime = 0;
    void Start()
    {
        if (TryGetComponent<SpriteRenderer>(out sprite)) 
        {
            color = sprite.color;
        }
    }
    void Update()
    {
        currTime += Time.deltaTime;
        color.a -= 1 / 255f;
        sprite.color = color;
        if (deleteTIme < currTime)
        {
            Destroy(gameObject);
        }
    }
}
