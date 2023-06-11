using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    CharacterController controller;
    AudioSource audioSource;
    bool enableControls = true;

    [Header("Controller")]
    public float speed = 100f;
    public float airSpeed = 50f;
    public float gravity = -9.81f;
    public float jumpHeight = 10f;

    public LayerMask groundMask;
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    public Vector3 velocity;
    public bool isGrounded;

    [Header("Camera")]
    public float Sensitivity = 100f;
    float xRotation = 0f;

    public float horizontal , vertical;
    bool jump;
    public float mouseX,mouseY;

    [Header("Animations")]
    public bool moving = false;
    public Animator animator;

    void Awake(){
        controller = GetComponent<CharacterController>();
        //animator = GetComponentInChildren<Animator>();
        //audioSource = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update(){
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(enableControls){
            MoveInput(); 
            LookInput();
        }
        Movement();
        Mouselook();
        //setting moving to true when detecting input
        moving = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical1") != 0);

        //ANIMATION
        //WALKING
        animator.SetBool("moving", moving);
    }

    void MoveInput(){
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical1");
        jump = Input.GetButtonDown("Jump");
    }

    void Movement(){

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 movementDirection = transform.right * horizontal + transform.forward * vertical;
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
        float airMagnitude = Mathf.Clamp01(movementDirection.magnitude) * airSpeed;
        movementDirection.Normalize();

        if(isGrounded){
           controller.Move(movementDirection * magnitude * Time.deltaTime); 
        }
        else{
            controller.Move(movementDirection * airMagnitude * Time.deltaTime); 
        }
    
        if(jump && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void LookInput(){
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }

    void Mouselook(){
        xRotation -= mouseY * Sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 60f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation,0f,0f);
        transform.Rotate(Vector3.up * mouseX * Sensitivity);
    }


}
