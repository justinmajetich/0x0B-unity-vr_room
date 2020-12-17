using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    GrabInfo grabInfo;
    OVRInput.Controller lastGrabbingController;
    Light flashlight;
    bool grabbedRotationSet = false;

    void Start()
    {
        grabInfo = GetComponent<GrabInfo>();
        flashlight = GetComponentInChildren<Light>();
    }

    private void Update() {

        // If flashlight is grabbed, set rotation and check for player input.
        if (grabInfo.isGrabbed) {

            // If rotation hasn't been set to that of parent, set it.
            // The second condition here makes sure a rotation set takes place when the flashlight
            // is grabbed by one controller from another.
            if (!grabbedRotationSet || grabInfo.grabbingController.controller != lastGrabbingController) {
                SetGrabbedRotation();
            }

            // Toggle light on player input.
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, grabInfo.grabbingController.controller)) {
                ToggleLight();
            }

            lastGrabbingController = grabInfo.grabbingController.controller;

        } else {
            // When light is no longer grabbed, mark flag as false.
            grabbedRotationSet = false;
        }
    }

    public void ToggleLight() {
        QuestDebug.Instance.Log($"Light is: {flashlight}");
        flashlight.enabled = !flashlight.enabled;

        if (flashlight.enabled)
            QuestDebug.Instance.Log("Flashlight is ON");
        else
            QuestDebug.Instance.Log("Flashlight is OFF");
    }

    private void SetGrabbedRotation() {

        // Vector3 parentEulers = transform.parent.transform.rotation.eulerAngles;
        // transform.rotation = Quaternion.Euler(parentEulers.y, parentEulers.y, parentEulers.z);
        transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        // transform.Rotate(90f, 0f, 0f, Space.World);

        grabbedRotationSet = true;
    }
}
