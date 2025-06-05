using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{
    

    public float normalSensitivity;
    public float aimSensitivity;
    public float turnSpeed = 10f;
    public bool isEquipped;

    public float aimRigWeight;

    [SerializeField] private bool AutoReload = true;

    [Header("Required References")]
    [SerializeField] private GameObject weaponHolder;

    
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Rig aimRig;
    
    [SerializeField] private Transform vfxHitEnvironment;
    [SerializeField] private Transform vfxHitTarget;
    [SerializeField] private Transform debugSphere;

    [Header("Abilities")]
    public float explosionDelay = 5f;
    public float explosionForce = 500f;
    public float explosionRadius = 20f;
    public GameObject ultEffect, ultIcon;
    public AudioClip ultSoundEffect;

    [Header("Filled at Runtime")]
    public StarterAssetsInputs inputs;
    private Camera mainCamera;
    private PlayerGunSelector GunSelector;
    private ThirdPersonController tpc;
    private Animator animator;

    private bool IsReloading, IsUsingAbility;



    private void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        GunSelector = GetComponent<PlayerGunSelector>();
        tpc = GetComponent<ThirdPersonController>();
        inputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
    }



    private void Update()
    {
        // Perform Raycasts to the Center of the Screen
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            hitTransform = raycastHit.transform;

            debugSphere.position = raycastHit.point;
        }

        if (!IsUsingAbility)
        {
            // Aim, Shoot, Reload
            HandleAiming(hitTransform, debugSphere);

            // Interpolate the Change in Aim Rig Weight
            aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 10f);
        }
        
            // Abilities
            HandleAbilities();
            
        


    }

    private void FixedUpdate()
    {
        // Rotate player with camera if gun is equipped
        if (isEquipped)
        {
            float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), Time.fixedDeltaTime * turnSpeed);
        }
        
    }

    #region Aim and Shoot Functions
    private void HandleAiming( Transform hitTransform, Transform debugSphere)
    {
        if (inputs != null)
        {
            
            //if ( Input.GetKeyDown(KeyCode.E))
            if (inputs.equipGun)
            {
                isEquipped = !isEquipped; // Toggle the aim state
                inputs.equipGun = false;

                StartCoroutine(HandleAimCoroutine());

            }

            if (inputs.aim && isEquipped)
            {
                aimVirtualCamera.gameObject.SetActive(true);
                //GunSelector.ActiveGun.UpdateCamera();
                tpc.SetSensitivity(aimSensitivity);
                
            }
            else 
            {
                aimVirtualCamera.gameObject.SetActive(false);
                //GunSelector.ActiveGun.UpdateCamera();
                tpc.SetSensitivity(normalSensitivity);
                
            }

            if (!IsReloading)
            {
                HandleShooting(hitTransform, debugSphere);
                HandleGunChange();
            }
            

            HandleReloading();

        }



    }

    private IEnumerator HandleAimCoroutine()
    {
        // Equip - Enable
        if (isEquipped)
        {
            if (weaponHolder != null)
            {
                weaponHolder.gameObject.SetActive(true);


                //animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1, Time.deltaTime * 10f));
                //animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 1, Time.deltaTime * 10f));
                animator.SetLayerWeight(1, 1);
                animator.SetLayerWeight(2, 1);
                aimRigWeight = 1f;

                tpc.SetRotatePlayerOnMove(false);

            }
        }
        // Equip - Disable
        else
        {

            if (weaponHolder != null)
            {
                if (!IsReloading)
                {
                    weaponHolder.gameObject.SetActive(false);

                    //animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, Time.deltaTime * 10f));
                    //animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0, Time.deltaTime * 10f));
                    animator.SetLayerWeight(1, 0);
                    animator.SetLayerWeight(2, 0); 
                    tpc.SetRotatePlayerOnMove(true);

                    aimRigWeight = 0f;
                }
                else
                {
                    //animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, Time.deltaTime * 10f));
                    animator.SetLayerWeight(1, 0);
                    tpc.SetRotatePlayerOnMove(true);

                    aimRigWeight = 0f;
                }

            }

        }
        yield return null;
    }


    private void HandleShooting(Transform hitTransform, Transform debugSphere)
    {
        // Shoot - Enable
        if (inputs.shoot && isEquipped)
        {
            // If has hit Anything
            if (hitTransform != null)
            {
                // If has Hit a Target
                if (hitTransform.GetComponent<EnemyHealth>() != null)
                {
                    GunSelector.ActiveGun.Tick(inputs.shoot,vfxHitTarget, debugSphere);
                    //GameObject hitEffect = Instantiate(vfxHitTarget, debugSphere.position, debugSphere.rotation).gameObject;
                    //Destroy(hitEffect, 1);
                }
                // If has hit Something Else
                else
                {
                    GunSelector.ActiveGun.Tick(inputs.shoot, vfxHitEnvironment, debugSphere);
                    //GameObject hitEffect = Instantiate(vfxHitEnvironment, debugSphere.position, debugSphere.rotation).gameObject;
                    //Destroy(hitEffect, 1);
                }

                animator.SetTrigger("Firing");

                if(GunSelector.ActiveGun.gunType == GunType.M19)
                {
                    inputs.shoot = false;
                }
            }
            //starterAssetsInputs.shoot = false;

        }
        else
        {
            inputs.shoot = false;
            
        }
        

    }


    #endregion

    #region Reloading Functions

    private void HandleReloading()
    {
        if (ShouldManualReload() || ShouldAutoReload())
        {

            GunSelector.ActiveGun.StartReload();
            IsReloading = true;
            animator.SetTrigger("Reload");
            aimRigWeight = 0f;
        }
        //else
        //{
        //    starterAssetsInputs.reload = false;
        //}
    }
    public void EndReload()
    {
        if (IsReloading)
        {
            GunSelector.ActiveGun.EndReload();
            aimRigWeight = 1f;
            inputs.reload = false;
            IsReloading = false;
        }
        
    }

    private bool ShouldManualReload()
    {
        return isEquipped
            && !IsReloading
            && inputs.reload
            && GunSelector.ActiveGun.CanReload();
    }
    private bool ShouldAutoReload()
    {
        return isEquipped
            && !IsReloading
            && AutoReload
            && GunSelector.ActiveGun.AmmoConfig.CurrentClipAmmo == 0
            && GunSelector.ActiveGun.CanReload();
    }

    #endregion

    #region Gun Functions

    private void HandleGunChange()
    {
        if ( inputs.swapGun)
        {
            GunSelector.ChangeGun();
            inputs.swapGun = false;
        }
        
    }



    #endregion

    #region Bullet Pooling System


    #endregion

    #region Ability Functions

    public void HandleAbilities()
    {
        WaveSystemInfo waveSystemInfo = GetComponent<HealthSystem>().waveSystemInfo;

        //Handle Abiltiy HUD
        if (waveSystemInfo.score >= 100)
        {
            ultIcon.SetActive(true);
        }
        else
        {
            ultIcon.SetActive(false);
        }

        //if (inputs.ultimate && !IsReloading)
        if (inputs.ultimate && !IsReloading && waveSystemInfo.score >= 100)
        {
            StartCoroutine(UltimateAbility());
            waveSystemInfo.score = waveSystemInfo.score / 2;
            inputs.ultimate = false;
        }
        else if (inputs.ultimate)
        {
            inputs.ultimate = false;
        }

        
    }


    IEnumerator UltimateAbility()
    {
        tpc.CanMove = false;
        
        weaponHolder.gameObject.SetActive(false);
        //animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, Time.deltaTime * 10f));
        //animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0, Time.deltaTime * 10f));
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 0);
        tpc.SetRotatePlayerOnMove(true);
        aimRigWeight = 0f;

        animator.SetTrigger("Ult");
        AudioSource.PlayClipAtPoint(ultSoundEffect, transform.position);
        GetComponent<HealthSystem>().CanTakeDamage = false;


        GameObject ultEffectGameObj = Instantiate(ultEffect, transform);
        Destroy(ultEffectGameObj, explosionDelay);
        
        yield return new WaitForSeconds(2f);

        Collider[] nearbyObjColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObjCollider in nearbyObjColliders)
        {
            EnemyHealth enemyHealth = nearbyObjCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(200);
            }
        }

        yield return new WaitForSecondsRealtime(0.25f);
        tpc.CanMove = true;
        GetComponent<HealthSystem>().CanTakeDamage = true;
        
        StartCoroutine(HandleAimCoroutine());
    }


    #endregion Ability Functions

}

