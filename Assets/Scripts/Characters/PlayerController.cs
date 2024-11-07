using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent), typeof(CharacterStats))]
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats stats;
    private GameObject attackTarget;
    private float lastAttackTime;
    private bool isDead;
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int criticalHash = Animator.StringToHash("Critical");
    private readonly int deathHash = Animator.StringToHash("Death");


    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponent<CharacterStats>();
    }

    void Start()
    {
        MouseManager.Instance.onMouseClickGround += MoveTo;
        MouseManager.Instance.onMouseClickEnemy += EventAttack;
        GameManager.Instance.RegisterPlayer(stats);
    }

    // void OnDisable()
    // {
    //     MouseManager.Instance.onMouseClickGround -= MoveTo;
    // }

    private void Update()
    {
        isDead = stats.currentHealth == 0;
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
        if (isDead)
        {
            GameManager.Instance.NotifyEndGameObservers();
        }
    }

    private void SwitchAnimation()
    {
        anim.SetFloat(speedHash, agent.velocity.sqrMagnitude);
        anim.SetBool(deathHash, isDead);
    }

    private void MoveTo(Vector3 position)
    {
        StopAllCoroutines();
        if (isDead) return;
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    private void EventAttack(GameObject target)
    {
        if (isDead) return;
        if (target == null) return;
        attackTarget = target;
        stats.isCritical = UnityEngine.Random.value < stats.attackData.criticalRate;
        StartCoroutine(MoveToAttackTarget());
    }

    private IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        transform.LookAt(attackTarget.transform);

        while (Vector3.Distance(attackTarget.transform.position, transform.position) > stats.attackData.attackRange)
        {
            agent.SetDestination(attackTarget.transform.position);
            yield return null;
        }

        agent.isStopped = true;
        if (lastAttackTime < 0)
        {
            anim.SetBool(criticalHash, stats.isCritical);
            anim.SetTrigger(attackHash);
            lastAttackTime = stats.attackData.coolDown;
        }
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
