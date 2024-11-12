using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Dependencies.Sqlite;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
using UnityEngine.TextCore;

public class PlayerController : MonoBehaviour
{
    public Player player;
    public Animator animator;
    private Rigidbody2D _playerBody;
    private float _horizontalMove;
    private float _deltaPlayerRay=1f;
    private bool _face=true;
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

    void Update()
    {
        //try to change
        AnimateSwitch();
        player.curentState=GetPlayerState();
        if(IsGround()){
        _horizontalMove = Input.GetAxis("Horizontal");
        }
        FlipPlayer();
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
        _playerBody.velocity = new Vector2(_horizontalMove * player.moveSpeed, _playerBody.velocity.y);
        
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

    private void FlipPlayer()
    {
        if (_horizontalMove > 0 && !_face || _horizontalMove < 0 && _face)
        {
            Vector3 temp = player.transform.localScale;
            temp.x*=-1;
            player.transform.localScale=temp;
            _face= !_face;
            
        }
        
    }

    private void AnimateSwitch(){
    //try to change
        switch(player.curentState){
            case Player.PlayerState.Idle:
                animator.SetBool("idle",true);
                animator.SetFloat("velocityY",0);
                animator.SetFloat("velocityX",0);
            break;
            case Player.PlayerState.Jumping:
                animator.SetBool("idle",false);
                animator.SetFloat("velocityY",_playerBody.velocity.y);
            break;
            case Player.PlayerState.Falling:
            break;
            case Player.PlayerState.Running:
                animator.SetBool("isGround",IsGround());
                animator.SetBool("idle",false);
                animator.SetFloat("velocityX",_playerBody.velocity.x);
            break;
        }
    }
}
