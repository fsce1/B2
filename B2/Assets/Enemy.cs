using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public bool isInitialised = false;
    public NavMeshAgent agent;
    public NavMeshSurface surface;
    public Animator anim;
    public GameObject ragdoll;
    [Header("Positions")]
    public Transform eyePos;
    public bool isMoving;
    public Vector3 finalTgt;

    [Header("Gun")]
    public int roundsLeftInBurst;
    public Vector2 cooldown;
    public int magSize = 30;
    public int roundsInMag = 30;
    public bool canShoot;
    public bool onCooldown;
    public float curRecoilInaccuracy;
    public GameObject bulletPrefab;


    [Header("Enemy")]
    public Vector2 reactionTime;
    public float surprise;
    public float curReactionTime;
    public float viewCone;
    public float timeSincePlayerSeen;
    public int health = 100;
    public int enemiesNearby;
    public bool canSeePlayer;
    public bool canHearPlayer;
    public Vector3 lastPositionPlayerSeen;
    public Vector3 lastPositionPlayerHeard;
    public Vector2 timeToMove;

    public void Initialize()
    {
        isInitialised = true;
        surface = Object.FindObjectsOfType<NavMeshSurface>()[0];
    }
    void FixedUpdate()
    {
        if (!isInitialised) return;

        if (isMoving)
        {
            anim.Play("Base Layer.Run");
        }
        else anim.Play("Base Layer.Idle");

        Vector3 playerEyePos = GameManager.GM.player.transform.position;
        playerEyePos.y += 1.75f;
        Vector3 playerDir = playerEyePos - eyePos.position;
        canSeePlayer = false;
        if (Physics.Raycast(eyePos.position, playerDir, out RaycastHit hit, Mathf.Infinity) && hit.collider.CompareTag("Player")) // less chance to spot if far away?
        {
            if (Vector3.Angle(transform.forward, playerDir) < viewCone)
            {
                if (!canSeePlayer) SpotPlayer();
                canSeePlayer = true;
                lastPositionPlayerSeen = GameManager.GM.player.transform.position;
                lastPositionPlayerSeen.y -= 0.5f;
                //if(GameManager.GM.player.leanHolder.localEulerAngles.z > 0)
                //{
                //    lastPositionPlayerSeen.x += 0.5f;
                //}
                //else if (GameManager.GM.player.leanHolder.localEulerAngles.z < 0)
                //{
                //    lastPositionPlayerSeen.x -= 0.5f;
                //}
            }
        }

        if (canSeePlayer)
        {
            
            eyePos.forward = lastPositionPlayerSeen - transform.position;
            eyePos.localEulerAngles += curRecoilInaccuracy * transform.right;
            eyePos.localEulerAngles += Random.Range(-0.5f, 0.5f) * transform.up;
            
            if (timeSincePlayerSeen == 0)
            {
                curReactionTime = Random.Range(reactionTime.x, reactionTime.y);
                onCooldown = true;
                CooldownShot();
                //roundsLeftInBurst = 1;
                ResetShot();
            }
            else if (!onCooldown && roundsLeftInBurst <= 0)
            {
                onCooldown = true;
                Invoke(nameof(CooldownShot), Random.Range(cooldown.x, cooldown.y));
                curRecoilInaccuracy = 0;
            }

            if (!onCooldown && canShoot && roundsLeftInBurst > 0 && timeSincePlayerSeen > curReactionTime)
            {
                Instantiate(bulletPrefab, eyePos.position, eyePos.rotation);
                canShoot = false;
                roundsLeftInBurst -= 1;
                roundsInMag -= 1;
                //Invoke(nameof(ResetShot), 50/650);
                curRecoilInaccuracy -= 0.01f;
                Invoke(nameof(ResetShot), 0.05f);
                anim.Play("Base Layer.Shoot");
            }
            else curRecoilInaccuracy += 0.0025f;


            timeSincePlayerSeen += Time.fixedDeltaTime;
        }
        else timeSincePlayerSeen = 0;

        //if (!isMoving || agent.isPathStale) FindCover();

        if (finalTgt == Vector3.positiveInfinity || finalTgt == Vector3.zero)
        {
            agent.destination = transform.position;
        }
        else agent.destination = finalTgt;

        if ((transform.position.x - finalTgt.x) <= 1f && (transform.position.z - finalTgt.z) <= 1f) isMoving = false;
    }

    void SpotPlayer()
    {
        Debug.Log("Spotted");
        float healthChance = Mathf.InverseLerp(0, 100, health);
        float magChance = Mathf.InverseLerp(0, magSize, roundsInMag);
        float surpriseChance = Mathf.InverseLerp(0, 180, surprise);

        
        float chanceToPush = Mathf.InverseLerp(0, 3, healthChance + magChance + surpriseChance);

        float decision = Random.Range(0f, 1f);
        if (decision < chanceToPush)
        {
            Invoke(nameof(RunAway), Random.Range(timeToMove.x, timeToMove.y));
            Debug.Log("Running away");
        }
        else
        {
            Invoke(nameof(RunTowards), Random.Range(timeToMove.x, timeToMove.y));
            Debug.Log("Pushing");
        }

    }
    void ResetShot() => canShoot = true;
    void CooldownShot()
    {
        float distFromPlayer = (lastPositionPlayerSeen - transform.position).magnitude;
        distFromPlayer = Mathf.InverseLerp(0, 500, distFromPlayer);
        float maxShots = Mathf.Lerp(6, 1, distFromPlayer);
        Debug.Log(maxShots);
        roundsLeftInBurst = (int)Random.Range(1, maxShots);
        onCooldown = false;
    }
    public void BulletWhiz()
    {
        finalTgt = transform.position + (GameManager.GM.player.transform.position - transform.position).normalized * 5;

        Vector3 playerEyePos = GameManager.GM.player.transform.position;
        playerEyePos.y += 1.75f;
        Vector3 playerDir = playerEyePos - eyePos.position;

        surprise = Vector3.Angle(transform.forward, playerDir);
        Debug.Log("Whiz");
    }
    void RunTowards()
    {
         Vector3 dir = (GameManager.GM.player.transform.position - transform.position).normalized;

    }
    void RunAway()
    {
        Vector3 dir = -(GameManager.GM.player.transform.position - transform.position).normalized;
    }
    void FindCover(Vector3 direction)
    {
        List<Vector3> checkPositions = new();

        Vector3 searchDir = direction;

        Debug.DrawRay(transform.position, searchDir, Color.red);

        for (int i = 0; i < 10; i++)
        {
            searchDir = new(Random.Range(searchDir.x - 30, searchDir.x + 30), transform.position.y, Random.Range(searchDir.z - 5, searchDir.z + 5));
            Vector3 position = transform.position - searchDir;
            checkPositions.Add(position);
            Debug.DrawLine(transform.position, position);
        }

        Vector3 bestCover = Vector3.positiveInfinity;
        float bestDist = 0;
        foreach (Vector3 pos in checkPositions)
        {
            if (Physics.Raycast(pos, searchDir, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.transform.CompareTag("Player")) return;
                else
                {
                    float dist = (GameManager.GM.player.transform.position - pos).magnitude;
                    if (dist < bestDist) return;
                    else
                    {
                        bestDist = dist;
                        bestCover = pos;
                    }
                }
            }

        }
        finalTgt = bestCover;
        isMoving = true;
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
        g.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.forward * 100, hitPoint);

        Destroy(gameObject);
    }
}
