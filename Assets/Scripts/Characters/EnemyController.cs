using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    GUARD,
    PATROL,
    CHASE,
    DEAD
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemyState enemyState;
    private Animator anim;
    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    private float speed;
    private GameObject attackTarget;
    [Header("Patrol State")]
    public float patrolRange;


    // Enemy States
    private bool isChase;
    private bool isWalk;
    private bool isFollow;
    private int chaseHash = Animator.StringToHash("Chase");
    private int walkHash = Animator.StringToHash("Walk");
    private int followHash = Animator.StringToHash("Follow");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed;
    }

    private void Update()
    {
        SwitchStates();
        SwitchAnimation();
    }

    private void SwitchAnimation()
    {
        anim.SetBool(chaseHash, isChase);
        anim.SetBool(walkHash, isWalk);
        anim.SetBool(followHash, isFollow);
    }

    private void SwitchStates()
    {
        if (FoundPlayer())
        {
            enemyState = EnemyState.CHASE;
        }
        switch (enemyState)
        {
            case EnemyState.GUARD:
                break;
            case EnemyState.PATROL:
                break;
            case EnemyState.CHASE:
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FoundPlayer())
                {
                    isFollow = false;
                    agent.destination = transform.position;
                } else
                {
                    isFollow = true;
                    agent.destination = attackTarget.transform.position;
                }
                break;
            case EnemyState.DEAD:
                break;
        }
    }

    private bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;
    }
}
