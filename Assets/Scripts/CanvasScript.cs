using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{
    Transform Fade;
    Transform Score;
    Transform BulletTimeBar;
    public Transform gameOverPanel;

    private void Awake()
    {
        Fade = transform.Find("FadeOut");
        Score = transform.Find("Score").Find("Text");
        BulletTimeBar = transform.Find("BulletTimeBar").Find("Fill");
        gameOverPanel = transform.Find("GameOverPanel");
        gameOverPanel.gameObject.SetActive(false);
    }
    private void Start()
    {
        StartCoroutine(FadeIn());
    }
    IEnumerator FadeIn() 
    {

        Fade.localScale = new Vector3(1, 1, 1);
        Color c = Fade.GetComponent<Image>().color;
        c.a = 1;
        Fade.GetComponent<Image>().color = c;
        while (Fade.GetComponent<Image>().color.a > 0) 
        {
            c.a -= Time.deltaTime;
            Fade.GetComponent<Image>().color = c;
            yield return null;
        }
        Fade.localScale = new Vector3(1, 0, 1);
    }
    bool isGameOver = false;
    private void Update()
    {
        Score.GetComponent<TextMeshProUGUI>().text = GameManager.score.ToString();
        player = GameObject.FindFirstObjectByType<PlayerState>().gameObject;
        if (Input.GetKey(KeyCode.Space) && isGameOver) 
        {
            Debug.Log("¿ÁΩ√¿€");
            SceneManager.LoadScene("LobbyScene");
        }
        PlayerInputScript p = player.GetComponent<PlayerInputScript>();
        if (p != null && p.bulletTimeMax != 0)
        {
            BulletTimeBar.localScale = new Vector3(p.currentLeftBulletTime / p.bulletTimeMax, 1, 1);
        }
        if (GameObject.FindFirstObjectByType<PlayerInputScript>().isDead)
        {
            StartCoroutine(GameOver());
        }
    }
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.5f);
        isGameOver = true;
        gameOverPanel.gameObject.SetActive(true);
    }

    public void UseFadeOut() 
    {
        StartCoroutine(FadeOut());
    }
    float moveSpeed = 0f;
    GameObject player;
    IEnumerator FadeOut() 
    {
        yield return new WaitForSeconds(0.5f);
        while (Fade.localScale.y < 1 || Fade.GetComponent<Image>().color.a < 1) 
        {
            if (Fade.localScale.y < 1)
            {
                Fade.localScale += new Vector3(1, moveSpeed * 0.0002f, 1);
            }
            Color c = Fade.GetComponent<Image>().color;
            c.a = 1;
            Fade.GetComponent<Image>().color = c;
            //if (Fade.GetComponent<Image>().color.a < 1)
            //{
            //    Color c = Fade.GetComponent<Image>().color;
            //    c.a += Time.deltaTime * 0.5f;
            //    Fade.GetComponent<Image>().color = c;
            //}
            moveSpeed += Time.deltaTime * 10;
            yield return null;
        }
        StopAllCoroutines();
        player = GameObject.FindFirstObjectByType<PlayerState>().gameObject;
        player.GetComponent<PlayerInputScript>()._inputActionMap.Enable();
        player.GetComponent<CircleCollider2D>().enabled = true;
        player.GetComponent<CapsuleCollider2D>().enabled = true;
        Coroutine goingOn = GameObject.FindFirstObjectByType<Elevato>().goingOn;
        StopCoroutine(goingOn);
        ReLoadScene();
    }

    void ReLoadScene() 
    {
        PlayerInputScript p = player.GetComponent<PlayerInputScript>();
        p.StopAction();

        player.GetComponent<PlayerInputScript>().StopAllCoroutines();
        SceneManager.LoadScene("BattleScene");
    }
}
