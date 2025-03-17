using System.Collections;
using UnityEngine;

public class Elevato : MonoBehaviour
{
    public CanvasScript canvas;
    void Start()
    {
        canvas = GameObject.FindFirstObjectByType<CanvasScript>();
    }
    public float pressedTime = 0f;
    public bool isOnPlayer = false;
    public bool isGoDown = false;
    void Update()
    {
        if (isOnPlayer&& canvas.isClear && Input.GetKey(KeyCode.E))
        {
            pressedTime += Time.deltaTime;
            if (pressedTime > 1f) 
            {
                GoDownFloor();
            }
        }
        else 
        {
            pressedTime = 0;
        }
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
        while (time < 1f) 
        {
            moveSpeed += Time.deltaTime*10;
            time += Time.deltaTime;
            playertransform.SetParent(transform);
            playertransform.GetComponent<SpriteRenderer>().sortingLayerName = "Elevator";
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
            Destroy(Camera.main.GetComponent<CameraScript>());
            yield return null;
        }
    }

    Transform playertransform;
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
