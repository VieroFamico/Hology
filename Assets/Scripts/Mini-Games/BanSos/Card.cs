using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform cardRectTransform;
    private CanvasGroup canvasGroup;
    private BanSos_Game_Manager gameManager;
    private Transform originalParent;
    private bool isPlaced = false;

    [Serializable]
    public class unlockablePosition
    {
        public int xUnlock;
        public int yUnlock;
    }
    public List<unlockablePosition> unlockablePositions;

    private void Start()
    {
        cardRectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        gameManager = FindObjectOfType<BanSos_Game_Manager>();
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlaced) return;  // Prevent dragging after placement
        canvasGroup.blocksRaycasts = false;
        originalParent = transform.parent;  // Remember the original parent

        transform.SetParent(gameManager.worldSpaceCanvas.transform);

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlaced) return;
        //cardRectTransform.position = Input.mousePosition;

        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(gameManager.worldSpaceCanvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out worldPosition);
        cardRectTransform.position = worldPosition;


        //cardRectTransform.anchoredPosition += eventData.delta / gameManager.canvas.scaleFactor; // For pixel perfect drag
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isPlaced) return;

        // Check for valid slot placement
        SlotPlacement nearestSlot = GetNearestSlot(eventData.position);
        if (nearestSlot != null && nearestSlot.isPlacable)
        {
            PlaceCardInSlot(nearestSlot);
        }
        else
        {
            // Return the card back to its original position if no valid slot was found
            cardRectTransform.SetParent(originalParent);
            cardRectTransform.anchoredPosition = Vector3.zero;  // Reset position
        }

        canvasGroup.blocksRaycasts = true;
    }

    private SlotPlacement GetNearestSlot(Vector2 pointerPosition)
    {
        SlotPlacement nearestSlot = null;
        float nearestDistance = float.MaxValue;

        // Convert screen position to world position
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(gameManager.worldSpaceCanvas.transform as RectTransform, pointerPosition, gameManager.mainCamera, out worldPosition);

        // Raycast to check if the pointer is over a slot
        foreach (GameObject[] row in gameManager.grid)
        {
            foreach (GameObject slot in row)
            {
                SlotPlacement slotPlacement = slot.GetComponent<SlotPlacement>();
                if (slotPlacement != null && slotPlacement.placedCard == null && slotPlacement.isPlacable)
                {
                    RectTransform slotRect = slot.GetComponent<RectTransform>();

                    // Check if the pointer is within the slot's rect before calculating the distance
                    if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, pointerPosition, gameManager.mainCamera))
                    {
                        // Convert the slot's world position to a world point
                        Vector3 slotWorldPosition = slotRect.position;

                        // Measure the distance from the pointer's world position to the slot's position
                        float distance = Vector3.Distance(worldPosition, slotWorldPosition);

                        // Find the nearest slot by comparing distances
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestSlot = slotPlacement;
                        }
                    }
                }
            }
        }
        return nearestSlot;

    }

    private void PlaceCardInSlot(SlotPlacement slot)
    {
        if (!gameManager.hasPlacedFirstCard)
        {
            gameManager.hasPlacedFirstCard = true;
            gameManager.LockAllSlots();

        }

        // Update slot placement
        slot.placedCard = this;
        slot.isPlacable = false; // Slot is no longer available

        // Move the card into the slot's position
        cardRectTransform.SetParent(slot.transform);
        cardRectTransform.anchoredPosition = Vector2.zero; // Center in slot
        isPlaced = true; // Mark card as placed

        gameManager.PlayCard(this);

        int x = slot.xCoordinate;
        int y = slot.yCoordinate;

        foreach (unlockablePosition pos in unlockablePositions)
        {
            int unlockX = x + pos.xUnlock;
            int unlockY = y + pos.yUnlock;

            // Ensure the position is valid (within the grid bounds)
            if (unlockX >= 0 && unlockX < gameManager.colSize && unlockY >= 0 && unlockY < gameManager.rowSize)
            {
                SlotPlacement slotToUnlock = gameManager.grid[unlockY][unlockX].GetComponent<SlotPlacement>();
                if (slotToUnlock != null && slotToUnlock.placedCard == null)
                {
                    slotToUnlock.UnlockSlot();  // Unlock the adjacent slot
                }
            }
        }

        

    }
}
