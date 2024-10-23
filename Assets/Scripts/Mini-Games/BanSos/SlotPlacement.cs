using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotPlacement : MonoBehaviour
{
    public int xCoordinate;
    public int yCoordinate;

    public Card placedCard;  // Holds the card placed in this slot
    public bool isPlacable = true;  // Determines if the slot is placable

    public void LockSlot()
    {
        isPlacable = false;
        GetComponent<Image>().color = Color.white;
    }

    public void UnlockSlot()
    {
        isPlacable = true;
        GetComponent<Image>().color = Color.green;
    }


}
