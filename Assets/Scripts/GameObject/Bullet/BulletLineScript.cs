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

        lineRenderer.startWidth = 0.0f;  // ���� ���� �β�
        lineRenderer.endWidth = 0.1f;    // ���� �� �β�
        lineRenderer.startColor = Color.white;  // ���� ���� ����
        lineRenderer.endColor = Color.white;    // ���� �� ����
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));  // �⺻ ���̴�
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
