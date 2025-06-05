using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectWallet : MonoBehaviour
{
    public static ConnectWallet instance;

    public string address;
    public GameObject connectBtn;
    public bool isConnected;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        //connectBtn = gameObject;
    }

    public void OnWalletConnected(string walletAddress)
    {
        Debug.Log("Wallet connected: " + walletAddress);
        address = walletAddress;
        LevelInfoManager.instance.accountId = address;
        LevelInfoManager.instance.SendGetStartGameRequest(address);
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
