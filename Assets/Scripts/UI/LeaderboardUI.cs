using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    public Text leaderboardText;
    private string url = "https://chonkyvibes.com/api/leaderboard";

    void Start()
    {
        StartCoroutine(FetchLeaderboardData());
    }

    IEnumerator FetchLeaderboardData()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                try
                {
                    string jsonResponse = www.downloadHandler.text;
                    Debug.Log("JSON Response: " + jsonResponse);

                    // Wrap the JSON array in an object
                    string wrappedJson = "{ \"leaderboard\": " + jsonResponse + " }";
                    Debug.Log("Wrapped JSON: " + wrappedJson);

                    LeaderboardWrapper leaderboardWrapper = JsonUtility.FromJson<LeaderboardWrapper>(wrappedJson);
                    if (leaderboardWrapper == null)
                    {
                        Debug.LogError("LeaderboardWrapper is null");
                    }
                    else if (leaderboardWrapper.leaderboard == null)
                    {
                        Debug.LogError("Leaderboard entries are null");
                    }
                    else
                    {
                        DisplayLeaderboard(new List<LeaderboardEntry>(leaderboardWrapper.leaderboard));
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Error parsing JSON response: " + ex.Message);
                }
            }
        }
    }

    void DisplayLeaderboard(List<LeaderboardEntry> leaderboardEntries)
    {
        leaderboardText.text = "Leaderboard:\n";
        foreach (var entry in leaderboardEntries)
        {
            leaderboardText.text += $"Player: {entry.player_name}, Score: {entry.score}\n";
        }
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string player_name;
    public int score;
}

[System.Serializable]
public class LeaderboardWrapper
{
    public LeaderboardEntry[] leaderboard;
}
