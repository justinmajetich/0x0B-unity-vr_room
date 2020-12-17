using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject teleportControl;
    [SerializeField] GameObject interactionControl;

    void Start()
    {
        // If unassigned, find and assign teleport control.
        if (!teleportControl) { teleportControl = GameObject.Find("TeleportControl"); }
        if (!interactionControl) { interactionControl = GameObject.Find("InteractionControl"); }
    }

    void Update()
    {
        // Activate teleport control object is R thumstick is moved upwards by 50%+.
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp) || Input.GetKey(KeyCode.Space)) {
            teleportControl.SetActive(true);
            interactionControl.SetActive(false);
        } else {
            teleportControl.SetActive(false);
            interactionControl.SetActive(true);
        }
    }
}
