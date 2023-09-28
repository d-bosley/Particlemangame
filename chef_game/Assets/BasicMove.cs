using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMove : MonoBehaviour
{
    float speed = 0f;
    public float accel_speed = 14f;
    public float decel_speed = 14f;
    public float friction = 8f;
    float test_speed = 30f;
    public float rotationSpeed;
    Rigidbody rb;
    public Vector3 joystick;
    Vector3 playerPos;
    public Vector3 playerLook;
    public float joystickPower;
    public Vector3 cross_v;
    public Vector3 cross_h;
    public Vector3 crossFinal;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void GetCharacterMove()
    
        {
        //Add accel or decel to speed and set speed to rigidbody velocity


        //Old Code
        /*if(joystickPower != 0)
            {
            //rb.AddForce(transform.forward * accel_speed, ForceMode.VelocityChange);
            if(rb.velocity.magnitude >= 60)
                {
                    rb.velocity = new Vector3 (0f, 0f, 60f);
                }
            else if((joystickPower == 0) && rb.velocity.magnitude > 0)
                {
            //rb.AddForce(transform.forward * Mathf.Max(-decel_speed, -rb.velocity.magnitude), ForceMode.Acceleration);
            //rb.velocity -=  new Vector3 (0f, 0f, Mathf.Min(decel_speed, rb.velocity.magnitude));
            rb.velocity =  Vector3.zero;
            //Debug.Log(rb.velocity.magnitude);
            } */

        joystick = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        joystickPower = joystick.magnitude;
        if(joystickPower != 0)
        {
        speed = Mathf.Clamp(speed + accel_speed, 0, 60);
        rb.velocity = transform.forward * speed;
        //rb.velocity = transform.forward * test_speed;
        }
        else if((joystickPower == 0) && rb.velocity.magnitude > 0)
        {
        speed -= Mathf.Min(decel_speed, rb.velocity.magnitude);
        rb.velocity = transform.forward * speed;
        //speed = Mathf.Lerp(speed, 0, .4f);
        //rb.velocity = Vector3.zero;
        }
        }
        
    public void TestMoving()
    {
        joystick = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        Vector3 justHorizontal = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        cross_v = Vector3.Cross(transform.up, -transform.right);
        cross_h = Vector3.Cross(transform.up, transform.forward);
        Vector3 joystickVertical = new Vector3(Input.GetAxis("Vertical") * cross_v.x, Input.GetAxis("Vertical") * cross_v.y, Input.GetAxis("Vertical") * cross_v.z);
        Vector3 joystickHorizontal = new Vector3(Input.GetAxis("Horizontal") * cross_h.x, Input.GetAxis("Horizontal") * cross_h.y, Input.GetAxis("Horizontal") * cross_h.z);
        Vector3 joystickAmplitude = joystickVertical + justHorizontal;
        //To get proper rotation upon angles, multiple Vectors VERT and HORI by the cross product
        //e.g. Vector HORI (Input.Hori, Input.Hori, Input.Hori) * Cross.(Up, Forward)
        //e.g. Vector VERT (Input.Vert, Input.Vert, Input.Vert) * Cross.(Up, -Right)
        //Then they could be added together to get the complete correct Vector to match rotation
        joystickPower = joystick.magnitude;
        playerPos = transform.position;
        Vector3 targetPos = joystick + playerPos;
        playerLook = rb.transform.forward;
        //playerLook = rb.transform.right;

        if (joystickPower > 0)
        {
        //rb.transform.forward = Vector3.Lerp(rb.transform.forward, joystick, rotationSpeed);
        playerLook = Vector3.Lerp(playerLook, joystick, rotationSpeed);
        //playerLook = Vector3.Lerp(playerLook, joystickAmplitude.normalized, rotationSpeed);
        }
        else
        {
        playerLook = playerLook;
        }
    }
    public void GetTurningPoint()
        {
        //i'll add notes that describe what each thing does so that it's easier to build off of in the future
        //keep in mind, none of this has to do with physics yet, just moving.

        //first, we find the player's current position
        //so we call a vector3 and link it to where ever our player is
        playerPos = transform.position;

        //to actually move the character, we have to link it to our input
        //create a vector3 that looks for the axis of our joystick's input and set them correctly
        //then, grab the magnitude of the input to emulate joystick sensitivity
        joystick = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        joystickPower = joystick.magnitude;
        //finally, create a target position for rotation by combining the current position and the position of the input
        //basically, we add 1 to where ever the player is in the direction of the joystick
        Vector3 targetPos = joystick + playerPos;

        //it's also best to normalize the length of the input, but this may make movement wonky so come back to it later
        //joyInput.Normalize();
        //joyInput = Vector2.ClampMagnitude(joyInput, 1f);

        //these are all values that are supposed to clamp the players speed, ignore these for now.
        //playerVelo = Mathf.Clamp(playerVelo, -24, 24);
        //transform.postion += playerVelo * Time.deltaTime;
        //Vector3 place = velo * Time.deltaTime;

        //finally, it's time to start rotating the player's position
        //set the speed at which rotations to a new direction will happen
    //rotationSpeed = .5f; <<<< YOU NEED THIS
        //PAY ATTENTION TO THIS NEXT PART
        //what we do here is create the rotation target by subtracting the target from the player's position
        //we want the difference between where the player is and where they need to be
        //this is why we subtract the target from the current position
        //finally, we interpolate the player's rotation to the target at a rate of the rotationspeed
        float turnDifference = Vector3.Dot(joystick.normalized, transform.forward.normalized);

        if (joystickPower > 0)
        {
            Quaternion rotationTarget = Quaternion.LookRotation(targetPos - playerPos);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, rotationTarget, rotationSpeed);
        }
        else
        {
            transform.localRotation = transform.localRotation;
        }

        //what we REALLY need is a way to give the player a new heading
        //that can probably be done by updating the player's roation with the new target rotation

        //Vector3 velo = new Vector3(joyInput.x, 0f, joyInput.y) * mspd;
        //Vector3 place = velo * Time.deltaTime;
        //transform.localPosition += place;
    }

    void TestRotation()
    {

    }

    void GetGroundInformation()
    {

    }
}
