using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualNote : MonoBehaviour
{
    public static VisualNote main;

    public Vector3 targetPosition;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
    }

    void Start()
    {
        // Initial position
        targetPosition = Vector3.zero;
    }

    private void Update()
    {
        float amplitude = PositionController.instance.amplitude;
        float lerpVelocity = PositionController.instance.lerpVelocity;

        Vector3 positionNoise = new Vector3(0, Random.Range(-amplitude, +amplitude), 0);
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition + positionNoise,
            lerpVelocity * Time.deltaTime);
    }
}
