using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    public Enemy enemy;
    public void Hit()
    {
        enemy.Hit(100, enemy.transform.position);
    }
}
