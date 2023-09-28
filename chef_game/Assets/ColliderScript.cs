using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScript : MonoBehaviour
{

    [System.NonSerialized] public bool GROUNDED = false;
    [System.NonSerialized] public bool FALLING = false;
    [System.NonSerialized] public bool JUSTCOLLIDED = false;
    [System.NonSerialized] public bool COLLIDING = false;
    [SerializeField, Range(0f, 90f)] float maxGroundAngle = 30f;
    float minGroundDotProduct;

    // Start is called before the first frame update
    void Start()
    {
        OnValidate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //ColliderUpdate();
    }

    void ColliderUpdate()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        //Check for initiated collisions and send Collision info to evaluation function
        CheckCollision(collision);
        JUSTCOLLIDED = true;
    }

    void OnCollisionStay(Collision collision)
    {
        //Check for sustained collisions and send Collision info to evaluation function
        CheckCollision(collision);
        COLLIDING = true;
    }

    void CheckCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++) {
			Vector3 normal = collision.GetContact(i).normal;
            Vector3 point = collision.GetContact(i).point;

/*             if(JUSTCOLLIDED)
            {
            GROUNDED = true;
            JUSTCOLLIDED = false;
            }
            else if(COLLIDING)
            {
            GROUNDED = true;
            }
            else {GROUNDED = false;} */
            
            GROUNDED |= normal.y >= minGroundDotProduct;
		}
    }

    void OnValidate ()
    {
		minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }
}
