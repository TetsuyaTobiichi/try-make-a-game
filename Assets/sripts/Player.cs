using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private int _health=100;
    public float moveSpeed=1f;
    public float jumpfore=5f;
    private int _lvl=0;
    public PlayerState curentState;
    
    void Start(){
        curentState=PlayerState.Idle;
    }
    
    public enum PlayerState
    {
        Idle,
        Running,
        Jumping,
        Falling,
        Attacking,
        Dead
    }
}
