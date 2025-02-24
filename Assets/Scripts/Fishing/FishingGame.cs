using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;


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
    //private InputAction catchFishAction;
    private InputAction finishWholeGame;


    [Header("Audio Clips")]
    public AudioClip backGroundWaterSound;
    public AudioClip fishBitingSound;
    public AudioClip caughtFishSound;
    private AudioSource audioSource;

    public GameObject tip0;
    public GameObject tip1;
    public GameObject TastList;

    [SerializeField]
    private GameObject _fishingFloat;

    public Transform playerTransform;

    public GrabableObjectGenerator grabableObjectGenerator;

    private Vector3 playerStillPos = new Vector3(-4.65f, 1, 3.35f);
    //private Vector3 playerCamView;
    //private Vector3 player

    //我决定钓竿不拿在手上，立在空中
    private GrabableObjectComponent fishRod;

    private bool _isPlayingFishingGame = false;

    private void Start()
    {
        var FishGameMap = inputActions.FindActionMap("FishGame");
        fishAction = FishGameMap.FindAction("Fishing");
        //catchFishAction = FishGameMap.FindAction("CatchFish");
        finishWholeGame = FishGameMap.FindAction("FinishWholeFishGame");

        fishAction.Enable();
        //catchFishAction.Enable();
        finishWholeGame.Enable();
        fishAction.performed += OnFishingGameStart;

        finishWholeGame.performed += OnFinishWholeGame;

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
        TastList.SetActive(false);
        _fishingFloat.SetActive(true);
        //PlayerStateManager.Instance.PlayerViewLock();
        PlayerStateManager.Instance.PlayerMoveLock();
        PlayerStateManager.Instance.PlayerViewLock("x");
        playerTransform.position = playerStillPos;

        PlayerCamera.Instance.LookAtSomeWhere(Vector3.zero); // point to the center of the pool

        //place the fish rod
        fishRod = playerGrabItems.grabbedObject;
        playerGrabItems.Release();
        //isKenematic
        //可以写一个动画
        _isPlayingFishingGame = true;
        fishRod.GetComponent<FishingRod>().StartFishingGame();
        


        int fishSize = Random.Range(1, 11);

        // 播放背景水声
        audioSource.clip = backGroundWaterSound;
        audioSource.loop = true;
        audioSource.Play();

        StartCoroutine(StartFishing(fishSize));
    }
    //-----------------------

    private bool caughtFish = false;

    private string didntCatchFish = "You didn't catch anything!";
    private string caughtFishMessage = "You caught a garbage!";

    public TextMeshPro Message;
    private IEnumerator StartFishing(int fishSize)
    {
        while (true)
        {
            Debug.Log(fishRod.GetComponent<FishingRod>().angle);
            float waitTime = Random.Range(3f, 20f);
            yield return new WaitForSeconds(waitTime);

            float amplitude = Mathf.Lerp(minAmplitude, maxAmplitude, (float)fishSize / 10f);
            float frequency = Mathf.Lerp(minFrequency, maxFrequency, (float)fishSize / 10f);
            float duration = Mathf.Lerp(minDuration, maxDuration, 1f - (float)fishSize / 10f);

            fishingFloat.StartCoroutine(fishingFloat.FloatUpAndDown(duration, amplitude, frequency));

            audioSource.Stop();
            audioSource.PlayOneShot(fishBitingSound);
            if (fishRod.GetComponent<FishingRod>().angle < 40)
                break;
            yield return new WaitForSeconds(duration);
        }

        FishingGameEnd();
    }

    private void FishingGameEnd()
    {
        PlayerStateManager.Instance.PlayerViewUnlock();
        Debug.Log("finsih the fishing game");   
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

    private void OnFinishWholeGame(InputAction.CallbackContext context)
    {
       if(!_isPlayingFishingGame)
            return;
        playerGrabItems.Grab(fishRod);
        PlayerStateManager.Instance.PlayerMoveUnlock();
        PlayerStateManager.Instance.PlayerViewUnlock("x");
        PauseAllSounds();
        fishRod.GetComponent<FishingRod>().EndFishingGame();
        _isPlayingFishingGame=false;
        TastList.SetActive(true);

    }
    private void PauseAllSounds()
    {
        audioSource.Pause();
        audioSource.clip = null;
    }
}
