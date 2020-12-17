using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Teleportation : MonoBehaviour
{
    [SerializeField] Transform playerRig;
    [SerializeField] Transform raycaster;
    [SerializeField, Tooltip("Required distance between teleport target and collider in front of it.")]
    float targetBuffer = 1f;

    Vector3 targetPos;
    RaycastHit hit;
    TeleportGuide guide;

    [SerializeField] Material fade;

    [SerializeField, Tooltip("Duration of fade out.")]
    float fadeOutTime = 0.075f;
    [SerializeField, Tooltip("Duration of fade in.")]
    float fadeInTime = 0.150f;


    void Start()
    {
        // Assign references where unassigned.
        if (!playerRig) { playerRig = GameObject.Find("OVRCameraRig").transform; }
        if (!raycaster) { raycaster = transform.Find("Raycaster"); }
        if (!fade) { fade = GameObject.Find("Fade").GetComponentInChildren<MeshRenderer>().material; }

        // Set initial fade color to transparent black.
        fade.color = new Color(0, 0, 0, 0);

        // Initialize guide Line Renderer values.
        guide = GetComponentInChildren<TeleportGuide>();

    }

    private void FixedUpdate() {
        // Calculate a valid teleport target with raycast.
        GetTeleportTarget();
    }

    void Update()
    {
        // Draw GUI teleport guide, using Line Renderer.
        guide.DrawGuide(targetPos);

        // On teleport action, teleport to new position.
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) {
            Teleport();
        }
    }

    void GetTeleportTarget() {

        // Cast ray to find a target position tagged Teleportable.
        if (Physics.Raycast(raycaster.position, raycaster.forward, out hit, 20f)) {

            // Debug.DrawRay(raycaster.position, raycaster.forward * 20f, Color.red);

            // If hit is Teleportable, set teleport target to hit position.
            if (hit.collider.tag == "Teleportable") {
                // Debug.DrawRay(hit.point, Vector3.up, Color.yellow);
                targetPos = hit.point;

            } 
            // If ray makes collision with object that's not floor, probe for 
            // nearest teleport target along path back to player rig.
            else {
                // Debug.DrawRay(raycaster.position, raycaster.forward * 20f, Color.blue);

                // Point to walk back toward player, source for ray cast.
                Vector3 probe = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                // Point at which probe will be considered over.
                Vector3 probeEnd = new Vector3(playerRig.position.x, 1.8f, playerRig.position.z);

                while (Vector3.Distance(probeEnd, probe) > 1f) {

                    RaycastHit probeHit;

                    // Step toward player rig.
                    probe = Vector3.MoveTowards(probe, probeEnd, 0.2f);

                    // Cast ray downward to probe for Teleportable tag.
                    if (Physics.Raycast(probe, -Vector3.up, out probeHit, 20f)) {
                        // Debug.DrawRay(probe, -Vector3.up * 10f, Color.green);

                        if (probeHit.collider.tag == "Teleportable") {
                            Debug.DrawRay(probeHit.point, Vector3.up, Color.yellow);
                            targetPos = probeHit.point;
                            break;
                        }
                    }
                }
            }
        }
    }

    // Ensure there is enough space between hit and collider in front of it.
    // Vector3 BufferTargetPosition(Vector3 hit) {

    //     // Additional spacing need to meet target buffer requirements.
    //     float additionalSpacing = 0f;
    //     Vector3 raycastPosition = hit + (Vector3.up * 0.2f);
    //     Vector3 raycastDirection = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * Vector3.forward;
    //     RaycastHit spacingHit;

    //     if (Physics.Raycast(raycastPosition, raycastDirection, out spacingHit, targetBuffer)) {
    //         // Debug.DrawRay(raycastPosition, raycastDirection * targetBuffer, Color.yellow);
    //         // Debug.DrawRay(spacingHit.point, Vector3.up, Color.yellow);

    //         // Calculate the magnitude needed to space target the given distance from collider.
    //         additionalSpacing = targetBuffer - Vector3.Distance(raycastPosition, spacingHit.point);
    //     }

    //     // Move target backward along its Z axis to meet required spacing from collider.
    //     return Vector3.MoveTowards(hit, -raycastDirection, additionalSpacing);
    // }

    void Teleport() {
        QuestDebug.Instance.Log("Teleporting!");
        StartCoroutine(TeleportTransition());
    }

    IEnumerator TeleportTransition() {
        
        // Lerp between 0 and 1 alpha over time.
        // Variable t tracks interpolation value of Lerp.
        for (float t = 0f; t <= 1f; t += Time.deltaTime / fadeOutTime) {

            // Initialize intermediate color/alpha.
            Color newColor = new Color(0, 0, 0, t);
            fade.color = newColor;

            yield return null;
        }

        // Change player position to teleport target.
        Vector3 offset = targetPos - playerRig.position;
        playerRig.position += offset;

        // Lerp from 1 to 0 alpha over time.
        for (float t = 0f; t <= 1f; t += Time.deltaTime / fadeInTime) {

            float newAlpha = Mathf.Lerp(1, 0, t);
            Color newColor = new Color(0, 0, 0, newAlpha);
            fade.color = newColor;

            yield return null;
        }
        // Make sure alpha has ended at 0.
        fade.color = new Color(0, 0, 0, 0);
    }

    private void OnDisable() {
        guide.Reset();
    }

    private void OnEnable() {
        GetTeleportTarget();
    }
}
