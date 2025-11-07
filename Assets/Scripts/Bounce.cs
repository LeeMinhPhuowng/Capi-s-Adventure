using Unity.VisualScripting;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    private Animator animator;
    private BoxCollider2D boxCollider;
    private Rigidbody2D playerRb;
    [SerializeField] private float bounceForce = 10f;
    private bool hasBounced = false;
    private AudioSource audioSource;
    void Start()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        playerRb = FindObjectOfType<Player>().GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (boxCollider.IsTouchingLayers(LayerMask.GetMask("Player")) && !hasBounced)
        {
            animator.SetTrigger("IsStamped");
            playerRb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
            hasBounced = true;
            audioSource.Play();
        }
        if (!boxCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            hasBounced = false;
        }
    }
}
