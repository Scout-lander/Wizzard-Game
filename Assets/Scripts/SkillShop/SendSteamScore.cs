using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using Steamworks;

public class SendSteamScore : MonoBehaviour
{
    public Button sendScoreButton;
    private string url = "https://chonkyvibes.com/api/trackScore";

    void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized.");
            return;
        }

        sendScoreButton.onClick.AddListener(SendScore);
    }

    void SendScore()
    {
        StartCoroutine(SendScoreCoroutine());
    }

    IEnumerator SendScoreCoroutine()
    {
        // Get Steam user data
        string playerName = SteamFriends.GetPersonaName();

        // Prepare form data
        WWWForm form = new WWWForm();
        form.AddField("playerName", playerName);
        form.AddField("score", 150135);
        form.AddField("avatarUrl", "null");  // Set avatarUrl to null
        form.AddField("kills", 1000);
        form.AddField("dps", 110);
        form.AddField("levelReached", 34);
        form.AddField("timeSurvived", 600);
        form.AddField("goldCollected", 10100);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
                Debug.LogError("Response: " + www.downloadHandler.text);  // Log the server response
            }
            else
            {
                Debug.Log("Score sent successfully.");
                //Debug.Log("Response: " + www.downloadHandler.text);  // Log the server response
            }
        }
    }
}
