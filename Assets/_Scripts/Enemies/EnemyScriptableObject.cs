using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Config", menuName = "Enemy/ Enemy Config")]
public class EnemyScriptableObject : ScriptableObject
{
    // Enemy Stats
    public int Health = 100;

    // NavMeshAgent Config
    public float AIUpdateInterval = 0.1f;


}
