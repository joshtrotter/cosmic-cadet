using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CadetController : MonoBehaviour {

    public float normalSpeed = 10f;
    public float secondsToAdjustToNormalSpeed = 0.5f;

    private float currentSpeed;
    private Orbit orbit;
    private bool orbitAllowed;

    private void Start()
    {
        currentSpeed = normalSpeed;
        orbit = null;
        orbitAllowed = true;
    }
	
	void FixedUpdate () {
        if (orbit == null)
        {
            //Lerp towards the target movement speed if not already there
            if (currentSpeed != normalSpeed)
            {
                if (currentSpeed - normalSpeed < 0.1f)
                {
                    currentSpeed = normalSpeed;
                }
                else
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, normalSpeed, Time.deltaTime / secondsToAdjustToNormalSpeed);
                }
            }

            transform.position += transform.up * currentSpeed * Time.deltaTime;
        } 	
	}

    void Update()
    {
        //Lock our cadet in his two dimensional prison - this just autocorrects minor rounding errors that nudge the cadet into the z axis
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        
        if (Input.anyKeyDown)
        {
            if (orbit != null)
            {
                currentSpeed = orbit.CurrentMoveSpeed();
                orbit.transform.DetachChildren();
                orbit = null;
                orbitAllowed = true;     
            }
        }
    }

    public IEnumerator Orbit(Orbit orbit, Transform orbitalEntryPoint)
    {
        //Prevent any other orbits from being triggered
        orbitAllowed = false;

        //First move into the orbital path of the planet
        yield return StartCoroutine(MoveToOrbitalEntryPoint(orbitalEntryPoint));

        //Then follow the orbital path
        FollowOrbitalPath(orbit);

        //Cleanup the orbitalEntryPoint object which is no longer needed
        Destroy(orbitalEntryPoint.gameObject);
    }

    private IEnumerator MoveToOrbitalEntryPoint(Transform orbitalEntryPoint)
    {      
        //Calculate the time it should take us to reach the entry point based on our current speed
        float moveTime = TimeToOrbitalEntry(orbitalEntryPoint);
        //Track the time we've taken so far
        float time = 0f;

        //Store where we started from
        Vector3 startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Quaternion startRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);

        //Lerp between where we started and the desired entry point over the total moveTime
        while (time < moveTime)
        {
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;

            transform.position = Vector3.Lerp(startPosition, orbitalEntryPoint.position, time / moveTime);
            transform.rotation = Quaternion.Lerp(startRotation, orbitalEntryPoint.rotation, time / moveTime);
        }

        //Make sure we finish at exactly the desired entry point
        transform.position = orbitalEntryPoint.position;
        transform.rotation = orbitalEntryPoint.rotation;
    }

    private float TimeToOrbitalEntry(Transform orbitalEntryPoint)
    {
        float distanceToEntry = (orbitalEntryPoint.position - transform.position).magnitude;
        return distanceToEntry / currentSpeed;
    }

    private void FollowOrbitalPath(Orbit orbit)
    {
        transform.SetParent(orbit.transform);
        this.orbit = orbit;
    }

    public float CurrentSpeed()
    {
        return currentSpeed;
    }

    //Orbiting is only allowed if we are not moving towards, or following an orbital path already
    public bool IsOrbitAllowed()
    {
        return orbitAllowed;
    }

    public bool IsOrbitting(Orbit orbit)
    {
        return orbit.Equals(this.orbit);
    }
}
