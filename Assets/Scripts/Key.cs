using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{   Player player;
    [SerializeField] AudioClip PickingSFX;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && player.isHoldingKey == false)
        {
            player.isHoldingKey = true;
            audioSource.PlayOneShot(PickingSFX);
            Destroy(gameObject, 0.225f);
        }
    }
}
