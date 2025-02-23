using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class FishingGame : MonoBehaviour
{
    public FishingFloat fishingFloat;
    public float minAmplitude = 0.05f;
    public float maxAmplitude = 0.3f;
    public float minFrequency = 0.5f;
    public float maxFrequency = 2f;
    public float minDuration = 5f;
    public float maxDuration = 15f;

    public InFishGameArea infishArea;
    public PlayerGrabItems playerGrabItems;

    public InputActionAsset inputActions;

    private InputAction fishAction;
    private InputAction catchFishAction;

    // 添加音效
    public AudioClip backGroundWaterSound;
    public AudioClip fishBitingSound;
    public AudioClip caughtFishSound;
    private AudioSource audioSource;

    public GameObject tip0;
    public GameObject tip1;

    [SerializeField]
    private  GameObject _fishingFloat;

    public Transform Player;

    private void Start()
    {
        var playerMap = inputActions.FindActionMap("Player");
        fishAction = playerMap.FindAction("Fishing");
        catchFishAction = playerMap.FindAction("CatchFish");

        fishAction.Enable();
        catchFishAction.Enable();

        fishAction.performed += OnFishingGameStart;

        // 获取AudioSource组件
        audioSource = GetComponent<AudioSource>();
    }

    private void OnFishingGameStart(InputAction.CallbackContext context)
    {
        if (!infishArea.IsInArea())
            return;
        if (playerGrabItems.grabbedObject == null || playerGrabItems.grabbedObject.objID != -2)
            return;
        tip0.SetActive(false);
        tip1.SetActive(false);
        _fishingFloat.SetActive(true);
        PlayerStateManager.Instance.PlayerViewLock();
        PlayerStateManager.Instance.PlayerMoveLock();
        Player.position = new Vector3(-4.65f, 1, 3.35f);
        //PlayerCamera.Instance.LookAtSomeWhere(Vector3.zero); // point to the center of the pool

        int fishSize = Random.Range(1, 11);

        // 播放背景水声
        audioSource.clip = backGroundWaterSound;
        audioSource.loop = true;
        audioSource.Play();

        StartCoroutine(StartFishing(fishSize));
    }

    private IEnumerator StartFishing(int fishSize)
    {
        while (!catchFishAction.triggered)
        {
            float waitTime = Random.Range(3f, 20f);
            yield return new WaitForSeconds(waitTime);

            float amplitude = Mathf.Lerp(minAmplitude, maxAmplitude, (float)fishSize / 10f);
            float frequency = Mathf.Lerp(minFrequency, maxFrequency, (float)fishSize / 10f);
            float duration = Mathf.Lerp(minDuration, maxDuration, 1f - (float)fishSize / 10f);

            fishingFloat.StartCoroutine(fishingFloat.FloatUpAndDown(duration, amplitude, frequency));

            // 停止播放背景水声，播放鱼咬钩音效
            audioSource.Stop();
            audioSource.PlayOneShot(fishBitingSound);

            yield return new WaitForSeconds(duration);
        }

        FishingGameEnd();
    }

    private void FishingGameEnd()
    {
        PlayerStateManager.Instance.PlayerViewUnlock();

        // 播放抓到鱼的音效
        audioSource.Stop();
        audioSource.PlayOneShot(caughtFishSound);

        // 等待抓到鱼的音效播放完毕后重新播放背景水声
        StartCoroutine(PlayBackgroundWaterSoundAfterCaughtFish());
    }

    private IEnumerator PlayBackgroundWaterSoundAfterCaughtFish()
    {
        yield return new WaitForSeconds(caughtFishSound.length);
        audioSource.clip = backGroundWaterSound;
        audioSource.loop = true;
        audioSource.Play();
    }
}
