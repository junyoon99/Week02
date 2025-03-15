using NUnit.Framework;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInputScript : MonoBehaviour
{
	public Rigidbody2D rigidbody2d;
	public float moveSpeed;
	public Vector2 _moveDirection;

	public InputActionAsset inputActionAsset;
	InputActionMap _inputActionMap;

	PlayerState playerState;

	//임시
	Color c;

	private void OnEnable()
	{
		rigidbody2d = GetComponent<Rigidbody2D>();
		moveSpeed = 15f;
		playerState = GetComponent<PlayerState>();

		_inputActionMap = inputActionAsset.FindActionMap("Player");
		_inputActionMap.FindAction("Attack").started += Attack;
		_inputActionMap.FindAction("Move").canceled += MoveEnd;
        _inputActionMap.FindAction("Roll").started += Roll;
		_inputActionMap.FindAction("BulletTime").started += BulletTimeOn;
		_inputActionMap.FindAction("BulletTime").canceled += BulletTimeOff;

		// 암전효과 준비
		backGroundScript = FindFirstObjectByType<BackGroundScript>();
		darkEffect = backGroundScript.darkEffect.GetComponent<SpriteRenderer>();

		//임시
		c = GetComponent<SpriteRenderer>().color;
	}
	private void Update()
	{
	}

	void FixedUpdate()
	{
		if (_inputActionMap.FindAction("Move").ReadValue<Vector2>() != Vector2.zero && 
			(playerState.currentState == PlayerState.playerState.Idle || playerState.currentState == PlayerState.playerState.move))
		{
			Moving();
		}
	}

	//-----------------------------------------------------------------------------------------------
	GameObject attackIMG;
	Coroutine attacking;
	void Attack(InputAction.CallbackContext ctx) 
	{
		AttackStart();
	}
	void AttackStart()
	{
        _inputActionMap.FindAction("Attack").Disable();
		_inputActionMap.FindAction("Roll").Disable();
        if (attacking == null) 
		{
            attacking = StartCoroutine(Attacking());
        }
    }
    IEnumerator Attacking()
    {
        //공격상태 시작
        playerState.currentState = PlayerState.playerState.attack;
        attackIMG = Resources.Load<GameObject>("Prefabs/Attack");
        attackIMG = Instantiate(attackIMG, transform);
		attackIMG.GetComponent<AttackScript>().player = gameObject;
        GetComponent<SpriteRenderer>().color = c;
        // 오브젝트 각도별 행동 바꾸기
        Vector2 advanceDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        float angle = Mathf.Atan2(advanceDirection.y, advanceDirection.x) * Mathf.Rad2Deg;
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
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        // 마우스 방향으로 힘부여
        float force = 50f;
        rigidbody2d.AddForce(advanceDirection * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.15f);
        //공격상태 끝
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
        _inputActionMap.FindAction("Attack").Enable();
        _inputActionMap.FindAction("Roll").Enable();
    }
	public void AttackBlocked(Transform target, float stunTime)
	{
		rigidbody2d.AddForce((transform.position - target.position).normalized * 50f, ForceMode2D.Impulse);
        Destroy(attackIMG);
        if (attacking != null)
        {
            StopCoroutine(attacking);
            attacking = null;
        }
        playerState.currentState &= ~PlayerState.playerState.attack;
		StartCoroutine(StunTime(stunTime));
        _inputActionMap.FindAction("Attack").Enable();
        _inputActionMap.FindAction("Roll").Enable();
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
    void Moving() 
	{
        playerState.currentState |= PlayerState.playerState.move;
        // 입력값 받기
        _moveDirection = _inputActionMap.FindAction("Move").ReadValue<Vector2>();
		// rotation
        float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
		// transform
        rigidbody2d.linearVelocity = _moveDirection * moveSpeed;
	}
	void MoveEnd(InputAction.CallbackContext ctx)
    {
        // 키가 때졌으면 move해제
        playerState.currentState &= ~PlayerState.playerState.move;
        _moveDirection = Vector2.zero;
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
        // 구르기 시작
        playerState.currentState |= PlayerState.playerState.roll;
        // 움직임과 구르기 해제
        _inputActionMap.FindAction("Roll").Disable();
        Debug.Log("구르기!");
        GetComponent<SpriteRenderer>().color = Color.blue;
		if (rolling == null) 
		{
            rolling = StartCoroutine(Rolling());
        }
    }
    IEnumerator Rolling()
    {
        // 구르는중
        float timer = 0.5f;
        rigidbody2d.AddForce(_moveDirection * 50f, ForceMode2D.Impulse);
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
        // 구르기 끝
        if (rolling != null)
        {
			StopCoroutine(rolling);
            rolling = null;
        }
        _inputActionMap.FindAction("Roll").Enable();
        GetComponent<SpriteRenderer>().color = c;
        playerState.currentState &= ~PlayerState.playerState.roll;
    }
	//-----------------------------------------------------------------------------------------------
	BackGroundScript backGroundScript;
	SpriteRenderer darkEffect;
	Coroutine bulletTime;
	void BulletTimeOn(InputAction.CallbackContext ctx) 
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
			c.a += Mathf.Abs(goalDarkAlpha - c.a)/10;
			darkEffect.color = c;
			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
		bulletTime = null;
	}
	void BulletTimeOff(InputAction.CallbackContext ctx) 
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
			Debug.Log(c.a);
			c.a -= Mathf.Abs(c.a - goalDarkAlpha) / 10;
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
}
