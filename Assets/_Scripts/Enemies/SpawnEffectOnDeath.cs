using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IDamageable))]
public class SpawnEffectOnDeath : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem DeathParticleSystem;
    public IDamageable Damageable;

    private void Awake()
    {
        Damageable = GetComponent<IDamageable>();
    }

    private void OnEnable()
    {
        Damageable.OnDeath += Damageable_OnDeath;
    }

    private void Damageable_OnDeath(Vector3 Position)
    {

        GameObject deathEffect = Instantiate(DeathParticleSystem, Position, Quaternion.identity).gameObject;
        Destroy(gameObject);
        Destroy(deathEffect, 2);

    }
}
