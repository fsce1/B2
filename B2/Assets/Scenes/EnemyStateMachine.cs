using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public enum EnemyState
{
    Idle,
    Attack,
    Move
}
public class EnemyStateMachine : MonoBehaviour
{
    [Header("Setup")]
    public bool isInitialised = false;
    public NavMeshAgent agent;
    public NavMeshSurface surface;
    public Animator anim;
    public GameObject ragdoll;

    [Header("Positions")]
    public Transform lookPos;
    public Transform eyePos;
    public Transform firearmPos;
    [Header("Enemy")]
    public EnemyState state = EnemyState.Idle;
    public int health = 100;
    public float viewCone;
    public List<EnemyStateMachine> nearbyTeam;

    public Vector2 reactionTime;
    public float curReactionTime;
    public Vector2 cooldown;

    [Header("Gun")]
    public GameObject bulletPrefab;

    public int magSize = 30;
    public int roundsInMag = 30;
    public float reloadTime = 3;

    public bool canShoot;
    public bool onCooldown;
    public bool isReloading;
    public int roundsLeftInBurst;

    [Header("Decision")]
    public float surprise;
    [Header("Audio")]
    public AudioSource source;
    public List<AudioClip> shotSounds;


    [Header("Player")]
    public bool canSeePlayer;
    public bool canHearPlayer;
    public Vector3 lastPositionPlayerSpotted;
    public float timeSincePlayerSeen;
    public float distFromPlayer;

    [Header("Debug")]
    public TMP_Text text;

    private void Start()
    {
        agent.SetDestination(transform.position);
        surface = FindFirstObjectByType<NavMeshSurface>();
    }
    public void Initialize() => isInitialised = true;
    public void FixedUpdate()
    {
        if (!isInitialised) return;


        distFromPlayer = (lastPositionPlayerSpotted - transform.position).magnitude;
        distFromPlayer = Mathf.InverseLerp(0, 150, distFromPlayer);


        nearbyTeam.Clear();
        Collider[] nearby = Physics.OverlapSphere(transform.position, 15);
        foreach (Collider collider in nearby)
        {
            EnemyStateMachine e = collider.GetComponent<EnemyStateMachine>();
            if (e != null) nearbyTeam.Add(e);
        }
        Vector3 playerEyePos =  GameManager.GM.player.transform.position;
        playerEyePos.y += 1.75f;
        if (Physics.Raycast(eyePos.position, playerEyePos - eyePos.position, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag("Player") && Vector3.Angle(lookPos.forward, GameManager.GM.player.transform.position - lookPos.position) < viewCone)
            {
                canSeePlayer = true;
                lastPositionPlayerSpotted = playerEyePos;
                timeSincePlayerSeen += Time.fixedDeltaTime;
            }
            else
            {
                canSeePlayer = false;
                timeSincePlayerSeen = -Time.fixedDeltaTime;
            }
        }
        lookPos.forward = Vector3.RotateTowards(lookPos.forward, playerEyePos- lookPos.position, 25, 25);
        //lookPos.LookAt(lastPositionPlayerSpotted);
        lookPos.transform.localEulerAngles = new(0, lookPos.transform.localEulerAngles.y, 0);

        if (text != null)
        {
            text.text = gameObject.name + '\n' + health + '\n' + state.ToString() + '\n' + "Sees Player" + canSeePlayer + '\n' + roundsInMag + "/" + magSize;
        }

        switch (state)
        {
            case EnemyState.Idle: UpdateIdle(); break;
            case EnemyState.Attack: UpdateAttack(); break;
            case EnemyState.Move: UpdateMove(); break;
        }
    }

