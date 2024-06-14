using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameMammoth : GeneralBoss
{
    [Header("Mammoth Specific")]
    public GameObject OilSpill;
    public BossState state;

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

        PositioningAndSpriting();
    }

    void Idle()
    {
        if (StateTimerDelay <= 0)
        {
            JumpTime = (int)((Time.time % 0.125f + Time.deltaTime) * 250) + 10;
            Velocity = Mathf.Clamp(Mathf.Abs((player.transform.position.x - boss.transform.position.x) * 4.5f), 2, 16) * ((player.transform.position.x - boss.transform.position.x) / Mathf.Abs(player.transform.position.x - boss.transform.position.x)) * (-1 * GravConstant);
            StateTimerDelay = JumpTime;
            Gravity = GravConstant * -1 * JumpTime;
            activeSprite = 3;

            y += 16;
            state = BossState.Jump;
        }

        
    }

    void Jump()
    {
        Gravity += GravConstant;
        y += Gravity;
        x += Velocity;
        CielingCheck();
        SideCheck();
        if (StateTimerDelay <= 0)
        {
            StateTimerDelay = 3;
            activeSprite = 1;
            state = BossState.Shoot;
        }
    }

    void Shoot()
    {
        //y--;
        if (StateTimerDelay <= 0)
        {
            FireAProjectile();
            StateTimerDelay = JumpTime - 1;
            activeSprite = 4;
            state = BossState.Dive;
        }
    }

    void Dive()
    {
        Gravity += GravConstant;
        y += Gravity;
        x += Velocity;
        GroundCheck();
        SideCheck();
        CielingCheck();
        if (grounded)
        {
            Velocity = 0;
            StateTimerDelay = 45;
            state = BossState.Taunt;
            activeSprite = 0;
        }
    }

    void Taunt()
    {
        if (StateTimerDelay <= 0)
        {
            StateTimerDelay = 30;
            activeSprite = 1;
            FireAProjectile();
            state = BossState.Idle;
        }
    }

    void FireAProjectile(){
        //Debug.Log(((Time.time % 0.25f + Time.deltaTime) * 50));
        if(((Time.time % 0.125f + Time.deltaTime) * 50) < 3){
            temp = Instantiate(OilSpill, new Vector3(boss.transform.position.x, boss.transform.position.y + 3, boss.transform.position.z), Quaternion.identity);
        } else {
            temp = Instantiate(projectile, new Vector3(boss.transform.position.x, boss.transform.position.y + 3, boss.transform.position.z), Quaternion.identity);
        }

        if(rend.flipX){
            temp.GetComponent<Projectile>().Direction = new Vector3(1,0,0);
        } else {
            temp.GetComponent<Projectile>().Direction = new Vector3(-1,0,0);
        }
    }
}
