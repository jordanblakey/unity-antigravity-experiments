// 12/25/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

public class ObjectBobUpDown : MonoBehaviour
{
    public float bobSpeed = 2f; // Speed of the bobbing motion
    public float bobHeight = 0.5f; // Height of the bobbing motion

    private Vector3 startPosition;

    void Start()
    {
        // Store the initial position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new position using a sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}