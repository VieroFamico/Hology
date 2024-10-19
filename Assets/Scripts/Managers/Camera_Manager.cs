using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camera_Manager : MonoBehaviour
{
    public static Camera_Manager instance;

    public CinemachineVirtualCamera virtualCamera;
    public Transform leftPosition;  // The transform of the left position
    public Transform centerPosition; // The transform of the center position
    public Transform rightPosition; // The transform of the right position
    public float cameraMoveSpeed = 2f; // Speed for the camera to move to target

    public Button leftButton;  // Left button UI
    public Button rightButton; // Right button UI
    public float uiShowThreshold = 0.05f; // 1/20th of the screen width = 0.05 (5% of the screen width)

    private Transform currentTarget;
    private Coroutine moveCoroutine;
    private int currPos = 1; // 0 = left, 1 = center, 2 = right

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Set the initial camera target to the center screen position or a default value
        currentTarget = centerPosition ? centerPosition : virtualCamera.transform;
        MoveCameraToTarget(currPos);  // Start at the center

        // Initially disable the buttons (they will show when mouse gets close to the screen edges)
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);

        // Add button click listeners
        leftButton.onClick.AddListener(() => MoveCameraToTarget(currPos - 1));
        rightButton.onClick.AddListener(() => MoveCameraToTarget(currPos + 1));
    }

    void Update()
    {
        if (!General_Game_Manager.instance.dayHasStarted)
        {
            return;
        }

        // Calculate the threshold in pixels based on screen width
        float screenThreshold = Screen.width * uiShowThreshold;

        // Show or hide the left button
        if (Input.mousePosition.x <= screenThreshold)
        {
            if (currPos > 0) // Show only if not at the leftmost position
            {
                leftButton.gameObject.SetActive(true);
            }
        }
        else
        {
            leftButton.gameObject.SetActive(false);
        }

        // Show or hide the right button
        if (Input.mousePosition.x >= Screen.width - screenThreshold)
        {
            if (currPos < 2) // Show only if not at the rightmost position
            {
                rightButton.gameObject.SetActive(true);
            }
        }
        else
        {
            rightButton.gameObject.SetActive(false);
        }
    }

    void MoveCameraToTarget(int target)
    {
        // Prevent invalid target positions
        if (target < 0 || target > 2) return;

        // Stop any previous movement coroutine before starting a new one
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        // Move camera based on the current target
        if (target == 0 && currPos != 0)
        {
            currPos = 0;
            moveCoroutine = StartCoroutine(LerpCameraToTarget(leftPosition));
            currentTarget = leftPosition;
        }
        else if (target == 1 && currPos != 1)
        {
            currPos = 1;
            moveCoroutine = StartCoroutine(LerpCameraToTarget(centerPosition));
            currentTarget = centerPosition;
        }
        else if (target == 2 && currPos != 2)
        {
            currPos = 2;
            moveCoroutine = StartCoroutine(LerpCameraToTarget(rightPosition));
            currentTarget = rightPosition;
        }
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
    }

    IEnumerator LerpCameraToTarget(Transform target)
    {
        Vector3 startPos = virtualCamera.transform.position;
        Quaternion startRot = virtualCamera.transform.rotation;
        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            virtualCamera.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime);
            virtualCamera.transform.rotation = Quaternion.Lerp(startRot, endRot, elapsedTime);
            elapsedTime += Time.deltaTime * cameraMoveSpeed; // Adjust by speed

            yield return null;
        }

        // Ensure final position and rotation is exactly at target
        virtualCamera.transform.position = endPos;
        virtualCamera.transform.rotation = endRot;
    }

    public int GetCameraPos()
    {
        return currPos;
    }
}
