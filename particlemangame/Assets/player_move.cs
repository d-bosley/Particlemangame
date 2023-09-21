using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_move : MonoBehaviour
{
    public float accel;
    public float speed;
    public float jump;
    public float fall;
    public float friction;
    public Rigidbody rb;
    public Animator anim;
    public Collider caps;
    Vector3 moveDirection;
    Vector3 planarVelocity;
    Vector3 trueVelocity;

    // Basic Movement
    // Basic Physics
    // Basic Interaction
    // Fixed 2.5D Cameraspace (Crash Bandicoot, Little Nightmares)
    // Work on Level Transition System
    // Finish "Eating Method"
    // Finish Advanced Movement

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float vert = Input.GetAxis("Vertical");
        float hori = Input.GetAxis("Horizontal");
        moveDirection = new Vector3(hori, 0, vert) * speed;
        rb.velocity = moveDirection;
    }
}
