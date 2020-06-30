using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class SSPlayerController : MonoBehaviour
{
    CharacterController m_controller;
    SSPlayerInputHandler m_InputHandler;

    [Header("References")]
    [Tooltip("Reference to the main camera used for the player")]
    public Camera playerCamera;



    [Header("Movement")]
    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    public float maxSpeedOnGround = 10f;
    public float sprintingSpeedOnGround= 30f;
   
    public Vector3 characterVelocity { get; set; }// you can get automatically the value of the property without coding

    [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
    public float movementSharpnessOnGround = 15;

    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]

    public float rotationSpeed = 200f;
    public float RotationMultiplier = 1;


    [Tooltip("Force applied downward when in the air")]
    public float gravityDownForce = 20f;
    [Header("Jump")]
    [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 9f;

    private bool isJumping = false;


    float m_CameraVerticalAngle = 0f;

    //crouch code

    private Vector3 cameraInitialPos;
    private bool isCrouching = false;

    //sprinting 

    private bool isSprinting = false; 

    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_InputHandler = GetComponent<SSPlayerInputHandler>();

        cameraInitialPos = playerCamera.transform.localPosition;
    }


    float jumpCounter = 0;
    float jumpTime = 0.5f;


    void Update()
    {
        if (isJumping == false)
        {
            GroundCheck();
        }
        else
        {
            jumpCounter += Time.deltaTime;//per farlo ritornare subito cioe dare una piccola tregua prima di saltare di nuovo
            if (jumpCounter > jumpTime)
            {
                jumpCounter = 0;
                isJumping = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = true;
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }


        MoveMent();
        Crouching();
    }

    public bool isGrounded { get; private set; }
    [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask groundCheckLayers = -1;
    public Transform groundCheckRayTransform;





    void GroundCheck()
    {

        isGrounded = false;

        Vector3 direction = -groundCheckRayTransform.up;
        Ray ray = new Ray(groundCheckRayTransform.position, direction);
        RaycastHit hit;

        Debug.DrawRay(groundCheckRayTransform.position, direction, Color.green);
        if (Physics.Raycast(ray, out hit, 1f, groundCheckLayers))//1f ray distance
        {
            isGrounded = true;
        }

    }


    void Crouching()
    {
        if (isCrouching == false)
        {
            if (m_InputHandler.GetCrouchingInputDown())
            {
                Debug.Log("crouch");
                isCrouching = true;
                Vector3 nl = cameraInitialPos;
                nl.y -= 0.3f;//cambia l'oofset del centro che all'inizio e uno
                playerCamera.transform.localPosition = nl;

                m_controller.height = 1.2f;
                m_controller.center = new Vector3(0, 0.7f, 0);
            }
        }
        else
        {
            if (m_InputHandler.GetCrouchingInputDown())
            {
                Debug.Log("not crouch");
                isCrouching = false;
                Vector3 nl = cameraInitialPos;
                playerCamera.transform.localPosition = nl;

                m_controller.height = 1.8f;
                m_controller.center = new Vector3(0, 1, 0);
            }
        }
    }



    void MoveMent()
    {
       
        // camera movements

        // horizontal movement
        // rotate the transform with the input speed around its local Y axis
        transform.Rotate(new Vector3(0f, (m_InputHandler.GetLookInputsHorizontal() * rotationSpeed * RotationMultiplier), 0f), Space.Self);

        // vertical camera rotation
        // add vertical inputs to the camera's vertical angle
        m_CameraVerticalAngle += (m_InputHandler.GetLookInputsVertical() * -1) * rotationSpeed * RotationMultiplier;
        // limit the camera's vertical angle to min/max
        m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);
        // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
        playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);



        // converts move input to a worldspace vector based on our character's transform orientation
        Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());
        //float speedModifier = isSprinting ? sprintSpeedModifier : 1f;
        float speedModifier = 1;
        // calculate the desired velocity from inputs, max speed, and current slope
        Vector3 targetVelocity = worldspaceMoveInput * maxSpeedOnGround * speedModifier;

        if(isSprinting && isCrouching == false)
        {
            targetVelocity = worldspaceMoveInput * sprintingSpeedOnGround * speedModifier;
        }

        // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
        characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);
        m_controller.Move(characterVelocity * Time.deltaTime);

        //Debug.Log("isGrounded: " + isGrounded);
        if (isGrounded == false)
        {
            // apply the gravity to the velocity
            characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
            // se non e grounded quindi sta in alto li dobbiamo applicare la forza di gravita alla velocita per portarlo giu
        }
        else
        {
            // when he is grounded only then he can jump
            if (isJumping == false && isCrouching == false )
            {
                if (m_InputHandler.GetJumpInputDown())
                {
                    Debug.Log("jump");
                    // then, add the jumpSpeed value upwards
                    characterVelocity += Vector3.up * jumpForce;
                    isJumping = true;// gravity check sara disatttivato per un po di tempo mentre salta
                }
            }
        }


        if (isCrouching)
        {
            characterVelocity /= 2;
        }

        m_controller.Move(characterVelocity * Time.deltaTime);

    }
}
