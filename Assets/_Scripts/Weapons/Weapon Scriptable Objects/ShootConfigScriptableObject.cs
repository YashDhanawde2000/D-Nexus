
using Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName ="Shoot Config", menuName ="Guns/ Shoot Config", order = 2)]
public class ShootConfigScriptableObject : ScriptableObject
{
    public LayerMask HitMask;

    public Vector3 Spread = new Vector3(0.1f, 0.1f,0.1f);

    public float FireRate = 0.25f;

    public float RecoilRecoverySpeed = 1f;

    public float MaxSpreadTime = 1f;

    public float screenShakePower;

    public CinemachineImpulseSource ScreenShake;
    
    public ShootType ShootType = ShootType.FromGun;

    public Vector3 GetSpread(float ShootTime = 0)
    {
        Vector3 spreadAmount = Vector3.Lerp(
            Vector3.zero, 
            new Vector3(
                    Random.Range(
                        -Spread.x,
                        Spread.x),
                    Random.Range(
                        -Spread.y,
                        Spread.y),
                    Random.Range(
                        -Spread.z,
                        Spread.z)
                    ),
            Mathf.Clamp01(ShootTime/ MaxSpreadTime)
            );
        return spreadAmount;
    }

    public void ShakeScreen(Vector3 recoilDirection)
    {
        ScreenShake.GenerateImpulseWithVelocity(recoilDirection);
    }

}
