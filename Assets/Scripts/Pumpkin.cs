using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pumpkin : MonoBehaviour
{
    [SerializeField] AudioClip PickingSFX;
    GameSession gameSession;
    AudioSource audioSource;
    bool wasCollected;
    // Start is called before the first frame update
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        wasCollected = false;
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!wasCollected && collision.CompareTag("Player") && !gameSession.boostReady)
        {
            wasCollected = true;
            audioSource.PlayOneShot(PickingSFX);
            GetComponent<Collider2D>().enabled = false; // Tắt va chạm
            GetComponent<SpriteRenderer>().enabled = false; // Ẩn hình ảnh

            Destroy(gameObject, PickingSFX != null ? PickingSFX.length : 0.5f);

            gameSession.IncreaseEnergy();
        }
    }


}
