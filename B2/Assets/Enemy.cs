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
    public float cooldown;
    public int magSize = 30;
    public int roundsInMag = 30;
    public bool canShoot;
    public bool onCooldown;
    public float curRecoilInaccuracy;
    public GameObject bulletPrefab;


    [Header("Enemy")]
    public float reactionTime;
    public float viewCone;
    public float timeSincePlayerSeen;
    public int health = 100;
    public int enemiesNearby;
    public bool canSeePlayer;
    public bool canHearPlayer;
    public Vector3 lastPositionPlayerSeen;
    public Vector3 lastPositionPlayerHeard;

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
            anim.Play("BaseLayer.Run");
        }
        else anim.Play("BaseLayer.Idle");

        Vector3 playerEyePos = GameManager.GM.player.transform.position;
        playerEyePos.y += 1.75f;
        Vector3 playerDir = playerEyePos - eyePos.position;
        canSeePlayer = false;
        if (Physics.Raycast(eyePos.position, playerDir, out RaycastHit hit, Mathf.Infinity) && hit.collider.CompareTag("Player")) // less chance to spot if far away?
        {
            if (Vector3.Angle(transform.forward, playerDir) < viewCone)
            {
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
            eyePos.localEulerAngles += Random.Range(-0.5f, 0.5f) * Vector3.up;

            if (timeSincePlayerSeen == 0)
            {
                Debug.Log("FirstFrame");
                //CooldownShot();
                ResetShot();
            }
            else if (roundsLeftInBurst <= 0)
            {
                Debug.Log("Cooldown Start");
                onCooldown = true;
                Invoke(nameof(CooldownShot), cooldown);
                curRecoilInaccuracy = 0;


            }

            if (!onCooldown && canShoot && roundsLeftInBurst > 0 && timeSincePlayerSeen > reactionTime)
            {
                Instantiate(bulletPrefab, eyePos.position, eyePos.rotation);
                canShoot = false;
                roundsLeftInBurst--;
                //Invoke(nameof(ResetShot), 50/650);
                curRecoilInaccuracy += 0.001f;
                Invoke(nameof(ResetShot), 0.1f);
                anim.Play("Base Layer.Shoot");
            }
            else curRecoilInaccuracy -= 0.01f;


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
    void ResetShot() => canShoot = true;
    void CooldownShot()
    {
        Debug.Log("Cooldown End");
        roundsLeftInBurst = Random.Range(1, 6);
        onCooldown = false;
    }

    void FindCover()
    {
        List<Vector3> checkPositions = new();

        Vector3 searchDir = -(GameManager.GM.player.transform.position - transform.position).normalized;
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
    public void Hit(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }
    public void Die()
    {
        Instantiate(ragdoll, transform).transform.parent = null;
        Destroy(gameObject);
    }
}
