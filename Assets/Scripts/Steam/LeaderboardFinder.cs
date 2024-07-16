using UnityEngine;
using Steamworks;

public class LeaderboardFinder : MonoBehaviour
{
    private SteamLeaderboard_t leaderboard;
    private bool leaderboardFound = false;
    private string leaderboardName;

    private CallResult<LeaderboardFindResult_t> leaderboardFindResult;
    private CallResult<LeaderboardScoreUploaded_t> leaderboardUploadResult;

    void Awake()
    {
        // Ensure Steam is initialized
        if (!SteamManager.Initialized)
        {
            Debug.LogError("SteamManager is not initialized!");
            return;
        }

        leaderboardFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult);
        leaderboardUploadResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderboardUploadResult);

        leaderboardName = "HIGHSCORE"; // Set the leaderboard name
        Debug.Log($"Finding or creating leaderboard: {leaderboardName}");
        FindOrCreateLeaderboard(leaderboardName);
    }

    void Update()
    {
        if (SteamManager.Initialized)
        {
            SteamAPI.RunCallbacks();
        }
    }

    private void FindOrCreateLeaderboard(string leaderboardName)
    {
        SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard(leaderboardName, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
        leaderboardFindResult.Set(handle, OnLeaderboardFindResult);
        Debug.Log($"Initiated leaderboard find or create for: {leaderboardName}");
    }

    private void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool bIOFailure)
    {
        if (bIOFailure || pCallback.m_bLeaderboardFound == 0)
        {
            Debug.LogError($"Failed to find or create leaderboard: {leaderboardName}. IO Failure: {bIOFailure}, Leaderboard Found: {pCallback.m_bLeaderboardFound}");
            return;
        }

        leaderboard = pCallback.m_hSteamLeaderboard;
        leaderboardFound = true;
        Debug.Log($"Leaderboard '{leaderboardName}' found or created successfully. Leaderboard ID: {leaderboard.m_SteamLeaderboard}");
    }

    public void UploadScore(int score)
    {
        if (score <= 0)
        {
            Debug.LogError("Invalid score. Score must be greater than zero.");
            return;
        }
        if (!leaderboardFound)
        {
            Debug.LogError("Leaderboard not found yet! Cannot upload score.");
            return;
        }

        Debug.Log($"Uploading score: {score} to leaderboard: {leaderboard.m_SteamLeaderboard}");
        SteamAPICall_t handle = SteamUserStats.UploadLeaderboardScore(leaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, null, 0);
        leaderboardUploadResult.Set(handle, OnLeaderboardUploadResult);
    }

    private void OnLeaderboardUploadResult(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)
    {
        if (bIOFailure)
        {
            Debug.LogError("Failed to upload score: IO failure.");
            return;
        }
        if (pCallback.m_bSuccess == 0)
        {
            Debug.LogError($"Failed to upload score. Success: {pCallback.m_bSuccess}, " +
                           $"Score: {pCallback.m_nScore}, " +
                           $"Previous Rank: {pCallback.m_nGlobalRankPrevious}, " +
                           $"New Rank: {pCallback.m_nGlobalRankNew}");
            return;
        }
        Debug.Log($"Score uploaded successfully. Score: {pCallback.m_nScore}, " +
                  $"Previous Rank: {pCallback.m_nGlobalRankPrevious}, " +
                  $"New Rank: {pCallback.m_nGlobalRankNew}");
    }
}
