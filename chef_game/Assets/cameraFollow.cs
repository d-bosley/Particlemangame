using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
	[SerializeField] Transform focus = default;
	[SerializeField] Rigidbody characterRigidbody;
	[SerializeField, Range(1f, 20f)] float distance = 5f;
    [SerializeField, Min(0f)] float focusRadius = 1f;
    [SerializeField, Range(1f, 360f)] float maxRotationSpeed = 90f;
	[SerializeField, Range(1f, 360f)] float rotationSpeed = 90f;
    [SerializeField, Range(-89f, 89f)] float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    [SerializeField, Min(0f)] float alignDelay = 5f;
    [SerializeField, Range(0f, 90f)] float alignSmoothRange = 45f;

    float lastManualRotationTime;
	public BasicPhysics characterScript;
	public AnimationCurve rotationCurve;
    public float animationDuration = 2f;
	private float startTime;
    Vector3 focusPoint, previousFocusPoint;
	Vector3 focusForward;
    Vector2 orbitAngles = new Vector2(45f, 0f);

	void Awake () {
		focusPoint = focus.position;
		focusForward = focus.forward;
        transform.localRotation = Quaternion.Euler(orbitAngles);
	}

	void LateUpdate () {
		//Vector3 focusPoint = focus.position;
		UpdateFocusPoint();
        Quaternion lookRotation;

		if (ManualRotation() || AutomaticRotation()) {
			ConstrainAngles();
			lookRotation = Quaternion.Euler(orbitAngles);
		}
		else {
			lookRotation = transform.localRotation;
		}
		Vector3 lookDirection = lookRotation * Vector3.forward;
		Vector3 lookPosition = focusPoint - lookDirection * distance;
		transform.SetPositionAndRotation(lookPosition, lookRotation);
	}

	void UpdateFocusPoint () {
        previousFocusPoint = focusPoint;
		Vector3 targetPoint = focus.position;
        if (focusRadius > 0f) {
			float distance = Vector3.Distance(targetPoint, focusPoint);
			if (distance > focusRadius) {
				    focusPoint = Vector3.Lerp(
					targetPoint, focusPoint, focusRadius / distance
				);
			}
		}
		else {
			focusPoint = targetPoint;
		}
    }

    void ConstrainAngles () {
		orbitAngles.x =
			Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

		if (orbitAngles.y < 0f) {
			orbitAngles.y += 360f;
		}
		else if (orbitAngles.y >= 360f) {
			orbitAngles.y -= 360f;
		}
	}

    bool ManualRotation () {
		Vector2 input = new Vector2(
			Input.GetAxis("Vertical Camera"),
			Input.GetAxis("Horizontal Camera")
		);
		const float e = 0.001f;
        if (input.magnitude > 0.001f) {
			orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            lastManualRotationTime = Time.unscaledTime;
			return true;
		}
		return false;
	}

    bool AutomaticRotation () {
    // Check if the player is manually rotating the camera
    Vector2 input = new Vector2(
        Input.GetAxis("Vertical Camera"),
        Input.GetAxis("Horizontal Camera")
    );

    if (input.magnitude > 0.001f)
    {
        lastManualRotationTime = Time.unscaledTime;
        return false;
    }

    // Check if the character is moving at max speed
    float characterSpeed = characterRigidbody.velocity.magnitude;
    float maxCameraSpeed = .5f; //The speed at which the auto cam is active

    if (characterSpeed < maxCameraSpeed)
    {
        lastManualRotationTime = Time.unscaledTime;
        return false;
    }

	if (characterScript.isGrounded != true)
	{
		lastManualRotationTime = Time.unscaledTime;
        return false;
	}

	// Get the character's velocity and normalize it
    Vector3 characterVelocity = characterRigidbody.velocity.normalized;

    // Calculate the heading angle based on the XZ components of the character's velocity
    float headingAngle = GetAngle(new Vector2(characterVelocity.x, characterVelocity.z));

    // Calculate the rotationChange based on the difference between the camera's current orbit and the heading angle
    float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
    float rotationChange =  rotationSpeed * Time.unscaledDeltaTime;
    if (deltaAbs < alignSmoothRange)
    {
        rotationChange *= deltaAbs / alignSmoothRange;
    }
    else if (180f - deltaAbs < alignSmoothRange)
    {
        rotationChange *= (180f - deltaAbs) / alignSmoothRange;
    }
    orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
    return true;
	}

    static float GetAngle (Vector2 direction)
	{
		float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
		return direction.x < 0f ? 360f - angle : angle;
	}

    void OnValidate () {
		if (maxVerticalAngle < minVerticalAngle) {
			maxVerticalAngle = minVerticalAngle;
		}
	}
}