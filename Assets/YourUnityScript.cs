using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class YourUnityScript : MonoBehaviour
{   
    public static YourUnityScript instance;

    public string address;
    public GameObject connectBtn;
    public bool isConnected;

    private void Awake()
    {
        instance = this;
    }

    public void OnWalletConnected(string walletAddress)
    {
        Debug.Log("Wallet connected: " + walletAddress);
        address = walletAddress;
        connectBtn.SetActive(false);
        isConnected = true;
    }

    public string GetWalletAddress()
    {
        return address;
    }

    public void ConnectWalletFromUnity()
    {
        // Call the connectWallet function from JavaScript
        Application.ExternalCall("ConnectWallet");
       
    }
}
