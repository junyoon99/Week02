using UnityEngine;

public class Enemy_2 : EnemyBase
{
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = 10f + GameManager.score/3;
    }
    public Vector3 goalPosition;
    float randomRange = 15f;
    float timer = 0;
    void Update()
    {
        timer += Time.deltaTime;
        if (player && currentState == State.idle)
        {
            currentState = State.foundPlayer;
        }
        else if(!player)
        {
            currentState = State.idle;
        }
        if (Vector2.Distance(goalPosition, transform.position) < 1f || timer > 2)
        {
            timer = 0;
            Vector2 randomPosition;
            if (currentState == State.foundPlayer)
            {
                randomPosition = RandomPosition(player.transform.position);
                //while (Physics2D.OverlapCircle(randomPosition, 1f) != null)
                //{
                //    randomPosition = RandomPosition(transform.position);
                //}
                goalPosition = randomPosition;
            }
            else
            {
                randomPosition = RandomPosition(transform.position);
                //while (Physics2D.OverlapCircle(randomPosition, 1f) != null)
                //{
                //    randomPosition = RandomPosition(transform.position);
                //}
                goalPosition = randomPosition;
            }
        }
        if (currentState == State.foundPlayer)
        {
            float Distance = Vector2.Distance(player.transform.position, transform.position);
            if (Distance < 2f)
            {
                goalPosition = player.transform.position;
            }
        }

        rb.AddForce((goalPosition - transform.position).normalized * moveSpeed);
    }

    Vector2 RandomPosition(Vector2 targetPosition) 
    {
        return new Vector2(Random.Range(targetPosition.x - randomRange, targetPosition.x + randomRange), Random.Range(targetPosition.y - randomRange, targetPosition.y + randomRange));
    }

    public override void TakeDamageAction()
    {
        Destroy(gameObject);
        Instantiate(Resources.Load<GameObject>("Prefabs/Particles/BloodParticle"), transform.position, Quaternion.identity);
        if(player)player.GetComponent<PlayerInputScript>().killCount++;
        GameManager.score++;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player") 
        {
            collision.collider.GetComponent<PlayerInputScript>().TakeDamage();
            collision.collider.GetComponent<Rigidbody2D>().AddForce((collision.transform.position - transform.position) * 50f);
        }
    }
}
