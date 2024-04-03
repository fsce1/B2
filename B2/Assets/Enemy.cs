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
    public bool isMoving;
    public Vector3 finalTgt;

    [Header("Gun")]
    public GameObject firearm;


    public void Initialize()
    {
        isInitialised = true;
    }
    void Update()
    {
        if (!isInitialised) return;

        agent.destination = finalTgt;

        FindCover();
        if ((transform.position - finalTgt).magnitude <= 1f) isMoving = false;
    }


    void FindCover()
    {
        if (isMoving) return;
        List<Vector3> checkPositions = new();

        Vector3 searchDir = (transform.position - GameManager.GM.player.transform.position);
        Debug.DrawLine(transform.position, searchDir, Color.red);
        for (int i = 0; i < 5; i++)
        {
            searchDir = new(Random.Range(searchDir.x - 10, searchDir.x + 10), transform.position.y, Random.Range(searchDir.z - 10, searchDir.z + 10));
            Vector3 position = transform.position + searchDir;
            checkPositions.Add(position);
            Debug.DrawLine(transform.position, position);
        }
        Vector3 bestCover = transform.position;
        float bestDist = Mathf.Infinity;
        foreach (Vector3 pos in checkPositions)
        {
            if (Physics.Raycast(pos, -searchDir, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.transform.CompareTag("Player")) return;
                else
                {
                    float dist = (pos - GameManager.GM.player.transform.position).magnitude;
                    if (dist > bestDist) return;
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
}
