using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu( fileName ="Gun", menuName ="Guns/ Gun", order = 0)]
public class GunScriptableObject : ScriptableObject, ICloneable
{
    //public ImpactType ImpactType;
    public GunType gunType;
    public string gunName;
    public GameObject GunModelPrefab;
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    public DamageConfigScriptableObject DamageConfig;
    public ShootConfigScriptableObject ShootConfig;
    public TrailConfigScriptableObject TrailConfig;
    public AmmoConfigScriptableObject AmmoConfig;
    public AudioConfigScriptableObject AudioConfig;

    private Transform vfxHitEffect;
    private MonoBehaviour ActiveMonoBehaviour;
    private GameObject Model;
    private AudioSource ShootingAudioSource;
    private Camera ActiveCamera;
    
    private float LastShootTime;
    private float InitialShootTime;
    private float StopShootingTime;
    private bool LastFrameWantedToShoot;

    private ParticleSystem ShootParticleSystem;
    private ObjectPool<TrailRenderer> TrailPool;


    public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour, Camera ActiveCamera = null)
    {
        this.ActiveMonoBehaviour = ActiveMonoBehaviour;
        this.ActiveCamera = ActiveCamera;

        //LastShootTime = 0;      //this won't reset properly in the editor but will work properly in the build
        TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        Model = Instantiate(GunModelPrefab);
        Model.transform.SetParent(Parent, false);
        Model.transform.localPosition = spawnPosition;
        Model.transform.localRotation = Quaternion.Euler(spawnRotation);

        ShootParticleSystem = Model.GetComponentInChildren<ParticleSystem>();
        ShootingAudioSource = Model.GetComponent<AudioSource>();
        ShootConfig.ScreenShake = Model.GetComponent <CinemachineImpulseSource>();
    }

    public void Despawn()
    {
        Model.SetActive(false);
        Destroy(Model);
    }

    public void UpdateCamera()
    {
        this.ActiveCamera = ActiveCamera;
    }

    public bool CanReload()
    {
        return AmmoConfig.CanReload();
    }

    public void StartReload()
    {
        AudioConfig.PlayReloadClip(ShootingAudioSource); 
    }

    public void EndReload()
    {
        AmmoConfig.Reload();
    }

    public void Tick(bool WantsToShoot, Transform vfxHitEffect, Transform debugSphere)
    {
        Model.transform.localRotation = Quaternion.Slerp(
            Model.transform.localRotation,
            Quaternion.Euler(spawnRotation),
            Time.deltaTime * ShootConfig.RecoilRecoverySpeed);

        if(WantsToShoot)
        {
            LastFrameWantedToShoot = true;
            if ( AmmoConfig.CurrentClipAmmo > 0 )
            {
                Shoot(vfxHitEffect, debugSphere);
            }
            
        }
        else if (!WantsToShoot && LastFrameWantedToShoot)
        {
            StopShootingTime = Time.time;
            LastFrameWantedToShoot = false;
        }
    }

    public void Shoot( Transform vfxHitEffect, Transform debugSphere)
    {
        if (Time.time - LastShootTime - ShootConfig.FireRate > Time.deltaTime)
        {
            float lastShootDuration = Mathf.Clamp(0, (StopShootingTime - InitialShootTime), ShootConfig.MaxSpreadTime);
            float lerpTime = (ShootConfig.RecoilRecoverySpeed - (Time.time - StopShootingTime) / ShootConfig.RecoilRecoverySpeed);
            InitialShootTime = Time.time - Mathf.Lerp(0, lastShootDuration, Mathf.Clamp01(lerpTime));
        }
        if ( Time.time > ShootConfig.FireRate + LastShootTime )
        {
            LastShootTime = Time.time;
            
            if( AmmoConfig.CurrentClipAmmo == 0 )
            {
                AudioConfig.PlayEmptyClip(ShootingAudioSource);
                return;
            }

            ShootParticleSystem.Play();
            AudioConfig.PlayShootingClip(ShootingAudioSource);

            Vector3 spreadAmount = ShootConfig.GetSpread(Time.time - InitialShootTime);
            Model.transform.forward += Model.transform.TransformDirection( spreadAmount);
            Vector3 shootDirection = Vector3.zero;

            if (ShootConfig.ShootType == ShootType.FromGun)
            {
                shootDirection = ShootParticleSystem.transform.forward + spreadAmount;
            }
            else
            {
                shootDirection = ActiveCamera.transform.forward + ActiveCamera.transform.TransformDirection(shootDirection) + spreadAmount;
            }

                
                 
            shootDirection.Normalize();

            ShootConfig.ShakeScreen( ActiveCamera.transform.forward * ShootConfig.screenShakePower);

            AmmoConfig.CurrentClipAmmo--;

            if (Physics.Raycast(
                    GetRaycastOrigin(),
                    shootDirection,
                    out RaycastHit hit,
                    float.MaxValue,
                    ShootConfig.HitMask
                    ) )
            {
                ActiveMonoBehaviour.StartCoroutine
                    (
                        PlayTrail
                        (
                            ShootParticleSystem.transform.position,
                            hit.point,
                            hit
                        )
                    );
                GameObject hitEffect = Instantiate(vfxHitEffect, debugSphere.position, debugSphere.rotation).gameObject;
                Destroy(hitEffect, 1);
            }
            else
            {
                ActiveMonoBehaviour.StartCoroutine
                    (
                        PlayTrail
                        (
                            ShootParticleSystem.transform.position,
                            ShootParticleSystem.transform.position + (shootDirection * TrailConfig.MissDistance),
                            new RaycastHit()
                        )
                    );
                GameObject hitEffect = Instantiate(vfxHitEffect, debugSphere.position, debugSphere.rotation).gameObject;
                Destroy(hitEffect, 1);
            }
        }
    }

    public Vector3 GetRaycastOrigin()
    {
        Vector3 origin = ShootParticleSystem.transform.position;

        if(ShootConfig.ShootType == ShootType.FromCamera)
        {
            origin = ActiveCamera.transform.position 
                + ActiveCamera.transform.forward 
                * Vector3.Distance(
                    ActiveCamera.transform.position,
                    ShootParticleSystem.transform.position
                    );
        }
        return origin;
    }

    private IEnumerator PlayTrail (Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit )
    {
        TrailRenderer instance = TrailPool.Get();
        instance.gameObject.SetActive (true);
        instance.transform.position = StartPoint;
        yield return null;      // avoid position carry-over from last frame if reused

        instance.emitting = true;

        float distance = Vector3.Distance( StartPoint, EndPoint );
        float remainingDistance = distance;
        while ( remainingDistance > 0 )
        {
            instance.transform.position = Vector3.Lerp
                (
                    StartPoint,
                    EndPoint,
                    Mathf.Clamp01 ( 1- (remainingDistance / distance))
                );
            remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = EndPoint;

        // for when we add dynamic bullet impact
        if (Hit.collider != null)
        {
            //SurfaceManager.Instance.HandleImpact
            //    (
            //    Hit.transform.gameObject,
            //    EndPoint,
            //    Hit.normal,
            //    ImpactType,
            //    0
            //    )
            if ( Hit.collider.TryGetComponent (out IDamageable damageable) )
            {
                damageable.TakeDamage(DamageConfig.GetDamage(distance));
            }
        }

        yield return new WaitForSeconds ( TrailConfig.Duration );
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive (false);
        TrailPool.Release(instance);

    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");

        TrailRenderer trail = instance.AddComponent<TrailRenderer>();

        trail.colorGradient = TrailConfig.Color;
        trail.material = TrailConfig.Material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;

    }

    public object Clone()
    {
        GunScriptableObject config = CreateInstance<GunScriptableObject>();

        config.gunType = gunType;
        config.gunName = gunName;
        config.GunModelPrefab = GunModelPrefab;
        config.spawnPosition = spawnPosition;
        config.spawnRotation = spawnRotation;
        config.DamageConfig = DamageConfig;
        config.ShootConfig = ShootConfig;
        config.TrailConfig = TrailConfig;
        config.AmmoConfig = AmmoConfig;
        config.AudioConfig = AudioConfig; 

        return config;
    }
}
