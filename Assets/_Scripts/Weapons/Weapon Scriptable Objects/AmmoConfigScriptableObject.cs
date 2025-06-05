using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo Config", menuName = "Guns/ Ammo Config", order = 3)]
public class AmmoConfigScriptableObject : ScriptableObject
{
    public int ClipSize = 30;

    public int CurrentClipAmmo = 30;

    public void Initialize()
    {
        CurrentClipAmmo = ClipSize;
    }

    public void Reload()
    {
        CurrentClipAmmo = ClipSize;
    }

    public bool CanReload()
    {
        return CurrentClipAmmo < ClipSize;
    }
}
