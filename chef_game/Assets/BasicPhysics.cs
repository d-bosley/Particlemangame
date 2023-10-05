using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class BasicPhysics : MonoBehaviour
{
    public Transform camera;
    public Rigidbody playerBody;
    public Collider eatBox;
    public TextMeshProUGUI testText;
    public float accv; // Acceleration Value (18f)
    public float decv; // Deceleration Value (21f)
    public float fric; // Friction (18f)
    public float airaacv; // Air Acceleration Value (24f)
    public float airdecv; // Air Deceleration Value (24f)
    public float airfric; // Air Friction (2f)
    public float grav; // Gravity Value (20f)
    public float msv;// Maxmimum Speed Value (15.42) around 20mph
    public float airmsv;// Maxmimum Air Speed Value
    public float mfv; // Maximum Fall Speed Value (64.5) 120mph
    public float forceJump; // How hard we press against the surface given personal force
    [SerializeField] AnimationCurve tiltingValue;
    [SerializeField] AnimationCurve leaningValue;
    [SerializeField] AnimationCurve bobbingValue;
    [SerializeField] AnimationCurve stretchingValue;
    float lsv;
    float vsv;
    float lvcv;
    float bvs;
    float forceMove = 0f;
    float forceFall = 0f;
    float groundAngle;
    float groundCheck = .8f;
    float rotationLock = 5;
    float resetGrav;
    float height; // How tall the Player is which will scale as they progress (Default: 1 unit tall)
    Vector3 worldUp = Vector3.up;
    Vector3 moveSpeed;
    Vector3 fallSpeed;
    Vector3 jumpPower;
    Vector3 playerUp;
    Vector3 playerInput;
    Vector3 orientation;
    int jumpInput;
    Vector3 verticalReset;
    Vector3 hitNormal;
    Vector3 hitPoint;
    Vector3 groundNormal;
    Vector3 groundPoint;
    public float jump; // The vector representing our jump
    Vector3 jumpAngle; // What direction we go after jumping given the ground angle
    RaycastHit hit;
    public bool isGrounded = false;
    public bool isFalling = false;
    public bool hasJumped;
    public bool canJump;
    public Collider crouchBox;
    public Collider standBox;
    public Color calm = Color.blue;
    public Color tense = Color.red;
    public Color ready = Color.white;
    public Renderer rend;
    public Transform meshTransform;


    // Start is called before the first frame update
    void Start()
    {
        playerUp = worldUp;
        verticalReset = new Vector3 (playerBody.velocity.x, 0f, playerBody.velocity.z);
        resetGrav = .46f;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, -playerUp.normalized * (groundCheck), Color.red, 0);
        float joyH = Input.GetAxis("Horizontal");
        float joyV = Input.GetAxis("Vertical");
        jumpInput = Input.GetButton("Jump") ? 1 : 0;
        playerInput = new Vector3(joyH, 0f, joyV);
        lvcv = playerBody.velocity.magnitude;
    }

    void FixedUpdate()
    {
        SetCorePhysics();
        DisplayText();
    }

    public void SetCorePhysics()
    {
    // Run through code steps to begin applying the physics

    // Raycast to detect ground
    if(Physics.Raycast(transform.position, -playerUp.normalized, out hit, groundCheck, GetGround()))
        {
            isGrounded = true;
            isFalling = false;
            groundNormal = hit.normal.normalized;
            groundPoint = hit.point;
            groundAngle = Vector3.Angle(worldUp, groundNormal);
            groundCheck = .5f;
            if(groundAngle <= 60){playerUp = worldUp;} else{playerUp = groundNormal;}
            playerBody.position = groundPoint + (Vector3.up * groundCheck);
            GetMoving();
        }
    else
        {
            isGrounded = false;
            isFalling = true;
            playerUp = worldUp;
            groundNormal = playerUp;
            groundCheck = .8f;
            GetFalling();
        }


        // Rotate the actual rigidbody
    if (isGrounded)
        {
            //transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
            //orientation = Vector3.MoveTowards(transform.up, groundNormal, Time.fixedDeltaTime * 15f);
            //Quaternion rotater = Quaternion.FromToRotation(transform.up, orientation) * transform.rotation;
            //transform.rotation = rotater; 
        }
    else
        {
            //transform.rotation = transform.rotation;
        }
    }

    void SetCoreControl(Vector3 joyInput, float accel, float decel, float friction, float maxspeed)
	{
	// Variables
	float currentSpeed = lvcv;
	float minRateChange = isGrounded ? Mathf.Max(19.925f - 12f * (lvcv / maxspeed), 4.215f) : 18f;
	float joyPower = joyInput.magnitude;
	float topSpeed = maxspeed * joyPower;
    int joyRound = (joyPower > 0) ? 1 : 0;
	accel *= joyRound;
	friction *= 1f - joyRound;
    Vector3 velocity = playerBody.velocity;
    vectorSplit(velocity, out Vector3 velocityLateral, out Vector3 velocityVertical);
    Vector3 trueVelocity;
    float lateralSpeed = velocityLateral.magnitude;
    Vector3 moveInput = joyInput.normalized;
    Vector3 moveSpeedLocalized = transform.InverseTransformDirection(moveSpeed);
	Vector3 inputDirection = camera.TransformDirection(joyInput);
	Vector3 projectedInput = Vector3.ProjectOnPlane(inputDirection, Vector3.up);
	Vector3 stabilizedInput = projectedInput.normalized * joyPower;

	// Start Moving
	forceMove -= Mathf.Min(friction * Time.fixedDeltaTime, forceMove);
	forceMove += Mathf.Min(accel * Time.fixedDeltaTime, maxspeed - forceMove);
	float turnPercentage = Vector3.Angle(velocityLateral, stabilizedInput) / 180f;
   	float speedRemainder = maxspeed - (maxspeed * turnPercentage);
    float dampenValue = Mathf.Max(lateralSpeed - speedRemainder, 0f);
    forceMove -= (8 * dampenValue * Time.fixedDeltaTime);
    Vector3 newForward = Vector3.Lerp(playerBody.transform.forward, stabilizedInput, minRateChange * Time.fixedDeltaTime);
   	playerBody.transform.forward = newForward;
	trueVelocity = transform.forward * forceMove;
    Vector3 projection = Vector3.ProjectOnPlane(trueVelocity, groundNormal);
    trueVelocity = Vector3.Lerp(trueVelocity, projection, .82f);
    vectorSplit(trueVelocity, out Vector3 trueLateral, out Vector3 trueVertical);
    velocityLateral = trueLateral;
    velocity = isGrounded ? velocityLateral + trueVertical : velocityLateral + velocityVertical;
    moveSpeed = velocity;
	playerBody.velocity = moveSpeed;

    // Stuff to control the object animation
    float signedTrajectory = Vector3.SignedAngle(velocityLateral, stabilizedInput, transform.up) / 180f;
    Quaternion objbase = Quaternion.Euler(0f, 0f, 0f);
    Quaternion control = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
    float tiltEvaluated = tiltingValue.Evaluate(Mathf.Abs(signedTrajectory));
    float tiltRotation = QuickMath(tiltEvaluated) * -40f * Mathf.Sign(signedTrajectory);
    Debug.Log(tiltEvaluated);
    Quaternion objtilt = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, tiltRotation);
    Quaternion tilting = Quaternion.Slerp(control, objtilt, 15f * Time.fixedDeltaTime);
    meshTransform.transform.rotation = tilting;
    // Get the Current Trajectory to evaluate what angle we must rotate on
    // Essentially if we got forward, tilt left, if we go left tilt forward
    // We also need to do it based on general direction, so we have to work in negatives
    // UP (0, 1, 0) tilts to (0, .96, 0) which means Fwd and Rht need to lean towards (.4)
    // meshTransform.transform.rotation
	}

    void SurfaceImpact()
    {
    // Controls the strength of recoil from impact
    // Can be used for animations and better physics simulations
    float velocity = playerBody.velocity.magnitude;
    float recoil;
    float opposingrecoil;
    Vector3 trajectory;
    Vector3 offset;
    }

    void GetMoving()
	{
	// Ground Movement
	SetCoreControl(playerInput, accv, decv, fric, msv);

    // Reset forceFall
    forceFall = 0f;

    // Setup Test Jumping
    Vector3 velocity = playerBody.velocity;
    vectorSplit(velocity, out Vector3 velocityLateral, out Vector3 velocityVertical);
    if(jumpInput == 1)
    {
    forceJump = jump * jumpInput;
    jumpPower = Vector3.up * forceJump;
    velocityVertical = jumpPower;
    velocity = velocityLateral + velocityVertical;
	playerBody.velocity = velocity;
    }

    // Reset Rotation
	rotationLock = 5f;
	}

    void GetFalling()
	{
	// Aerial Movement
	SetCoreControl(playerInput, airaacv, airdecv, airfric, msv);

	// Add forceFall
    Vector3 velocity = playerBody.velocity;
    vectorSplit(velocity, out Vector3 velocityLateral, out Vector3 velocityVertical);
    forceFall = grav;
    fallSpeed = Vector3.up * -forceFall;
    velocityVertical = Vector3.Max(velocityVertical, Vector3.up * -mfv);
    velocityVertical += fallSpeed * Time.fixedDeltaTime;
    velocity = velocityLateral + velocityVertical;
	playerBody.velocity = velocity;

    // Apply Rotation to Object
    Vector3 heightCheck = new Vector3(0f, .5f, 0f);
    var step = .52f;
    rotationLock -= .5f;

    if(rotationLock <= 0f){transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), step);}
	}

    float QuickMath(float x)
    {
        float product = Mathf.Round(x * 10f) * .1f;
        return product;
    }

    void vectorSplit(Vector3 vector, out Vector3 lateralVector, out Vector3 verticalVector)
    {
    lateralVector = vector - (Vector3.up * vector.y);
    verticalVector = Vector3.up * vector.y;
    }

    void DisplayText()
    {
        testText.text = "Velocity: " + playerBody.velocity.ToString() + "\nJumping: " + jumpInput.ToString();
    }
    
    public LayerMask GetGround()
    {
        LayerMask ground = LayerMask.GetMask("Ground");
        return ground;
    }

    void OnCollisionEnter(Collision collision)
    {
        //Check for initiated collisions and send Collision info to evaluation function
        CheckCollision(collision);
        //JUSTCOLLIDED = true;
    }

    void OnCollisionStay(Collision collision)
    {
        //Check for sustained collisions and send Collision info to evaluation function
        CheckCollision(collision);
        //ISCOLLIDING = true;
    }

    void CheckCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++) {
			Vector3 normal = collision.GetContact(i).normal;
            Vector3 point = collision.GetContact(i).point;
            hitNormal = normal;
            hitPoint = point;
		}
    }
    

}
