using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XL_Swarmers : XL_Enemy
{
    [SerializeField] private GameObject model;
    protected Material shader;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackWidth;
    [SerializeField] private float attackAnimationSpeed;
    private GameObject attackParticles;
    private bool attacking;
    private List<GameObject> playersHit = new List<GameObject>();

    [SerializeField] private int nbRaycast;

    [Header("DeathVFX")]
    [SerializeField] private GameObject deathVFX;

    private void Awake()
    {
        shader = model.GetComponent<MeshRenderer>().material;
    }

    protected override void Initialize()
    {
        base.Initialize();
        attacking = false;
        shader.SetFloat("_AtkSldr", 0);
    }

    protected override void Update()
    {
        base.Update();
        Move();
        DebugRaycast();
        if (attacking) shader.SetFloat("_AtkFloat", (Time.time - atkStartingTime) / attackAnimationSpeed + 0.5f);
    }

    private void DebugRaycast()
    {
        for (int i = 0; i < nbRaycast + 1; i++)
        {
            Debug.DrawRay(transform.position - transform.right * 0.5f + (transform.right / nbRaycast) * i + transform.up * 0.5f, attackRange * transform.forward * 1.2f, Color.red);
        }
    }

    public override void Die()
    {
        base.Die();
        KLD_EventsManager.instance.InvokeEnemyKill(Enemy.SWARMER);

        deathVFX = XL_Pooler.instance.PopPosition("Swarmer_DeathVFX", transform.position);
        XL_Pooler.instance.DelayedDePop(1.5f, "Swarmer_DeathVFX", deathVFX);

        XL_Pooler.instance.DePop("Swarmer", transform.gameObject);
        StopAllCoroutines();
    }

    public override void Alert()
    {
        StartCoroutine(FindTargetedPlayerCoroutine(targetedPlayerUpdateRate));
        isAlerted = true;
    }

    public override void Move()
    {
        if (isAlerted)
        {
            if (targetedPlayer != null)
            {
                agent.destination = targetedPlayer.position;
                if ((transform.position - targetedPlayer.position).magnitude < attackRange)
                {
                    agent.isStopped = true;
                    if (canAttack) Attack();
                }
            }
        }

    }

    public override void Attack()
    {
        selfAudioManager.PlaySound("Attack");
        //Debug.Log("Attacking");
        StartCoroutine(AttackCoroutine(attackAnimationSpeed));
        StartCoroutine(AttackCooldownCoroutine(attackAnimationSpeed));
    }

    float atkStartingTime;
    private RaycastHit[] hits;
    IEnumerator AttackCoroutine(float t)
    {
        attacking = true;
        transform.LookAt(targetedPlayer);
        playersHit.Clear();
        atkStartingTime = Time.time;
        shader.SetFloat("_AtkSldr", 1);
        yield return new WaitForSeconds(t);



        for (int i = 0; i < nbRaycast + 1; i++)
        {
            hits = Physics.RaycastAll(transform.position - transform.right * 0.5f + (transform.right / nbRaycast) * i + transform.up * 0.5f, attackRange * transform.forward * 1.2f); //Can add a layer but I didn't make it work
            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].transform.CompareTag("Player") && !playersHit.Contains(hits[j].transform.gameObject))
                {
                    attackParticles = XL_Pooler.instance.PopPosition("SwarmerAttackVFX", hits[j].transform.position, hits[j].transform);
                    XL_Pooler.instance.DelayedDePop(2, "SwarmerAttackVFX", attackParticles);
                    attackParticles.GetComponentInChildren<ParticleSystem>().Play();
                    playersHit.Add(hits[j].transform.gameObject);
                    if (hits[j].transform != null) hits[j].transform.GetComponent<XL_IDamageable>().TakeDamage(damage); //PERF
                }
            }
        }
        attacking = false;

        shader.SetFloat("_AtkSldr", 0);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("Player was attacked");
        }
    }
}
