using System.Collections;
using UnityEngine;

public class HandGunBullet : MonoBehaviour
{
    public Vector3 fireFromVector;
    public string fireFromObjectTag;
    float moveSpeed = 100f;
    Rigidbody2D rb;
    GameObject bulletLine;

    Vector2 beforePos;
    Vector2 currentPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletLine = Resources.Load<GameObject>("Prefabs/Bullets/BulletLine");
        bulletLine = Instantiate(bulletLine, transform.position, transform.rotation);
        bulletLine.GetComponent<BulletLineScript>().parentBullet = gameObject;

        beforePos = transform.position;
        currentPos = transform.position;
    }

    void Update()
    {
        if (Vector2.Distance(fireFromVector, transform.position) > 50f) 
        {
            Destroy(gameObject);
        }
        beforePos = currentPos;
        rb.linearVelocity = transform.right * moveSpeed;
        currentPos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(beforePos, (currentPos - beforePos).normalized, Vector2.Distance(beforePos, currentPos));
        if (hit &&  hit.collider.tag != fireFromObjectTag)
        {
            hit.collider.TryGetComponent<PlayerState>(out PlayerState playerState);
            if (playerState != null && ((playerState.currentState & PlayerState.playerState.roll) == PlayerState.playerState.roll)) return;

            switch (hit.collider.tag)
            {
                case "Player":
                    Destroy(gameObject);
                    break;

                case "AttackEffect":
                    Debug.Log("นÝป็!");
                    transform.position = new Vector3(hit.point.x, hit.point.y, transform.position.z);

                    hit.collider.transform.parent.GetComponent<PlayerInputScript>().AttackEnd();

                    Vector2 reflectDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                    float angle = Mathf.Atan2(reflectDirection.y, reflectDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    moveSpeed *= 2f;
                    fireFromObjectTag = hit.collider.tag;
                    bulletLine.GetComponent<BulletLineScript>().addPoint();
                    break;

                case "Enemy":
                    hit.collider.GetComponent<EnemyBase>().TakeDamageAction();
                    //HitStop.Instance.Stop(0.1f);
                    Destroy(gameObject);
                    break;

                case "Obstacle":
                    Destroy(gameObject);
                    break;

                default:
                    break;
            }
            currentPos = transform.position;
        }

        bulletLine.transform.position = currentPos;
    }
}
