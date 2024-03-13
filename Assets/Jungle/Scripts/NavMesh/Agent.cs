using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    public NavMeshAgent NavMeshAgent;
    public GameObject Target;
    
    // Start is called before the first frame update
    void Start()
    {
        NavMeshAgent.SetDestination(Target.transform.position);
    }
}
