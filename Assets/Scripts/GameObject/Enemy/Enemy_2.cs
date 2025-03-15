using UnityEngine;

public class Enemy_2 : EnemyBase
{
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = 45f;
    }
    public Vector3 goalPosition;
    float randomRange = 15f;
    float timer = 0;
    void Update()
    {
        timer += Time.deltaTime;
        if (player)
        {
            currentState = State.foundPlayer;
        }
        else
        {
            currentState = State.idle;
        }
        if (Vector2.Distance(goalPosition, transform.position) < 1f || timer > 2)
        {
            timer = 0;
            Vector2 randomPosition;
            if (player)
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
    }
}
