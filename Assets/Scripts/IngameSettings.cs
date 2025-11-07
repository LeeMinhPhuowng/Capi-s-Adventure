using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameSettings : MonoBehaviour
{   GameManager gameManager; 
    public AudioMixer audioMixer;
    public GameObject settings;
    public Slider BGMSlider;
    public Slider SFXSlider;
    public Sprite Muted;
    public Sprite Unmuted;
    public GameObject MuteButton;
    private bool isMuted;
    Player player;
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        settings.SetActive(false);
        isMuted = false;
    }
    private void Update()
    {
        float bgmValue, sfxValue;
        if(audioMixer.GetFloat("BGM", out bgmValue))
        {
            BGMSlider.value = bgmValue;
        }
        if (audioMixer.GetFloat("SFX", out sfxValue))
        {
            SFXSlider.value = sfxValue;
        }
        if (bgmValue == -80f && sfxValue == -80f)
        {
            MuteButton.GetComponent<Image>().sprite = Muted;
            isMuted = true;
        } 
        else MuteButton.GetComponent<Image>().sprite = Unmuted;
    }
    public void GetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }    
    public void GetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", volume);
    }
    public void Show()
    {   
        
        settings.SetActive(true);
        Time.timeScale = 0f;
    }    
    public void Hide() 
    {
        settings.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OnPlayButtonClick()
    {
        SceneManager.LoadScene(1);
        settings.SetActive(false);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    public void OnStartAgainButtonClick()
    {
        SceneManager.LoadScene("Level 1");
        settings.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OnMuteButtonClick()
    {
        if(!isMuted)
        {
            audioMixer.SetFloat("BGM", -80f);
            audioMixer.SetFloat("SFX", -80f);
            MuteButton.GetComponent<Image>().sprite = Muted;
            isMuted = true;
        }
        else
        {
            audioMixer.SetFloat("BGM", 0f);
            audioMixer.SetFloat("SFX", 0f);
            MuteButton.GetComponent<Image>().sprite = Unmuted;
            isMuted = false;
        }
    }
    public void OnMainMenuButtonClick()
    {
        SceneManager.LoadScene("Main Menu");
        settings.SetActive(false);
        Time.timeScale = 1f;
    }
}
