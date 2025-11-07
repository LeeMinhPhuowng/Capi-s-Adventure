using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EdgyKing : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 1f;
    [SerializeField] GameObject player;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject shooter;
    Rigidbody2D myBody;
    BoxCollider2D myCollider;
    Transform myTransform;
    float FireRate = 5f;
    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        myTransform = GetComponent<Transform>();
        StartCoroutine(FireBullet());
    }
    IEnumerator FireBullet()
    {
        Player playerScript = player.GetComponent<Player>();
        yield return new WaitForSecondsRealtime(Random.Range(8f,11f));
        while (playerScript.isAlive == true && playerScript.playerInput != false)
        {
            Instantiate(bullet, shooter.transform.transform.position, shooter.transform.rotation);
            FireRate = Random.Range(8f, 11f);
            yield return new WaitForSecondsRealtime(FireRate);
        }
    }
    // Update is called once per frame
    void Update()
    {
        myBody.velocity = new Vector2(moveSpeed, 0f);
        float followingY = Mathf.Lerp(myTransform.position.y, player.transform.position.y + 2f, 0.05f);
        myTransform.position = new Vector2(myTransform.position.x, followingY);
        if (Mathf.Abs(myTransform.position.x - player.transform.position.x) > 18f) myTransform.position = new Vector2(player.transform.position.x - 17.8f, myTransform.position.y);
    }
   
}