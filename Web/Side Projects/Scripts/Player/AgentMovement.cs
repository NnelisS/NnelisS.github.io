using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class AgentMovement : MonoBehaviour
{
    public static AgentMovement instance;

    protected Rigidbody rb;

    protected Vector3 movementDirection;

    [field : SerializeField] public bool movementOff { get; set; }

    [field: SerializeField]
    public float currentVelocity { get; private set; }
    
    [Header("Player Info")]
    public Transform playerBody;

    [SerializeField] private Animator playerAnim;
    [SerializeField] private Slider stamina;

    [Header("Player Settings")]
    public float MaxWalkSpeed = 2.5f;
    public float MaxCrouchSpeed = 2;
    public float MaxRunSpeed = 5f;
    public float MaxSideStepSpeed = 2;
    public float MaxCrawlSpeed = 2;
    public float JumpForce = 2.0f;
    public float gravity = 5.0f;

    [Space(10)]
    [Header("Crouch Settings")]
    [SerializeField] private float crouchOffTime = 1;
    private bool crouchUsable = true;

    [Space(30)]
    [Header("Cam Settings")]
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject wallJumpCam;
    [SerializeField] private GameObject sideStepCam;
    [SerializeField] private GameObject crawlCam;

    [Space(10)]
    [Header("Vcam Settings")]
    [SerializeField] private CinemachineFreeLook mainVcam;
    [SerializeField] private CinemachineVirtualCamera sideStepVcam;

    [Space(50)]
    [Header("GroundCheck Settings")]
    [SerializeField] private LayerMask ground;

    [Header("GroundCheck Info")]
    [SerializeField] private bool grounded = false;

    [Space(20)]
    [Header("Interactable Settings")]
    [SerializeField] private LayerMask pickups;
    [SerializeField] private LayerMask interactiveLayer;
    [SerializeField] private Pickup pickupItem; 
    [SerializeField] private ClosetDoors door;

    [Space(20)]
    [Header("Punch Info")]
    public BoxCollider FistColl;

    [Space(60)]
    [Header("Crawl Settings")]
    [SerializeField] private LayerMask crawlLayer;

    [field: Space(20)]
    [Header("Crawl Info")]
    public bool onCrawlStart = false;

    [SerializeField] private Crawl crawl;
    private Quaternion crawlRot;

    [Space(20)]
    [Header("SideStep Settings")]
    [SerializeField] private LayerMask sideStepLayer;

    [field: Space(20)]
    [Header("SideStep Info")]
    public bool OnSideStepStart = false;

    [SerializeField] private WallWalk wall;

    private Quaternion sideStepRot;

    [field: Space(20)]
    [Header("Jump Settings")]
    [SerializeField] private LayerMask jumpLayer;

    [field: Space(20)]
    [Header("Jump Info")]
    [SerializeField] private Vector3 jumpPos;

    [field: Space(20)]
    [Header("Hide Settings")]
    [SerializeField] private LayerMask coverLayer;

    [field: Space(20)]
    [Header("Hide Info")]
    [SerializeField] private HideWall hideWall;
    [SerializeField] private Transform hideTooltip;
    [SerializeField] private Vector3 coverPos;

    [field : Space(20)]
    [field : SerializeField] public bool inCrouch { get; private set; }
    [field : SerializeField] public bool inWallJump { get; private set; }
    [field : SerializeField] public bool inSprint { get; private set; }
    [field : SerializeField] public bool inSideStep { get; private set; }
    [field : SerializeField] public bool inCrawl { get; private set; }
    [field : SerializeField] public bool inPunch { get; private set; }
    [field: Space(20)]

    public bool takingCover = false;

    public float gravityValue = -9.81f;
    public Vector3 playerVelocity;
    public bool jumpPressed = false;

    private void Awake()
    {
        instance = this;  
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (movementOff == false)
        {
            FunctionAble();
            Doors();
            Pickup();

            OnJump();

            SideStep(); 
            Crawl();
        }

        Stamina();
        Crawling();
        SideStepping();
        MoveToCover();
        JumpOverWall();

        Gravity();
        GroundCheck();
    }

    private void FixedUpdate()
    {
        RotateAgent();

        rb.velocity = movementDirection.normalized * currentVelocity;
    }

    private void OnCollisionEnter()
    {
        jumpPressed = false;
        JumpForce = 2.5f;
        gravity = 0;
    }

    private void FunctionAble()
    {
        if (inPunch == false)
        {
            WallJump();
            TakeCover();
        }

        if (inSideStep == false && inPunch == false && inWallJump == false && inCrawl == false)
            Crouch();

        if (inCrouch == false && inSideStep == false && inPunch == false && inWallJump == false && inCrawl == false && currentVelocity > 0.1f)
        {
            Zoom();
            Sprint();
        }
        else if (currentVelocity == 0)
            inSprint = false;

        if (inCrouch == false && inSideStep == false && inWallJump == false && inCrawl == false)
            Attack();
    }

    private void DefaultMovementsOff()
    {
        inCrouch = false;
        inSprint = false;

        playerAnim.SetBool("Running", false);
        playerAnim.SetBool("Crouch", false);

        VcamSettings.instance.FOVValue = 50;
    }

    /// <summary>
    /// Does check if groundcheck
    /// </summary>
    private void GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down, out hit, 1, ground))
        {
            Debug.DrawLine(transform.position, hit.point, Color.green);
            grounded = true;
        }
        else
            grounded = false;
    }

    private void Zoom()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("right");
            mainVcam.m_Lens.FieldOfView = 20;
        }
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            movementOff = true;
            inPunch = true;
            playerAnim.SetBool("InPunch", true);
        }
    }

    public void PunchCol()
    {
        FistColl.enabled = true;
    }

    public void OutPunch()
    {
        FistColl.enabled = false;
        movementOff = false;
        inPunch = false;
        playerAnim.SetBool("InPunch", false);
    }

    private void Stamina()
    {
        if (currentVelocity > 0 && currentVelocity <= 2.5f && inSprint == false)
            stamina.value = Mathf.MoveTowards(stamina.value, stamina.maxValue, 0.6f * Time.deltaTime);
        else if (currentVelocity == 0)
            stamina.value = Mathf.MoveTowards(stamina.value, stamina.maxValue, 3 * Time.deltaTime);
    }

    /// <summary>
    /// When using Shift u can sprint with Fov effect of cinemachine cam
    /// </summary>
    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            stamina.value = Mathf.MoveTowards(stamina.value, 0, 3 * Time.deltaTime);
            if (stamina.value >= 0.3f)
            {
                VcamSettings.instance.FOVValue = 60;
                inSprint = true;
            }
            else
            {
                VcamSettings.instance.FOVValue = 50;
                inSprint = false;
            }

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            inSprint = false;
            VcamSettings.instance.FOVValue = 50;
        }
        else if (inCrouch == true)
            VcamSettings.instance.FOVValue = 50;

        if (inSprint == false && stamina.value != stamina.maxValue)
            stamina.value = Mathf.MoveTowards(stamina.value, stamina.maxValue, 1 * Time.deltaTime);
    }

    private void SideStep()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10, sideStepLayer))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Vector3.Distance(hit.transform.position, transform.position) <= 10)
                {
                    DefaultMovementsOff();
                    inSideStep = true;
                    movementOff = true;
                    sideStepCam.SetActive(true);
                    wall = hit.transform.gameObject.GetComponentInParent<WallWalk>();
                    GetComponentInChildren<CapsuleCollider>().radius = 0.4f;
                    playerAnim.Play("Left Turn");
                }
            }
        }
    }

    private void SideStepping()
    {
        if (inSideStep && OnSideStepStart)
        {
            if (Input.GetKey(KeyCode.W))
                transform.Translate(Vector3.forward * MaxSideStepSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.W))
                playerAnim.speed = 1;
            else if (Input.GetKeyUp(KeyCode.W))
                playerAnim.speed = 0.02f;
        }
        if (inSideStep)
        {
            if (transform.rotation != sideStepRot)
            {
                Vector3 lookPos = wall.EndPos.position - transform.position;
                lookPos.y = 0;
                sideStepRot = Quaternion.LookRotation(lookPos);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, sideStepRot, 10 * Time.maximumDeltaTime);
            }

            if (Vector3.Distance(transform.position, new Vector3(wall.StartPos.position.x, wall.StartPos.position.y, wall.StartPos.position.z)) > 0.1f && OnSideStepStart == false)
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(wall.StartPos.position.x, wall.StartPos.position.y, wall.StartPos.position.z), 2 * Time.deltaTime);

            if (Vector3.Distance(transform.position, new Vector3(wall.StartPos.position.x, wall.StartPos.position.y, wall.StartPos.position.z)) <= 0.1f)
                OnSideStepStart = true;

            if (Vector3.Distance(transform.position, wall.EndPos.transform.position) <= 0.5f)
            {
                GetComponentInChildren<CapsuleCollider>().radius = 0.5f;
                playerAnim.Play("Right Turn");
                mainCamera.transform.position = sideStepCam.transform.position;
                sideStepCam.SetActive(false);
                StartCoroutine(OutSideStep());
                OnSideStepStart = false;
                wall = null;
                inSideStep = false;
            }
        }
    }

    private void Crawl()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 5, crawlLayer))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Vector3.Distance(hit.transform.position, transform.position) <= 5)
                {
                    DefaultMovementsOff();
                    inCrawl = true;
                    movementOff = true;
                    crawlCam.SetActive(true);
                    crawl = hit.transform.gameObject.GetComponentInParent<Crawl>();
                    playerAnim.Play("Laying Down");
                }
            }
        }
    }

    private void Crawling()
    {
        if (inCrawl && onCrawlStart)
        {
            if (Input.GetKey(KeyCode.W))
                transform.Translate(Vector3.forward * MaxCrawlSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.W))
                playerAnim.speed = 1;
            else if (Input.GetKeyUp(KeyCode.W))
                playerAnim.speed = 0.02f;
        }
        if (inCrawl)
        {
            playerBody.transform.localPosition = new Vector3(playerBody.transform.localPosition.x, Mathf.MoveTowards(playerBody.transform.localPosition.y, -0.2f, 2.5f * Time.deltaTime), playerBody.transform.localPosition.z);
            playerBody.transform.localScale = new Vector3(playerBody.transform.localScale.x, Mathf.Lerp(playerBody.transform.localScale.y, 0.5f, 0.5f * Time.maximumDeltaTime), playerBody.transform.localScale.z);

            if (transform.rotation != crawlRot)
            {
                Vector3 lookPos = crawl.EndPos.position - transform.position;
                lookPos.y = 0;
                crawlRot = Quaternion.LookRotation(lookPos);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, crawlRot, 10 * Time.maximumDeltaTime);
            }

            if (Vector3.Distance(transform.position, new Vector3(crawl.StartPos.position.x, crawl.StartPos.position.y, crawl.StartPos.position.z)) > 0.1f && onCrawlStart == false)
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(crawl.StartPos.position.x, crawl.StartPos.position.y, crawl.StartPos.position.z), 1 * Time.deltaTime);

            if (Vector3.Distance(transform.position, new Vector3(crawl.StartPos.position.x, crawl.StartPos.position.y, crawl.StartPos.position.z)) <= 0.1f)
                onCrawlStart = true;

            if (Vector3.Distance(transform.position, crawl.EndPos.transform.position) <= 0.5f)
            {
                GetComponentInChildren<CapsuleCollider>().radius = 0.5f;
                playerAnim.Play("Getting Up");
                mainCamera.transform.position = crawlCam.transform.position;
                crawlCam.SetActive(false);
                StartCoroutine(OutCrawl());
                onCrawlStart = false;
                crawl = null;
                inCrawl = false;
            }
        }
    }

    private void WallJump()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10, jumpLayer))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);

            if (Vector3.Distance(hit.transform.position, transform.position) <= 5)
            {
                hideTooltip.gameObject.SetActive(true);
                hideTooltip.position = new Vector3(hit.point.x, hit.transform.position.y + 1.75f, hit.point.z);
            }
            else
                hideTooltip.gameObject.SetActive(false);

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Vector3.Distance(hit.transform.position, transform.position) <= 5)
                {
                    if (hit.transform.gameObject.GetComponent<WallJump>().Front == true)
                    {
                        jumpPos = new Vector3(transform.position.x, transform.position.y, hit.transform.position.z - 2);
                        transform.rotation = Quaternion.LookRotation(-Vector3.forward, Vector3.up);
                        hit.transform.gameObject.GetComponent<WallJump>().Front = false;
                    }
                    else if (hit.transform.gameObject.GetComponent<WallJump>().Back == true)
                    {
                        jumpPos = new Vector3(transform.position.x, transform.position.y, hit.transform.position.z + 2);
                        transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                        hit.transform.gameObject.GetComponent<WallJump>().Back = false;
                    }

                    inWallJump = true;
                    wallJumpCam.SetActive(true);
                    movementOff = true;
                    GetComponentInChildren<CapsuleCollider>().enabled = false;
                    playerAnim.Play("Jump Over");
                }
            }
        }
    }

    private void JumpOverWall()
    {
        if (jumpPos != new Vector3(0, 0, 0))
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(jumpPos.x, transform.position.y, jumpPos.z), 2 * Time.deltaTime);

            if (Vector3.Distance(transform.position, jumpPos) <= 0.6f)
            {
                inWallJump = false;
                wallJumpCam.SetActive(false);
                movementOff = false;
                GetComponentInChildren<CapsuleCollider>().enabled = true;
                jumpPos = new Vector3(0, 0, 0);
            }
        }
    }
            
    /// <summary>
    /// Get wall Position where you can hide
    /// </summary>
    private void TakeCover()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 15, coverLayer))
        {
            if (Vector3.Distance(hit.transform.position, transform.position) >= 3)
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);

                if (Vector3.Distance(hit.transform.position, transform.position) <= 10)
                {
                    hideTooltip.gameObject.SetActive(true);
                    hideTooltip.position = new Vector3(hit.point.x, hit.transform.position.y + 1.75f, hit.point.z);
                }
                else
                    hideTooltip.gameObject.SetActive(false);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (Vector3.Distance(hit.transform.position, transform.position) <= 10)
                    {
                        inSprint = false;
                        movementOff = true;
                        currentVelocity = 0;
                        hideWall = hit.transform.GetComponentInChildren<HideWall>();
                        takingCover = true;
                        playerAnim.CrossFade("TakeCover", 0.1f);
                        inCrouch = true;
                        coverPos = hit.point;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Moves to position TakeCover(); got
    /// </summary>
    private void MoveToCover() 
    {
        if (coverPos != new Vector3(0, 0, 0))
        {
            Vector3 lookPos = coverPos - transform.position;
            lookPos.y = 0;
            Quaternion coverRot = Quaternion.LookRotation(lookPos);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, coverRot, 10 * Time.maximumDeltaTime);

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(coverPos.x, transform.position.y, coverPos.z), 3 * Time.deltaTime);

            if (Vector3.Distance(transform.position, coverPos) <= 1)
            {
                movementOff = false;
                playerAnim.CrossFade("Crouch Idle", 0.5f);
                coverPos = new Vector3(0, 0, 0);
            }

            if (currentVelocity > 0)
            {
                inCrouch = false;
                coverPos = new Vector3(0, 0, 0);
            }
        }

        if (takingCover && currentVelocity > 0 && hideWall.hiding == false)
        {
            takingCover = false;
        }
    }

    /// <summary>
    /// Applies gravity when not grounded
    /// </summary>
    private void Gravity()
    {
        if (jumpPressed && grounded == false)
        {
            rb.AddForce(0, JumpForce, 0, ForceMode.VelocityChange);
            JumpForce -= 10 * Time.deltaTime;
        }

        if (grounded == false && jumpPressed == false)
        {
            rb.AddForce(0, gravity, 0, ForceMode.VelocityChange);
            gravity -= 10 * Time.deltaTime;
        }
    }

    /// <summary>
    /// when jumping u jump keeping the same velocity
    /// </summary>
    private void OnJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded && jumpPressed == false)
        {
            rb.velocity = movementDirection.normalized * currentVelocity;
            jumpPressed = true;
            grounded = false;
        }
    }

    public void MovementChange(Vector3 movementInput)
    {
        if (jumpPressed)
        {
            if (inSprint)
                MoveAgent(movementInput, 20, 25, MaxRunSpeed);
            else
                MoveAgent(movementInput, 0.1f, 0.1f, MaxWalkSpeed);
        }


        if (jumpPressed == false && inSprint == false)
            MoveAgent(movementInput, 20, 25, MaxWalkSpeed);
        else if (inSprint == true)
            MoveAgent(movementInput, 20, 25, MaxRunSpeed);

        if (inCrouch && inSprint == false)
            MoveAgent(movementInput, 20, 25, MaxCrouchSpeed);
    }

    private void MoveAgent(Vector3 movementInput, float _acceleration, float _deacceleration, float _maxSpeed)
    {
        if (movementOff == false)
        {
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            forward.y = 0;
            right.y = 0;
            forward = forward.normalized;
            right = right.normalized;

            Vector3 forwardRelativeVerticalInput = movementInput.z * forward;
            Vector3 rightRelativeVerticalInput = movementInput.x * right;

            Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeVerticalInput;

            if (movementInput.magnitude > 0)
                movementDirection = cameraRelativeMovement;

            currentVelocity = CalculateSpeed(movementInput, _acceleration, _deacceleration, _maxSpeed);
        }
        else
            currentVelocity = 0;
    }

    /// <summary>
    /// Rotate player to where ur walking to
    /// </summary>
    private void RotateAgent()
    {
        if (currentVelocity != 0)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 8 * Time.deltaTime);
        }
    }

    /// <summary>
    /// if pressing crouch button move player down and scale down movement speed and fov
    /// </summary>
    private void Crouch()
    {
        if (Input.GetKey(KeyCode.LeftControl) && crouchUsable == true)
        {
            VcamSettings.instance.FOVValue = 40;
            playerBody.transform.localPosition = new Vector3(playerBody.transform.localPosition.x, Mathf.MoveTowards(playerBody.transform.localPosition.y, -0.2f, 2.5f * Time.deltaTime), playerBody.transform.localPosition.z);
            playerBody.transform.localScale = new Vector3(playerBody.transform.localScale.x, Mathf.Lerp(playerBody.transform.localScale.y, 0.5f, 0.5f * Time.maximumDeltaTime), playerBody.transform.localScale.z);
            inSprint = false;
            inCrouch = true;
        }
        else
        {
            playerBody.transform.localPosition = new Vector3(playerBody.transform.localPosition.x, Mathf.MoveTowards(playerBody.transform.localPosition.y, 0.3f, 0.5f * Time.maximumDeltaTime), playerBody.transform.localPosition.z);
            playerBody.transform.localScale = new Vector3(playerBody.transform.localScale.x, Mathf.Lerp(playerBody.transform.localScale.y, 1, 0.5f * Time.maximumDeltaTime), playerBody.transform.localScale.z);

            if (crouchUsable == false)
            {
                crouchOffTime -= Time.deltaTime;
                if (crouchOffTime <= 0)
                {
                    crouchUsable = true;
                    crouchOffTime = 0.4f;
                }
            }

            if (takingCover == false)
                inCrouch = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            crouchUsable = false;
            VcamSettings.instance.FOVValue = 50;
            inCrouch = false;
        }
    }

    public void GoCrouch()
    {
        playerBody.transform.localPosition = new Vector3(playerBody.transform.localPosition.x, Mathf.MoveTowards(playerBody.transform.localPosition.y, 0, 0.5f * Time.maximumDeltaTime), playerBody.transform.localPosition.z);
        playerBody.transform.localScale = new Vector3(playerBody.transform.localScale.x, Mathf.Lerp(playerBody.transform.localScale.y, 0.5f, 0.5f * Time.maximumDeltaTime), playerBody.transform.localScale.z);
    }

    private void Doors()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10, interactiveLayer))
        {
            if (door != null)
            {
                if (door != hit.transform.GetComponent<ClosetDoors>())
                {
                    door.OnObject = false;
                    door = null;
                }
            }
            else
            {
                door = hit.transform.GetComponent<ClosetDoors>();

                if (Vector3.Distance(hit.transform.position, transform.position) <= 5)
                    door.OnTheObject();
            }

            Debug.DrawLine(transform.position, hit.point, Color.red);
        }
        else
        {
            if (door != null)
            {
                door.OnObject = false;
                door = null;
            }
        }
    }    
    
    private void Pickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10, pickups))
        {
            pickupItem = hit.transform.GetComponent<Pickup>();
            pickupItem.outline.SetActive(true);

            Debug.Log("Pickup");
            Debug.DrawLine(transform.position, hit.point, Color.red);


            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Vector3.Distance(hit.transform.position, transform.position) <= 5)
                {
                    print("picked up");
                }
            }
        }
        else
        {
            if (pickupItem != null)
            {
                pickupItem.outline.SetActive(false);
                pickupItem = null;
            }
        }
    }

    private float CalculateSpeed(Vector3 movementInput, float _acceleration, float _deacceleration, float _maxSpeed)
    {
        if (movementInput.magnitude > 0)
            currentVelocity += _acceleration * Time.deltaTime;
        else
            currentVelocity -= _deacceleration * Time.deltaTime;

        return Mathf.Clamp(currentVelocity, 0, _maxSpeed);
    }   
    
    private IEnumerator OutSideStep()
    {
        yield return new WaitForSeconds(1);
        movementOff = false;
    }    

    private IEnumerator OutCrawl()
    {
        yield return new WaitForSeconds(4.5f);
        movementOff = false;
    }
}