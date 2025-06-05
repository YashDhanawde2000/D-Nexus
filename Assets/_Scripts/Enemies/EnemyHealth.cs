using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int _MaxHealth = 100;
    [SerializeField]
    private int _Health;

    public AudioClip enemyHurtAudioClip;
    public AudioClip enemyDeathAudioClip;
    [Range(0f, 1f)]
    public float enemyAudioVolume = 1f;

    public int CurrentHealth { get => _Health; private set => _Health = value; }

    public int MaxHealth { get => _MaxHealth; private set => _MaxHealth = value; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    private void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }
    public void TakeDamage(int Damage)
    {
        int damageTaken = Mathf.Clamp(Damage, 0, CurrentHealth);
        CurrentHealth -= damageTaken;
        if (damageTaken != 0)
        {
            OnTakeDamage?.Invoke(damageTaken);
            
            AudioSource.PlayClipAtPoint(enemyHurtAudioClip, transform.position, enemyAudioVolume);

        }
        if (CurrentHealth <= 0 && damageTaken != 0)
        {
            OnDeath?.Invoke(transform.position);

            AudioSource.PlayClipAtPoint(enemyDeathAudioClip, transform.position, enemyAudioVolume);



        }
    }
}
