using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    //public GameObject tether;
    [SerializeField] private GameObject hook;           // The "hook" in the grappling hook
    [SerializeField] private GameObject shot;           // Our player's projectile
    [SerializeField] private GameObject attack;         // A hitbox representing a melee weapon
    [SerializeField] private GameObject cam;            // The primary virtual camera
    [SerializeField] private CharacterController2D controller;      // The script that processes our movement inputs

    [SerializeField] private float speed = 0.0f;   // Character ground speed

    private float timer;
    
    private bool fire;                      // Attack key input
    public float fireRate;                  // How fast you can shoot
    private float nextFire;                 //counter for fire rate
    private GameObject settingshot;
    
    private bool teather;                  // Teather key input
    private bool crouch;                    // Crouch key input
    private bool up;                        // Look up key input
    private bool jump;                      // Jump key input
    private bool canDouble;                 // bool for being able to double dump
    private bool doubleJump;                // double jump bool
    private bool facing;                    // True = right, False = left
    private bool grounded;                  // On the ground as opposed to in the air?
    private bool camFollow;                 // Camera is in follow mode?
    [System.NonSerialized] public float hMove = 0.0f;             // Ground movement
    [System.NonSerialized] public bool teatherOut;                 // Grappling hook deployed?
    private GameObject GrappleHook;         // Active Grappling Hook Object
    private Animator animate;
    private Rigidbody2D body;


    void Start()
    {
        //Player starts facing right
        facing = true;
        //Player has not double jumped
        doubleJump = false;
        //assign rigidbody to variable
        body = GetComponent<Rigidbody2D>();
        SetInitialState();
    }

    void SetInitialState()      // Sets variables 
    {
        camFollow = true;
    }

    // Update is called once per frame
    void Update()
    {
        //timer
        timer += Time.deltaTime;

        hMove = Input.GetAxisRaw("Horizontal");
        // animate.SetBool("Moving", hMove != 0);
        // animate.SetBool("Crouch", Input.GetButtonDown("Crouched");

        #region Keys
        if (Input.GetButtonDown("Jump"))
        {
            // animate.SetTrigger("Jumping");
            if (grounded) { jump = true; }
            //double jump
            else if (!grounded && canDouble) { doubleJump = true; canDouble = false; }
        }

        if (Input.GetButtonUp("Jump") && !grounded)     // Short hop code
        {
            if (body.velocity.y > 0)
                body.velocity = new Vector2(body.velocity.x, body.velocity.y * .5f);
        }

        //crouch button press/release
        if (Input.GetButtonDown("Upwards")) { up = true; }
        else if (Input.GetButtonUp("Upwards")) { up = false; }

        //look/aim up
        if (Input.GetButtonDown("Downwards")) { crouch = true; }
        else if (Input.GetButtonDown("Downwards")) { crouch = false; }

        //Attack button press/release
        if (Input.GetButtonDown("Attack")) { fire = true; }
        else if (Input.GetButtonUp("Attack")) { fire = false; }

        if (Input.GetButtonDown("Teather")) { teather = true; }
        #endregion
    }

    void FixedUpdate()
    {
        controller.Move(hMove * speed * Time.fixedDeltaTime, crouch, jump, doubleJump);

        //direction facing
        if (hMove > 0) { facing = true; }
        else if (hMove < 0) { facing = false; }

        //Assign grounded
        grounded = controller.m_Grounded;
        //Reset double jump if player is grounded
        if (grounded) { canDouble = true; }

        //Fire if enough time has passed between shots and fire button is pressed
        if (fire && timer > fireRate)
        {
            Attack();
            timer = 0.0f;
        }

        //If grapple key is pressed
        if (teather) { CastTeather(); }
        //reset bools at the end of a FixedUpdate
        jump = false;
        doubleJump = false;
        teather = false;
    }

    void CastTeather()           // Currently non functional
    {
        if (!teatherOut)
        {
            teatherOut = true;
            GrappleHook = Instantiate(hook, new Vector3(transform.position.x + .2f, transform.position.y + .2f, transform.position.z), Quaternion.identity);
        }
    }

    void Attack()
    {
        //place where the shot spawns
        Vector3 attackSpawn = GetShotSpawn();
        //placeholder rotation
        Quaternion placeholderRotation = new Quaternion();
        //shoot the shot
        settingshot = Instantiate(shot, attackSpawn, placeholderRotation);
        //set the shot direction
        SetShotDirection();
    }

    private Vector3 GetShotSpawn()
    {
        Vector3 attackSpawn = body.position;

        //if up is held always shoot up
        if(up)
        {
            attackSpawn.y++;
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                attackSpawn.x++;
            }
            else if(Input.GetAxisRaw("Horizontal") < 0)
            {
                attackSpawn.x--;
            }
        }
        else
        {
            //if crouch in air
            if(Input.GetAxisRaw("Vertical") < 0 && !grounded)
            {
                attackSpawn.y--;
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    attackSpawn.x++;
                }
                else if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    attackSpawn.x--;
                }
            }
            else
            {
                //if facing right
                if(facing)
                {
                    attackSpawn.x++;
                }
                //if facing left
                else
                {
                    attackSpawn.x--;
                }
                //if crouched on ground
                if(crouch && grounded)
                {
                    attackSpawn.y -= 0.5f;
                }
            }
        }

        return attackSpawn;
    }

    private void SetShotDirection()
    {
        //if shooting up
        if (up)
        {
            settingshot.GetComponent<ShotController>().shootVertical = true;

            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                settingshot.GetComponent<ShotController>().diagonal = true;
                settingshot.GetComponent<ShotController>().diagonal = true;
                settingshot.GetComponent<ShotController>().shootDiagonal = true;
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                settingshot.GetComponent<ShotController>().diagonal = true;
                settingshot.GetComponent<ShotController>().diagonal = true;
                settingshot.GetComponent<ShotController>().shootDiagonal = false;
            }
        }
        else
        {
            if (!grounded && Input.GetAxisRaw("Vertical") < 0)
            {
                    settingshot.GetComponent<ShotController>().shootVertical = false;
                    //if crouch in air and holding horizontal
                    if (Input.GetAxisRaw("Horizontal") > 0)
                    {
                        settingshot.GetComponent<ShotController>().diagonal = true;
                        settingshot.GetComponent<ShotController>().shootDiagonal = true;
                    }
                    else if (Input.GetAxisRaw("Horizontal") < 0)
                    {
                        settingshot.GetComponent<ShotController>().diagonal = true;
                        settingshot.GetComponent<ShotController>().shootDiagonal = false;
                    }
            }
            else
            {
                //not a vertical shot
                settingshot.GetComponent<ShotController>().vertical = false;

                Debug.Log(settingshot.GetComponent<ShotController>().vertical);

                //if facing right
                if (facing)
                {
                    settingshot.GetComponent<ShotController>().shootHorizontal = true;
                }
                //if facing left
                else
                {
                    settingshot.GetComponent<ShotController>().shootHorizontal = false;
                }
            }
        }
    }
}