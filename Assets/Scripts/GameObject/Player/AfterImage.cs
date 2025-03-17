using UnityEngine;

public class AfterImage : MonoBehaviour
{
    SpriteRenderer sprite;
    Color color;
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
        color.a -= Time.deltaTime;
        sprite.color = color;
        if (sprite.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
