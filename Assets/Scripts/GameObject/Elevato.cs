using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Elevato : MonoBehaviour
{
    public CanvasScript canvas;
    void Start()
    {
        canvas = GameObject.FindFirstObjectByType<CanvasScript>();
    }
    GameObject player;
    public float pressedTime = 0f;
    public bool isOnPlayer = false;
    public bool isGoDown = false;
    public bool isStart = false;
    Transform playertransform;

    private void Awake()
    {
        fill = transform.Find("PressE").Find("Fill");
        fill.localScale = new Vector3(1, 0, 1);
    }

    GameObject[] enemies;
    public bool isClear = false;
    bool isDown = false;
    Transform fill;
    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            isClear = true;
            fill.parent.gameObject.SetActive(true);
        }
        else 
        {
            fill.parent.gameObject.SetActive(false);
        }

        if (!playertransform)
        {
            playertransform = GameObject.FindFirstObjectByType<PlayerState>().transform;
        }
        else
        {
            if (isStart)
            {
                isStart = false;
                GoOnFloor();
            }

            if (isOnPlayer && isClear && Input.GetKey(KeyCode.E))
            {
                pressedTime += Time.deltaTime;
                fill.localScale = new Vector3(1, pressedTime / 1, 1);
                if (pressedTime > 1f && !isDown)
                {
                    isDown = true;
                    GoDownFloor();
                }
            }
            else
            {
                pressedTime = 0;
                fill.localScale = new Vector3(1, pressedTime / 1, 1);
            }
        }
    }

    void GoOnFloor()
    {
        goingOn = StartCoroutine(GoingOn());
    }
    public Coroutine goingOn;
    IEnumerator GoingOn() 
    {
        Debug.Log("GoingOn");
        Vector2 targetPos = Vector2.zero; // 목표 위치
        playertransform.GetComponent<CircleCollider2D>().enabled = false;
        playertransform.GetComponent<CapsuleCollider2D>().enabled = false;
        playertransform.GetComponent<PlayerInputScript>().StopAllCoroutines();
        playertransform.GetComponent<PlayerInputScript>().StopAllCoroutines();
        playertransform.GetComponent<PlayerInputScript>()._inputActionMap.Disable();
        GetComponent<TilemapRenderer>().sortingLayerName = "FakeHeight";
        playertransform.GetComponent<SpriteRenderer>().sortingLayerName = "FakeHeight";

        while (Vector3.Distance(transform.position, targetPos) > 0.1f) 
        {
            float speed = Vector2.Distance(targetPos, transform.position);
            speed = Mathf.Clamp(speed, 2f, 100f);
            if (speed < 3f) 
            {
                GetComponent<TilemapRenderer>().sortingLayerName = "Elevator";
                playertransform.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
            }
            transform.Translate(Vector2.down * speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos; // 정확한 위치에 고정

        // 위에서 내려옴 끝
        GameObject.Find("SafeZone").gameObject.SetActive(false);
        playertransform.GetComponent<CircleCollider2D>().enabled = true;
        playertransform.GetComponent<CapsuleCollider2D>().enabled = true;
        playertransform.GetComponent<PlayerInputScript>()._inputActionMap.Enable();
        playertransform.SetParent(null);
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<EnemyBase>().player = playertransform.gameObject;
        }
        //playertransform.GetComponent<PlayerInputScript>().BulletTimeOn();
    }

    float moveSpeed = 0f;
    void GoDownFloor() 
    {
        if (!isGoDown) 
        {
            isGoDown = true;
            StartCoroutine(GoingDown());
        }
    }

    IEnumerator GoingDown() 
    {
        float time = 0;
        playertransform.GetComponent<PlayerInputScript>().StopAllCoroutines();
        playertransform.GetComponent<PlayerInputScript>()._inputActionMap.Disable();
        playertransform.GetComponent<CircleCollider2D>().enabled = false;
        playertransform.GetComponent<CapsuleCollider2D>().enabled = false;
        canvas.GetComponent<CanvasScript>().UseFadeOut();
        while (time < 5f) 
        {
            moveSpeed += Time.deltaTime*10;
            time += Time.deltaTime;
            playertransform.SetParent(transform);
            playertransform.GetComponent<SpriteRenderer>().sortingLayerName = "Elevator";
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
            Camera.main.GetComponent<CameraScript>();
            yield return null;
        }
        //playertransform.GetComponent<PlayerInputScript>().inputActionAsset.Enable();
        //아래로내려감 끝

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("on");
        if (collision.tag == "Player") 
        {
            playertransform = collision.transform;
            isOnPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("off");
        if (collision.tag == "Player")
        {
            isOnPlayer = false;
        }
    }
}
