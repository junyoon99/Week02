using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using TMPro;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraScript : MonoBehaviour
{
    Vector3 MousePos;
    public GameObject player;

    public Camera cam;
    private float halfHeight;
    private float halfWidth;

    Vector3 playerPos;
    Vector3 posDiff;
    Vector3 targetPos;
    void FixedUpdate()
    {
        //if (player)
        //{
        //    MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    playerPos = player.transform.position;
        //    posDiff = (MousePos - playerPos);
        //    targetPos = playerPos + posDiff / 10;
        //    targetPos.z = -10;

        //    targetPos.x = Mathf.Clamp(targetPos.x, -(halfWidth + 10f), (halfWidth + 10f));
        //    targetPos.y = Mathf.Clamp(targetPos.y, -(halfHeight + 10f), (halfHeight + 10f));
        //    transform.position = targetPos;
        //}
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
    }

    private void Start()
    {
        cam = Camera.main;
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
        player = GameObject.FindFirstObjectByType<PlayerState>().gameObject;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition; // 원래 위치 저장
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        transform.localPosition = originalPosition; // 원래 위치로 복구
    }
}
