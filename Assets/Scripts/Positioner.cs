using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provider for providing position information
/// </summary>
public static class PositionProvider
{
    public static List<Vector3> possiblePositions = new List<Vector3>();

    public static void UpdateBoundaries()
    {
        Camera cam = Camera.main;
        double vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;
        double horzExtent = vertExtent * Screen.width / Screen.height;

        Boundaries.minX = -horzExtent;
        Boundaries.maxX = horzExtent;
        Boundaries.minY = -vertExtent;
        Boundaries.maxY = vertExtent;

        Boundaries.deltaX = horzExtent * 2;
        Boundaries.deltaY = vertExtent * 2;

        Boundaries.initialized = true;
    }

    public static void SetUpPossiblePositions(Orientation orientation)
    {
        if (!Boundaries.initialized)
        {
            Debug.LogError("Boundaries not initialized");
        }

        double delta, min;
        bool isHorizontal = false;
        if (orientation == Orientation.HORIZONTAL)
        {
            delta = Boundaries.deltaX;
            min = Boundaries.minX;
            isHorizontal = true;
        }
        else
        {
            delta = Boundaries.deltaY;
            min = Boundaries.minY;
        }

        // Get total of notes available (counting spaces between octaves and nonexisting tones as E# and B#)
        int octaves = 1;    // TODO get from audio manager
        double positionsCount = (
            octaves * 13 +
            ((octaves > 1) ? (octaves - 1) : 0) +
            2
        );
        double distanceBetween = delta / positionsCount;

        for (int i = 0; i < positionsCount; i++)
        {
            // Removing positions of nonexisting tones E# and B# (do not exists)
            if (i % 14 != 0 && i % 14 != 6)
            {
                Vector3 possiblePosition = Vector3.zero;
                float pos = (float)(min + i * distanceBetween);
                if (isHorizontal)
                {
                    possiblePosition.x = pos;
                }
                else
                {
                    possiblePosition.y = pos;
                }
                possiblePositions.Add(possiblePosition);
            }
        }
    }
}

public static class Boundaries
{
    public static double minX, maxX, minY, maxY, deltaX, deltaY;
    public static bool initialized = false;
}

public enum Orientation
{
    VERTICAL = 0,
    HORIZONTAL = 1
}
