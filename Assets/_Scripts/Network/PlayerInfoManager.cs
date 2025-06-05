using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;


public class PlayerInfoManager : MonoBehaviour
{

    //private string postApiUrl = "https://backend.decentrawood.com/death/userPlayDeathGame";

    public GameOverMenu gameOverMenu;

    public static PlayerInfoManager instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        
        DebugPostRequest();                                             //
    }
    public void DebugPostRequest()
    {
        // You may need to modify these parameters based on your game logic
        //Debug.Log(LevelInfoManager.instance.accountId);
        string accountId = LevelInfoManager.instance.accountId;
        int score = 0;
        string currentLevel = SceneManager.GetActiveScene().name;

        SendWinGameRequest(accountId, score, currentLevel);
    }

    #region GET API




    #endregion GET API


    #region POST API
    public void SendWinGameRequest(string accountId, int score, string currentLevel)
    {
        StartCoroutine(PostWinGameRequest(accountId, score, currentLevel));
    }
    public IEnumerator PostWinGameRequest(string accountId, int score, string currentLevel)
    {
        // Create the payload data
        WinGamePostRequest payload = new WinGamePostRequest
        {
            accountId = accountId,
            score = score,
            currentLevel = currentLevel
        };
        Debug.Log(accountId + "\n" + score + "\n" + currentLevel);


        // Convert payload to JSON format using Newtonsoft.Json
        string jsonData = JsonConvert.SerializeObject(payload);
        //Debug.Log("payload converted");

        // Create UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest("https://backend.decentrawood.com/death/userPlayDeathGame", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            //Debug.Log("made request");
            // Send the request
            yield return request.SendWebRequest();

            
            // Check for errors
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error sending request: " + request.error);
            }
            else
            {
                // Request successful, parse the response
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                // You can parse the JSON response here if needed
                WinGamePostResponse responseData = JsonConvert.DeserializeObject<WinGamePostResponse>(jsonResponse);
                bool status = responseData.status;
                string message = responseData.message;

                // Use the status and message as needed
                Debug.Log("Status: " + status);
                Debug.Log("Message: " + message);

                // Pass the response data to another function if needed
                // HandleApiResponse(status, message);
            }
        }


        //yield return new WaitForSeconds(5);

        //gameOverMenu.pauseMenu.inputs.pause = true;
        //gameOverMenu.pauseMenu.PauseGame();
        //gameOverMenu.PauseMenuCanvas.SetActive(false);
        //gameOverMenu.GameLostCanvas.SetActive(true);
    }


    #endregion POST API


    #region DEFINED DATA

    [System.Serializable]
    public class WinGamePostRequest
    {
        public string accountId;
        public int score;
        public string currentLevel;
    }

    [System.Serializable]
    public class WinGamePostResponse
    {
        public bool status;
        public string message;
    }

    #endregion DEFINED DATA

}
