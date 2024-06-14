using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormEagle : GeneralBoss
{
    [Header("Eagle Specific")]
    public BossState state;
    public float DiveCounter;

    public int JumpTime;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        Health = MaxHealth;
        state = BossState.Idle;
        boss = gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(player == null || Health <= 0){
            Destroy(gameObject);
        }

        if(Mathf.Abs(player.transform.position.x - boss.transform.position.x) > 16){
            return;
        }

        if (state == BossState.Idle)
        {
            Idle();
        }
        else if (state == BossState.Jump)
        {
            Jump();
        }
        else if (state == BossState.Shoot)
        {
            Shoot();
        }
        else if (state == BossState.Dive)
        {
            Dive();
        }
        else if (state == BossState.Taunt)
        {
            Taunt();
        }
        else
        {
            state = BossState.Idle;
        }
        if (StateTimerDelay >= 0)
        {
            StateTimerDelay--;
        }

        if(state == BossState.Dive){
            canFlip = false;
        } else {
            canFlip = true;
        }

        PositioningAndSpriting();
    }

    void Idle()
    {
        if (StateTimerDelay <= 0)
        {
            StateTimerDelay = 90;
            Gravity = 2;
            activeSprite = 3;

            y += 4;
            state = BossState.Jump;
        }

        
    }

    void Jump()
    {
        y += 2;
        CielingCheck();

        if(StateTimerDelay % 18 > 12){
            activeSprite = 1;
        } else if(StateTimerDelay % 18 > 6){
            activeSprite = 2;
        } else {
            activeSprite = 3;
        }

        if (StateTimerDelay <= 0)
        {
            StateTimerDelay = 3;
            activeSprite = 1;
            state = BossState.Shoot;
        }
    }

    void Shoot()
    {
        if (StateTimerDelay <= 0)
        {
            StateTimerDelay = 45;
            activeSprite = 4;
            state = BossState.Dive;
            if(rend.flipX){
                Velocity = 4;
            } else {
                Velocity = -4;
            }
        }
    }

    void Dive()
    {
        y += -4;
        x += Velocity;
        GroundCheck();
        SideCheck();
        CielingCheck();
        if (grounded)
        {
            StateTimerDelay = 90;
            state = BossState.Jump;
            DiveCounter += Mathf.Abs(x % 15 + y % 15) / 16;
            y += 16;
        }

        if(DiveCounter > 1){
            Velocity = 0;
            StateTimerDelay = 45;
            state = BossState.Taunt;
            activeSprite = 0;
            DiveCounter = 0;
        }
    }

    void Taunt()
    {
        if (StateTimerDelay <= 0)
        {
            StateTimerDelay = 30;
            activeSprite = 0;
            FireAProjectile();
            state = BossState.Idle;
        }
    }

    void FireAProjectile(){
        temp = Instantiate(projectile, new Vector3(boss.transform.position.x, boss.transform.position.y + 3, boss.transform.position.z), Quaternion.identity);

        if(rend.flipX){
            temp.GetComponent<Projectile>().Direction = new Vector3(1,0,0);
        } else {
            temp.GetComponent<Projectile>().Direction = new Vector3(-1,0,0);
        }
    }
}
