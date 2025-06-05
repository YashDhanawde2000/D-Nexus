using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class LevelInfoManager : MonoBehaviour
{
    public static LevelInfoManager instance;


    public string accountId;
    public TextMeshProUGUI userBalanceDisplay;

    int totalScore;

    private void Start()
    {
        if ( instance != null)
        {
            Destroy (gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad (gameObject);
    }



    #region GET API

    public void SendGetStartGameRequest(string accountId)
    {
        StartCoroutine(GetUserDeathScoreRequestCoroutine(accountId));
    }

    public IEnumerator GetUserDeathScoreRequestCoroutine(string accountId)
    {
        GetStartGameRequestPayload requestPayload = new GetStartGameRequestPayload
        {
            accountId = accountId
        };

        string jsonData = JsonConvert.SerializeObject(requestPayload);

        using (UnityWebRequest request = UnityWebRequest.Get("https://backend.decentrawood.com/death/getUserGamePlay?accountId=" + accountId))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error sending request: " + request.error);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                GetStartGameResponse response = JsonConvert.DeserializeObject<GetStartGameResponse>(jsonResponse);

                bool status = response.status;
                string message = response.message;

                Debug.Log("Status: " + status);
                Debug.Log("Message: " + message);

                if (status)
                {
                    UserGetStartGameData userData = response.data;
                    string accountIdResponse = userData.accountId;
                    List<string> levelsArray = userData.levels;
                    totalScore = userData.totalScore;



                    Debug.Log("Account ID: " + accountIdResponse);

                    Debug.Log("Levels: " + string.Join(",", levelsArray));

                    Debug.Log("Total Score: " + totalScore);
                    
                    // Set the User Balance Display Text
                    if (userBalanceDisplay != null)
                    {
                        userBalanceDisplay.text = "Score: " + totalScore;
                    }

                    // Handle the parsed data as needed
                    // HandleParsedData(accountIdResponse, levelsArray, totalScore);
                }
                else
                {
                    Debug.LogError("Error: " + message);
                }
            }
        }
    }


    #endregion GET API





    #region DEFINED DATA

    [System.Serializable]
    public class GetStartGameRequestPayload
    {
        public string accountId;
    }

    [System.Serializable]
    public class GetStartGameResponse
    {
        public bool status;
        public string message;
        public UserGetStartGameData data;
    }

    [System.Serializable]
    public class UserGetStartGameData
    {
        public string accountId;
        public List<string> levels;
        public int totalScore;
    }

    #endregion DEFINED DATA

}