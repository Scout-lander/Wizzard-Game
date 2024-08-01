using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class SendFakeScore : MonoBehaviour
{
    public Button sendFakeScoreButton;
    private string url = "https://chonkyvibes.com/api/trackScore";

    void Start()
    {
        // Add a listener to the button to call the SendScore method when clicked
        sendFakeScoreButton.onClick.AddListener(SendScore);
    }

    void SendScore()
    {
        StartCoroutine(SendScoreCoroutine());
    }

    IEnumerator SendScoreCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("playerName", "TestPlayer");
        form.AddField("score", 999);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log("Fake score sent successfully.");
            }
        }
    }
}
