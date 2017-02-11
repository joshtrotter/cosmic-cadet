using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Orbit : MonoBehaviour {

    public CadetController cadet;
    public SphereCollider orbitalPath;
    public float secondsToAdjustToTargetRotations = 1f;

    private float targetSecondsPerRotation = 1f;
    private float currentSecondsPerRotation;
    private int currentOrbitEntryAngle;
    private Vector3 currentOrbitRotation;
    private float orbitalPathCircumference;

	// Use this for initialization
	void Start () {
        orbitalPath.gameObject.SetActive(false);
        currentSecondsPerRotation = targetSecondsPerRotation;
        currentOrbitRotation = Vector3.zero;
        orbitalPathCircumference = 2 * Mathf.PI * orbitalPath.radius * orbitalPath.transform.lossyScale.x;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && cadet.IsOrbitAllowed())
        {
            //Activate the orbital path so we can check if the cadet will intersect it
            orbitalPath.gameObject.SetActive(true);
            //Set the initial orbit speed to match the players current speed
            currentSecondsPerRotation = orbitalPathCircumference / cadet.CurrentSpeed();
            //Set whether the orbit should be clockwise or anticlockwise
            currentOrbitEntryAngle = CalculateOrbitDirection();
            UpdateCurrentOrbitRotation();
            //If the cadet is moving to intercept with the orbital path we will smooth the angle of entry
            SmoothEntryToOrbitalPath(currentOrbitEntryAngle);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Deactivate this planets orbital path so it can't be accidentally hit from another planets orbit
            orbitalPath.gameObject.SetActive(false);
        }
    }

    private void UpdateCurrentOrbitRotation()
    {
        currentOrbitRotation.z = (360f / currentSecondsPerRotation) * currentOrbitEntryAngle;
    }

    void Update () {
        if (cadet.IsOrbitting(this))
        {
            //Lerp towards the target orbit speed if not already there
            if (currentSecondsPerRotation != targetSecondsPerRotation)
            {
                if (Mathf.Abs(currentSecondsPerRotation - targetSecondsPerRotation) < 0.1f)
                {
                    currentSecondsPerRotation = targetSecondsPerRotation;
                }
                else
                {
                    currentSecondsPerRotation = Mathf.Lerp(currentSecondsPerRotation, targetSecondsPerRotation, Time.deltaTime / secondsToAdjustToTargetRotations);                   
                }
                UpdateCurrentOrbitRotation();
            }

            //Rotate the orbit object which should have the cadet attached as a child
            transform.Rotate(currentOrbitRotation * Time.deltaTime);
        }
	}

    //Calculates whether the player should enter orbit in a clockwise or anticlockwise direction 
    private int CalculateOrbitDirection()
    {
        Vector3 currentPath = cadet.transform.up;
        Vector3 pathToPlanetCentre =  transform.position - cadet.transform.position;

        //Get the angle between the path to the planet and the players current path. We will enter orbit in the direction that gives the smoothest change in direction. 
        float orbitEntryAngle = Mathf.Atan2(
            Vector3.Dot(Vector3.forward, Vector3.Cross(pathToPlanetCentre, currentPath)),
            Vector3.Dot(pathToPlanetCentre, currentPath)) * Mathf.Rad2Deg;

        return orbitEntryAngle >= 0f ? -1 : 1;
    }

    private void SmoothEntryToOrbitalPath(int orbitEntryAngle)
    {
        RaycastHit hit;
        if (CalculateOrbitInterceptPoint(out hit))
        {
            //Create a new transform at the current orbit intercept point
            Transform orbitHitPoint = new GameObject().transform;
            orbitHitPoint.position = hit.point;
            orbitHitPoint.rotation = cadet.transform.rotation;

            //Move the transform further along the orbital path, this will allow us to lerp smoothly into the path from our current position
            orbitHitPoint.RotateAround(transform.position, Vector3.forward, 45f * orbitEntryAngle);
            //Rotate the transform to look at the planet centre, this will allow us to lerp the look direction of the cadet
            orbitHitPoint.LookAt(transform.position, orbitHitPoint.up);
            //Offset rounding errors that creep in from the use of the LookAt function to lock the z axis
            orbitHitPoint.rotation = FixOrbitHitPointRotation(orbitHitPoint.rotation);
            //Start lerping the cadet to the desired transform position and rotation
            StartCoroutine(cadet.Orbit(this, orbitHitPoint));
        }
    }

    private bool CalculateOrbitInterceptPoint(out RaycastHit hit)
    {
        //Should only hit the orbital path collider to check where the cadet would intercept it
        int orbitMask = 1 << 8;
        return (Physics.Raycast(cadet.transform.position, cadet.transform.up, out hit, 50f, orbitMask));
    }

    //The Z rotation of the orbit hit point needs to be locked at 0 or 180
    private Quaternion FixOrbitHitPointRotation(Quaternion rotation)
    {
        float zRotationLock = rotation.eulerAngles.z;
        zRotationLock = Mathf.Abs(zRotationLock) > 90f && Mathf.Abs(zRotationLock) < 270f ? 180f : 0f;
        Debug.Log(rotation.eulerAngles.z + " TO " + zRotationLock);
        return Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, zRotationLock);
    }

    //Return the current movement speed of the cadet orbiting this planet
    public float CurrentMoveSpeed()
    {
        return orbitalPathCircumference / currentSecondsPerRotation;
    }

    public void SetTargetRotationsPerSecond(float newTarget)
    {
        targetSecondsPerRotation = newTarget;
    }

}
