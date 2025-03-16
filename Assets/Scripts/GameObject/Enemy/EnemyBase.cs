using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D rb;
    public float moveSpeed;
    public bool canDead = true;
    public bool isBlock = false;
    public Coroutine attackSkill;
    public bool canMove = true;
    public enum State 
    {
        idle = 0,
        foundPlayer = 1 << 1,
        stuned = 1 << 2,
        attack = 1 << 3
    }
    public State currentState;

    virtual public void TakeDamageAction()
    {
        //юс╫ц
        GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color == Color.yellow ? Color.red : Color.yellow;
    }

    virtual public void Block() 
    {
    }
}
