using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelExit : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] float WaitTime = 5f;
    [SerializeField] Sprite UnlockedSprite;
    Player player;
    public GameObject lockObject;
    GameSession gameSession;
    SpriteRenderer LockSpriteRenderer;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = FindObjectOfType<Player>();
        gameSession = FindObjectOfType<GameSession>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(SceneManager.GetActiveScene().buildIndex == 7)
            {
                EdgyKing edgyKing = FindObjectOfType<EdgyKing>();
                edgyKing.moveSpeed = 1f;
            }
            if (player.isHoldingKey)
            {
                audioSource.Play();
                LockSpriteRenderer = lockObject.GetComponent<SpriteRenderer>();
                StartCoroutine(LoadNextScene());
                LockSpriteRenderer.sprite = UnlockedSprite;
                StartCoroutine(DestroyLock());
            }
        }
    }
    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(WaitTime);
        var CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(CurrentSceneIndex + 1);
    } 
    IEnumerator DestroyLock()
    {
        yield return new WaitForSeconds(WaitTime / 5);
        LockSpriteRenderer.enabled = false;
    }
}
