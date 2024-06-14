using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
//using UnityEngine.InputSystem;
//using UnityEngine.SceneManagement;
//this was imported from mini ranger 2 because its player controller had a lot more integrations set up. SceneManagement is really only used for dying, however.

//note - healthbars ARE a pain in my ass, i moved them to another script because its easier
public class RockmanControll : MonoBehaviour
{ 
    [Header("Various Bits of Data")]
    public int specialWeaponIndex, ladderTime, ImageIndex, dashEnd = 30;
    int dashTimer, wjTimer, airTime, fallTime, slideTime, shotTimer, invuln, stun, walkTime;
    public float x, y;
    float yVel, xVel, pixPerfYVel, yVelOverFlow, animTimer, jumpVel, ladderx, floordist, gravity;
    bool firing, jumping, Switching, pausing, grounded, canJump, inWall, canFire;
    public bool leftFacing, dashing, touchingWall, wallJumpPossible, canDash, canEnterLadder;
    bool headbonk, inBossBattle, onLadder, inDialouge, dashLock, canChangeWeapon;

    [Header("Movement Data")]

    public Vector2 LeftStick;// a mix value so I can get the right stick info without needing to use them seperately. honestly, you can seperate them if you wanted to
    Vector3 oldpos, fakeCam; //position, but in 3d
    public Transform player, renderorTransform;
    public float floorSnap, wallSnap, headBonkSnap;

    public LayerMask mask;

    //public Material material;
     //may or may not be needed

    [Header("Projectiles")]

    public GameObject bullet;
    public SpecialWeapons[] weapons;
    public int[] WeaponEnergy;
    public Transform WeaponBar;

    [Header("Rendering")]

    public SpriteRenderer renderor;
    public Material mat;
    private Skin skin;
    private int skinIndex;
    public Skin[] skins;
    public Color[] OverArmorPalletes, UnderArmorPalletes;
    public Shader shade;
    //public Canvas canva;

    [Header("Particles")]

    public GameObject[] particles;

    [Header ("Sounds (Need to implement)")]

    //public AudioSource shootSound;
    //public AudioSource jumpSound, dashSound, hurtSound;

    [Header ("Camera Locking")]

    public Transform cam;
    public string lockType;
    public float lockx, locky, lockEnd, lockStart;

    [Header("Health")]

    public Transform HealthBar;
    public int health, maxHealth;

    //public HealthBarHandler hbh;

    [Header("Armor")]

    //armor 'n shit
    public bool armorHead;
    public bool armorBody, armorArms, armorLegs;

    public ArmorRenderer headRender, BodyRender, ArmsRender, LegsArmor;

    // Start is called before the first frame update
    void Start(){
        //hbh = this.gameObject.GetComponent<HealthBarHandler>();
        renderorTransform = renderor.gameObject.GetComponent<Transform>();
        //just sets the player transform to unity's player transform prefab. the gameObject type in scripts is really powerful when used with GetObject
        player = gameObject.GetComponent<Transform>();
        //pauseScreen.SetActive(false);
        skinIndex = PlayerPrefs.GetInt("skin");
        skin = skins[skinIndex];

        WeaponEnergy = new int[weapons.Length];
        for (int a = 0; a < weapons.Length; a++)
        {
            WeaponEnergy[a] = weapons[a].MaxEnergy;
        }

        OverArmorPalletes = new Color[weapons.Length];
        UnderArmorPalletes = new Color[weapons.Length];
        for(int a = 0; a < weapons.Length; a++){
            OverArmorPalletes[a] = weapons[a].OverArmor;
            UnderArmorPalletes[a] = weapons[a].UnderArmor;
        }
    }

    void Update(){
        GetInput();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(health <= 0){
            //SceneManager.LoadScene("IntroductionScene");
        }
        if(onLadder){
            LadderControl();
        } else if(inDialouge){
            //LOLOLOLOL
        } else {
            RegularControl();
        }
        HealthRender();


        Render();
        RenderArmors();
    }

    void Render()
    {
        renderor.sprite = skin.frames[ImageIndex];
        mat.SetTexture("_MainTex", skin.frames[ImageIndex].texture);
        if(leftFacing){
            renderorTransform.localScale = new Vector3(skin.scale * -1, skin.scale, 1);
        } else {
            renderorTransform.localScale = new Vector3(skin.scale, skin.scale, 1);
        }
        renderorTransform.position = new Vector3(skin.offset.x, skin.offset.y, 0) + player.position;
    }

