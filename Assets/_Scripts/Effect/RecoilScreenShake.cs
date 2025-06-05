using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilScreenShake : MonoBehaviour
{
    [SerializeField] CinemachineImpulseSource screenShake;
    [SerializeField] float screenShakePower;

    public void ShakeScreen(Vector3 recoilDirection)
    {
        screenShake.GenerateImpulseWithVelocity(-recoilDirection);
    }

}
