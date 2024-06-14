using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEnemy : MonoBehaviour
{
    public EnemyShoot shootControls;
    public EnemyJump jumpControls;

    private float floordist;
    public float height;
    public int contactDamage = 2, health = 3;
    public GameObject Player;
    public LayerMask mask;
    public SpriteRenderer renderor;

    public Sprite[] images;

    public void Start()
    {
        if(Player == null){
            Player = GameObject.Find("Rockman(Clone)");
        }
        renderor = GetComponent<SpriteRenderer>();
        floordist = 0;
        //long story short this is to prevent wallzipping but it actually adds a walljump mechanic like how metroid dread handles the walljumping
        if (Physics2D.Raycast(new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y), Vector2.down, height, mask))
        {
            floordist = Physics2D.Raycast(new Vector2(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y), Vector2.down, height, mask).distance;
            GetComponent<Transform>().position = new Vector3(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y + (height - floordist), 0);
        }
    }

    private void Update()
    {
        if (Player == null || health <= 0)
        {
            Destroy(gameObject);
        }

        if (this.gameObject.transform.position.x < Player.transform.position.x)
        {
            renderor.flipX = true;
        } else
        {
            renderor.flipX = false;
        }
    }

     void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Projectile"){
            Debug.Log("OW!");
            if(col.gameObject.GetComponent<Projectile>().ownedByPlayer){
                health -= col.gameObject.GetComponent<Projectile>().damage;
            }
        }
    }
}