    void EnterIdle()
    {
        state = EnemyState.Idle;
        anim.Play("BaseLayer.demo_combat_idle");
    }
    void UpdateIdle()
    {
        if (canSeePlayer) ExitIdle(EnemyState.Attack);
    }
    void ExitIdle(EnemyState nextState)
    {
        switch (nextState)
        {
            case EnemyState.Attack: EnterAttack(); break;
            case EnemyState.Move: EnterMove(); break;
        }
    }
    void EnterAttack()
    {
        state = EnemyState.Attack;

        curReactionTime = Mathf.Lerp(reactionTime.x, reactionTime.y, distFromPlayer);
        Debug.Log("curReactionTime " + curReactionTime);
        //curReactionTime = Random.Range(reactionTime.x, reactionTime.y);
        onCooldown = true;
        CooldownShot();
        //roundsLeftInBurst = 1;

        ResetShot();
        foreach (EnemyStateMachine e in nearbyTeam)
        {
            e.lastPositionPlayerSpotted = lastPositionPlayerSpotted;
        }
    }
    void UpdateAttack()
    {

        if (canSeePlayer)
        {
            firearmPos.LookAt(lastPositionPlayerSpotted);
            firearmPos.localEulerAngles += Random.Range(-0.25f, 0.25f) * transform.up;
            firearmPos.localEulerAngles += Random.Range(-0.25f, 0.25f) * transform.right;

            //if (timeSincePlayerSeen == 0)
            //{
            //    Debug.Log("Setting Reaction Time");
            //    curReactionTime = Mathf.Lerp(reactionTime.x, reactionTime.y, distFromPlayer);
            //    //Debug.Log("curReactionTime " + curReactionTime);
            //    //curReactionTime = Random.Range(reactionTime.x, reactionTime.y);
            //    onCooldown = true;
            //    CooldownShot();
            //    //roundsLeftInBurst = 1;
            //    ResetShot();
            //}
            if (!onCooldown && roundsLeftInBurst <= 0)
            {
                onCooldown = true;
                Invoke(nameof(CooldownShot), Random.Range(cooldown.x, cooldown.y));
            }
            if (roundsInMag <= 0)
            {
                isReloading = true;
                Invoke(nameof(Reload), reloadTime);
            }

            if (!isReloading && !onCooldown && canShoot && roundsLeftInBurst > 0 && timeSincePlayerSeen > curReactionTime)
            {
                Instantiate(bulletPrefab, firearmPos.position, firearmPos.rotation);
                canShoot = false;
                roundsLeftInBurst -= 1;
                roundsInMag -= 1;

                source.PlayOneShot(shotSounds[Random.Range(0, shotSounds.Count)]);
                anim.Play("BaseLayer.demo_combat_shoot");
                Invoke(nameof(ResetShot), 0.1f);
            }
        }
        else ExitAttack(EnemyState.Move);
    }
    void ResetShot() => canShoot = true;
    void CooldownShot()
    {
        float healthChance = Mathf.InverseLerp(50, 100, health);
        float teamChance = Mathf.InverseLerp(0, 4, nearbyTeam.Count);
        float magChance = Mathf.InverseLerp(0, 10, roundsInMag);
        float surpriseChance = Mathf.InverseLerp(100, 0, surprise);
        //Debug.Log("healthChance " + healthChance);
        //Debug.Log("teamChance " + teamChance);
        //Debug.Log("magChance " + magChance);
        //Debug.Log("surpriseChance " + surpriseChance);

        float chanceToMove = Mathf.InverseLerp(0, 3, healthChance + magChance + surpriseChance + teamChance);

        float decision = Random.Range(0f, 1f);

        //Debug.Log("chanceToMove " + chanceToMove);
        //Debug.Log("decision " + decision);
        if (decision > chanceToMove)
        {
            ExitAttack(EnemyState.Move);
        }


        float maxShots = Mathf.Lerp(10, 1, distFromPlayer);
        roundsLeftInBurst = (int)Random.Range(1, maxShots);
        onCooldown = false;
    }
    void Reload()
    {
        isReloading = false;
        roundsInMag = magSize;
    }
    void ExitAttack(EnemyState nextState)
    {
        switch (nextState)
        {
            case EnemyState.Idle: EnterIdle(); break;
            case EnemyState.Move: EnterMove(); break;
        }
    }
    void EnterMove()
    {
        state = EnemyState.Move;

        Vector3 pos = transform.position;
        pos.x -= Random.Range(0, 25);
        pos.z -= Random.Range(0, 25);
        agent.SetDestination(pos);

        anim.Play("BaseLayer.demo_combat_run");
    }
    void UpdateMove()
    {
        if (agent.remainingDistance < 0.5f)
        {
            ExitMove(EnemyState.Idle);
        }
    }
    void ExitMove(EnemyState nextState)
    {
        switch (nextState)
        {
            case EnemyState.Idle: EnterIdle(); break;
            case EnemyState.Attack: EnterAttack(); break;
        }
    }

    public void BulletWhizz(Transform round)
    {
        lastPositionPlayerSpotted = GameManager.GM.player.transform.position;
        surprise = Vector3.Angle(eyePos.forward, GameManager.GM.player.transform.position - eyePos.position);
    }
    public void Hit(int damage, Vector3 hitPoint)
    {
        health -= damage;
        if (health <= 0) Die(hitPoint);
    }
    public void Die(Vector3 hitPoint)
    {
        GameObject g = Instantiate(ragdoll, transform);
        g.transform.parent = null;
        g.GetComponent<Rigidbody>().AddForceAtPosition(-Vector3.forward * 100, hitPoint);

        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Round"))
        {
            //Debug.Log("Whizz");
            BulletWhizz(other.transform);
        }
    }
}
