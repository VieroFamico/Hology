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
    public Canvas worldSpaceCanvas;
    public Camera mainCamera;

    public bool hasPlacedFirstCard = false;

    void Start()
    {
        mainCamera = Camera.main;

        AssignSlotCoordinates();
        UnlockAllSlots();

        shuffleButton.onClick.AddListener(ShuffleCards);

        // Initial draw to fill up the 3 slots at the start
        DrawCards(3);

        StartCoroutine(DrawCards(3));


    }

    // Shuffle the drawn cards back into the deck
    public void ShuffleCards()
    {
        if (cardInDeck.Count == 0 || drawnCardList.Count == 3)
        {
            Debug.Log("No cards to shuffle.");
            return;
        }

        StartCoroutine(ShuffleAnimation());
    }

    // Animation for drawing cards
    IEnumerator DrawCards(int numberOfCards)
    {
        if (cardInDeck.Count == 0)
        {
            Debug.Log("No cards left in the deck.");
            yield break;
        }

        int cardsToDraw = Mathf.Min(numberOfCards, cardInDeck.Count);

        for (int i = 0; i < cardsToDraw; i++)
        {
            int randomIndex = Random.Range(0, cardInDeck.Count);
            Card drawnCard = cardInDeck[randomIndex];

            if (i < drawnCardContainers.Count)
            {
                GameObject container = drawnCardContainers[i];
                Card cardInstance = Instantiate(drawnCard, container.transform);
                cardInstance.transform.localPosition = Vector3.zero;
                cardInstance.gameObject.SetActive(true);

                drawnCardList.Add(cardInstance);
                cardInDeck.RemoveAt(randomIndex);

                // Animate the card
                yield return StartCoroutine(DrawCardAnimation(cardInstance.gameObject, container));
            }
        }
    }

    // Animation coroutine for drawing a card
    IEnumerator DrawCardAnimation(GameObject card, GameObject targetContainer)
    {
        RectTransform cardRect = card.GetComponent<RectTransform>();
        RectTransform shuffleButtonRect = shuffleButton.GetComponent<RectTransform>();

        Vector3 startPos = shuffleButtonRect.position;
        Vector3 targetPos = targetContainer.transform.position;
        Vector2 startSize = cardRect.sizeDelta;

        // Start with height set to 0 (invisible)
        cardRect.sizeDelta = new Vector2(cardRect.sizeDelta.x, 0);
        cardRect.position = startPos;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Move the card to its target
            cardRect.position = Vector3.Lerp(startPos, targetPos, t);

            // Increase the height of the card as it moves up
            cardRect.sizeDelta = new Vector2(cardRect.sizeDelta.x, Mathf.Lerp(0, startSize.y, t));

            yield return null;
        }

        // Ensure it reaches the final position
        cardRect.position = targetPos;
        cardRect.sizeDelta = new Vector2(cardRect.sizeDelta.x, startSize.y);
    }

    // Animation for shuffling cards back into the deck
    IEnumerator ShuffleAnimation()
    {
        foreach (Card card in drawnCardList)
        {
            Card cardInstance = Instantiate(card);
            GameObject cardObj = card.gameObject;
            GameObject cardContainer = cardObj.transform.parent.gameObject;

            cardInDeck.Add(cardInstance);

            yield return StartCoroutine(ShuffleCardAnimation(cardObj, cardContainer));
        }

        drawnCardList.Clear();

        // Shuffle deck after the animation completes
        StartCoroutine(DrawCards(3));
    }

    // Animation coroutine for shuffling a card
    IEnumerator ShuffleCardAnimation(GameObject card, GameObject sourceContainer)
    {
        RectTransform cardRect = card.GetComponent<RectTransform>();
        RectTransform shuffleButtonRect = shuffleButton.GetComponent<RectTransform>();

        Vector3 startPos = sourceContainer.transform.position;
        Vector3 targetPos = shuffleButtonRect.position;
        Vector2 startSize = cardRect.sizeDelta;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Move the card to the shuffle button
            cardRect.position = Vector3.Lerp(startPos, targetPos, t);

            // Decrease the height of the card as it moves toward the shuffle button
            cardRect.sizeDelta = new Vector2(cardRect.sizeDelta.x, Mathf.Lerp(startSize.y, 0, t));

            yield return null;
        }

        Destroy(card.gameObject);  // Remove the card after it is shuffled
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
