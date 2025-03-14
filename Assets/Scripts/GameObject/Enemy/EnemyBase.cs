using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D rb;
    public float moveSpeed;
    public enum State 
    {
        idle = 0,
        foundPlayer = 1 << 1
    }
    public State currentState;
}
