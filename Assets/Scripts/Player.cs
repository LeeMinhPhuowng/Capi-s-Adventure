using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    [SerializeField] public float runSpeed = 10f;
    [SerializeField] public float jumpSpeed = 5f;
    [SerializeField] public float climbSpeed = 0.1f;
    [SerializeField] public float boostedRunSpeed = 15f;
    [SerializeField] public float boostedClimbSpeed = 15f;
    [SerializeField] public float boostedJumpSpeed = 15f;
    [SerializeField] GameObject player;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] PostProcessVolume postProcessVolume;
    [SerializeField] public AudioClip[] myAudioClips;
    [SerializeField] GameObject Projectile;
    [SerializeField] Transform Shooter;
    [SerializeField] float coolDown;
    Image Rockcheck;
    Image Keycheck;

    Vector2 moveValue;

    Rigidbody2D myBody;
    Animator myAnimator;
    BoxCollider2D myBodyCollider;
    CapsuleCollider2D myFeetCollider;
    ColorGrading colorGradingLayer;
    AudioSource myAudioSource;
    public PlayerInput playerInput;
    GameSession gameSession;
    ParticleSystem boostFX;

    float gravityScaleAtStart;
    public bool isAlive = true;
    public bool canEat = true;
    public bool isHoldingKey = false;
    public bool canThrow = true;
    public bool boostSFXplayed = false;
    
    bool moveLeft = false, moveRight = false, moveUp = false, moveDown = false;
    bool isJumping = false;

    public Button Up;
    public Button Down;
    public Button Right;
    public Button Left;
    public Button Shoot;
    public Button Jump;
    public Button Boost;

    void AddEventTriggerListener(Button button, EventTriggerType eventType, System.Action<BaseEventData> action)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(action.Invoke);
        trigger.triggers.Add(entry);
    }

    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<BoxCollider2D>();
        myFeetCollider = GetComponent<CapsuleCollider2D>();
        gravityScaleAtStart = myBody.gravityScale;
        if (postProcessVolume.profile.TryGetSettings(out colorGradingLayer))
        {
            colorGradingLayer.saturation.value = 0;
            colorGradingLayer.contrast.value = 0;
        }
        myAudioSource = GetComponent<AudioSource>();
        playerInput = GetComponent<PlayerInput>();
        myBodyCollider.enabled = true;
        myFeetCollider.enabled = true;
        boostFX = FindObjectOfType<ParticleSystem>();      
        Rockcheck = GameObject.Find("Rockcheck").GetComponent<Image>();
        Keycheck = GameObject.Find("Keycheck").GetComponent<Image>();
        Rockcheck.fillAmount = 1f;
        gameSession = FindObjectOfType<GameSession>();

        Up = GameObject.Find("Up")?.GetComponent<Button>();
        Down = GameObject.Find("Down")?.GetComponent<Button>();
        Left = GameObject.Find("Left")?.GetComponent<Button>();
        Right = GameObject.Find("Right")?.GetComponent<Button>();
        Shoot = GameObject.Find("Shoot")?.GetComponent<Button>();
        Boost = GameObject.Find("Boost")?.GetComponent<Button>();
        Jump = GameObject.Find("Jump")?.GetComponent<Button>();


        Shoot.onClick.AddListener(OnFire);
        Boost.onClick.AddListener(OnBoost);

        AddEventTriggerListener(Jump, EventTriggerType.PointerDown, (eventData) => OnJumpDown());
        AddEventTriggerListener(Jump, EventTriggerType.PointerUp, (eventData) => OnJumpUp());

        AddEventTriggerListener(Up, EventTriggerType.PointerDown, (eventData) => MoveUpDown());
        AddEventTriggerListener(Up, EventTriggerType.PointerUp, (eventData) => MoveUpUp());

        AddEventTriggerListener(Down, EventTriggerType.PointerDown, (eventData) => MoveDownDown());
        AddEventTriggerListener(Down, EventTriggerType.PointerUp, (eventData) => MoveDownUp());

        AddEventTriggerListener(Right, EventTriggerType.PointerDown, (eventData) => MoveRightDown());
        AddEventTriggerListener(Right, EventTriggerType.PointerUp, (eventData) => MoveRightUp());

        AddEventTriggerListener(Left, EventTriggerType.PointerDown, (eventData) => MoveLeftDown());
        AddEventTriggerListener(Left, EventTriggerType.PointerUp, (eventData) => MoveLeftUp());
    }


    void FixedUpdate()
    { if (!isAlive) return;
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
        OnJump();
        if (isHoldingKey && myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Portal")))
        {
            StartCoroutine(ReduceContrastGradually());
            playerInput.enabled = false;
            myBody.constraints = RigidbodyConstraints2D.FreezeAll;
            isHoldingKey = false;
        }
        Keycheck.enabled = isHoldingKey;
        Shoot.interactable = canThrow;
        Debug.Log(canThrow);
    }

    IEnumerator ReduceContrastGradually()
    {
        while (colorGradingLayer.contrast.value > -100f)
        {
            colorGradingLayer.contrast.value = Mathf.MoveTowards(colorGradingLayer.contrast.value, -100f, 1.5f);
            yield return new WaitForSeconds(0.05f);
        }
    }
//  Movement
    void OnMove(InputValue value)
    {
        
        moveValue = value.Get<Vector2>();
        Debug.Log("Joystick Input: " + moveValue);
    }

    void Run()
    {
        if (!isAlive) return;
        float moveDirection = 0f;
        if (moveRight) moveDirection = 1f;
        if (moveLeft) moveDirection = -1f;
        Vector2 playerVelocity = new Vector2(moveDirection * runSpeed, myBody.velocity.y);
        myBody.velocity = playerVelocity;
    }
//  Jumping
    public void OnJump()
    {
        if (!isAlive) return;
        if (isJumping && myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myBody.velocity = new Vector2(myBody.velocity.x, jumpSpeed);
            if(!myAudioSource.isPlaying)
            {
                myAudioSource.PlayOneShot(myAudioClips[2]);
            }
        }
        //isJumping = false;
    }
//  Shooting
    public void OnFire()
    {
        if (!isAlive || !canThrow || myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"))) return;
        Instantiate(Projectile, Shooter.position, transform.rotation);
        canThrow = false;
        StartCoroutine(FireCoolDown());
        myAudioSource.PlayOneShot(myAudioClips[4]);
    }

 /*   bool IsClickingOnOverlayUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            // Kiểm tra nếu UI này thuộc Canvas Overlay
            Canvas canvas = result.gameObject.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return true;
            }
        }
        return false;
    }
*/

    IEnumerator FireCoolDown()
    {
        float elapsedTime = 0f;
        while (elapsedTime < coolDown)
        {
            Rockcheck.fillAmount = elapsedTime / coolDown;
            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        Rockcheck.fillAmount = 1f; 
        canThrow = true;
    }
//  Boosting
    public void OnBoost()
    {
        if(gameSession.boostReady)
        {
            boostFX.Play();
            gameSession.RemoveSpeedBoostEffect();
            if(!boostSFXplayed)
            {
                AudioClip powerUp = myAudioClips[3];
                myAudioSource.PlayOneShot(powerUp);
                boostSFXplayed = true;
            }
        }
    }    
//  Flip sprite
    public void FlipSprite()
    {
        bool hasHorizontalSpeed = Mathf.Abs(myBody.velocity.x) > Mathf.Epsilon;
        if (hasHorizontalSpeed)
        {
            myAnimator.SetBool("isRunning", true);
            transform.localScale = new Vector2(Mathf.Sign(myBody.velocity.x), 1f);
        }
        else
        {
            myAnimator.SetBool("isRunning", false);
        }
    }
//  Climbing
    public void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            myBody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return;
        }
        float moveDirection = 0f; 
        if(moveUp) moveDirection = 1f;
        if (moveDown) moveDirection = -1f;
        Vector2 playerVelocity = new Vector2(myBody.velocity.x, moveDirection * climbSpeed);
        myBody.velocity = playerVelocity;
        myBody.gravityScale = 0f;
        bool hasVerticalSpeed = Mathf.Abs(myBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", hasVerticalSpeed);
    }
//  Dying
    public void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Spikes", "Boss", "Bullet")) || myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Spikes", "Bullet"))) 
        { 
            isAlive = false;
            myAnimator.SetBool("IsAlive", isAlive);
            myBody.velocity = deathKick;
            colorGradingLayer.saturation.value = -100f;
            myBodyCollider.enabled = false;
            myFeetCollider.enabled = false;
            int randomIndex = Random.Range(0, 2);
            myAudioSource.PlayOneShot(myAudioClips[randomIndex]);
            gameSession.ProcessPlayerDeath();
            gameSession.boostReady = false;
            isHoldingKey = false;
        }
    }
    public void MoveLeftDown()
    {
        moveLeft = true;
    }    
    public void MoveLeftUp()
    {
        moveLeft = false;
    }
    public void MoveRightDown()
    {
        moveRight = true;
    }
    public void MoveRightUp()
    {
        moveRight = false;
    }
    public void MoveUpDown()
    {
        moveUp = true;
    }    
    public void MoveUpUp()
    {
        moveUp = false;
    }    
    public void MoveDownDown()
    {
        moveDown = true;
    }    
    public void MoveDownUp()
    {
        moveDown = false;
    } 
    public void OnJumpDown()
    {
        isJumping = true;
    }
    public void OnJumpUp()
    {
        isJumping = false;
    }
}

