using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

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

    [Header("Gun")]
    public GameObject bulletPrefab;

    public int magSize = 30;
    public int roundsInMag = 30;
    public float reloadTime = 3;

    public bool canShoot;
    public bool onCooldown;
    public bool isReloading;
    public int roundsLeftInBurst;
    public Vector2 cooldown;

    [Header("Audio")]
    public AudioSource source;
    public List<AudioClip> shotSounds;

    [Header("Enemy")]
    public int health = 100;
    public float viewCone;
    public List<Enemy> nearbyTeam;

    public Vector2 reactionTime;
    public float curReactionTime;

    public bool canSeePlayer;
    public bool canHearPlayer;
    public Vector3 lastPositionPlayerSpotted;
    public float timeSincePlayerSeen;

    private void Start()
    {
        agent.SetDestination(transform.position);
        surface = FindFirstObjectByType<NavMeshSurface>();
    }
    public void FixedUpdate()
    {
        if (agent.remainingDistance < 0.5f)
        {
            Vector3 pos = transform.position;
            pos.x += Random.Range(0, 10);
            pos.z += Random.Range(0, 10);
            agent.SetDestination(pos);
        }

        if(Physics.Raycast(eyePos.position, GameManager.GM.player.transform.position - eyePos.position, out RaycastHit hit, Mathf.Infinity)){
            if (hit.transform.CompareTag("Player"))
            {
                canSeePlayer = true;
            }
            else
            {
                canSeePlayer = false;
            }
        }

        lookPos.LookAt(lastPositionPlayerSpotted);
    }
}
