using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum playerState 
    {
        Idle = 0,
        move = 1 << 1,
        attack = 1 << 2,
        charge = 1 << 3,
        roll = 1 << 4
    }
    public playerState currentState;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
