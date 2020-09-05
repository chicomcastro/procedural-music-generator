using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Positioner
{
    public static void UpdateBoundaries()
    {
        Camera cam = Camera.main;
        double vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;
        double horzExtent = vertExtent * Screen.width / Screen.height;

        Boundaries.minX = horzExtent;
        Boundaries.maxX = horzExtent;
        Boundaries.minY = -vertExtent;
        Boundaries.maxY = vertExtent;

        Boundaries.deltaX = horzExtent * 2;
        Boundaries.deltaY = vertExtent * 2;

        // // Calculations assume map is position at the origin
        // Boundaries.minX = horzExtent - MapDimensions.mapX / 2.0;
        // Boundaries.maxX = MapDimensions.mapX / 2.0 - horzExtent;
        // Boundaries.minY = vertExtent - MapDimensions.mapY / 2.0;
        // Boundaries.maxY = MapDimensions.mapY / 2.0 - vertExtent;
    }
}

public static class Boundaries
{
    public static double minX, maxX, minY, maxY, deltaX, deltaY;
}

// public static class MapDimensions
// {
//     public static double mapX = vertExtent * Screen.width / Screen.height;
//     public static double mapY = Camera.main.GetComponent<Camera>().orthographicSize;
// }
