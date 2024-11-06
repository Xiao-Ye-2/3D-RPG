using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public enum EnemyState
{
    GUARD,
    PATROL,
    CHASE,
    DEAD
}

[RequireComponent(typeof(NavMeshAgent), typeof(CharacterStats))]
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;
    private EnemyState enemyState;
    private Animator anim;
    private Collider coll;
    private CharacterStats stats;
    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    public float pauseInterval;
    private float remainPauseTime;
    private float speed;
    private GameObject attackTarget;
    private float lastAttackTime;
    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;
    private Quaternion guardRotation;

    // Enemy States
    private bool isChase;
    private bool isWalk;
    private bool isFollow;
    private bool isDead;
    private readonly int chaseHash = Animator.StringToHash("Chase");
    private readonly int walkHash = Animator.StringToHash("Walk");
    private readonly int followHash = Animator.StringToHash("Follow");
    private readonly int deathHash = Animator.StringToHash("Death");
    private readonly int criticalHash = Animator.StringToHash("Critical");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        stats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainPauseTime = pauseInterval;
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyState = EnemyState.GUARD;
        } else
        {
            enemyState = EnemyState.PATROL;
            GetNewWayPoint();
        }
    }

    private void Update()
    {
        isDead = stats.currentHealth == 0;
        SwitchStates();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetBool(chaseHash, isChase);
        anim.SetBool(walkHash, isWalk);
        anim.SetBool(followHash, isFollow);
        anim.SetBool(criticalHash, stats.isCritical);
        anim.SetBool(deathHash, isDead);
    }

    private void SwitchStates()
    {
        if (isDead)
        {
            enemyState = EnemyState.DEAD;
        } else if (FoundPlayer())
        {
            enemyState = EnemyState.CHASE;
        }

        switch (enemyState)
        {
            case EnemyState.GUARD:
                isChase = false;
                if (Vector3.Distance(guardPos, transform.position) > agent.stoppingDistance)
                {
                    isWalk = true;
                    agent.destination = guardPos;
                } else
                {
                    isWalk = false;
                    transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                }
                break;
            case EnemyState.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;

                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainPauseTime > 0)
                    {
                        remainPauseTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();
                    }
                } else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyState.CHASE:
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FoundPlayer())
                {
                    isFollow = false;
                    if (remainPauseTime > 0)
                    {
                        agent.destination = transform.position;
                        remainPauseTime -= Time.deltaTime;
                    } else if (isGuard)
                    {
                        enemyState = EnemyState.GUARD;
                        remainPauseTime = pauseInterval;
                    } else
                    {
                        enemyState = EnemyState.PATROL;
                    }
                } else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }

                if (TargetInRange(stats.attackData.attackRange) || TargetInRange(stats.attackData.skillRange))
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = stats.attackData.coolDown;

                        stats.isCritical = UnityEngine.Random.value < stats.attackData.criticalRate;
                        Attack();
                    }
                }
                break;
            case EnemyState.DEAD:
                coll.enabled = false;
                agent.enabled = false;
                Destroy(gameObject, 2f);
                break;
        }
    }

    private void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInRange(stats.attackData.attackRange))
        {
            anim.SetTrigger("Attack");
        } else if (TargetInRange(stats.attackData.skillRange))
        {
            anim.SetTrigger("Skill");
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

    private bool TargetInRange(float distance)
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= distance;
        }
        return false;
    }

    private void GetNewWayPoint()
    {
        float randomX = UnityEngine.Random.Range(-patrolRange, patrolRange);
        float randomZ = UnityEngine.Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
        remainPauseTime = pauseInterval;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    private void Hit()
    {
        if (attackTarget != null)
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(stats, targetStats);
        }
    }
}
