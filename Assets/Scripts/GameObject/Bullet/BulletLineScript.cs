using UnityEngine;

public class BulletLineScript : MonoBehaviour
{
    public GameObject parentBullet;
    LineRenderer lineRenderer;
    int linePointCount = 2;
    Vector2 currentPos;

    private void Awake()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);

        lineRenderer.startWidth = 0.0f;  // 선의 시작 두께
        lineRenderer.endWidth = 0.1f;    // 선의 끝 두께
        lineRenderer.startColor = Color.white;  // 선의 시작 색상
        lineRenderer.endColor = Color.white;    // 선의 끝 색상
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));  // 기본 쉐이더
    }

    void Update()
    {
        lineRenderer.SetPosition(linePointCount - 1, transform.position);
        if (!parentBullet) 
        {
            Color c = lineRenderer.startColor;
            c.a -= Time.deltaTime*5;
            lineRenderer.startColor = c;
            lineRenderer.endColor = c;
        }
        if (lineRenderer.startColor.a <= 0) 
        {
            Destroy(gameObject);
        }
    }

    public void addPoint() 
    {
        linePointCount++;
        lineRenderer.positionCount = linePointCount;
    }
}
