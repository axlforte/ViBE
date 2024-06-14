using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public int frameDelay;
    private int frame;
    public Sprite[] images;
    public SpriteRenderer rend;

    void Start(){
        rend.sprite = images[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        frame++;
        if(frame % frameDelay == 0){
            if(frame / frameDelay > images.Length - 2){
                Destroy(this.gameObject);
                return;
            }
            rend.sprite = images[(frame / frameDelay) + 1];
        }
    }
}
