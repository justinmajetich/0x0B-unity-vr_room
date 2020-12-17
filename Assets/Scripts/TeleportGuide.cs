using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportGuide : MonoBehaviour
{
    [SerializeField] Transform targetSprite;
    Vector3 targetEulersConstant;
    [SerializeField] Transform bezierControl;
    [SerializeField] int lineSegments = 100;
    [SerializeField] float lineWidth = 0.01f;
    [SerializeField, Tooltip("Line Renderer will be automatically assigned at runtime.")]
    LineRenderer guide;


    void Start()
    {
        guide = GetComponent<LineRenderer>();
        guide.positionCount = lineSegments;
        guide.startWidth = lineWidth;
        guide.endWidth = lineWidth / 2;

        if (!bezierControl) {
            bezierControl = transform.Find("BezierControl");
        }
        if (!targetSprite) {
            targetSprite = transform.Find("TargetSprite");
        }

        // Store initial rotation of target.
        targetEulersConstant = new Vector3(targetSprite.rotation.eulerAngles.x, 0f,
                                           targetSprite.rotation.eulerAngles.z);
    }

    public void DrawGuide(Vector3 target) {

        // Define start and end points for guide.
        Vector3 guideStart = transform.position;
        Vector3 guideEnd = target;
        float guideDistance = Vector3.Distance(guideStart, guideEnd);

        // Update position of bezier control based on distance between start and end.
        bezierControl.localPosition = new Vector3(bezierControl.localPosition.x, -(1 - Mathf.Clamp(guideDistance * 0.1f, 0.6f, 10f)), guideDistance * 0.5f);

        // Optimize position count relative to guide distance.
        lineSegments = (int)(guideDistance * 25);
        guide.positionCount = lineSegments;

        // Create an array to store pre-computed points for Line Renderer.
        Vector3[] points = new Vector3[lineSegments];
        float t = 0;

        // Calculate point positioning along a bezier curve and store in array.
        for (int i = 0; i < lineSegments; i++) {
            
            Vector3 bezier = (1 - t) * (1 - t) * guideStart + 2 * (1 - t) * t * bezierControl.position + t * t * guideEnd;
            
            points[i] = new Vector3(bezier.x, bezier.y, bezier.z);

            t += 1 / (float)lineSegments;
        }

        // Apply pre-computed point to Line Renderer.
        guide.SetPositions(points);

        // Draw sprite on guide target.
        DrawTargetSprite(target);
    }

    void DrawTargetSprite(Vector3 target) {
        // Draw target sprite.
        targetSprite.position = target;

        // Zero out target sprite's rotation in world space.
        // This cancels out the parent's (i.e. touch controller) rotation.
        targetSprite.rotation = Quaternion.Euler(targetEulersConstant.x,
                                                 transform.rotation.eulerAngles.y,
                                                 targetEulersConstant.z);
    }

    public void Reset() {
        guide.positionCount = 0;
    }
}
