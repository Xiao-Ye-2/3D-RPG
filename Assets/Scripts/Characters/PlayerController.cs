using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject attackTarget;
    private float lastAttackTime;
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int attackHash = Animator.StringToHash("Attack");

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        MouseManager.Instance.onMouseClickGround += MoveTo;
        MouseManager.Instance.onMouseClickEnemy += EventAttack;
    }

    // void OnDisable()
    // {
    //     MouseManager.Instance.onMouseClickGround -= MoveTo;
    // }

    private void Update()
    {
        SwitchSpeedAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchSpeedAnimation()
    {
        anim.SetFloat(speedHash, agent.velocity.sqrMagnitude);
    }

    private void MoveTo(Vector3 position)
    {
        StopAllCoroutines();
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    private void EventAttack(GameObject target)
    {
        if (target == null) return;
        attackTarget = target;
        StartCoroutine(MoveToAttackTarget());
    }

    private IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        transform.LookAt(attackTarget.transform);

        // TODO: fix range
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > 1)
        {
            agent.SetDestination(attackTarget.transform.position);
            yield return null;
        }

        agent.isStopped = true;
        if (lastAttackTime < 0)
        {
            lastAttackTime = 0.5f;
            anim.SetTrigger(attackHash);
        }
    }
}
