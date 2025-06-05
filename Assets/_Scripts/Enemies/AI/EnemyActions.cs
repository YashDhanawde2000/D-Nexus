using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyActions : MonoBehaviour
{
    public Transform playerTransform;

    [Header("Follow Player")]
    public float maxFollowTargetInterval = 1f;
    public float targetMaxDistanceFromDestination = 1f;

    [Header("Combat")]
    public float attackCD = 3f;
    public float attackRange = 2f;

    [Header("Audio")]
    public AudioClip enemyFootstepAudioClip;
    [Range(0f, 1f)]
    public float enemyFootstepAudioVolume = 1f;

    private NavMeshAgent enemyAgent;
    private Animator animator;
    float followTickTimer;
    float attackTicktimer;

    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        enemyAgent.stoppingDistance = attackRange;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", enemyAgent.velocity.magnitude);

        EnemyFollowTick();
        AttackPlayerTick();

        
        
    }

    private void EnemyFollowTick()
    {
        if (playerTransform != null)
        { 

            if (followTickTimer < 0)
            {
                float distanceFromTarget = (playerTransform.position - enemyAgent.destination).sqrMagnitude;
                if (distanceFromTarget > targetMaxDistanceFromDestination * targetMaxDistanceFromDestination)
                {
                    enemyAgent.SetDestination(playerTransform.position);
                }
                followTickTimer = maxFollowTargetInterval;
            }

            followTickTimer -= Time.deltaTime;

        }
        
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        transform.LookAt(playerTransform);

    }

    public void AttackPlayerTick()
    {
        if ( playerTransform != null )
        {
            if (attackTicktimer >= attackCD)
            {
                if (Vector3.Distance(playerTransform.position, transform.position) <= attackRange)
                {
                    animator.SetTrigger("Attack");
                    attackTicktimer = 0;
                }
            }

            attackTicktimer += Time.deltaTime;
        }
    }


    public void StartDealDamage()
    {
        GetComponentInChildren<EnemyDamageDealer>().StartDealDamage();

    }
    public void EndDealDamage()
    {
        GetComponentInChildren<EnemyDamageDealer>().EndDealDamage();

    }


    public void OnFootstep(AnimationEvent animationEvent)
    {
        AudioSource.PlayClipAtPoint(enemyFootstepAudioClip, transform.position, enemyFootstepAudioVolume);
    }

}
