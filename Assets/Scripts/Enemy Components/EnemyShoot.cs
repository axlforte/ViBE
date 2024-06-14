using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public GeneralEnemy ge;
    public float waitTime, time, animWait;
    public GameObject projectile, temp;

    void Start(){
        ge = GetComponent<GeneralEnemy>();
    }

    void Update(){
        

        if(time > waitTime){
            time = 0;
            temp = Instantiate(projectile, gameObject.transform.position, Quaternion.identity);
            animWait = 5;
            if(!ge.renderor.flipX){
                temp.GetComponent<Projectile>().Direction = new Vector3(-1,0,0);
            }
            ge.renderor.sprite = ge.images[2];
        } else {
            time += Time.deltaTime;
        }
    }

    void FixedUpdate(){
        if(ge.renderor.sprite == ge.images[2] && animWait < 0){
            ge.renderor.sprite = ge.images[1];
            animWait = 2;
        } else if(ge.renderor.sprite == ge.images[1] && animWait < 0) {
            ge.renderor.sprite = ge.images[0];
        }

        if(animWait >= 0){
            animWait --;
        }
    }
}
