using System.Collections.Generic;
using UnityEngine;

public class bottomCollisionScript : MonoBehaviour
{
    public LayerMask solidSurfaces; // Layer mask for solid surfaces
    public BoxCollider2D boxCollider2D;
    public float boxSize = 1.44f; // Size of the box for the BoxCast
    private float boxHeight = 0.1f; // Height of the box for the BoxCast
    private float checkDistance = 0.01f; // Distance to check for collisions
    private bool isGrounded;

    void Update()
    {
        // Calculate the center of the box for the BoxCast
        Vector2 boxCenter = new Vector2(transform.position.x, transform.position.y + boxCollider2D.offset.y + 0.04f);

        // Perform the BoxCastAll
        RaycastHit2D[] hits = Physics2D.BoxCastAll(boxCenter, new Vector2(boxSize, boxHeight), 0f, Vector2.down, checkDistance, solidSurfaces);

        // Initialize isGrounded as false
        isGrounded = false;

        // Iterate through all hits to check if any of them are grounded
        foreach (var hit in hits)
        {
            // Check if the hit collider is not null and the normal is pointing upwards
            if (hit.collider != null && Mathf.Abs(hit.normal.y) > 0)
            {
                isGrounded = true;

                // Calculate the penetration distance
                float penetrationDistance = Mathf.Abs(hit.distance);

                print(penetrationDistance);
                // Check if there is any penetration
                if (penetrationDistance > 0f)
                {
                    print("PD: " + penetrationDistance.ToString());
                    // Calculate the correction vector
                    Vector2 correctionVector = hit.normal * penetrationDistance;

                    // Move the character out of the collider
                    transform.position += new Vector3(correctionVector.x, correctionVector.y, 0f);
                }

                break; // Exit the loop once a grounded condition is found
            }
        }

        // Debug visualization of the BoxCast
        DebugBoxCast(boxCenter, new Vector2(boxSize, boxHeight), Vector2.down, checkDistance);
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    private void DebugBoxCast(Vector2 center, Vector2 size, Vector2 direction, float distance)
    {
        // Calculate the corners of the box
        Vector2 halfSize = size / 2;
        Vector2 topLeft = center + new Vector2(-halfSize.x, halfSize.y);
        Vector2 topRight = center + new Vector2(halfSize.x, halfSize.y);
        Vector2 bottomLeft = center + new Vector2(-halfSize.x, -halfSize.y);
        Vector2 bottomRight = center + new Vector2(halfSize.x, -halfSize.y);

        // Draw lines between the corners to visualize the box
        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        // Draw a line in the direction of the BoxCast to visualize the direction and distance
        Debug.DrawLine(center, center + direction * distance, Color.blue);
    }
}
