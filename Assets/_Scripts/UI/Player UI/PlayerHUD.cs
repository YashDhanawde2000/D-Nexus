using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public GameObject playerControlsHUD; // Reference to the HUD GameObject
    public float delayBeforeHide = 10f;

    void Start()
    {
        // Make sure the HUD is initially visible
        if (playerControlsHUD != null)
        {
            
            StartCoroutine(HideHUDAfterDelay());
        }
        else
        {
            Debug.Log("Player HUD GameObject is not assigned!");
        }
    }

    void Update()
    {
        // Example: Press "H" key to hide the HUD
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHUDVisibility();
        }
    }

    void ToggleHUDVisibility()
    {
        if (playerControlsHUD != null)
        {
            playerControlsHUD.SetActive(!playerControlsHUD.activeSelf); // Toggle visibility
        }
    }


    IEnumerator HideHUDAfterDelay()
    {
        yield return new WaitForSeconds(3);

        playerControlsHUD.SetActive(true);

        yield return new WaitForSeconds(delayBeforeHide);

        // After the delay, hide the HUD
        if (playerControlsHUD != null )
        {
            playerControlsHUD.SetActive(false);
        }
    }
}
