using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingFloat : MonoBehaviour
{

    private Vector3 initialPosition=new Vector3(-1.191f,0,0.137f);
    //public float minAmplitude = 0.05f;
    //public float maxAmplitude = 0.3f;
    //public float minFrequency = 0.5f;
    //public float maxFrequency = 2f;
    //public float minDuration = 5f;
    //public float maxDuration = 15f;

    private void Start()
    {
        this.transform.position = initialPosition;
    }
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
