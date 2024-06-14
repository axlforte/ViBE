using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralBoss : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Jump,
        Midair,
        Dive,
        Walk,
        Shoot,
        Taunt
    };
    public int Health, MaxHealth = 28;
    public int ContactDamage = 2;
    public SpriteRenderer rend;
    public int activeSprite;
    public Sprite[] images;
    public GameObject projectile, boss, player, temp;
    public int StateTimerDelay;

    [Header("Positional Code")]
    public float x;
    public float y;
    public float StunTime;
    public float Gravity, GravConstant = -0.98f;
    public float Velocity;
    public bool grounded, touchingLeftWall, touchingRightWall, canFlip;
    float floordist;
    public float floorSnap, headBonkSnap,wallSnap;
    public LayerMask mask;

    public void PositioningAndSpriting()
    {
        boss.transform.position = new Vector3(x / 16,y / 16,0);
        rend.sprite = images[activeSprite];
        if(!canFlip){
            return;
        }
        if(player.transform.position.x > boss.transform.position.x){
            rend.flipX = true;
        } else {
            rend.flipX = false;
        }
    }

    public void GroundCheck()
    {
        grounded = false;
        floordist = 0;
        if (Physics2D.Raycast(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y), Vector2.down, floorSnap * 1.5f, mask))
        {
            floordist = Physics2D.Raycast(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y), Vector2.down, floorSnap * 1.5f, mask).distance;
            y += (floorSnap - floordist) * 16;
            grounded = true;
        }
    }

    public void CielingCheck()
    {
        //this is for headbonking, or in other words, the last collision check that i did because i was lazy. 
        if (Physics2D.Raycast(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y), Vector2.up, headBonkSnap, mask))
        {
            y -= headBonkSnap;
            StateTimerDelay = 0;
            Gravity = 0;
        }
    }

    public void SideCheck()
    {
        touchingLeftWall = false;
        touchingRightWall = false;
            if (Physics2D.Raycast(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y - 0.4f), Vector2.left, wallSnap, mask).collider != null)
        {
            touchingLeftWall = true;
        }
        if (Physics2D.Raycast(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y - 0.4f), Vector2.right, wallSnap, mask).collider != null )
        {
            touchingRightWall = true;
        }

        if (touchingLeftWall)
        {
            x += wallSnap - Physics2D.Raycast(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y), Vector2.left, wallSnap * 1.1f, mask).distance + 1;
            Velocity = Velocity * -0.9f;
            x += Velocity;
        } 
        if(touchingRightWall)
        {
            x -= wallSnap - Physics2D.Raycast(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y), Vector2.right, wallSnap * 1.1f, mask).distance + 1;
            Velocity = Velocity * -0.9f;
            x += Velocity;
        }
        Debug.Log(touchingLeftWall + " " + touchingRightWall);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Projectile"){
            Debug.Log("OW!");
            if(col.gameObject.GetComponent<Projectile>().ownedByPlayer){
                Health -= col.gameObject.GetComponent<Projectile>().damage;
            }
        }
    }
}
