using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    public Transform[] waypointsArray;
    
    private NavMeshAgent navMeshAgent;
    private int currentWaypointIndex;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.SetDestination(waypointsArray[0].position);
        
    }

    private void Update()
    {
        if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypointsArray.Length;
            navMeshAgent.SetDestination(waypointsArray[currentWaypointIndex].position);
            //Debug.Log(waypointsArray[currentWaypointIndex].name);
        }
    }
}
