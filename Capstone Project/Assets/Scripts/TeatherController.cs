using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeatherController : MonoBehaviour
{
    private DistanceJoint2D joint;
    private Rigidbody2D body;
    private PlayerController player;
    private bool reached;
    private bool released;
    private bool contact;
    private float distance = 1;
    public float deployAngle;
    public float speed;
    public float teatherRange;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        joint = player.GetComponent<DistanceJoint2D>();
        joint.enabled = false;

        Vector3 targetVelocity;

        if (player.hMove != 0)
        {
            transform.Rotate(0.0f, 0.0f, deployAngle, Space.Self);
            targetVelocity = new Vector2(Mathf.Cos(deployAngle / 180 * Mathf.PI) * Mathf.Abs(player.hMove) / player.hMove, Mathf.Sin(deployAngle / 180 * Mathf.PI));
        }
        else
        {
            transform.Rotate(0.0f, 0.0f, 90.0f, Space.Self);
            targetVelocity = new Vector2(0.0f, 1.0f);
        }
        body.velocity = targetVelocity * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Teather"))
            released = true;
        if (distance > teatherRange)
            reached = true;

        distance = Mathf.Sqrt(Mathf.Pow(transform.position.x - player.transform.position.x, 2) + Mathf.Pow(transform.position.y - player.transform.position.y, 2));
    }

    void FixedUpdate()
    {
        if (released || reached)
        {
            Retract();
        }
        else if (contact)
        {
            // Swing player
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision Detected.");
        if (other.gameObject.tag == "Terrain" && other.GetComponent<Collider>().gameObject.GetComponent<Rigidbody2D>() /*&& other.gameObject.Grappable*/)
        {
            body.velocity = Vector2.zero;
            contact = true;
            joint.connectedAnchor = new Vector2(transform.position.x, transform.position.y); ;
            joint.enabled = true;
            joint.connectedBody = other.GetComponent<Collider>().gameObject.GetComponent<Rigidbody2D>();
            joint.distance = teatherRange;
        }
        else if (other.gameObject.tag == "Terrain")
        {
            released = true;
        }
        else if ((reached || released) && other.gameObject.tag == "Player")
        {
            player.teatherOut = false;
            Destroy(gameObject);
        }
    }

    void Retract()
    {
        joint.enabled = false;
        float xT = (transform.position.x - player.transform.position.x) / Mathf.Abs(transform.position.x - player.transform.position.x);
        float yT = (transform.position.y - player.transform.position.y) / Mathf.Abs(transform.position.y - player.transform.position.y);
        float x = Mathf.Abs(transform.position.x - player.transform.position.x) / distance * xT;
        float y = Mathf.Abs(transform.position.y - player.transform.position.y) / distance * yT;
        body.velocity = new Vector3(-x, -y, 0) * speed;
    }
}
