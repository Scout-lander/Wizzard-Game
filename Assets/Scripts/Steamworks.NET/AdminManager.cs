using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class AdminManager : MonoBehaviour
{
    public GameObject adminMenu; // Assign your admin menu Canvas in the inspector
    private List<ulong> whitelistedSteamIDs = new List<ulong>()
    {
        76561198227854704,  //Cam
        76561198012921946,  //james
        76561198115445389,  //Steve
        76561198067080798   //ric
    };

    void Start()
    {
        if (IsAdmin(SteamUser.GetSteamID().m_SteamID))
        {
            // Admin access granted, show the admin menu button
            //adminMenu.SetActive(true);
            if (DebugConsole.instance != null)
            {
                DebugConsole.instance.ClearLog();
            }
            Debug.Log("Admin access granted");
        }
        else
        {
            // Admin access denied, hide the admin menu button
            adminMenu.SetActive(false);
            if (DebugConsole.instance != null)
            {
                DebugConsole.instance.ClearLog();
            }
            Debug.Log("Admin access denied");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote)) // The backtick key (`) is represented by BackQuote
        {
            ToggleAdminMenu();
        }
    }

    private bool IsAdmin(ulong steamID)
    {
        return whitelistedSteamIDs.Contains(steamID);
    }

    private void ToggleAdminMenu()
    {
        if (IsAdmin(SteamUser.GetSteamID().m_SteamID))
        {
            adminMenu.SetActive(!adminMenu.activeSelf); // Toggle the active state of the admin menu
        }
    }

    // Call this method to open the admin menu
    public void OpenAdminMenu()
    {
        if (IsAdmin(SteamUser.GetSteamID().m_SteamID))
        {
            adminMenu.SetActive(true);
        }
    }

    // Call this method to close the admin menu
    public void CloseAdminMenu()
    {
        adminMenu.SetActive(false);
    }
}
