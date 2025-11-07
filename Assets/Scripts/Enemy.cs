using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D myBody;
    BoxCollider2D myCollider;
    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        myBody.velocity = new Vector2(moveSpeed, 0f);
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {     
        moveSpeed = -moveSpeed;
        Flip();
    }
    void Flip()
    {
        transform.localScale = new Vector2((-Mathf.Sign(myBody.velocity.x)), 1f);
    }
}