//using Cinemachine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using StarterAssets;
//using UnityEngine.Animations.Rigging;
//using UnityEngine.UIElements;
//using UnityEngine.InputSystem;

//public class ThirdPersonShooterController : MonoBehaviour
//{
//    //UNCOMMENT shoot fn IF YOU WANT TO SHOOT

//    public float normalSensitivity;
//    public float aimSensitivity;
//    public float turnSpeed = 10f;

//    private float aimRigWeight;

//    [SerializeField] private bool AutoReload = true;


//    [SerializeField] private GameObject weaponHolder;

//    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
//    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
//    [SerializeField] private Rig aimRig;

//    [SerializeField] private Transform vfxHitEnvironment;
//    [SerializeField] private Transform vfxHitTarget;
//    [SerializeField] private Transform debugSphere;


//    private PlayerGunSelector GunSelector;
//    private ThirdPersonController tpc;
//    private StarterAssetsInputs starterAssetsInputs;
//    private Animator animator;

//    private bool IsReloading;



//    private void Awake()
//    {
//        GunSelector = GetComponent<PlayerGunSelector>();
//        tpc = GetComponent<ThirdPersonController>();
//        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
//        animator = GetComponent<Animator>();
//    }



//    private void Update()
//    {
//        // Perform Raycasts to the Center of the Screen
//        Vector3 cursorWorldPosition = Vector3.zero;
//        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
//        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
//        Transform hitTransform = null;
//        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
//        {
//            cursorWorldPosition = raycastHit.point;
//            hitTransform = raycastHit.transform;

//            debugSphere.position = raycastHit.point;
//        }

//        HandleAiming(hitTransform, debugSphere);



