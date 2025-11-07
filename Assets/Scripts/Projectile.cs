using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D body;
    [SerializeField] float flyingSpeed = 1f;
    Transform player;
    Transform flip;
    float Direction;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform; 
        Direction = player.localScale.x;
        flip = GetComponent<Transform>();
    }


    // Update is called once per frame
    void Update()
    {
        body.velocity = new Vector2(flyingSpeed * Direction, 0f);
        if (Direction == 1f) flip.localRotation = Quaternion.Euler(0f, 0f, 0f);

        else if (Direction == -1f)  flip.localRotation = Quaternion.Euler(0f, 180f, 0f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
        }
        Destroy(gameObject);
    }
}
