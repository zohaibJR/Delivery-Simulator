using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("Raycast Settings")]
    public Camera playerCamera;
    public float interactDistance = 2f;
    public LayerMask interactLayer;

    [Header("Interactable Tags")]
    public List<string> detectableTags = new List<string>() { "Package", "Door" };

    [Header("Pickup Settings")]
    public Transform holdPosition;  // Assign an empty GameObject in front of camera
    public float pickupSmoothness = 15f;

    private GameObject heldObject = null;
    private Rigidbody heldRb = null;

    [Header("UI Texts")]
    public GameObject PickupText;
    public GameObject dropText;

    public PlayerPickupManager playerPIckupManager;

    void Update()
    {
        if (heldObject == null)
        {
            DetectAndPickup();
        }
        else
        {
            HoldObject();
            DropCheck();
        }
    }

    void DetectAndPickup()
{
    // Always hide text by default
    PickupText.SetActive(false);

    Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
    RaycastHit hit;

    int layerMask = (interactLayer.value == 0) ? ~0 : interactLayer;

    if (Physics.Raycast(ray, out hit, interactDistance, layerMask))
    {
        string hitTag = hit.collider.tag;

        if (detectableTags.Contains(hitTag))
        {
            // Show text when looking at a valid object
            PickupText.SetActive(true);

            Debug.Log("Ray cast on: " + hitTag);

            // Press E to pick up
            if (Input.GetKeyDown(KeyCode.E) && hitTag == "Package")
            {
                PickupObject(hit.collider.gameObject);
                PickupText.SetActive(false);
            }
        }
    }
}

void PickupObject(GameObject obj)
{
    dropText.SetActive(true);
    heldObject = obj;
    heldRb = obj.GetComponent<Rigidbody>();

    // --- NEW LINE: Get and disable the Collider ---
    Collider packageCollider = obj.GetComponent<Collider>();
    if (packageCollider != null)
    {
        packageCollider.enabled = false;
    }
    // ------------------------------------------------

    PackageData data = obj.GetComponent<PackageData>();
    if (data != null)
    {
        Debug.Log($"Picked Package ID: {data.packageID}");
        Debug.Log($"Status: {data.status}");
        Debug.Log($"Address: {data.address}");

        // update status
        data.status = PackageStatus.Picked;
    }

    if (heldRb != null)
    {
        heldRb.useGravity = false;
        heldRb.isKinematic = true;
    }

    obj.transform.SetParent(holdPosition);
    obj.transform.localPosition = Vector3.zero;
    obj.transform.localRotation = Quaternion.identity;
    playerPIckupManager.ObjectPickedUp();

}


    void HoldObject()
    {
        // Smooth movement to hold position
        heldObject.transform.localPosition = Vector3.Lerp(
            heldObject.transform.localPosition,
            Vector3.zero,
            Time.deltaTime * pickupSmoothness
        );
    }

    void DropCheck()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Press Q to drop
        {
            DropObject();
        }
    }

    void DropObject()
{
    dropText.SetActive(false);
    if (heldObject == null) return;

    // --- NEW LINE: Get and re-enable the Collider ---
    Collider packageCollider = heldObject.GetComponent<Collider>();
    if (packageCollider != null)
    {
        packageCollider.enabled = true;
    }
    // ------------------------------------------------

    PackageData data = heldObject.GetComponent<PackageData>();
    if (data != null)
    {
        data.status = PackageStatus.Waiting;
    }

    heldObject.transform.SetParent(null);

    if (heldRb != null)
    {
        heldRb.isKinematic = false;
        heldRb.useGravity = true;
        heldRb.linearVelocity = Vector3.zero;
        heldRb.angularVelocity = Vector3.zero;
    }

    heldObject = null;
    heldRb = null;

    playerPIckupManager.ObjectDropped();
}

}