    void GroundCheck()
    {
        //ive done this before but setting something to false then checking if its true is great if you cant just set it to false if a statement
        //is wrong. this is the grounded check code, which is almost as bad as wall check, but only from a decorational standpoint. works well beside the fact that its kinda shit.
        grounded = false;
        floordist = 0;
        //long story short this is to prevent wallzipping but it actually adds a walljump mechanic like how metroid dread handles the walljumping
        if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y), Vector2.down, floorSnap * 1.1f, mask) && stun <= 0)
        {
            floordist = Physics2D.Raycast(new Vector2(player.position.x, player.position.y), Vector2.down, floorSnap * 1.1f, mask).distance;
            //Debug.Log(floordist + ", " + (floorSnap - floordist));
            y += (floorSnap - floordist);
            dashLock = false;
            grounded = true;
        }
    }

    void SideCheck()
    {
        //the great wall of shit. this is purely to make sure the player doesnt go thru walls. and guess what? IT DOESNT WORK (entirely) WITH WALLJUMPING 
        //no, im not adding in boxcasts cos this is more accurate
        if (leftFacing)
        {
            if (wjTimer != 0) 
            {
                RightSide();
            } else
            {
                leftSide();
            }
        }
        else
        {
            if (wjTimer != 0)
            {
                leftSide();
            } else
            {
                RightSide();
            }
        }

        if (Physics2D.Raycast(new Vector2(player.position.x - 0.5f, player.position.y), Vector2.right, 1f, mask).collider != null)
        {
            if (Physics2D.Raycast(new Vector2(player.position.x - 0.5f, player.position.y), Vector2.right, 1f, mask).transform.gameObject.tag == "damage")
            {
                wallJumpPossible = false;
                touchingWall = false;
            }
        }

        if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y), Vector2.right, 1f, mask).collider != null && Physics2D.Raycast(new Vector2(player.position.x, player.position.y), Vector2.left, 1f, mask).collider != null)
        {
            wallJumpPossible = false;
        }

        if (touchingWall && leftFacing)
        {
            if(wjTimer != 0)
            {
                x -= wallSnap - Physics2D.Raycast(new Vector2(player.position.x, player.position.y), Vector2.right, wallSnap * 1.1f, mask).distance;
            } else
            {
                x += wallSnap - Physics2D.Raycast(new Vector2(player.position.x, player.position.y), Vector2.left, wallSnap * 1.1f, mask).distance;
            }
        } else if (touchingWall && leftFacing)
        {
            if (wjTimer != 0)
            {
                x += wallSnap - Physics2D.Raycast(new Vector2(player.position.x, player.position.y), Vector2.left, wallSnap * 1.1f, mask).distance;
            } else {
                x -= wallSnap - Physics2D.Raycast(new Vector2(player.position.x, player.position.y), Vector2.right, wallSnap * 1.1f, mask).distance;
            }
        }
        //touchingWall = !touchingWall;
    }

    void leftSide()
    {
        if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y - 0.4f), Vector2.left, wallSnap, mask).collider != null)
        {
            touchingWall = true;
        }
        else if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y), Vector2.left, wallSnap, mask).collider != null)
        {
            touchingWall = true;
        }
        else if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y + 0.4f), Vector2.left, wallSnap, mask).collider != null)
        {
            touchingWall = true;
        }
        else
        {
            touchingWall = false;
        }

        if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y - 0.4f), Vector2.left, wallSnap, mask).collider != null && LeftStick.x != 0)
        {
            wallJumpPossible = true;
        }
        else if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y), Vector2.left, wallSnap, mask).collider != null && LeftStick.x != 0)
        {
            wallJumpPossible = true;
        }
        else if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y + 0.4f), Vector2.left, wallSnap, mask).collider != null && LeftStick.x != 0)
        {
            wallJumpPossible = true;
        }
        else
        {
            wallJumpPossible = false;
        }
    }

    void RightSide()
    {
        if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y - 0.4f), Vector2.right, wallSnap, mask).collider != null)
        {
            touchingWall = true;
        }
        else if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y), Vector2.right, wallSnap, mask).collider != null)
        {
            touchingWall = true;
        }
        else if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y + 0.4f), Vector2.right, wallSnap, mask).collider != null)
        {
            touchingWall = true;
        }
        else
        {
            touchingWall = false;
        }

        if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y - 0.4f), Vector2.right, wallSnap, mask).collider != null && LeftStick.x != 0)
        {
            wallJumpPossible = true;
        }
        else if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y), Vector2.right, wallSnap, mask).collider != null && LeftStick.x != 0)
        {
            wallJumpPossible = true;
        }
        else if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y + 0.4f), Vector2.right, wallSnap, mask).collider != null && LeftStick.x != 0)
        {
            wallJumpPossible = true;
        }
        else
        {
            wallJumpPossible = false;
        }
    }

    void CielingCheck()
    {
        //this is for headbonking, or in other words, the last collision check that i did because i was lazy. 
        if (Physics2D.Raycast(new Vector2(player.position.x, player.position.y), Vector2.up, headBonkSnap, mask))
        {
            if (yVel >= 0 && !grounded)
            {
                yVel = -64;
                y -= headBonkSnap;
            }
            else
            {
                headbonk = false;
                canJump = false;
            }
        }
        else
        {
            headbonk = true;
        }
    }

    void RegularControl(){
        xVel = 0;
        GroundCheck();

        SideCheck();

        if(invuln >= 1){
            invuln--;
        }

        if(LeftStick.x != 0){
            walkTime++;
        } else {
            walkTime = 0;
        }

        if(grounded){
            yVel = 0;
            if(jumping && canJump){
                yVel = 1363;
                if (dashing)
                {
                    dashLock = true;
                }
                //jumpSound.Play();
            } else if(stun >= 0) {
                yVel = 600;
                y += 0.1f;
            }
            //controls animations for when the player is on the ground. the first part is walking, then standing, then dashing.
            if(dashTimer <= 0 | dashTimer >= dashEnd){
                if(LeftStick.x != 0){
                    animTimer += 1;
                    if(shotTimer >= 1){
                        if(walkTime <= 1){
                            ImageIndex = 5;
                        } else if(walkTime <= 2){
                            ImageIndex = 60;
                        } else if(animTimer <= 0){
                            ImageIndex = 16;
                        } else if(animTimer <= 3){
                            ImageIndex = 17;
                        } else if(animTimer <= 6){
                            ImageIndex = 18;
                        } else if(animTimer <= 9){
                            ImageIndex = 19;
                        } else if(animTimer <= 12){
                            ImageIndex = 20;
                        } else if(animTimer <= 15){
                            ImageIndex = 21;
                        } else if(animTimer <= 18){
                            ImageIndex = 22;
                        } else if(animTimer <= 21){
                            ImageIndex = 23;
                        } else if(animTimer <= 24){
                            ImageIndex = 24;
                        } else if(animTimer <= 27){
                            ImageIndex = 25;
                        } else {
                            animTimer = -1;
                            ImageIndex = 16;
                        }
                    } else {
                        if(walkTime <= 1){
                            ImageIndex = 5;
                        } else if(walkTime <= 2){
                            ImageIndex = 60;
                        } else if(animTimer <= 0){
                            ImageIndex = 6;
                        } else if(animTimer <= 3){
                            ImageIndex = 7;
                        } else if(animTimer <= 6){
                            ImageIndex = 8;
                        } else if(animTimer <= 9){
                            ImageIndex = 9;
                        } else if(animTimer <= 12){
                            ImageIndex = 10;
                        } else if(animTimer <= 15){
                            ImageIndex = 11;
                        } else if(animTimer <= 18){
                            ImageIndex = 12;
                        } else if(animTimer <= 21){
                            ImageIndex = 13;
                        } else if(animTimer <= 24){
                            ImageIndex = 14;
                        } else if(animTimer <= 27){
                            ImageIndex = 15;
                        } else {
                            animTimer = -1;
                            ImageIndex = 6;
                        }
                    }
                } else {
                    if(shotTimer >= 13){
                        ImageIndex = 4;
                    } else if(shotTimer >= 1){
                        ImageIndex = 3;
                    } else {
                        ImageIndex = 0;
                    }
                }
            } else { // dashing animation
                if(shotTimer >= 1){
                    if(dashTimer <= 3 ||dashTimer >= dashEnd - 3){
                        ImageIndex = 42;
                    } else {
                        ImageIndex = 43;
                        if(dashTimer % 5 == 0){
                            Instantiate(particles[0], new Vector3(player.position.x, player.position.y - 0.5f, player.position.z), Quaternion.identity);
                        }
                    }
                } else {
                    if(dashTimer <= 3 ||dashTimer >= dashEnd - 3){
                        ImageIndex = 40;
                    } else {
                        ImageIndex = 41;
                        if(dashTimer % 5 == 0){
                            Instantiate(particles[0], new Vector3(player.position.x, player.position.y - 0.5f, player.position.z), Quaternion.identity);
                        }
                    }
                }
            }
            airTime = 0;
            slideTime = 0;
            fallTime = 0;
        } else { 
            // this controls terminal velocity. gravity itself does not affect terminal velocity, just how fast you get there.
            yVel -= 64;
            if(wallJumpPossible && airTime >= 5){
                if(yVel != -256){
                    yVel = -256;
                }
                if(jumping && airTime >= 5){
                    dashLock = false;
                    if(dashTimer != 0){
                        wjTimer = 4;
                    } else {
                        wjTimer = 5;
                    }
                    yVel = 1363;
                    y += 0.1f;
                    Instantiate(particles[1], new Vector3(player.position.x, player.position.y - 0.5f, player.position.z), Quaternion.identity);
                    //jumpSound.Play();
                    if(dashing){
                        dashLock = true;
                    }
                }
                slideTime++;
                fallTime = 0;
            } else {
                if(yVel <= -1024){
                    yVel = -1024;
                }
            } // wallsliding and jumping animations. in reality, when i get jumping dialed in, the animations should be changed to a
            //fixed duration and stick on falling until the player lands.
            if(airTime >= 1){
                if(wallJumpPossible){
                    if(shotTimer >= 1){
                        ImageIndex = 51;
                    } else {
                        if(slideTime <= 3){
                            ImageIndex = 44;
                        } else if(slideTime <= 6){
                            ImageIndex = 45;
                        } else {
                            ImageIndex = 46;
                        }
                    }
                } else {
                    if(shotTimer >= 1){
                        if(wjTimer >= 1){
                            ImageIndex = 53;
                        } else if(airTime <= 1){
                            ImageIndex = 31;
                        } else if(airTime <= 2){
                            ImageIndex = 33;
                        } else if(airTime <= 4){
                            ImageIndex = 34;
                        } else {
                            if(fallTime >= 4){
                                ImageIndex = 37;
                            } else if(fallTime >= 2){
                                ImageIndex = 36;
                            } else {
                                ImageIndex = 35;
                            }
                        }
                    } else {
                        if(wjTimer >= 3){
                            ImageIndex = 47;
                        } else if(wjTimer >= 2){
                            ImageIndex = 48;
                        } else if(airTime <= 1){
                            ImageIndex = 31;
                        } else if(airTime <= 2){
                            ImageIndex = 26;
                        } else if(airTime <= 4){
                            ImageIndex = 27;
                        } else {
                            if(fallTime >= 4){
                                ImageIndex = 30;
                            } else if(fallTime >= 2){
                                ImageIndex = 29;
                            } else {
                                ImageIndex = 28;
                            }
                        }
                        
                    }
                    slideTime = 0;
                }
            }
            if(yVel <= 0){
                fallTime++;
            }
            airTime ++;
        }

        if(stun >= 0){
            canJump = false;
            wallJumpPossible = false;
            touchingWall = false;
            canFire = false;
            grounded = false;
            stun--;
            ImageIndex = 54;
            y += 0.01f * stun;
            yVel = 0;
            if(leftFacing){
                xVel = 138;
            } else {
                xVel = -138;
            }
        }

        //adding y velocity to the y position. 
        if(stun <= 0){
            y += yVel / 256f / 16f;
        }

        //this controls walljumping and checking which direction x is supposed to face in. taking damage from behind should change your facing direction too, 
        //like how the snes games do it
        if((wjTimer != 0)){
            wjTimer--;
            if(dashTimer <= dashEnd && dashTimer >= 1){
                if(leftFacing){
                    xVel = 885;
                    /*if(Physics2D.Raycast(new Vector2(player.position.x , player.position.y), Vector2.left, 0.5f).collider != null && wjTimer <= 150 && wjTimer >= 5){
                        Debug.Log(Physics2D.Raycast(new Vector2(player.position.x , player.position.y), Vector2.left, 0.5f).distance);
                        x -= Physics2D.Raycast(new Vector2(player.position.x , player.position.y), Vector2.left, 0.5f).distance * 1.1f;
                    }*/
                } else {
                    xVel = -885;
                    /*if(Physics2D.Raycast(new Vector2(player.position.x - 0.1f, player.position.y), Vector2.right, 0.5f).collider != null && wjTimer <= 150 && wjTimer >= 5){
                        Debug.Log(Physics2D.Raycast(new Vector2(player.position.x - 0.1f, player.position.y), Vector2.right, 0.5f).distance);
                        x += Physics2D.Raycast(new Vector2(player.position.x - 0.1f, player.position.y), Vector2.right, 0.5f).distance * 1.1f;
                    }*/
                }
            } else {
                if(leftFacing){
                    xVel = 376;
                } else {
                    xVel = -376;
                }
            }
        } else {
            if(LeftStick.x >= 0.1f){
                leftFacing = false;
            } else if(LeftStick.x <= -0.1f){
                leftFacing = true;
            }
        }

        if (dashing && canDash)
        {
            dashTimer = 1;
            canDash = false;
        } else if (!dashing)
        {
            dashTimer = 0;
            canDash = true;
        }

        if (dashLock)
        {
            dashing = true;
            dashTimer = 4;
        }

        //movement code. the top part moves x in his forward direction if he is dashing, the bottom part is normal control.
        //added a hack to make the dash not actually function for a frame. just a frame of delay is fine by me.
        if((dashing == true || grounded == false) && ((!armorHead && dashTimer <= dashEnd) || armorHead) && dashTimer >= 1){

            if((armorHead && dashTimer <= 10) || !armorHead){
                dashTimer ++;
            }
            if(dashTimer >= 2){
                if(grounded == false){//basically holds a dash for the entire time youre airborne
                    dashTimer --;
                }
                if(leftFacing){
                    if(!touchingWall){
                        if(wallJumpPossible){
                            xVel = 885;
                            dashTimer = 0;
                        } else {
                            xVel = -885;
                            if(Physics2D.Raycast(new Vector2(player.position.x - 0.05f, player.position.y), Vector2.left, 0.42f).collider != null){
                                dashTimer = 0;
                                xVel = 885;
                            }
                        }
                    }
                } else {
                    if(!touchingWall){
                        if(wallJumpPossible){
                            xVel = -885;
                            dashTimer = 0;
                        } else {
                            xVel = 885;
                            if(Physics2D.Raycast(new Vector2(player.position.x + 0.05f, player.position.y), Vector2.right, 0.42f).collider != null){
                                dashTimer = 0;
                                xVel = -885;
                            }
                        }
                    }
                }
            }
        } else if(wjTimer == 0){
            if(leftFacing){
                if(!touchingWall && !wallJumpPossible){
                    xVel = LeftStick.x * 376;
                    if(Physics2D.Raycast(new Vector2(player.position.x + 0.05f, player.position.y), Vector2.right, 0.42f).collider != null){
                        dashTimer = 0;
                        xVel = -885;
                    }
                }
            } else {
                if(!touchingWall && !wallJumpPossible){
                    xVel = LeftStick.x * 376;
                    if(Physics2D.Raycast(new Vector2(player.position.x - 0.05f, player.position.y), Vector2.left, 0.42f).collider != null){
                        dashTimer = 0;
                        xVel = 885;
                    }
                }
            }
        }

        // this is to prevent dashing, immediately jumping, releasing dash, and dashing accrost the ground.
        if(dashing == false && grounded == true){
            dashTimer = 0;
        }

        if(leftFacing){
            renderorTransform.localScale = new Vector3(-1, 1, 1);
        } else {
            renderorTransform.localScale = new Vector3(1, 1, 1);
        }

        CielingCheck();

        canJump = headbonk;

        PositioningCode();

        if(Switching && canChangeWeapon){
            specialWeaponIndex ++;
            if(specialWeaponIndex > OverArmorPalletes.Length - 1){
                specialWeaponIndex = 0;
            }
            canChangeWeapon = false;
            mat.SetColor("_Over_Color", OverArmorPalletes[specialWeaponIndex]);
            mat.SetColor("_Under_Color", UnderArmorPalletes[specialWeaponIndex]);
        } else if (!Switching)
        {
            canChangeWeapon = true;
        }

        //this is after everything updates so its as accurate as possible. it also make it harder to edit movement code!
        //need to implement charging and shooting special weapons
        if(firing & canFire){
            fireLemon();
            canFire = false;
        } else if(!firing) {
            canFire = true;
        }

        if(shotTimer >= 1){
            shotTimer --;
        }

        x += xVel / 16f / 256f;
    }

    void GetInput(){
        if(Input.GetKey(KeyCode.S)){
            LeftStick.y = -1;
        } else if(Input.GetKey(KeyCode.W)){
            LeftStick.y = 1;
        } else {
            LeftStick.y = 0;
            canEnterLadder = true;
        }

        if(Input.GetKey(KeyCode.A)){
            LeftStick.x = -1;
        } else if(Input.GetKey(KeyCode.D)){
            LeftStick.x = 1;
        } else {
            LeftStick.x = 0;
        }

        if(Input.GetKey(KeyCode.R)){
            Switching = true;
        } else {
            Switching = false;
        }

        if(Input.GetKey(KeyCode.E) && grounded){
            dashing = true;
        } else {
            dashing = false;
        }

        if(Input.GetKey(KeyCode.Space)){
            jumping = true;
        } else {
            jumping = false;
        }

        if(Input.GetKey(KeyCode.Q)){
            firing = true;
        } else {
            firing = false;
        }

        /*

        GetKeyDown gets when a key has started being pressed
        GetKey gets when a key is pressed, even if it has been pressed for a while
        GetKeyUp gets when a key stops being pressed

        */
    }

    void LadderControl(){
        if(jumping && canJump){
            onLadder = false;
            ladderTime = -1;
                renderor.flipX = false;
            yVel = 0;
        }else {
            canJump = true;
        }


        if(LeftStick.y >= 0.1f){
            y += (365f / 256f) / 16;
            ladderTime ++;
        } else if(LeftStick.y <= -0.1f){
            y -= (365f / 256f) / 16;
            ladderTime ++;
        }

        if(ladderTime <= 3){
            ImageIndex = 61;
        } else {
            if(ladderTime % 20 <= 3){
                ImageIndex = 62;
                renderor.flipX = true;
            } else if(ladderTime % 20 <= 6){
                ImageIndex = 63;
                renderor.flipX = true;
            } else if(ladderTime % 20 <= 9){
                ImageIndex = 63;
                renderor.flipX = false;
            } else if(ladderTime % 20 <= 12){
                ImageIndex = 62;
                renderor.flipX = false;
            } else if(ladderTime % 20 <= 16){
                ImageIndex = 63;
                renderor.flipX = false;
            } else {
                ImageIndex = 63;
                renderor.flipX = true;
            }
        }

        x = ladderx;
        PositioningCode();
    }

    void PositioningCode(){
        player.position = new Vector3(x, y, -1);
        cam.position = new Vector3(player.position.x, player.position.y, -10);
    }

    void fireLemon(){
        if (WeaponEnergy[specialWeaponIndex] <= 0 && specialWeaponIndex != 0)
        {
            return;
        }
        shotTimer = 16;
        
        bullet =  Instantiate(weapons[specialWeaponIndex].Projectile, new Vector3(player.position.x, player.position.y + 0.25f, player.position.z), Quaternion.identity);
        if (leftFacing) {
            bullet.GetComponent<Projectile>().Direction = new Vector3(-1,0,0);
        } else
        {
            bullet.GetComponent<Projectile>().Direction = new Vector3(1,0,0);
        }
        if (specialWeaponIndex != 0)
        {
            WeaponEnergy[specialWeaponIndex]--;
        }
    }

    void RenderArmors(){
        /*headRender.gameObject.SetActive(armorHead);
        headRender.renderor.sprite = headRender.images[ImageIndex];
        headRender.renderor.flipX = leftFacing;*/
    }

    bool CheckCollision(Vector2 position, float width, float height)
    {
        RaycastHit2D hit = Physics2D.BoxCast(position, new Vector2(width, height), 0, Vector2.down, height);
        if (hit.collider == null || hit.collider == this.gameObject || hit.transform.gameObject.tag == "damage" || hit.transform.gameObject.tag == "Player")
        {
            return false;
        } else {
            return true;
        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Untagged" || col.gameObject.name == "explosion(Clone)" || col.gameObject.name == "BossHandler"){
            return;
        }
        //Debug.Log(col.tag);

        if(col.tag == "EndLevel"){
            SceneManager.LoadScene("SelectLevel");
        }

        if(col.tag == "Upgrade"){
            /*if(col.gameObject.GetComponent<UpgradeHandler>().upgradeName == "HealthTank"){
                health+=2;
                maxHealth += 2;
                Destroy(col.gameObject);
            } else if(col.gameObject.GetComponent<UpgradeHandler>().upgradeName == "HeadUpgrade"){
                armorHead = true;
                inDialouge = true;
                Dialouge = "Spark Helmet \nRemoves limitations on dashing,\nturning it froma burst of \nenergy into a constant amount of\nforce ";
                Destroy(col.gameObject);
            }*/
            return;
        }

        if(col.tag == "ladder"){
            if(LeftStick.y != 0 && canEnterLadder){
                canJump = false;
                onLadder = true;
                jumping = false;
                ladderx = col.gameObject.transform.position.x;
                yVel = 0;
            }
            return;
        }
        /*if(col.gameObject.GetComponent<CameraZone>() != null){
            lockType = col.gameObject.GetComponent<CameraZone>().type;
            lockx = col.gameObject.GetComponent<CameraZone>().x;
            lockEnd = col.gameObject.GetComponent<CameraZone>().end;
            lockStart = col.gameObject.GetComponent<CameraZone>().start;
            locky = col.gameObject.GetComponent<CameraZone>().y;
            return;
        }
        if(invuln >= 1){
            return;
        }*/

        if(col.gameObject.GetComponent<Projectile>() != null){
            if(col.gameObject.GetComponent<Projectile>().ownedByPlayer){
                return;
            }
        }

        if(col.tag == "Enemy"){
            if(stun > 0){
                return;
            }
            health -= col.gameObject.GetComponent<GeneralEnemy>().contactDamage;
        }

        /*invuln = 30;
        if(col.tag == "damage"){
            /*if(col.gameObject.GetComponent<BossHandler>() != null || col.gameObject.GetComponent<BurnerHandler>() != null){
                health -= 2;
                stun = 10;
            hurtSound.Play();
            Debug.Log("hit by " + col);
            }
            return;
        }*/
        //health -= col.gameObject.GetComponent<Projectile>().damage;


        stun = 10;
        //hurtSound.Play();
        Debug.Log("hit by " + col);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.tag == "ladder"){
            ladderx = col.gameObject.transform.position.x;
            if(LeftStick.y != 0 && canEnterLadder){
                onLadder = true;
                jumping = false;
                yVel = 0;
            }
            return;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.tag == "ladder"){
            onLadder = false;
            canJump = false;
            canEnterLadder = false;
                renderor.flipX = false;
            LeftStick = new Vector2(LeftStick.x, 0);
            return;
        }
    }

    void HealthRender(){
        HealthBar.localScale = new Vector3((float)health / (float)maxHealth, HealthBar.localScale.y, HealthBar.localScale.z);
        HealthBar.localPosition = new Vector3((maxHealth - health) / ((float)maxHealth * -2), 0, -1);

        if (specialWeaponIndex != 0 && WeaponEnergy[specialWeaponIndex] > 0) {
            WeaponBar.localScale = new Vector3(WeaponEnergy[specialWeaponIndex] / (float)weapons[specialWeaponIndex].MaxEnergy, WeaponBar.localScale.y, WeaponBar.localScale.z);
            WeaponBar.localPosition = new Vector3((weapons[specialWeaponIndex].MaxEnergy - WeaponEnergy[specialWeaponIndex]) / -(weapons[specialWeaponIndex].MaxEnergy * 2), 0, -1);
        } else
        {
            WeaponBar.localScale = new Vector3(0, WeaponBar.localScale.y, WeaponBar.localScale.z);
            WeaponBar.localPosition = new Vector3(0,0,-1);
        }
    }
}
