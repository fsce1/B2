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
    public int magSize = 30;
    public int roundsInMag = 30;
    public bool canShoot;
    public GameObject bulletPrefab;
    public int bulletsToShoot;


    [Header("Enemy")]
    public float reactionTime;
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

        Vector3 playerDir = GameManager.GM.player.transform.position - eyePos.position;
        if (Physics.Raycast(eyePos.position, playerDir, out RaycastHit hit, playerDir.magnitude)) // less chance to spot if far away?
        {
            Debug.DrawLine(eyePos.position, hit.point);





        }




        if (canSeePlayer)
        {
            transform.forward = GameManager.GM.player.transform.position - transform.position;
            //canShoot = false;

            timeSincePlayerSeen += Time.fixedDeltaTime;



            //if (firstFrameSeen) Invoke(nameof(ReactionTime), reactionTime);


            //if (canShoot) Shoot();

        }





        if(!isMoving || agent.isPathStale) FindCover();





        if (finalTgt == Vector3.positiveInfinity)
        {
            agent.destination = transform.position;
        }
        else agent.destination = finalTgt;

        if ((transform.position.x - finalTgt.x) <= 1f && (transform.position.z - finalTgt.z) <= 1f) isMoving = false;
    }
    void ReactionTime()
    {
        canShoot = true;
    }
    void Shoot()
    {
        

        Instantiate(bulletPrefab, eyePos.position, eyePos.rotation);
        Invoke(nameof(ResetShot), 50/650); //AK-74 RPM 
    }
    void ResetShot()
    {
        canShoot = true;
        roundsLeftInBurst--;
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
