using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity;
    public float walkingGravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    public bool isGrounded;

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    public bool isMoving = false;

    public bool isSwimming = false;
    public bool isUnderwater = false;
    public float swimmingGravity = -0.5f;

    private void Start()
    {
        lastPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (DialogSystem.Instance.dialogUIActive == false && StorageManager.Instance.storageUIOpen == false && CampfireUIManager.Instance.isUIOpen == false)
        {
            Movement();  
        }

    }

    public void Movement()
    {
        if (isSwimming)
        {
            if (isUnderwater)
            {
                gravity = swimmingGravity;
            }
            else
            {
                velocity.y = 0;
            }
        }
        else
        {
            gravity = walkingGravity;
        }

        //checking if we hit the ground to reset our falling velocity, otherwise we will fall faster the next time
        isGrounded = Physics.CheckSphere(groundCheck.position,
                                         groundDistance,
                                         groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        //disable left/right/forward/back when fishing
        if (!FishingSystem.Instance.hasPulled)
        {
            //right is the red Axis, foward is the blue axis
            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);
        }
        

        


        //check if the player is on the ground so he can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //the equation for jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);


        if (isGrounded && (lastPosition.x != gameObject.transform.position.x || lastPosition.z != gameObject.transform.position.z))
        {
            //Debug.Log(lastPosition +" - "+gameObject.transform.position);
            lastPosition = gameObject.transform.position;
            isMoving = true;
            SoundManager.Instance.PlaySound(SoundManager.Instance.grassWalkSound);
        }
        else
        {
            isMoving = false;
            SoundManager.Instance.grassWalkSound.Stop();
        }
    }
}
