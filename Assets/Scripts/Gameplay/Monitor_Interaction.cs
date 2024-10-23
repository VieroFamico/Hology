using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class Monitor_Interaction : MonoBehaviour
{
    public Camera_Manager cameraManager;
    public CinemachineVirtualCamera virtualCamera;
    public float originalFOV = 55f;
    public float zoomedFOV = 40f;
    public float zoomSpeed = 2f;  // Speed for smooth FOV transition
    public LayerMask monitorLayer;  // Layer to identify monitors
    public float raycastDistance = 100f;

    public Monitor leftMonitor;
    public Monitor centerMonitor;
    public Monitor rightMonitor;


    private bool isZoomedIn = false;
    private Monitor zoomedInMonitor;
    private Monitor hoveredMonitor;

    void Start()
    {
        cameraManager = Camera_Manager.instance;
    }

    void Update()
    {
        if (!General_Game_Manager.instance.dayHasStarted) return;

        if(!isZoomedIn)
        {
            HandleMonitorSelection();
        }
        
        if (isZoomedIn && Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ZoomOut());
            zoomedInMonitor.SetZoomStatus(false);  // Notify the monitor it's no longer zoomed in
            zoomedInMonitor = null;
            isZoomedIn = false;
            General_Game_Manager.instance.isInMiniGames = false;
        }

    }

    private void HandleMonitorSelection()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Monitor monitor = hit.collider.GetComponent<Monitor>();

            if (hoveredMonitor)
            {
                DeHighligtMonitor(hoveredMonitor);
            }

            if (monitor != null && GetCurrentMonitor(Camera_Manager.instance.GetCameraPos()) == monitor)  // Only interact if it's the current monitor
            {
                hoveredMonitor = monitor;

                // Highlight the monitor with outline or effects
                HighlightMonitor(hoveredMonitor);

                // Zoom in on the monitor if clicked
                if (Input.GetMouseButtonDown(0))
                {
                    ZoomInOnMonitor(hoveredMonitor);
                    DeHighligtMonitor(hoveredMonitor);
                }
            }
            else
            {
                if (hoveredMonitor)
                {
                    DeHighligtMonitor(hoveredMonitor);
                }
            }
        }
        else
        {
            if (hoveredMonitor)
            {
                DeHighligtMonitor(hoveredMonitor);
            }
        }
    }

    private Monitor GetCurrentMonitor(int monitorId)
    {
        if(monitorId == 0)
        {
            return leftMonitor;
        }
        else if (monitorId == 1)
        {
            return centerMonitor;
        }
        else if(monitorId == 2)
        {
            return rightMonitor;
        }
        else
        {
            Debug.Log("Outside of Monitor Index Error, returning centerMonitor Value");
            return centerMonitor;
        }
    }
    
    private void HighlightMonitor(Monitor monitor)
    {
        monitor.outline.enabled = true;
    }

    private void DeHighligtMonitor(Monitor monitor)
    {
        monitor.outline.enabled = false;
    }

    private void ZoomInOnMonitor(Monitor monitor)
    {
        if (!General_Game_Manager.instance.isInMiniGames)
        {
            StartCoroutine(ZoomIn());
            monitor.SetZoomStatus(true);
            zoomedInMonitor = monitor;
            isZoomedIn = true;
            General_Game_Manager.instance.isInMiniGames = true;
        }
    }

    IEnumerator ZoomIn()
    {
        isZoomedIn = true;
        cameraManager.HandleChangingCamera(false);  // Disable camera switching when zoomed in

        float currentFOV = virtualCamera.m_Lens.FieldOfView;
        while (Mathf.Abs(currentFOV - zoomedFOV) > 0.1f)
        {
            currentFOV = Mathf.Lerp(currentFOV, zoomedFOV, Time.deltaTime * zoomSpeed);
            virtualCamera.m_Lens.FieldOfView = currentFOV;
            yield return null;
        }

        virtualCamera.m_Lens.FieldOfView = zoomedFOV;  // Ensure it ends exactly at zoomed FOV
    }

    IEnumerator ZoomOut()
    {
        isZoomedIn = false;
        cameraManager.HandleChangingCamera(true);  // Re-enable camera switching

        float currentFOV = virtualCamera.m_Lens.FieldOfView;

        Debug.Log(currentFOV);

        while (Mathf.Abs(currentFOV - originalFOV) > 0.1f)
        {
            currentFOV = Mathf.Lerp(currentFOV, originalFOV, Time.deltaTime * zoomSpeed);
            virtualCamera.m_Lens.FieldOfView = currentFOV;
            yield return null;
        }
        Debug.Log(originalFOV);
        
        virtualCamera.m_Lens.FieldOfView = originalFOV;  // Restore to default FOV

        Debug.Log(virtualCamera.m_Lens.FieldOfView);
    }

}
