using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderInfo : MonoBehaviour
{
    public ContactPoint contact;
    public Vector3 contactPosition;
    public RaycastHit hit;
    public Vector3 playerRotation;
    public bool onGround;

    public void OnCollisionEnter(Collision collision)
    {
            if (collision.gameObject.CompareTag("Ground"))
            {
            onGround = true;
            Debug.Log("Hitting Ground");
            }
            //contact = collision.contacts[0];
            //contactPosition = contact.point;
    }

    public void GetGroundAngle()
    {
        LayerMask ground = LayerMask.GetMask("Ground");

        if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity, ground))
        {
            //Debug.DrawRay(transform.position, -transform.up * hit.distance, Color.green, 1);
            Debug.Log("Up Vector: " + playerRotation);
            playerRotation = hit.normal.normalized;

            //return true;
        }
        else
        {
            //Debug.DrawRay(transform.position, -transform.up * Mathf.Infinity, Color.blue, 1);
            Debug.Log("Up Vector: " + playerRotation);
            playerRotation = new Vector3 (0, 1, 0);

            //return false;
        }
    }
}
