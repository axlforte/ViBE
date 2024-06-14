using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public int speed, speedIncrease, damage = 2;
    public float waitTime, deathTime = 120;
    public Vector3 Direction;
    public Sprite[] images;
    private Transform proj;
    public bool movingPlatform, ownedByPlayer;

    public int AnimWaitTime, wait, animIndex;
    public SpriteRenderer renderor;

    // Start is called before the first frame update
    void Start()
    {
        proj = GetComponent<Transform>();
        renderor = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (waitTime <= 0) {
            proj.Translate(Direction * speed * Time.deltaTime);
        } else
        {
            waitTime -= Time.deltaTime;
        }
    }

    public void FixedUpdate()
    {
        if (deathTime <= 0)
        {
            Destroy(this.gameObject);
        } else
        {
            deathTime--;
        }

        if(images.Length > 1){
            if(wait <= 0){
                wait = AnimWaitTime;
                animIndex++;
                if(animIndex == images.Length){
                    animIndex = 0;
                }
                renderor.sprite = images[animIndex];
            } else {
                wait--;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "platform")
        {
            other.transform.parent = proj;

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "platform")
        {
            other.transform.parent = null;

        }
    }
}
