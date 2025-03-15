using UnityEngine;

public class Enemy_1 : EnemyBase
{
	public Transform WeaponTransform;
	public WeaponBase Weapon;

	void Start()
	{
		WeaponTransform = transform.Find("Weapon");
		Weapon = WeaponTransform.GetChild(0).GetComponent<WeaponBase>();
		rb = GetComponent<Rigidbody2D>();
		moveSpeed = 5f;
	}

	void Update()
	{
		if (player)
		{
			currentState = State.foundPlayer;
		}
		else 
		{
			currentState = State.idle;
		}

		if (currentState == State.foundPlayer) 
		{
            Vector3 gunDirection = (player.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(gunDirection.y, gunDirection.x) * Mathf.Rad2Deg;
            WeaponTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
			float Distance = Vector2.Distance(player.transform.position, transform.position);
			if (Distance < 10f) 
			{
                rb.linearVelocity = (transform.position - player.transform.position).normalized * moveSpeed;
                Weapon.fireCoolTime += Time.deltaTime;
            }
			else if (Distance < 20f)
			{
				Weapon.fireCoolTime += Time.deltaTime;
			}
			else 
			{
				rb.linearVelocity = (player.transform.position - transform.position).normalized * moveSpeed;
				Weapon.fireCoolTime = 0;
			}
        }
	}
	public override void TakeDamageAction() 
	{
		Destroy(gameObject);
    }
}
