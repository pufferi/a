using UnityEngine;
using System.Collections;

public class FishingGameManager : MonoBehaviour
{
    public FishingFloat fishingFloat;
    public float minAmplitude = 0.05f;
    public float maxAmplitude = 0.3f;
    public float minFrequency = 0.5f;
    public float maxFrequency = 2f;
    public float minDuration = 5f;
    public float maxDuration = 15f;

    private void OnFishingGameStart()
    {
        PlayerStateManager.Instance.PlayerViewLock();
        PlayerCamera.Instance.LookAtSomeWhere(Vector3.zero); // 指向泳池中心

        int fishSize = Random.Range(1, 11);

        StartCoroutine(StartFishing(fishSize));
    }

    private IEnumerator StartFishing(int fishSize)
    {
        while (!Input.GetKeyDown(KeyCode.E))
        {
            float waitTime = Random.Range(3f, 20f);
            yield return new WaitForSeconds(waitTime);

            float amplitude = Mathf.Lerp(minAmplitude, maxAmplitude, (float)fishSize / 10f);
            float frequency = Mathf.Lerp(minFrequency, maxFrequency, (float)fishSize / 10f);
            float duration = Mathf.Lerp(minDuration, maxDuration, 1f - (float)fishSize / 10f);

            fishingFloat.StartCoroutine(fishingFloat.FloatUpAndDown(duration, amplitude, frequency));

            yield return new WaitForSeconds(duration);
        }

        FishingGameEnd();
    }

    private void FishingGameEnd()
    {
        PlayerStateManager.Instance.PlayerViewUnlock();
    }
}
