using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Dependencies.Sqlite;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player player;
    public Animator animator;
    private Rigidbody2D _playerBody;
    private float _horizontalMove;
    private float _deltaPlayerRay=1.25f;
    private Dictionary<System.Func<bool>, Player.PlayerState> stateConditions;
    void Start()
    {
        _playerBody=player.GetComponent<Rigidbody2D>();
        stateConditions = new Dictionary<System.Func<bool>, Player.PlayerState>
        {
            { () => IsGround() && _horizontalMove != 0, Player.PlayerState.Running},
            { () => IsGround() && _horizontalMove == 0, Player.PlayerState.Idle },
            { () => !IsGround() && _playerBody.velocity.y > 0, Player.PlayerState.Jumping },
            { () => !IsGround() && _playerBody.velocity.y < 0, Player.PlayerState.Falling }
        };
    }

    // Update is called once per frame
    void Update()
    {
        AnimateSwitch();
        player.curentState=GetPlayerState();
        _horizontalMove = Input.GetAxis("Horizontal");
        if(Input.GetButtonDown("Jump") && IsGround()){
            Jump();
        }
        
    }
    
    void FixedUpdate(){
        move();
    }
    void Jump(){
        _playerBody.AddForce(Vector2.up*player.jumpfore,ForceMode2D.Impulse);
        
    }
    void move(){
        if(IsGround()){
        _playerBody.velocity = new Vector2(_horizontalMove * player.moveSpeed, _playerBody.velocity.y);
        }
        
    }
    private bool IsGround(){
        //try refactor
        RaycastHit2D hit2D =Physics2D.Raycast(new Vector2(_playerBody.position.x,_playerBody.position.y-_deltaPlayerRay), Vector2.down, 0.1f);
        Debug.DrawRay(new Vector2(_playerBody.position.x,_playerBody.position.y-_deltaPlayerRay), Vector2.down * 0.1f, Color.red);
        //Debug.Log("collider is "+hit2D.collider);
        return hit2D.collider!=null;
    }
    private Player.PlayerState GetPlayerState()
    {
        foreach (var condition in stateConditions)
        {
            if (condition.Key())
                return condition.Value;
        }
        return Player.PlayerState.Idle;
    }

    private void AnimateSwitch(){

        switch(player.curentState){
            case Player.PlayerState.Idle:
                animator.SetBool("idle",true);
            break;
            case Player.PlayerState.Jumping:
                animator.SetBool("idle",false);
                animator.SetFloat("velocityY",_playerBody.velocity.y);
            break;
            case Player.PlayerState.Falling:
                animator.SetFloat("velocityY",_playerBody.velocity.y);
            break;
            case Player.PlayerState.Running:
            break;
        }
    }
}
