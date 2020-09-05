using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionController : MonoBehaviour
{
    private List<double> possiblePositions = new List<double>();

    public static PositionController instance;

    private Vector3 targetPosition;

    public float lerpVelocity = 5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initial position
        targetPosition = Vector3.zero;

        // Getting boundaries from our camera size
        Positioner.UpdateBoundaries();

        // Get total of notes available (counting spaces between octaves and nonexisting tones as E# and B#)
        double positionsCount = (
            AudioManager.instance.octaves * 13 +
            ((AudioManager.instance.octaves > 1) ? (AudioManager.instance.octaves - 1) : 0) +
            2
        );
        double distanceBetween = Boundaries.deltaY / positionsCount;

        for (int i = 0; i < positionsCount; i++)
        {
            // Removing positions of nonexisting tones E# and B# (do not exists)
            if (i % 14 != 0 && i % 14 != 6)
            {
                possiblePositions.Add(Boundaries.minY + i * distanceBetween);
            }
        }
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Moves to a desired position setting targetPosition to a new value to interpolate
    /// </summary>
    /// <param name="index">Represents the index of the note (see LivePlayer.cs)</param>
    public void MoveToPosition(int index)
    {
        if (index >= possiblePositions.Count)
        {
            Debug.LogWarning("Sending to nonexisting position");
            return;
        }

        targetPosition = new Vector3(
            transform.position.x,
            (float)possiblePositions[index],
            transform.position.z
        );
    }
}
