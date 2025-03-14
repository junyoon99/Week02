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

	//�ӽ�
    Color c;

    private void OnEnable()
	{
		rigidbody2d = GetComponent<Rigidbody2D>();
		moveSpeed = 15f;
		playerState = GetComponent<PlayerState>();

		_inputActionMap = inputActionAsset.FindActionMap("Player");
		_inputActionMap.FindAction("Attack").started += Attack;
		_inputActionMap.FindAction("Move").started += Move;
		_inputActionMap.FindAction("Roll").started += Roll;
		_inputActionMap.FindAction("BulletTime").started += BulletTimeOn;
		_inputActionMap.FindAction("BulletTime").canceled += BulletTimeOff;

        // ����ȿ�� �غ�
        backGroundScript = FindFirstObjectByType<BackGroundScript>();
        darkEffect = backGroundScript.darkEffect.GetComponent<SpriteRenderer>();

        //�ӽ�
        c = GetComponent<SpriteRenderer>().color;
    }
	private void Update()
	{
		_moveDirection = _inputActionMap.FindAction("Move").ReadValue<Vector2>();
		if (_moveDirection == Vector2.zero)
		{
			playerState.currentState &= ~PlayerState.playerState.move;
		}
	}

	void FixedUpdate()
	{
		if (playerState.currentState == PlayerState.playerState.move || playerState.currentState == PlayerState.playerState.Idle)
		{
			rigidbody2d.linearVelocity = _moveDirection * moveSpeed;
			if (_moveDirection != Vector2.zero) 
			{
                float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg + 90;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
        }
	}

    //-----------------------------------------------------------------------------------------------
    GameObject attackIMG;
	void Attack(InputAction.CallbackContext ctx)
	{
		//Debug.Log("Attack!");
		// ���� ����
		if ((playerState.currentState & PlayerState.playerState.attack) != PlayerState.playerState.attack) 
		{
            StartCoroutine(Attacking());
        }
	}

	IEnumerator Attacking() 
	{
        //���ݻ��� ����
		playerState.currentState = PlayerState.playerState.attack;
        attackIMG = Resources.Load<GameObject>("Prefabs/Attack");
        attackIMG = Instantiate(attackIMG,transform);
		GetComponent<SpriteRenderer>().color = c;
		// ������Ʈ ������ �ൿ �ٲٱ�
		Vector2 advanceDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
		float angle = Mathf.Atan2(advanceDirection.y, advanceDirection.x) * Mathf.Rad2Deg;
		if (angle < 0) angle += 360;

		if ((angle >= 0 && angle < 45f) || (angle >= 360-45 && angle < 360))
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
		// ���콺 �������� ���ο�
		float force = 25f;
		rigidbody2d.AddForce(advanceDirection * force, ForceMode2D.Impulse);
		yield return new WaitForSeconds(0.15f);

		playerState.currentState &= ~PlayerState.playerState.attack;    //���ݻ��� ��
		Destroy(attackIMG);
	}

	void Move(InputAction.CallbackContext ctx) 
	{
		playerState.currentState |= PlayerState.playerState.move;
	}

	//-----------------------------------------------------------------------------------------------
	void Roll(InputAction.CallbackContext ctx) 
	{
		// ��������¿� ���ݻ��°� ���� �� ������ ����
		if (!hasState(PlayerState.playerState.roll) && !hasState(PlayerState.playerState.attack))
		{
            StartCoroutine(Rolling());
        }
	}
	IEnumerator Rolling() 
	{
		// ������ ����
		playerState.currentState |= PlayerState.playerState.roll;
        Debug.Log("������!");
        GetComponent<SpriteRenderer>().color = Color.blue;
		yield return new WaitForSeconds(2f);
		GetComponent<SpriteRenderer>().color = c;
		// ������ ��
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
        while (c.a < goalDarkAlpha)
		{
            Debug.Log(c.a);
			c.a += (goalDarkAlpha - c.a)/10;
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
        while (c.a > goalDarkAlpha)
        {
            Debug.Log(c.a);
            c.a -= (c.a - goalDarkAlpha) / 10;
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
