using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeRotateUI : MonoBehaviour, IDragHandler
{
    public Transform cube;
    public float rotateSpeed = 0.3f;
    public float damping = 5f; // higher = faster slowdown

    private Vector2 rotationVelocity;

    public void OnDrag(PointerEventData eventData)
    {
        rotationVelocity.x = eventData.delta.y * rotateSpeed;
        rotationVelocity.y = -eventData.delta.x * rotateSpeed;
    }

    void Update()
    {
        // Apply rotation
        cube.Rotate(Vector3.right, rotationVelocity.x, Space.World);
        cube.Rotate(Vector3.up, rotationVelocity.y, Space.World);

        // Smoothly slow down
        rotationVelocity = Vector2.Lerp(rotationVelocity, Vector2.zero, damping * Time.deltaTime);
    }
}
