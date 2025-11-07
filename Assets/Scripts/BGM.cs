using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    AudioSource audioSource;
    Player player; 
    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
        player = FindObjectOfType<Player>();
        audioSource.enabled = true;
    }
    private void Update()
    {
        if(!player.isAlive)
        {
            audioSource.enabled = false;
        }
    }
}
