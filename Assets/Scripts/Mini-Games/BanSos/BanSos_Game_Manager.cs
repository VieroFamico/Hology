using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BanSos_Game_Manager : MonoBehaviour
{
    public int rowSize = 2;  // Number of rows (vertical axis)
    public int colSize = 3;  // Number of columns (horizontal axis)
    public GameObject[][] grid;  // 2D array to store the grid slots

    public GameObject[] gridSlots;  // This should hold the 6 slot GameObjects in the scene

    // Card deck and drawing variables
    public List<Card> cardInDeck = new List<Card>();  // The deck of cards available
    public List<Card> drawnCardList = new List<Card>();  // Currently drawn cards
    public List<GameObject> drawnCardContainers;  // UI containers for displaying drawn cards (3 slots)

    public Button shuffleButton;  // Button for shuffling

    public bool hasPlacedFirstCard = false;

    void Start()
    {
        AssignSlotCoordinates();
        UnlockAllSlots();

        shuffleButton.onClick.AddListener(ShuffleCards);

        // Initial draw to fill up the 3 slots at the start
        DrawCards(3);

    }

    // Shuffle the drawn cards back into the deck
    public void ShuffleCards()
    {
        // Cannot shuffle if there are fewer than 3 cards left in the deck or if no cards are drawn
        if (drawnCardList.Count == 0 || cardInDeck.Count == 0)
        {
            Debug.Log("No cards to shuffle.");
            return;
        }

        // Add drawn cards back into the deck
        cardInDeck.AddRange(drawnCardList);
        
        // Clear the drawn card containers visually
        foreach (Card card in drawnCardList)
        {
            Destroy(card.gameObject);
        }

        drawnCardList.Clear();

        DrawCards(3);
    }

    // Draw cards from the deck and place them in the drawnCardContainers
    public void DrawCards(int numberOfCards)
    {
        if (cardInDeck.Count == 0)
        {
            Debug.Log("No cards left in the deck.");
            return;
        }

        // Draw cards up to the maximum available or the requested amount
        int cardsToDraw = Mathf.Min(numberOfCards, cardInDeck.Count);

        for (int i = 0; i < cardsToDraw; i++)
        {
            // Select a random card from the deck
            int randomIndex = Random.Range(0, cardInDeck.Count);
            Card drawnCard = cardInDeck[randomIndex];

            

            // Place the drawn card into its container (visually)
            if (i < drawnCardContainers.Count)
            {
                GameObject container = drawnCardContainers[i];

                // Instantiate the card as a child of the container
                Card cardInstance = Instantiate(drawnCard, container.transform);
                cardInstance.transform.localPosition = Vector3.zero;  // Reset position in case of layout issues

                drawnCardList.Add(cardInstance);
            }

            cardInDeck.RemoveAt(randomIndex);
        }
    }


    void AssignSlotCoordinates()
    {
        grid = new GameObject[rowSize][];

        for (int i = 0; i < rowSize; i++)
        {
            grid[i] = new GameObject[colSize];
        }

        // Loop through each slot and assign coordinates
        int index = 0;
        for (int y = 0; y < rowSize; y++)  // From bottom to top (0 to rowSize - 1)
        {
            for (int x = 0; x < colSize; x++)  // From left to right (0 to colSize - 1)
            {
                if (index < gridSlots.Length)
                {
                    Debug.Log(x + " " + y);
                    GameObject slot = gridSlots[index];
                    grid[y][x] = slot;

                    // Assign the SlotPlacement script and set coordinates
                    SlotPlacement slotPlacement = slot.GetComponent<SlotPlacement>();
                    if (slotPlacement == null)
                    {
                        slotPlacement = slot.AddComponent<SlotPlacement>();
                    }

                    slotPlacement.xCoordinate = x;
                    slotPlacement.yCoordinate = y;

                    index++;
                }
            }
        }
    }

    // Called when a card is placed on the board
    public void PlayCard(Card card)
    {
        if (drawnCardList.Contains(card))
        {
            // Remove the card from the drawn list
            drawnCardList.Remove(card);
        }
    }

    public void LockAllSlots()
    {
        foreach (GameObject[] row in grid)
        {
            foreach (GameObject slot in row)
            {
                SlotPlacement slotPlacement = slot.GetComponent<SlotPlacement>();
                if (slotPlacement != null)
                {
                    slotPlacement.LockSlot();  // Lock each slot
                }
            }
        }
    }

    public void UnlockAllSlots()
    {
        foreach (GameObject[] row in grid)
        {
            foreach (GameObject slot in row)
            {
                SlotPlacement slotPlacement = slot.GetComponent<SlotPlacement>();
                if (slotPlacement != null)
                {
                    slotPlacement.UnlockSlot();  // Unlock all slots if needed (for testing or future logic)
                }
            }
        }
    }
}
