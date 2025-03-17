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
		if (player && currentState == State.idle)
		{
			currentState = State.foundPlayer;
		}
		else if(!player)
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
		Debug.Log("�����غ�");
        isAttacking = true;
        moveSpeed = 0;
        // ����
        yield return new WaitForSeconds(0.5f);
        Vector2 rushDirection = (player.transform.position - transform.position).normalized;
        currentState = State.attack;
        // �������� �� ����
        yield return new WaitForSeconds(0.15f);
        Debug.Log("����");
        GetComponent<SpriteRenderer>().color = Color.blue; //��������ǥ��
		rb.AddForce(rushDirection * 1000, ForceMode2D.Impulse);
        float timer = 0.3f;
        // �������� �÷��̾�� �浹üũ
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            Collider2D myCollider = GetComponent<Collider2D>();
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            if (myCollider.IsTouching(playerCollider)) 
            {
                Debug.Log("�÷��̾� ����");
                player.GetComponent<PlayerInputScript>().AttackBlocked(transform, 1f);
                player.GetComponent<PlayerInputScript>().TakeDamage();
                Block();
            }
            yield return null;
        }
		GetComponent<SpriteRenderer>().color = Color.yellow;
        isBlock = false;
        currentState = State.stuned;
        yield return new WaitForSeconds(2f);
        //Debug.Log("��Ÿ�ӳ�");
		AttackEnd();
    }

	void AttackEnd() 
	{
        currentState = State.idle;
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
        Instantiate(Resources.Load<GameObject>("Prefabs/Particles/BloodParticle"), transform.position, Quaternion.identity);
        if(player)player.GetComponent<PlayerInputScript>().killCount++;
        GameManager.score++;
        Destroy(gameObject);
    }
	public override void Block()
	{
		StartCoroutine(KnockBack(player.transform,400f,1f));
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
			//Debug.Log("���� ���Ͻð� : " + stunTime);
            canMove = false;
            stunTime -= Time.deltaTime;
            yield return null;
        }
        canMove = true;
    }
}
