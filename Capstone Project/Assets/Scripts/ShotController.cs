using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    //true = right, false = left
    public bool shootVertical;
    public bool shootHorizontal;
    public bool shootDiagonal;

    public bool vertical = true;
    public bool diagonal = false;

    private Rigidbody2D body;

    public float time;
    private float timer;

    public GameObject shooter;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(vertical.ToString() + shootDiagonal.ToString() + shootVertical.ToString() + diagonal.ToString() + shootHorizontal.ToString());
        body = GetComponent<Rigidbody2D>();

        //shoot direction
        if (shootVertical && vertical)
        {
            body.velocity = transform.up * speed;

            if(diagonal && shootDiagonal)
            {
                body.velocity = new Vector2(speed, speed);
            }
            else if (diagonal && !shootDiagonal)
            {
                body.velocity = new Vector2(-speed, speed);
            }
        }
        else
        {
            //if crouch in air
            if (!shootVertical && vertical)
            {
                body.velocity = transform.up * -1.0f * speed;

                if (diagonal && shootDiagonal)
                {
                    body.velocity = new Vector2(speed, -speed);
                }
                else if (diagonal && !shootDiagonal)
                {
                    body.velocity = new Vector2(-speed, -speed);
                }
            }
            else
            {
                //if facing right
                if (shootHorizontal)
                {
                    body.velocity = transform.right * speed;
                }
                //if facing left
                else
                {
                    body.velocity = transform.right * -1.0f * speed;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //instantiate small animationg
        if (collision.gameObject.tag == "Environment") { Destroy(gameObject); }
        if (collision.gameObject.tag == "Enemy1") { Destroy(collision.gameObject.transform.parent.gameObject); }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > time) { Destroy(gameObject); }
    }
}