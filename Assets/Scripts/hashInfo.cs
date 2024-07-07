using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class ShowHashInfo : MonoBehaviour
{
    public string url = "https://chonkyvibes.com/hash.txt"; // URL to fetch the data from
    public TextMeshProUGUI textMeshProUGUI; // Reference to the TMP text element in the UI

    void Start()
    {
        StartCoroutine(FetchDataFromURL());
    }

    IEnumerator FetchDataFromURL()
    {
        textMeshProUGUI.text = "Checking for updates...";
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
                textMeshProUGUI.text = "Error fetching data.";
            }
            else
            {
                string data = webRequest.downloadHandler.text;
                textMeshProUGUI.text = data;
            }
        }
    }
}