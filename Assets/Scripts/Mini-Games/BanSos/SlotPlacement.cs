using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotPlacement : MonoBehaviour
{
    public int xCoordinate;
    public int yCoordinate;

    public Card placedCard;  // Holds the card placed in this slot
    public bool isPlacable = true;  // Determines if the slot is placable

    public void LockSlot()
    {
        isPlacable = false;
    }

    public void UnlockSlot()
    {
        isPlacable = true;
    }


}
