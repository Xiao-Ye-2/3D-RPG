using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float knockBackForce = 15f;

    public void KnockBack()
    {
        if (attackTarget == null) return;
        transform.LookAt(attackTarget.transform);

        Vector3 direction = attackTarget.transform.position - transform.position;
        direction.Normalize();
        attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
        attackTarget.GetComponent<NavMeshAgent>().velocity = direction * knockBackForce;
        attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
    }
}
