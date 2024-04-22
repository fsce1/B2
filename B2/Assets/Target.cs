using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public AudioSource source;

    public void OnColliderEnter(Collider col)
    {
        Debug.Log("Hit");
        source.Play();
        if (col.CompareTag("Round"))
        {

        }
    }
}