//        //Interpolate the Change in Aim Rig Weight

//        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 10f);




//    }

//    private void FixedUpdate()
//    {
//        if (starterAssetsInputs != null)
//        {
//            if (starterAssetsInputs.aim)
//            {
//                float yawCamera = aimVirtualCamera.transform.rotation.eulerAngles.y;
//                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), Time.fixedDeltaTime * turnSpeed);
//            }
//        }
//    }

//    #region Aim and Shoot Functions
//    private void HandleAiming(Transform hitTransform, Transform debugSphere)
//    {
//        if (starterAssetsInputs != null)
//        {
//            // Aim - Enable
//            if (starterAssetsInputs.aim)
//            {
//                if (weaponHolder != null)
//                {
//                    weaponHolder.gameObject.SetActive(true);
//                    aimVirtualCamera.gameObject.SetActive(true);

//                    tpc.SetSensitivity(aimSensitivity);
//                    tpc.SetRotatePlayerOnMove(false);

//                    animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1, Time.deltaTime * 10f));
//                    animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 1, Time.deltaTime * 10f));
//                    aimRigWeight = 1f;

//                }
//            }
//            // Aim - Disable
//            else
//            {
//                if (weaponHolder != null)
//                {
//                    if (!IsReloading)
//                    {
//                        weaponHolder.gameObject.SetActive(false);
//                        aimVirtualCamera.gameObject.SetActive(false);

//                        tpc.SetSensitivity(normalSensitivity);
//                        tpc.SetRotatePlayerOnMove(true);

//                        animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, Time.deltaTime * 10f));
//                        animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0, Time.deltaTime * 10f));
//                        aimRigWeight = 0f;
//                    }
//                    else
//                    {
//                        aimVirtualCamera.gameObject.SetActive(false);

//                        tpc.SetSensitivity(normalSensitivity);
//                        tpc.SetRotatePlayerOnMove(true);

//                        //animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, Time.deltaTime * 10f));
//                        aimRigWeight = 0f;
//                    }

//                }

//            }

//            if (!IsReloading)
//            {
//                HandleShooting(hitTransform, debugSphere);
//                HandleGunChange();
//            }


//            HandleReloading();

//        }



//    }

//    private void HandleShooting(Transform hitTransform, Transform debugSphere)
//    {
//        // Shoot - Enable
//        if (starterAssetsInputs.shoot && starterAssetsInputs.aim)
//        {
//            // If has hit Anything
//            if (hitTransform != null)
//            {
//                // If has Hit a Target
//                if (hitTransform.GetComponent<EnemyHealth>() != null)
//                {
//                    GunSelector.ActiveGun.Tick(starterAssetsInputs.shoot, vfxHitTarget, debugSphere);
//                    //GameObject hitEffect = Instantiate(vfxHitTarget, debugSphere.position, debugSphere.rotation).gameObject;
//                    //Destroy(hitEffect, 1);
//                }
//                // If has hit Something Else
//                else
//                {
//                    GunSelector.ActiveGun.Tick(starterAssetsInputs.shoot, vfxHitEnvironment, debugSphere);
//                    //GameObject hitEffect = Instantiate(vfxHitEnvironment, debugSphere.position, debugSphere.rotation).gameObject;
//                    //Destroy(hitEffect, 1);
//                }

//                animator.SetTrigger("Firing");

//                if (GunSelector.ActiveGun.gunType == GunType.M19)
//                {
//                    starterAssetsInputs.shoot = false;
//                }
//            }
//            //starterAssetsInputs.shoot = false;

//        }
//        else
//        {
//            starterAssetsInputs.shoot = false;

//        }


//    }


//    #endregion

//    #region Reloading Functions

//    private void HandleReloading()
//    {
//        if (ShouldManualReload() || ShouldAutoReload())
//        {
//            GunSelector.ActiveGun.StartReload();
//            IsReloading = true;
//            animator.SetTrigger("Reload");
//            aimRigWeight = 0f;
//        }
//        //else
//        //{
//        //    starterAssetsInputs.reload = false;
//        //}
//    }
//    public void EndReload()
//    {
//        if (IsReloading)
//        {
//            GunSelector.ActiveGun.EndReload();
//            aimRigWeight = 1f;
//            starterAssetsInputs.reload = false;
//            IsReloading = false;
//        }

//    }

//    private bool ShouldManualReload()
//    {
//        return starterAssetsInputs.aim
//            && !IsReloading
//            && starterAssetsInputs.reload
//            && GunSelector.ActiveGun.CanReload();
//    }
//    private bool ShouldAutoReload()
//    {
//        return starterAssetsInputs.aim
//            && !IsReloading
//            && AutoReload
//            && GunSelector.ActiveGun.AmmoConfig.CurrentClipAmmo == 0
//            && GunSelector.ActiveGun.CanReload();
//    }

//    #endregion

//    #region Gun Functions

//    private void HandleGunChange()
//    {
//        if (starterAssetsInputs.swapGun)
//        {
//            GunSelector.ChangeGun();
//            starterAssetsInputs.swapGun = false;
//        }

//    }



//    #endregion

//    #region Bullet Pooling System


//    #endregion

//}

