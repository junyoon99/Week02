using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public GameObject player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("유효타!");
        if (collision.tag == "Enemy") 
        {
            collision.TryGetComponent<EnemyBase>(out EnemyBase enemy);
            if (enemy.canDead && !enemy.isBlock)
            {
                //HitStop.Instance.Stop(0.1f);
                enemy.TakeDamageAction();
            }
            else if (enemy.isBlock)
            {
                Debug.Log("공격방어!");
                player.GetComponent<PlayerInputScript>().AttackBlocked(collision.transform, 0.5f);
                enemy.Block();
            }
        }
    }
}
