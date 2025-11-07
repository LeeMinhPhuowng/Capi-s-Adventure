using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerMenu : MonoBehaviour
{
    [SerializeField] GameObject SettingCanvas;
    [SerializeField] GameObject IngameSettingCanvas;
    
    void Start()
    {
        SettingCanvas.SetActive(false);
        IngameSettingCanvas.SetActive(false);
    }

    public void OnPlayButtonClick()
    {
        SceneManager.LoadScene(1);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }
    public void OnMenuSettingsButtonClick()
    {
    }
    public void OnIngameSettingsButtonClick()
    {
    }

    public void OnBackButtonClick()
    {
    }

    public void OnStartAgainButtonClick()
    {
        SceneManager.LoadScene("Level 1");
    }
    public void OnMuteButtonClick()
    {

    }
    public void OnMainMenuButtonClick()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
