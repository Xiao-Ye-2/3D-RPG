using System;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates
    {
        HitPlayer,
        HitEnemy,
        HitNothing
    }
    public RockStates rockStates;
    public GameObject breakEffect;
    private Rigidbody rb;
    [Header("Basic Settings")]
    public int damage;
    public float force;
    public GameObject target;
    private Vector3 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rockStates = RockStates.HitPlayer;
    }

    private void Start()
    {
        flyToTarget();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }

    private void flyToTarget()
    {
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision other)
    {
        var otherStats = other.gameObject.GetComponent<CharacterStats>();
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    otherStats.TakeDamage(damage, otherStats);
                    rockStates = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            case RockStates.HitNothing:
                break;
        }
    }
}
