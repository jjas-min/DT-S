// MoveDestination.cs
using UnityEngine;
using UnityEngine.AI;

public class MoveToTargetObject : MonoBehaviour
{

    public Transform goal;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        agent.destination = goal.position;
    }
}