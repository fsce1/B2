using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWhizz : MonoBehaviour
{
    public AudioClip whizz;
    GameObject g;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Round"))
        {
            Debug.Log("Player Whizz");
            g = Instantiate(new GameObject(), transform);
            g.AddComponent<AudioSource>().PlayOneShot(whizz);
            Invoke(nameof(EndAudio), whizz.length);
        }
    }
    void EndAudio()
    {
        Destroy(g);
    }

}
