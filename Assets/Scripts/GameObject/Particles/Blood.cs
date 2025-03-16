using UnityEngine;

public class Blood : MonoBehaviour
{
    Rigidbody2D rb;
    GameObject player;
    SpriteRenderer spriteRenderer;
    Color c;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindFirstObjectByType<PlayerState>().gameObject;
        Vector3 randomPos = (transform.position - player.transform.position).normalized + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

        spriteRenderer = GetComponent<SpriteRenderer>();
        c = spriteRenderer.color;
        c.r -= Random.Range(0f, 0.2f);
        //c.a -= Random.Range(0f, 0.1f);
        spriteRenderer.color = c;

        rb.AddForce(randomPos.normalized*Random.Range(25f,75f), ForceMode2D.Impulse);
        rb.linearDamping = Random.Range(7f, 11f);
        rb.angularDamping = Random.Range(1f, 2f);
        rb.AddTorque(Random.Range(-10f, 10f));
    }

    void Update()
    {
        if (rb != null && rb.linearVelocity == Vector2.zero && rb.angularVelocity == 0)
        {
            Destroy(rb);
        }
    }
}
