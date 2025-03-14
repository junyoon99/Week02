using System.Collections;
using UnityEngine;

public class HandGunBullet : MonoBehaviour
{
    public Vector3 fireFromVector;
    public GameObject fireFromObject;
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
        if (hit && hit.collider.gameObject != fireFromObject)
        {
            transform.position = new Vector3(hit.point.x, hit.point.y, transform.position.z);
            currentPos = transform.position;
            switch (hit.collider.tag)
            {
                case "Player":
                    break;
                case "AttackEffect":
                    Debug.Log("นÝป็!");
                    Vector2 reflectDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                    float angle = Mathf.Atan2(reflectDirection.y, reflectDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    moveSpeed *= 2f;
                    fireFromObject = hit.collider.gameObject;
                    bulletLine.GetComponent<BulletLineScript>().addPoint();
                    break;
                case "Enemy":
                    hit.collider.GetComponent<Enemy_1>().TakeDamageAction();
                    HitStop.Instance.Stop(0.3f);
                    Destroy(gameObject);
                    break;
            }
        }

        bulletLine.transform.position = currentPos;
    }
}
