using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Security.Cryptography;

public class GameSession : MonoBehaviour
{
    [SerializeField] private Image[] lives = new Image[3];
    [SerializeField] private Image[] energyBar = new Image[5];
    public int energyIdx = 0;
    private int livesIdx = 2;
    public bool boostReady = false;
    Player player;
    float defaultRunSpeed;
    float defaultJumpSpeed;
    float defaultClimbSpeed;
    public GameObject BoostButton;

    void Awake()
    {
        GameSession[] gameSessions = FindObjectsOfType<GameSession>();

        if (gameSessions.Length > 1)
        {
            DestroyImmediate(gameObject); // Xóa chính GameSession mới
        }
        else
        {
            DontDestroyOnLoad(gameObject); // Giữ lại nếu là duy nhất
        }
    }


    private void Start()
    {
        FillLivesBar();
        ResetEnergyBar();
        player = FindObjectOfType<Player>();
        defaultRunSpeed = player.runSpeed;
        defaultJumpSpeed = player.jumpSpeed;
        defaultClimbSpeed = player.climbSpeed;
 
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            defaultRunSpeed = player.runSpeed;
            defaultJumpSpeed = player.jumpSpeed;
            defaultClimbSpeed = player.climbSpeed;
        }
    }

    private void Update()
    {
        BoostButton.GetComponent<Button>().interactable = boostReady;
    }

    public void ProcessPlayerDeath()
    {
        
        if (livesIdx > 0)
        {
            TakeLive();
        }
        else ResetGameSession();
    }

    void TakeLive()
    {
        lives[livesIdx].enabled = false;
        livesIdx--;
        ResetEnergyBar();
        StartCoroutine(ReturnToStart());
    }

    IEnumerator ReturnToStart()
    {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void FillLivesBar()
    {
        for (int i = 0; i < 3; i++)
        {
            lives[i].enabled = true;
        }
        livesIdx = 2; // Reset index when filling lives bar
    }

    public void IncreaseEnergy()
    {
        if (boostReady) return;
        if (energyBar[energyIdx] == null)
        {
            energyBar[energyIdx] = GameObject.Find("EnergyBar" + energyIdx).GetComponent<Image>();
        }
        if (energyIdx < 4)
        {
            energyBar[energyIdx].enabled = true;
            energyIdx++;
        }
        else if (energyIdx == 4)
        {
            energyBar[energyIdx].enabled = true;
            boostReady = true;
        }
    }


    public void ResetEnergyBar()
    {
        for (int i = 0; i < 5; i++)
        {
            if (energyBar[i] == null)
            {
                energyBar[i] = GameObject.Find("EnergyBar" + i).GetComponent<Image>();
            }
            energyBar[i].enabled = false;
        }
        energyIdx = 0;
    }

    public void ApplySpeedBoostEffect()
    {
        player.runSpeed = player.boostedRunSpeed;
        player.jumpSpeed = player.boostedJumpSpeed;
        player.climbSpeed = player.boostedClimbSpeed;
        StartCoroutine(BoostTimeRunOut());
    }

    public void RemoveSpeedBoostEffect()
    {
        StartCoroutine(ResetSpeedBoost());
    }


    IEnumerator ResetSpeedBoost()
    {
        ApplySpeedBoostEffect();

        for (int i = 4; i >= 0; i--)
        {
            yield return new WaitForSecondsRealtime(0.25f);
            if (energyBar[i] == null)
            {
                energyBar[i] = GameObject.Find("EnergyBar" + i).GetComponent<Image>();
            }
            energyBar[i].enabled = false;
        }
        energyIdx = 0; // Reset energy index when resetting energy bar
        boostReady = false;
        player.boostSFXplayed = false;
    }

    IEnumerator BoostTimeRunOut()
    {
        yield return new WaitForSecondsRealtime(3f);
        player.runSpeed = defaultRunSpeed;
        player.climbSpeed = defaultClimbSpeed;
        player.jumpSpeed = defaultJumpSpeed;
    }

    public void ResetGameSession()
    {
        SceneManager.LoadScene(2);
        Destroy(gameObject);
    }

}