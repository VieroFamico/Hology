using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini_Games : MonoBehaviour
{
    // This function is called by the Monitor script when zoom status changes
    public void OnZoomStatusChanged(bool isZoomedIn)
    {
        if (isZoomedIn)
        {
            // Enable minigame functionality
            Debug.Log("Minigame activated: Player zoomed in on this monitor.");
            // Add logic for enabling the minigame here
        }
        else
        {
            // Disable minigame functionality
            Debug.Log("Minigame deactivated: Player zoomed out from this monitor.");
            // Add logic for disabling the minigame here
        }
    }
}
