using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Steamworks;

public class UploadScore : MonoBehaviour
{
    private string url = "https://chonkyvibes.com/api/trackScore";

    public void UploadGameScore(float score, float totalGoldCollected, int killCount, float dps, float stopwatchTime, int levelReached)
    {
        StartCoroutine(UploadScoreCoroutine(score, totalGoldCollected, killCount, dps, stopwatchTime, levelReached));
    }

    private IEnumerator UploadScoreCoroutine(float score, float totalGoldCollected, int killCount, float dps, float stopwatchTime, int levelReached)
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized.");
            yield break;
        }

        // Get Steam user data
        string playerName = SteamFriends.GetPersonaName();
        CSteamID steamID = SteamUser.GetSteamID();

        // Get the avatar URL
        string avatarUrl = GetSteamAvatarUrl(steamID);

        // Prepare form data
        WWWForm form = new WWWForm();
        form.AddField("playerName", playerName);
        form.AddField("score", score.ToString());
        form.AddField("avatarUrl", "null");
        form.AddField("kills", killCount);
        form.AddField("dps", dps.ToString());
        form.AddField("levelReached", levelReached);
        form.AddField("timeSurvived", stopwatchTime.ToString());
        form.AddField("goldCollected", totalGoldCollected.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log("Score uploaded successfully.");
            }
        }
    }

    private string GetSteamAvatarUrl(CSteamID steamID)
    {
        int avatarInt = SteamFriends.GetLargeFriendAvatar(steamID);
        uint imageWidth, imageHeight;
        bool success = SteamUtils.GetImageSize(avatarInt, out imageWidth, out imageHeight);
        if (!success)
        {
            Debug.LogError("Failed to get image size.");
            return null;
        }

        byte[] imageData = new byte[imageWidth * imageHeight * 4];
        success = SteamUtils.GetImageRGBA(avatarInt, imageData, (int)(imageWidth * imageHeight * 4));
        if (!success)
        {
            Debug.LogError("Failed to get image RGBA.");
            return null;
        }

        Texture2D avatarTexture = new Texture2D((int)imageWidth, (int)imageHeight, TextureFormat.RGBA32, false);
        avatarTexture.LoadRawTextureData(imageData);
        avatarTexture.Apply();

        byte[] pngData = avatarTexture.EncodeToPNG();
        string base64Png = System.Convert.ToBase64String(pngData);

        return "data:image/png;base64," + base64Png;
    }
}
