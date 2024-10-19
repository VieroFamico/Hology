using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tools_Manager : MonoBehaviour
{
    [Header("Camera")]
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin vCamShake;

    [Header("Drill Tools")]
    public Button drillButton;
    public Transform drillRadiusIndicator;
    public float drillRadius;
    public float drillStabilityDecrease;
    public float drillDuration = 5f; // Time it takes for the drill to complete in seconds

    [Header("Acoustic Detector")]
    public Button acousticDetectorButton;
    public SpriteRenderer acousticDetectorIndicator;
    public float acousticDetector_MaxRadius;
    public float acousticDetector_MinRadius;
    public AudioSource acousticAudioSource;
    public LayerMask victimLayerMask;

    public LayerMask terrainLayerMask;

    private bool isUsingDrill = false;
    private bool isUsingAcoustic = false;

    private bool isDrilling = false;

    private float drillTimer = 0f;
    private Victim currentVictim;
    private Tanah_Longsor_Manager tanahLongsorManager;

    void Start()
    {
        vCamShake = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();


        // Add listeners to the buttons to switch tools
        drillButton.onClick.AddListener(() => SelectTool("Drill"));
        acousticDetectorButton.onClick.AddListener(() => SelectTool("Acoustic"));

        // Initialize the drill as off
        isUsingDrill = false;
        isUsingAcoustic = false;

        tanahLongsorManager = FindObjectOfType<Tanah_Longsor_Manager>();
    }

    void Update()
    {
        if (isUsingAcoustic)
        {
            HandleAcousticDetector();
        }

        if (isUsingDrill)
        {
            HandleDrilling();
        }
    }

    // Function to switch between tools
    void SelectTool(string toolName)
    {
        if (isDrilling)
        {
            return;
        }

        switch (toolName)
        {
            case "Drill":
                isUsingDrill = true;
                isUsingAcoustic = false;
                acousticAudioSource.Stop(); // Stop acoustic detector sound
                break;
            case "Acoustic":
                isUsingDrill = false;
                isUsingAcoustic = true;
                break;
        }
    }

    // Handle the Acoustic Detector functionality
    void HandleAcousticDetector()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayerMask))
        {
            // Check if we are over terrain
            if (hit.collider.CompareTag("Terrain"))
            {
                // Display an area around the mouse pointer like a radar
                Vector3 detectPosition = hit.point;

                acousticDetectorIndicator.transform.position = detectPosition;

                // Detect victims within range using a circular radius (like a radar sweep)
                Collider[] victims = Physics.OverlapSphere(detectPosition, acousticDetector_MaxRadius, victimLayerMask);

                // Play sound and give a visual cue for detection
                foreach (Collider victim in victims)
                {
                    currentVictim = victim.GetComponent<Victim>();
                    if (currentVictim != null)
                    {
                        float distanceToVictim = Vector3.Distance(detectPosition, currentVictim.transform.position);

                        // Adjust audio volume based on proximity to the victim
                        float volume = Mathf.Lerp(1f, 0f, distanceToVictim / acousticDetector_MaxRadius);
                        acousticAudioSource.volume = volume;

                        // Optionally, you can visualize proximity with color change (e.g., Red to Green)
                        Color proximityColor = Color.Lerp(Color.red, Color.green, 1 - (distanceToVictim / acousticDetector_MaxRadius));
                        // Update UI or radar visualization using this color
                        acousticDetectorIndicator.color = proximityColor;
                    }
                }
            }
        }
    }

    // Handle the drilling functionality
    void HandleDrilling()
    {
        if (currentVictim == null || isDrilling)
        {
            return; // No victim detected yet
        }

        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayerMask))
        {
            // Check if we are over terrain
            if (hit.collider.CompareTag("Terrain"))
            {
                // Display an area around the mouse pointer like a radar
                Vector3 drillPosition = hit.point;
                drillRadiusIndicator.position = drillPosition;
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    StartCoroutine(StartDrilling(drillPosition));
                }
                
            }
        }

    }

    private IEnumerator StartDrilling(Vector3 drillPosition)
    {
        drillTimer = 0;
        isDrilling = true;
        drillButton.enabled = false;
        acousticDetectorButton.enabled = false;

        vCamShake.m_FrequencyGain = 1;

        while (drillTimer < drillDuration)
        {
            drillTimer += Time.deltaTime;
            tanahLongsorManager.currStability -= drillStabilityDecrease * Time.deltaTime;
            yield return null;
        }

        tanahLongsorManager.currStability = drillStabilityDecrease * drillDuration;

        float distanceToVictim = Vector3.Distance(drillPosition, currentVictim.transform.position);

        if (distanceToVictim < drillRadius)
        {
            RescueVictim(currentVictim);
        }
        else
        {

        }

        isDrilling = false;
        drillButton.enabled = true;
        acousticDetectorButton.enabled = true;

        vCamShake.m_FrequencyGain = 0;
    }

    // Rescue the victim
    void RescueVictim(Victim victim)
    {
        // Logic for rescuing a victim, like despawning the victim and updating score
        Destroy(victim.gameObject);
        currentVictim = null;
        tanahLongsorManager.CurrentVictimRescued(); // Spawn the next victim
    }

    public void SetVictim(Victim victim)
    {
        currentVictim = victim;
    }
}
