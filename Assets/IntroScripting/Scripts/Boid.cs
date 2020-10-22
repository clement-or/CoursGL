using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Boid : MonoBehaviour
{
    // Settings
    public BoidSettings boidSettings;
    public BoidDebug debug;

    // Public variables
    public Vector3 velocity
    {
        set
        {
            curSpeed = value.magnitude;
            targetDirection = value;
        }
        get { return rb.velocity; }
    }

    // Working variables
    private float curSpeed = 0;
    private Vector3 targetDirection = Vector3.forward;
    private Quaternion targetRotation;
    private List<Boid> boidsInRange = new List<Boid>();

    private int boidCount = 0;

    // Components
    private Rigidbody rb;
    private SphereCollider boidDetectionArea;
    private Animator anim;

    private enum BoidArea
    {
        Attraction,
        Alignment,
        Repulsion
    }

    #region Monobehaviour
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Set the boid detection area
        boidDetectionArea = GetComponent<SphereCollider>();
        
        // Set the animation
        anim = GetComponentInChildren<Animator>();
        if (anim != null)
            anim.Play("fly");
    }
    
    private void Update()
    {
        // Boid controls, we implement the three rules
        foreach (Boid boid in boidsInRange)
        {
            // We don't care about boids who are behind
            Vector3 vecToBoid = boid.transform.position - transform.position;
            if (Vector3.Dot(transform.forward, vecToBoid) < 0)
            {
                //DrawArrow.ForDebug(transform.position, vecToBoid, Color.red);
                continue;
            }

            boidCount++;

            if (IsInArea(boid, BoidArea.Attraction))
            {
                Attraction(vecToBoid);
                DrawDebug(vecToBoid, Color.green);
            } else if (IsInArea(boid, BoidArea.Alignment))
            {
                Alignment(boid);
                DrawDebug(vecToBoid, Color.cyan);
            } else if (IsInArea(boid, BoidArea.Repulsion))
            {
                Repulsion(vecToBoid);
                DrawDebug(vecToBoid, Color.red);
            }
        }

        //DrawArrow.ForDebug(transform.position, targetDirection, Color.red);
        
        // Move
        curSpeed = Mathf.Lerp(curSpeed, boidSettings.maxSpeed, .1f);
        rb.velocity = transform.forward * curSpeed;

        // Apply Steering
        targetDirection /= boidCount;
        if (targetDirection == Vector3.zero) return;
        
        // Lerp the rotation to the desired one
        targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .1f);

        // Check if the boid has reached the direction
        var diff = (targetDirection - transform.forward).magnitude;
        if (diff < 1 && diff > -1)
        {
            targetDirection = Vector3.zero;
        }
    }
    
    #endregion


    #region Helpers

    private void Attraction(Vector3 vecToOther)
    {
        Vector3 force = vecToOther.normalized * boidSettings.attractionForce;
        targetDirection += force;
        //Debug.Log("Attraction : " + boidSettings.attractionForce);
    }

    private void Alignment(Boid other)
    {
        Vector3 force = other.transform.forward * boidSettings.alignmentForce;
        targetDirection += force;
        //Debug.Log("Alignment : " + boidSettings.alignmentForce);
    }

    private void Repulsion(Vector3 vecToOther)
    {
        Vector3 force = - vecToOther.normalized * boidSettings.repulsionForce;
        targetDirection += force;
        //Debug.Log("Repulsion : " + boidSettings.repulsionForce);
    }

    private void DrawDebug(Vector3 vecToOther, Color color)
    {
        if (!debug.displayDebug)
            return;
        
        // Debug
        if (Selection.gameObjects.Length > 0)
            if (!gameObject.Equals(Selection.gameObjects[0]))
            {
                DrawArrow.ForDebug(transform.position, vecToOther, color);
                Debug.Log(transform.forward);
            }
    }

    #endregion
    
    private bool IsInArea(Boid other, BoidArea area)
    {
        float minDistance, maxDistance;

        switch (area)
        {
            case BoidArea.Attraction:
                minDistance = boidSettings.alignmentDistance;
                maxDistance = boidSettings.attractionDistance;
                break;
            case BoidArea.Alignment:
                minDistance = boidSettings.repulsionDistance;
                maxDistance = boidSettings.alignmentDistance;
                break;
            case BoidArea.Repulsion:
                minDistance = 0;
                maxDistance = boidSettings.repulsionDistance;
                break;
            default:
                throw new Exception("Unknown area : " + area);
        }

        var distanceToOther = Vector3.Distance(transform.position, other.transform.position);
        return distanceToOther > minDistance && distanceToOther <= maxDistance;
    }
    
    #region Events
    
    private void OnTriggerEnter(Collider other)
    {
        var boid = other.GetComponent<Boid>();
        if (boid != null)
            if (!boidsInRange.Contains(boid))
                boidsInRange.Add(boid);
    }

    private void OnTriggerExit(Collider other)
    {
        var boid = other.GetComponent<Boid>();
        if (boid != null) 
            if (boidsInRange.Contains(boid))
                boidsInRange.Remove(boid);
    }
    
    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        DrawArrow.ForGizmo(transform.position, transform.forward * debug.directionArrowLength, debug.directionArrowColor);

        var sphere = GetComponent<SphereCollider>();
        if (sphere == null) return;
        
        if (sphere.radius.Equals(boidSettings.attractionDistance))
            sphere.radius = boidSettings.attractionDistance;
    }
    
    private void OnDrawGizmosSelected()
    {
        var pos = transform.position;
        Gizmos.color = debug.repulsionDistanceColor;
        Gizmos.DrawWireSphere(pos, boidSettings.repulsionDistance);        
        Gizmos.color = debug.alignmentDistanceColor;
        Gizmos.DrawWireSphere(pos, boidSettings.alignmentDistance);
        Gizmos.color = debug.attractionDistanceColor;
        Gizmos.DrawWireSphere(pos, boidSettings.attractionDistance);
        
        foreach (var boid in boidsInRange)
        {
            Gizmos.color = Color.yellow;
            var dir = boid.transform.position - transform.position;
            //DrawArrow.ForGizmo(transform.position, dir);
        }
    }
    
    #endregion
}


#region Classes additionnelles

[Serializable]
public class BoidSettings
{
    [Header("Règles des boids")] 
    public float repulsionDistance = 5;
    public float alignmentDistance = 9;
    public float attractionDistance = 50;

    public float repulsionForce = 15;
    public float alignmentForce = 3;
    public float attractionForce = 20;

    [Header("Variables")] 
    public float maxSpeed = 10f;
    [Tooltip("Degrés par seconde")] public float steerSpeed = 1;
}

[Serializable]
public class BoidDebug
{
    public bool displayDebug;

    public float directionArrowLength = 2f;
    public Color directionArrowColor = Color.cyan;

    [Header("Zones")] 
    public Color repulsionDistanceColor = Color.red;
    public Color alignmentDistanceColor = Color.blue;
    public Color attractionDistanceColor = Color.green;
}

#endregion