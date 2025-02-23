using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerGrabFishingRod : MonoBehaviour
{
    private float grabDistance = 3f;
    public InputActionAsset inputActions;

    private InputAction grabAction;
    private Camera playerCamera;
    public FishingRod fishingRod;

    private void Start()
    {
        playerCamera = Camera.main;
        var playerMap = inputActions.FindActionMap("Player");

        grabAction = playerMap.FindAction("Interact");

        grabAction.Enable();
        grabAction.performed += OnGrab;
    }

    private void OnGrab(InputAction.CallbackContext context)
    {
        if (fishingRod == null)
        {
            Debug.Log("No fishing rod");
            RaycastHit hit;
            Vector3 rayOrigin = playerCamera.transform.position;
            Vector3 rayDirection = playerCamera.transform.forward;
            Debug.Log($"Ray origin: {rayOrigin}, Ray direction: {rayDirection}");

            if (Physics.Raycast(rayOrigin, rayDirection, out hit, grabDistance))
            {
                Debug.Log($"Hit object: {hit.collider.name}, Tag: {hit.collider.tag}");
                if (hit.collider.CompareTag("fishRod"))
                {
                    Debug.Log("is Fish rod");
                    fishingRod = hit.collider.GetComponent<FishingRod>();
                    if (fishingRod != null)
                    {
                        fishingRod.Grab();
                    }
                }
            }
        }
        else
        {
            fishingRod.Release();
            fishingRod = null;
        }
    }

}

