using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingFloat : MonoBehaviour
{

    private Vector3 initialPosition=Vector3.zero;
    public float minAmplitude = 0.05f;
    public float maxAmplitude = 0.3f;
    public float minFrequency = 0.5f;
    public float maxFrequency = 2f;
    public float minDuration = 5f;
    public float maxDuration = 15f;

    public IEnumerator FloatUpAndDown(float duration,float amplitude, float frequency)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newY = initialPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;
            transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
            yield return null;
        }

        transform.position = initialPosition;
    }


}
