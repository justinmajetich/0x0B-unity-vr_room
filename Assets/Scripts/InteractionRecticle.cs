using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionRecticle : MonoBehaviour
{
    public OVRInput.Controller controller;
    SpriteRenderer m_Renderer;
    [SerializeField] Sprite defaultSprite = null;
    [SerializeField] Sprite grabbableSprite = null;
    [SerializeField] Sprite usableSprite = null;

    [SerializeField, Tooltip("Player camera. Automatically assigned if not assigned in editor.")]
    Transform rotationAnchor;

    [SerializeField, Tooltip("Distance between recticle position and surface of collided object.")]
    float recticleToCollisionSpacing = 0.2f;


    void Start()
    {
        m_Renderer = GetComponent<SpriteRenderer>();

        // Flip sprite to represent appropriate hand.
        if (controller == OVRInput.Controller.LTouch) {
            m_Renderer.flipX = true;
        }

        // Find appropriate anchor for given hand.
        string anchorName = (controller == OVRInput.Controller.LTouch ? "LeftControllerAnchor" : "RightControllerAnchor");
        if (!rotationAnchor) { rotationAnchor = GameObject.Find(anchorName).transform; }

        QuestDebug.Instance.Log($"Recticle Set: {controller}");
        QuestDebug.Instance.Log($"Rotation Anchor for {controller}: {rotationAnchor.name}");
    }

    void Update()
    {
        // Set rotation to camera rotation, sprite will always appear parallel to view plane.
        transform.rotation = Quaternion.Euler(rotationAnchor.rotation.eulerAngles.x, rotationAnchor.rotation.eulerAngles.y, rotationAnchor.rotation.eulerAngles.z);
    }

    /// <summary>
    /// Set recticle sprite based on the interaction type.
    /// </summary>
    /// <param name="type">Type of interaction detected by controller.</param>
    public void SetSprite(InteractionType type) {

        switch (type)
        {
            case InteractionType.Default:
                m_Renderer.sprite = defaultSprite;
                break;
            case InteractionType.Grabbable:
                m_Renderer.sprite = grabbableSprite;
                break;
            case InteractionType.Usable:
                m_Renderer.sprite = usableSprite;
                break;
            case InteractionType.None:
                m_Renderer.sprite = null;
                break;
            default:
                m_Renderer.sprite = null;
                break;
        }
    }

    /// <summary>
    /// Sets the position of the recticle according to new collision data.
    /// </summary>
    /// <param name="hitPosition">Position of raycast hit.</param>
    /// <param name="anchor">Position of anchor object (object which cast ray), used to determine spacing from collided object.</param>
    public void SetPosition(Vector3 hitPosition, Vector3 anchor) {

        // Calculate a new position for recticle that is backed off from actual collision point.
        // This avoids recticle clipping into collided object.
        Vector3 newPosition = Vector3.MoveTowards(hitPosition, anchor, recticleToCollisionSpacing);

        transform.position = newPosition;
    }
}