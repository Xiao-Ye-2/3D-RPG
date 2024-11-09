using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float knockBackForce = 25f;
    public GameObject rockPrefab;
    public Transform handPos;

    public void KnockBack()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * knockBackForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
            targetStats.TakeDamage(stats);
        }
    }

    public void ThrowRock()
    {
        if (attackTarget == null) attackTarget = FindObjectOfType<PlayerController>().gameObject;
        var rock = Instantiate(rockPrefab, handPos.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        rock.GetComponent<Rock>().target = attackTarget;
    }
}
