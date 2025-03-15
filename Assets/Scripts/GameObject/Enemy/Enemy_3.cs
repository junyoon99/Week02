using System.Collections;
using UnityEngine;

public class Enemy_3 : EnemyBase
{
	Coroutine attackCoroutine;
	bool isAttacking = false;

    Color c;
    void Start()
	{
		moveSpeed = 10f;
		rb = GetComponent<Rigidbody2D>();
		c = GetComponent<SpriteRenderer>().color;
		isBlock = true;
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
			float Distance = Vector2.Distance(player.transform.position, transform.position);
			if (Distance < 5f && !isAttacking && canMove)
			{
				attackSkill = StartCoroutine(Attack());
			}
		}
	}
    private void FixedUpdate()
    {
        if (currentState == State.foundPlayer && !isAttacking && canMove)
        {
            rb.linearVelocity = (player.transform.position - transform.position).normalized * moveSpeed;
        }
    }
    IEnumerator Attack() 
	{
		Debug.Log("돌진준비");
        isAttacking = true;
        moveSpeed = 0;
        yield return new WaitForSeconds(0.5f);
        Vector2 rushDirection = (player.transform.position - transform.position).normalized;
        yield return new WaitForSeconds(0.5f);
        Debug.Log("돌진");
        GetComponent<SpriteRenderer>().color = Color.blue; //돌진상태표시
        rb.linearDamping = 0f;
		rb.AddForce(rushDirection * 500, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
		rb.linearDamping = 10f;
		GetComponent<SpriteRenderer>().color = Color.yellow;
        isBlock = false;
        yield return new WaitForSeconds(2f);
        Debug.Log("쿨타임끝");
		AttackEnd();
    }

	void AttackEnd() 
	{
        isBlock = true;
        GetComponent<SpriteRenderer>().color = c;
        moveSpeed = 10f;
        isAttacking = false;
    }

    public override void TakeDamageAction()
    {
		if (attackSkill != null) 
		{
            StopCoroutine(attackSkill);
			attackSkill = null;
        }
        
        Destroy(gameObject);
    }
	public override void Block()
	{
		StartCoroutine(KnockBack(player.transform,500f,1f));
    }

	IEnumerator KnockBack(Transform target, float force, float stunTime)
	{
        canMove = false;
        if (attackSkill != null)
        {
            StopCoroutine(attackSkill);
			AttackEnd();
            attackSkill = null;
        }
        rb.AddForce((transform.position - target.position).normalized * force, ForceMode2D.Impulse);
        while (stunTime >= 0)
        {
			Debug.Log("남은 스턴시간 : " + stunTime);
            canMove = false;
            stunTime -= Time.deltaTime;
            yield return null;
        }
        canMove = true;
    }
}
