using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider realtimeHealthBarSlider;
    public Slider easeHealthBarSlider;

    public HealthSystem healthSystem;
    public EnemyHealth enemyHealthSystem;

    float health, maxHealth;

    public float easeSpeed = 0.01f;

    private void Start()
    {
        if (healthSystem != null)
        {
            GameObject.FindGameObjectWithTag("Player").TryGetComponent(out HealthSystem _healthSystem);
            healthSystem = _healthSystem;
        }
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //for enemy
        if (healthSystem == null && enemyHealthSystem != null)
        {
            health = enemyHealthSystem.CurrentHealth;
            maxHealth = enemyHealthSystem.MaxHealth;
        }
        
        // for player
        if (healthSystem != null && enemyHealthSystem == null)
        {
            health = healthSystem.currentHealth;
            maxHealth = healthSystem.maxHealth;
        }

    
        // for realtime
        if (realtimeHealthBarSlider.value != health)
        {

            realtimeHealthBarSlider.maxValue = maxHealth;
            realtimeHealthBarSlider.value = health;
        }

        //for ease
        if (realtimeHealthBarSlider.value != easeHealthBarSlider.value)
        {
            easeHealthBarSlider.maxValue = maxHealth;
            easeHealthBarSlider.value = Mathf.Lerp (easeHealthBarSlider.value, realtimeHealthBarSlider.value, Time.deltaTime * easeSpeed);
        }
    }
}
