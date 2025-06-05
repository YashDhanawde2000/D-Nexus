using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGunSelector : MonoBehaviour
{
    public Camera mainCamera;

    //[SerializeField] private PlayerIK playerBodyIK;
    [SerializeField] private GunType GunType;
    [SerializeField] private Transform GunParent;
    [SerializeField] private List<GunScriptableObject> GunsList;

    [Space]
    [Header("Filled at Runtime")]
    public GunScriptableObject ActiveGun, PreviousGun;

    // Start is called before the first frame update
    void Start()
    {
        GunScriptableObject gun = GunsList.Find(gun => gun.gunType == GunType);

        if (gun == null)
        {
            Debug.LogError($"No GunScriptableObject found for GunType: {gun} ");
            return;
        }

        foreach (var gunElement in GunsList)
        {
            gunElement.AmmoConfig.Initialize();
        }

        
        SetupGun(gun);
        GunParent.gameObject.SetActive(false);
        

    }

    public void SetupGun(GunScriptableObject gun)
    {
        ActiveGun = gun.Clone() as GunScriptableObject;
        //ActiveGun.GunModelPrefab.SetActive(true);
        ActiveGun.Spawn(GunParent, this, mainCamera);
        GunType = ActiveGun.gunType;

        //go here for IK Magic https://youtu.be/E-vIMamyORg?si=5l3ancyDifm1gXRZ&t=857
        //DoIKMagic();
        //playerBodyIK.SetGunStyle(ActiveGun.gunType == GunType.M19);
        //playerBodyIK.Setup(GunParent);
        
    }

    public void DespawnActivegun()
    {
        ActiveGun.Despawn();
        Destroy(ActiveGun);

    }

    public void ChangeGun()
    {
        GunScriptableObject currentgun = GunsList.Find(gun => gun.gunType == GunType);

        int currentGunIndex = GunsList.IndexOf(currentgun);
        int nextGunIndex = currentGunIndex + 1;
        if(nextGunIndex >= GunsList.Count)
        {
            nextGunIndex = 0;
        }
        
        GunScriptableObject nextGun = GunsList[nextGunIndex];

        //PreviousGun = ActiveGun;
        DespawnActivegun();
        SetupGun (nextGun);

    }



}
