using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HealthSystem : MonoBehaviour
{

    public int currentHealth = 100;
    public int maxHealth = 100;
    public bool IsAlive = true;
    public bool CanTakeDamage = true;

    public AudioClip playerHurtAudioClip;

    public WaveSystemInfo waveSystemInfo;
    public TextMeshProUGUI lowHealthText;
    private bool lhtChanged;
    public float hideLowHealthTextDelay = 5f;
    public Volume lowHealthVolume;
    public AudioLowPassFilter lowHealthAudioFilter;
    
    private ColorAdjustments colorAdjustments;
    private Vignette vignette;

    //public AudioClip playerDeathAudioClip;
    [Range(0f, 1f)]
    public float playerAudioVolume = 1f;

    public GameOverMenu gameOverMenu;

    Animator animator;
    CinemachineImpulseSource ScreenShake;
    ThirdPersonShooterController tps;

    public Vector3 screenShakePowerVector;
    


    void Start()
    {
        animator = GetComponent<Animator>();
        ScreenShake = GetComponent<CinemachineImpulseSource>();
        tps = GetComponent<ThirdPersonShooterController>();

        lowHealthVolume.profile.TryGet (out colorAdjustments);
        lowHealthVolume.profile.TryGet(out vignette);
        
        IsAlive = true;
    }

    private void Update()
    {
        
        if (currentHealth <= (maxHealth * 0.25))
        {
            colorAdjustments.saturation.value = -50;
            vignette.intensity.value = 0.6f;
            lowHealthAudioFilter.enabled = true;
            //Debug.Log(" text activated");

            lowHealthText.gameObject.SetActive(true);
            if(!lhtChanged)
            {
                lowHealthText.text = "Low Health!!! \n " +
                "Press [B] to Buy Health";
                lhtChanged = true;
            }

        }
        else
        {
            colorAdjustments.saturation.value = 0;
            vignette.intensity.value = 0;
            lowHealthAudioFilter.enabled = false;
            lowHealthText.gameObject.SetActive(false);
        }

        // Healing Functionality
        //if (Input.GetKeyDown(KeyCode.B))
        if (tps.inputs.heal)
        {
            if (waveSystemInfo.score > 50)
            {
                waveSystemInfo.score -= 50;
                currentHealth = maxHealth;
                lowHealthText.gameObject.SetActive(true);
                lowHealthText.text = "Health Restored";
                StartCoroutine(HideLowHealthText());
            }
            else
            {
                lowHealthText.gameObject.SetActive(true);
                lowHealthText.text = "Not Enough Score!";
                StartCoroutine(HideLowHealthText());
            }

            tps.inputs.heal = false;
        }



    }

    public void TakeDamage(int damageAmount)
    {
        if (CanTakeDamage)
        {
            int damageTaken = Mathf.Clamp(damageAmount, 0, currentHealth);
            currentHealth -= damageTaken;

            animator.SetTrigger("Hit");

            ScreenShake.GenerateImpulseWithVelocity(screenShakePowerVector);
            AudioSource.PlayClipAtPoint(playerHurtAudioClip, transform.position, playerAudioVolume);

            if (currentHealth <= 0 && IsAlive)
            {
                Die();
            }
        }  
    }

    public void Die()
    {
        GetComponent<PlayerInput>().enabled = false;
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 0);
        tps.aimRigWeight = 0;
        animator.SetTrigger("Die");
        

        IsAlive = false;

    }

    public void DeathScreen()
    {  
        GetComponent<PlayerInput>().enabled = true;
        gameOverMenu.GameLost();
        Destroy(this.gameObject, 5f);
    }

    IEnumerator HideLowHealthText()
    {
        yield return new WaitForSecondsRealtime(hideLowHealthTextDelay);

        lowHealthText.gameObject.SetActive (false);
        lhtChanged = false;
    }

}