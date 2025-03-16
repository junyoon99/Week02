using NUnit.Framework;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerInputScript : MonoBehaviour
{
	public Rigidbody2D rigidbody2d;
	public float moveSpeed;
	public Vector2 _moveDirection;
    public PlayerCanvas playerCanvas;

	public InputActionAsset inputActionAsset;
	InputActionMap _inputActionMap;

	PlayerState playerState;

	//�ӽ�
	Color c;

	private void OnEnable()
	{
		rigidbody2d = GetComponent<Rigidbody2D>();
		moveSpeed = 15f;
		playerState = GetComponent<PlayerState>();
        playerCanvas = GetComponent<PlayerCanvas>();

        _inputActionMap = inputActionAsset.FindActionMap("Player");
		_inputActionMap.FindAction("Attack").started += Attack;
		_inputActionMap.FindAction("Attack").started += ChargeAttack;
		_inputActionMap.FindAction("Move").started += Move;
		_inputActionMap.FindAction("Move").canceled += MoveKeyUp;
        _inputActionMap.FindAction("Roll").started += Roll;
		_inputActionMap.FindAction("BulletTime").started += UseBulletTime;
		_inputActionMap.FindAction("BulletTime").canceled += CancleBulletTime;

		// ����ȿ�� �غ�
		backGroundScript = FindFirstObjectByType<BackGroundScript>();
		darkEffect = backGroundScript.darkEffect.GetComponent<SpriteRenderer>();

		//�ӽ�
		c = GetComponent<SpriteRenderer>().color;

        killCount = 3;
        SkillCoolKillCount = 3;
    }
	private void Update()
	{
        DrawSkillBar();
    }

	//-----------------------------------------------------------------------------------------------
	GameObject attackIMG;
	Coroutine attacking;
    float ChangeAngle(float angle)
    {
        if (angle < 0) angle += 360;

        if ((angle >= 0 && angle < 45f) || (angle >= 360 - 45 && angle < 360))
        {
            angle = 0;
        }
        else if (angle >= 45 && angle < 135)
        {
            angle = 90;
        }
        else if (angle >= 135 && angle < 225)
        {
            angle = 180;
        }
        else if (angle >= 225 && angle < 360)
        {
            angle = 270;
        }
        angle += 90;

        return angle;
    }
    void Attack(InputAction.CallbackContext ctx) 
	{
		AttackStart();
	}
	void AttackStart()
	{
        _inputActionMap.FindAction("Attack").started -= Attack;
		_inputActionMap.FindAction("Roll").started -= Roll;
        if (attacking == null) 
		{
            attacking = StartCoroutine(Attacking());
        }
    }
    IEnumerator Attacking()
    {
        //���ݻ��� ����
        playerState.currentState |= PlayerState.playerState.attack;
        attackIMG = Resources.Load<GameObject>("Prefabs/Attack");
        attackIMG = Instantiate(attackIMG, transform);
		attackIMG.GetComponent<AttackScript>().player = gameObject;
        GetComponent<SpriteRenderer>().color = c;
        // ������Ʈ ������ �ൿ �ٲٱ�
        Vector2 advanceDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        float angle = Mathf.Atan2(advanceDirection.y, advanceDirection.x) * Mathf.Rad2Deg;
		angle = ChangeAngle(angle);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        // ���콺 �������� ���ο�
        float force = 50f;
        rigidbody2d.AddForce(advanceDirection * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.15f);
        //���ݻ��� ��
        AttackEnd();
    }
    public void AttackEnd() 
	{
        Destroy(attackIMG);
        if (attacking != null) 
		{
            StopCoroutine(attacking);
			attacking = null;
        } 
        playerState.currentState &= ~PlayerState.playerState.attack;
        _inputActionMap.FindAction("Attack").started += Attack;
        _inputActionMap.FindAction("Roll").started += Roll;
    }
	public void AttackBlocked(Transform target, float stunTime)
	{
		rigidbody2d.AddForce((transform.position - target.position).normalized * 40f, ForceMode2D.Impulse);
        Destroy(attackIMG);
        if (attacking != null)
        {
            StopCoroutine(attacking);
            attacking = null;
        }
        playerState.currentState &= ~PlayerState.playerState.attack;
		StartCoroutine(StunTime(stunTime));
        _inputActionMap.FindAction("Attack").started += Attack;
        _inputActionMap.FindAction("Roll").started += Roll;
    }

	IEnumerator StunTime(float time) 
	{
        _inputActionMap.Disable();
        GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(time);
		GetComponent<SpriteRenderer>().color = c;
		_inputActionMap.Enable();
    }

    //-----------------------------------------------------------------------------------------------
    public float chargeTime = 0f;
    Coroutine charging;
    public float killCount = 3;
    public float SkillCoolKillCount = 3;
    void ChargeAttack(InputAction.CallbackContext ctx) 
	{
        if (killCount >= SkillCoolKillCount)
        {
            ChargeStart();
        }
    }
	void ChargeStart()
    {
		Debug.Log("��������!");
        playerState.currentState |= PlayerState.playerState.charge;
		if (charging == null)
		{
            charging = StartCoroutine(Charging());
        }
    }
	IEnumerator Charging()
	{
        bool useBulletTime = false;
        while (_inputActionMap.FindAction("Attack").IsPressed())
		{
			chargeTime += Time.deltaTime;
			yield return null;
            if (!useBulletTime && chargeTime > 0.3f) 
            {
                useBulletTime = true;
                BulletTimeOn();
            }
		}
		ChargeEnd();
    }
    void ChargeEnd()
	{
        BulletTimeOff();
        playerState.currentState &= ~PlayerState.playerState.charge;

        if (charging != null) 
		{
            StopCoroutine(charging);
            charging = null;
        }
        if (chargeTime > 0.2f)
        {
            Debug.Log("��������!");
            killCount = 0;
            // ������ ���콺 ����
            Vector2 goalPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 advanceDirection = (goalPos - (Vector2)transform.position);
            float angle = Mathf.Atan2(advanceDirection.y, advanceDirection.x) * Mathf.Rad2Deg;
            angle = ChangeAngle(angle);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            //����ĳ��Ʈ�������� ���鿡�� ������
            RaycastHit2D[] Hits = Physics2D.CircleCastAll(transform.position, 1.5f, advanceDirection.normalized, Vector2.Distance(goalPos, transform.position));
            Transform iswall = null;
            foreach (RaycastHit2D hit in Hits)
            {
                switch (hit.collider.tag)
                {
                    case "Enemy":
                        hit.collider.TryGetComponent<EnemyBase>(out EnemyBase enemy);
                        if (enemy)
                        {
                            if (!enemy.isBlock)
                            {
                                enemy.TakeDamageAction();
                            }
                            else
                            {
                                enemy.Block();
                            }
                        }
                        break;
                    case "Obstacle":
                        Debug.Log("��ֹ�!");
                        goalPos = hit.point;
                        advanceDirection = (goalPos - (Vector2)transform.position);
                        iswall = hit.transform;
                        break;

                    default:
                        break;
                }
                if (iswall) break;
            }
            // �̵� �� ��¦ �ݵ�
            Vector2 beforePos = transform.position;
            transform.position = (Vector2)goalPos;
            if (iswall) //���̸� ���ݴ��������
            {
                AttackBlocked(iswall, 0.5f);
            }
            else 
            {
                float force = 50f;
                rigidbody2d.AddForce(advanceDirection.normalized * force, ForceMode2D.Impulse);
            }
            //         //�̵� �ܻ�
            //         float step = 0.5f;
            //for (Vector2 pos = beforePos; Vector2.Distance(pos, goalPos) > 0.1f; pos += advanceDirection.normalized*step)
            //{
            //             Debug.Log("�̵���!");
            //             GameObject afterImage = new GameObject("AfterImage");
            //	afterImage.AddComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            //	afterImage.AddComponent<AfterImage>();
            //             afterImage.transform.position = pos;
            //         }
            // �̵� �ܻ�
            float step = 0.5f; // ���� ũ�� ����
            Vector2 direction = advanceDirection.normalized * step;
            for (Vector2 pos = beforePos; (goalPos - pos).sqrMagnitude > step * step; pos += direction)
            {
                Debug.Log("�̵���!");
                GameObject afterImage = new GameObject("AfterImage");
                afterImage.AddComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                afterImage.AddComponent<AfterImage>();
                afterImage.transform.position = pos;
            }
        }
        else 
		{
			Debug.Log("��������!");
        }

		chargeTime = 0f;
    }
    //-----------------------------------------------------------------------------------------------
    Coroutine moving;
    void Move(InputAction.CallbackContext ctx) 
	{
        playerState.currentState |= PlayerState.playerState.move;
		if (moving == null) 
		{
            StartCoroutine(Moving());
        }
    }
	IEnumerator Moving() 
	{
		while (true) 
		{
			if (playerState.currentState == PlayerState.playerState.Idle || playerState.currentState == PlayerState.playerState.move) 
			{
                // �Է°� �ޱ�
                _moveDirection = _inputActionMap.FindAction("Move").ReadValue<Vector2>();
				if (_moveDirection != Vector2.zero) 
				{
                    // rotation
                    float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg + 90;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                    // transform
                    rigidbody2d.linearVelocity = _moveDirection * moveSpeed;
                }
            }
            yield return null;
        }
    }
	void MoveKeyUp(InputAction.CallbackContext ctx)
    {
		Debug.Log("Ű����!");
        MoveEnd();
    }
	void MoveEnd()
	{
        if (moving != null)
        {
            StopCoroutine(moving);
            moving = null;
        }
        // Ű�� �������� move����
        playerState.currentState &= ~PlayerState.playerState.move;
    }
    //-----------------------------------------------------------------------------------------------
    void Roll(InputAction.CallbackContext ctx)
	{
		if (_moveDirection != Vector2.zero)
		{
            RollStart();
        }
	}
	Coroutine rolling;
	void RollStart()
	{
        // ������ ����
        playerState.currentState |= PlayerState.playerState.roll;
		MoveEnd();
        // �����Ӱ� ������ ����
        _inputActionMap.FindAction("Roll").started -= Roll;
        _inputActionMap.FindAction("Move").started -= Move;
        Debug.Log("������!");
        rigidbody2d.AddForce(_moveDirection * 50f, ForceMode2D.Impulse);
        GetComponent<SpriteRenderer>().color = Color.blue;
		if (rolling == null) 
		{
            rolling = StartCoroutine(Rolling());
        }
    }
    IEnumerator Rolling()
    {
        // ��������
        float timer = 0.5f;
        while (timer > 0f)
        {
            if (hasState(PlayerState.playerState.attack)) break;
            timer -= Time.deltaTime;
            yield return null;
        }
		RollEnd();
    }
    void RollEnd()
    {
        // ������ ��
        if (rolling != null)
        {
			StopCoroutine(rolling);
            rolling = null;
        }
        _inputActionMap.FindAction("Roll").started += Roll;
        _inputActionMap.FindAction("Move").started += Move;
        GetComponent<SpriteRenderer>().color = c;
        playerState.currentState &= ~PlayerState.playerState.roll;
    }
	//-----------------------------------------------------------------------------------------------
	BackGroundScript backGroundScript;
	SpriteRenderer darkEffect;
	Coroutine bulletTime;
    void UseBulletTime(InputAction.CallbackContext ctx) 
    {
        BulletTimeOn();
    }
	void BulletTimeOn() 
	{
		Time.timeScale = 0.2f;
		HitStop.Instance.timeScale = Time.timeScale;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;
		if (bulletTime != null)
		{
			StopCoroutine(bulletTime);
			bulletTime = null;
		}
		bulletTime = StartCoroutine(darkEffectOn());
	}
	IEnumerator darkEffectOn() 
	{
		Color c = darkEffect.color;
		float goalDarkAlpha = 230/255f;
		while (c.a < goalDarkAlpha - 1 / 255f)
		{
			Debug.Log(c.a);
			c.a += Mathf.Abs(goalDarkAlpha - c.a)/5;
			darkEffect.color = c;
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
		bulletTime = null;
	}
    void CancleBulletTime(InputAction.CallbackContext ctx) 
    {
        BulletTimeOff();
    }
	void BulletTimeOff() 
	{
		Time.timeScale = 1f;
		HitStop.Instance.timeScale = Time.timeScale;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;
		if (bulletTime != null)
		{
			StopCoroutine(bulletTime);
			bulletTime = null;
		}
		bulletTime = StartCoroutine(darkEffectOff());
	}
	IEnumerator darkEffectOff() 
	{
		Color c = darkEffect.color;
		float goalDarkAlpha = 0f;
		while (c.a > goalDarkAlpha + 1 / 255f)
		{
			//Debug.Log(c.a);
			c.a -= Mathf.Abs(c.a - goalDarkAlpha) / 5;
			darkEffect.color = c;
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
		bulletTime = null;
	}
	//-----------------------------------------------------------------------------------------------
	bool hasState(PlayerState.playerState state) 
	{
		if ((playerState.currentState & state) == state)
		{
			return true;
		}
		else 
		{
			return false;
		}
	}
    public void TakeDamage() 
    {
        GetComponent<SpriteRenderer>().color = Color.blue;
        rigidbody2d.linearDamping = 0.5f;
        _inputActionMap.Disable();
        StopAllCoroutines();
        this.enabled = false;
    }

    public void DrawSkillBar()
    {
        killCount = Mathf.Clamp(killCount, 0, SkillCoolKillCount);
        playerCanvas.skillBar.Find("Fill").localScale = new Vector3(killCount / SkillCoolKillCount, 1, 1);
    }
}
