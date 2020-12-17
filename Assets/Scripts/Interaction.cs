using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public OVRInput.Controller controller;
    InteractionType interactionType = InteractionType.None;

    [SerializeField, Tooltip("Maximum distance at which an object can be interacted with.")]
    float reach = 1.5f;
    RaycastHit hit;

    InteractionRecticle recticle;
    LineRenderer line;
    float lineWidth = 0.01f;
    float maxLineLength = 0.25f;

    Transform grabbedItem;
    Transform grabbedItemOriginalParent;
    GrabInfo currentItemGrabInfo = null;
    Transform grabPosition;
    bool isGrabbing = false;

    private void Start() {
        grabPosition = transform.Find("GrabPosition");

        // Assign/setup line renderer.
        line = transform.GetComponentInChildren<LineRenderer>();
        line.startWidth = lineWidth;
        line.endWidth = lineWidth / 2;
        line.enabled = true;

        recticle = transform.GetComponentInChildren<InteractionRecticle>();

        QuestDebug.Instance.Log($"Active Controller: {controller}");
    }

    private void FixedUpdate() {

        // If item is not grabbed search for an interaction.
        if (!isGrabbing) {
            SeekInteractable();
        } else {
            // No interaction is possible while item is grabbed.
            interactionType = InteractionType.None;
        }
    }

    private void SeekInteractable() {

        // Cast ray to detect usable/grabbable object within reach.
        if (Physics.Raycast(transform.position, transform.forward, out hit, reach)) {

            // Set interaction type based on tag of hit collider.
            if (hit.collider.tag == "Grabbable") {
                interactionType = InteractionType.Grabbable;
            } else if (hit.collider.tag == "Usable") {
                interactionType = InteractionType.Usable;
            } else {
                interactionType = InteractionType.Default;
            }

            // Set recticle position to collision point.
            recticle.SetPosition(hit.point, transform.position);

        } else {
            // If no collision detected, set interaction to none.
            interactionType = InteractionType.None;
        }
    }

    private void Update() {

        // Set recticle sprite based on interaction detected in FixedUpdate.
        recticle.SetSprite(interactionType);

        if (isGrabbing) {

            // Disable Line Renderer if item is grabbed.
            line.enabled = false;

            // If grab button is released, release grabbed item.
            if (!OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, controller)) {
                ReleaseItem();
            }
        } else {

            // Enable Line Renderer if item is grabbed.
            line.enabled = true;
            DrawLine();

            // If collision with usable item, check for use input.
            if (interactionType == InteractionType.Usable && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller)) {
                Use();
            }

            // If collision with grabbable item, check for interact input.
            if (interactionType == InteractionType.Grabbable && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller)) {
                QuestDebug.Instance.Log("Grabbable: " + hit.transform);
                GrabItem(hit.transform);
            }
        }
    }

    private void Use() {
        QuestDebug.Instance.Log($"I'm using: {hit.collider.name}!");
        hit.collider.GetComponent<IUsable>().Use();
    }

    private void GrabItem(Transform item) {
    
        // If the item is currently being grabbed by other controller, call it's release method.
        currentItemGrabInfo = item.GetComponent<GrabInfo>();

        if (currentItemGrabInfo.isGrabbed) {
            currentItemGrabInfo.grabbingController.ReleaseItem();
        }

        // Store reference to newly grabbed item.
        grabbedItem = item;

        // Store reference to item's parent object in order to restore heirarchy when item is released.
        // But first, we check to see if item is passed from grabPosition of opposite controller.
        if (!grabbedItemOriginalParent || grabbedItemOriginalParent.name != "GrabPosition") {
            grabbedItemOriginalParent = grabbedItem.parent;
            QuestDebug.Instance.Log($"Origin Parent: {grabbedItemOriginalParent}");
        }

        // Parent grabbed item to controller's grab position. This easily handles position/rotation changes while grabbed.
        grabbedItem.parent = grabPosition;
        grabbedItem.position = new Vector3(grabPosition.position.x, grabPosition.position.y, grabPosition.position.z);

        // Get reference to item's rigidbody and disable physics interactions by marking isKinematic as true.
        Rigidbody itemRB = grabbedItem.GetComponent<Rigidbody>();
        if (itemRB) {
            itemRB.isKinematic = true;
        }

        // Set item's grab info properties.
        currentItemGrabInfo.grabbingController = this;
        currentItemGrabInfo.isGrabbed = true;

        // Mark this controller as grabbing.
        isGrabbing = true;
    }

    public void ReleaseItem() {

        // Get reference to item's rigidbody.
        Rigidbody itemRB = grabbedItem.GetComponent<Rigidbody>();

        // Set isKinematic back to false to enable physics interactions.
        if (itemRB) {
            itemRB.isKinematic = false;
        }

        // Apply current velocity and angular velocity to item being released.
        itemRB.velocity = OVRInput.GetLocalControllerVelocity(controller);
        itemRB.angularVelocity = -OVRInput.GetLocalControllerAngularVelocity(controller);

        // Restore item to it's original parent and remove reference to object.
        grabbedItem.parent = grabbedItemOriginalParent;
        grabbedItem = null;

        // Clear item's grab info properties.
        currentItemGrabInfo.grabbingController = null;
        currentItemGrabInfo.isGrabbed = false;
        currentItemGrabInfo = null;


        // Mark this controller as no longer grabbing.
        isGrabbing = false;
    }

    void DrawLine() {

        float lineLength = maxLineLength;

        // If raycast hit is closer than max line length, set length to hit distance.
        if (hit.collider && hit.distance <= 0.5f) {
            float difference = 0.5f - hit.distance;
            lineLength = Mathf.Clamp(0.5f - (difference * 2f), 0.03f, maxLineLength);
        }

        QuestDebug.Instance.Log($"Line Length: {lineLength}");

        // Create an array to store pre-computed points for Line Renderer.
        Vector3[] points = new Vector3[10];

        // Calculate point positioning and store in array.
        for (int i = 0; i < 10; i++) {
            points[i] = new Vector3(0f, 0f, (lineLength / 10) * (i + 1));
        }

        line.SetPositions(points);
    }
}