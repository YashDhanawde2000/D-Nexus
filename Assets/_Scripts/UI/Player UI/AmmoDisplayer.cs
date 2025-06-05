using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(TextMeshProUGUI))]
public class AmmoDisplayer : MonoBehaviour
{
    
    private PlayerGunSelector GunSelector;

    private TextMeshProUGUI AmmoText;
    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").TryGetComponent(out PlayerGunSelector gunSelector);
        GunSelector = gunSelector;
        AmmoText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        AmmoText.SetText($"{GunSelector.ActiveGun.AmmoConfig.CurrentClipAmmo} / "
            + $"{GunSelector.ActiveGun.AmmoConfig.ClipSize}");
    }
}
