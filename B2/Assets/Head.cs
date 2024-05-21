using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    public EnemyStateMachine enemy;
    public void Hit()
    {
        enemy.Hit(100, enemy.transform.position);
    }
}
