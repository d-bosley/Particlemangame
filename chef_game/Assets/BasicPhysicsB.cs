using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPhysicsB : MonoBehaviour
{
    public Transform playerInputSpace;
    public ColliderScript Interpreter;
    public BasicMove groundScript;
    public Vector3 forceGravity;
    public Vector3 forceVelocity;
    public float forceAcceleration = .48f;
    public float forceDeceleration = 4f;
    public float forceAirAcceleration = 1f;
    public float forceAirDeceleration = 1f;
    public float forceFriction = 2f;
    public float forceAerialDrag;
    public float forceJumping;
    float counterForce;
    public float msv = 50f;
    float lsv;
    float vsv;
    float psv;
    float bvs;
    private float forceMoveSpeed = 0f;
    Vector3 worldUp;
    public Collider playerCollider;
    public Rigidbody playerBody;
    Vector3 playerRotation;
    Vector3 playerForward;
    Vector3 playerUp;
    Vector3 playerSide;
    Vector3 playerInput;
    public RaycastHit hit;
    public Vector3 groundNormal;
    public Vector3 groundPoint;
    public Vector3 groundBary;
    public float groundThreshold;
    public float groundCheck = .6f;
    public float thresholdCheck;
    float maxGroundDot;
    float rotationLock = 5;
    public float centerMass = 10f;
    public Vector3 hitNormal;
    public Vector3 hitPoint;
    public ContactPoint contact;
    public Vector3 contactPosition;
    bool GROUNDED = false;
    bool FALLING = false;
    bool canLAND = false;
    bool canSTALL = false;
    bool JUSTCOLLIDED = false;
    bool ISCOLLIDING = false;
    public float fall_speed = 14f;
    private Rigidbody rb;

    //Other
    float MaxAngleDifference = 40f;
    float GroundCheckDistance = 0.6f;
    float SpeedFixOvershoot = 0.2f;
    [SerializeField, Range(0f, 90f)] float maxGroundAngle = 30f;
    float minGroundDotProduct;
    Renderer playerMaterial;



    // Start is called before the first frame update
    void Start()
    {
        worldUp = new Vector3 (0, 1f, 0);
        playerUp = worldUp;
        forceGravity = new Vector3 (0, -.68f, 0);
        psv = playerBody.velocity.magnitude;
        playerMaterial = this.GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        setCorePhysics();
    }

    // Update is called once per frame
    void Update()
    {
        float joyH = Input.GetAxis("Horizontal");
        float joyV = Input.GetAxis("Vertical");
        playerInput = new Vector3(joyH, 0f, joyV);
        playerMaterial.material.SetColor("_Color", GROUNDED ? Color.black : Color.white);
    }

    public void setCorePhysics()
    {
    //Run through code steps to begin applying the physics
        //if(Interpreter.GROUNDED == false) {Debug.Log("nope");}

        if(Interpreter.GROUNDED == true) {GetMoving();} 
        else{GetFalling();}

    //ROTATIONAL GROUND CODE
/*     if(Physics.Raycast(transform.position, -playerUp.normalized, out hit, groundCheck, GetGround()))
        {
            GROUNDED = true;
            FALLING = false;
            groundNormal = hit.normal.normalized;
            groundPoint = hit.point;
            groundBary = hit.barycentricCoordinate;
            playerUp = groundNormal;
            //transform.position = new Vector3(transform.position.x, hit.point.y + .6f, transform.position.z);
            GetMoving();
        }
    else
        {
            GROUNDED = false;
            FALLING = true;
            playerUp = worldUp;
            GetFalling();
        }


    //Look and see if there's ground below us


        //Rotate the actual rigidbody
        if (GROUNDED)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
        }
        else
        {
            //transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            transform.rotation = transform.rotation;
        } */
    }

    public void GetMoving()
    {
    //Basic actions if player is on the ground
        //Call CoreControl
        setCoreControl(playerInput);

        rotationLock = 5;
    }

    public void GetFalling()
    {
    //Basic actions if player is falling
        //Set forceGravity While Falling
        playerBody.velocity = playerBody.velocity + forceGravity;
    }

    public void setCoreControl(Vector3 joyInput)
    {
    //Read inputs and translate them to core movement

        //Declare variables to set velocity
        Vector3 inputDirection = playerInputSpace.TransformDirection(joyInput);
        Vector3 playerSpeed = new Vector3 (0f, 0f, 0f);
        float joyPower = joyInput.magnitude;
        
        //Declare variables to set rotation
        Vector3 playerPos = transform.position;
        Vector3 playerTo = joyInput;
        Vector3 playerFrom = playerBody.transform.forward;

        float dotAngle = Vector3.Angle(playerFrom, playerTo);
        float turnForce = .25f;
        float turnRadius = Mathf.Round(2f * forceMoveSpeed * 3.14f);
        float turnTorque = turnRadius/4;
        float turnDifference = turnTorque - forceMoveSpeed;
        float turnAngle = 360/dotAngle;
        float turnDistance = turnRadius/turnAngle;

        //Start moving if there is input
        if(joyPower != 0)
        {
        //forceMoveSpeed detects Acceleration and increases the magnitude to values clamped between 0 and the msv (msv)
        //Then the movement is scaled along the current forward vector of the playerCharacter
        //forceMoveSpeed = Mathf.Clamp(forceMoveSpeed + forceAcceleration, 0, msv);        
        //playerSpeed = transform.forward * forceMoveSpeed;
        playerSpeed = transform.forward * msv;


        //Then, rotate the player towards the proposed vector within the local frame (inputDirection) at the turn rate (turnForce)
        playerBody.transform.forward = Vector3.MoveTowards(playerBody.transform.forward, new Vector3 (inputDirection.x, 0f, inputDirection.z), turnForce);


        //Use this when there is no playerInputSpace
        //playerBody.transform.forward = Vector3.MoveTowards(playerBody.transform.forward, joyInput, turnForce);
            //if (dotAngle > 60)
            //{
                //counterForce = forceAcceleration * forceMoveSpeed/5;
                //forceMoveSpeed += forceAcceleration - counterForce;
                //Debug.Log("spd " + forceMoveSpeed);
                //Debug.Log("dist " + turnDistance);
                //Debug.Log("dot " + dotAngle);
            //}
        }

        //Stop moving if there's no input detected and there is still leftover speed
        else if((joyPower == 0) && playerBody.velocity.magnitude != 0)
        {
        //forceMoveSpeed -= Mathf.Min(forceFriction, forceMoveSpeed);
        //playerSpeed = transform.forward * forceMoveSpeed;
        playerSpeed = transform.forward * 0;
        playerBody.transform.forward = playerBody.transform.forward;
        }

        //Reset the player's position if there's no input or speed so we stop in the same direction
        else if((joyPower == 0) && playerBody.velocity.magnitude == 0)
        {
        playerBody.transform.forward = playerBody.transform.forward;
        }

        //Apply all movement values to our velocity
        playerBody.velocity = playerSpeed;
    }


/*     public bool hasFoundGroundPoint()
    {
    //Look to see if we can find a ground point
    } */

    
    public void stayOnGround()
    {
    //Make sure we stay Grounded
    }

    public void setVelocityForce()
    {
    //Keep track of velocity to pass to other functions
    }

    public void setPhysicalForce()
    {
    //Apply forces within the world to effect character
    }

    public LayerMask GetGround()
    {
        LayerMask ground = LayerMask.GetMask("Ground");
        return ground;
//        if(Physics.Raycast(transform.position, -transform.up.normalized, out hit, 4, ground))
//        {
//            Vector3 hitNormal = hit.normal.normalized;
//            Vector3 hitPoint = hit.point;
//            //Debug.Log("Dot: " + Vector3.Dot(groundNormal, worldUp) + "GroundLimit: " + groundThreshold);
//            if(hitPoint.y >= transform.position.y - .5f && Vector3.Dot(hitNormal, worldUp) >= groundThreshold)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }
//        else
//        {
//            return false;
//        }
    }   

}
