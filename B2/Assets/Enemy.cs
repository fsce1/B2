using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public bool isInitialised = false;
    public NavMeshAgent agent;
    public NavMeshSurface surface;

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
            }
        }

        if (canSeePlayer)
        {
            eyePos.forward = lastPositionPlayerSeen - transform.position;

            if (timeSincePlayerSeen == 0)
            {
                Debug.Log("FirstFrame");
                CooldownShot();
                ResetShot();
            }
            else if (roundsLeftInBurst <= 0)
            {
                Debug.Log("Cooldown Start");
                canShoot = false;
                Invoke(nameof(CooldownShot), cooldown);
            }

            //if (timeSincePlayerSeen < reactionTime) canShoot = false;

            if (canShoot && roundsLeftInBurst > 0)
            {
                Debug.Log(roundsLeftInBurst);
                Instantiate(bulletPrefab, eyePos.position, eyePos.rotation);
                canShoot = false;
                roundsLeftInBurst--;
                //Invoke(nameof(ResetShot), 50/650);
                Invoke(nameof(ResetShot), 0.1f);
            }


            timeSincePlayerSeen += Time.fixedDeltaTime;
        }
        else timeSincePlayerSeen = 0;

        //if (!isMoving || agent.isPathStale) FindCover();

        if (finalTgt == Vector3.positiveInfinity)
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
        roundsLeftInBurst = Random.Range(0, 4);
        canShoot = true;
    }

    void FindCover()
    {
        List<Vector3> checkPositions = new();

        Vector3 searchDir = (GameManager.GM.player.transform.position - transform.position).normalized;
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
    public void Hit(float damage)
    {

    }
}
