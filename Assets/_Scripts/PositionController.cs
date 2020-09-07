using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Service to control position of VisualNotes
/// </summary>
public class PositionController : MonoBehaviour
{
    public static PositionController instance;

    public float lerpVelocity = 5f;
    public float amplitude = 0.1f;

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
        PositionProvider.UpdateBoundaries();
        PositionProvider.SetUpPossiblePositions(Orientation.VERTICAL);
    }

    /// <summary>
    /// Moves to a desired position setting targetPosition to a new value to interpolate
    /// </summary>
    /// <param name="positionIndex">Bijection with notes index (see LivePlayer.cs)</param>
    public static void MoveNoteToPosition(VisualNote noteToMove, int positionIndex)
    {
        List<Vector3> possiblePositions = PositionProvider.possiblePositions;
        if (possiblePositions == null || positionIndex >= possiblePositions.Count || possiblePositions[positionIndex] == null)
        {
            Debug.LogWarning("Sending to nonexisting position");
            return;
        }

        noteToMove.targetPosition = possiblePositions[positionIndex];
    }
}
