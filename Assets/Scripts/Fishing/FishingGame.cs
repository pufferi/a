using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;
using UnityEditor;


public class FishingGame : MonoBehaviour
{
    // Existing variables...
    public FishingFloat fishingFloat;
    public float minAmplitude = 0.05f;
    public float maxAmplitude = 0.3f;
    public float minFrequency = 2f;
    public float maxFrequency = 5f;
    public float minDuration = 5f;
    public float maxDuration = 15f;
    private Vector3 playerStillPos = new Vector3(-4.65f, 1, 3.35f);


    public InFishGameArea infishArea;
    public PlayerGrabItems playerGrabItems;


    public InputActionAsset inputActions;
    private InputAction fishAction;
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

    //private Vector3 playerCamView;
    //private Vector3 player

    //我决定钓竿不拿在手上，立在空中
    private GrabableObjectComponent fishRod;


    //flags
    private bool _isPlayingFishingGame = false;//1
    private bool _hasCaughtFish = false;//2
    private bool _isFishBiting = false;//3



    private float biteDuration = 5f; // am i going to torture my player?
    //private Coroutine biteCoroutine;

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

        PlayerStateManager.Instance.PlayerMoveLock();
        PlayerStateManager.Instance.PlayerViewLock("x");
        playerTransform.position = playerStillPos;
        PlayerCamera.Instance.LookAtSomeWhere(Vector3.zero); // point to the center of the pool

        //place the fish rod
        fishRod = playerGrabItems.grabbedObject;
        playerGrabItems.Release();
        fishRod.GetComponent<FishingRod>().StartFishingGame_PlacingTheFishRod();

        int fishSize = Random.Range(1, 11);

        //sound
        audioSource.clip = backGroundWaterSound;
        audioSource.loop = true;
        audioSource.Play();

        _isPlayingFishingGame = true;

        StartCoroutine(StartFishing(fishSize));
    }
    //-----------------------

    private string didntCatchFish = "You didn't catch anything!";
    private string caughtFishMessage = "You caught a garbage!";
    public TextMeshProUGUI Message;
    // Coroutine references
    private Coroutine startFishingCoroutine;


    private IEnumerator StartFishing(int fishSize)
    {
        while (true)
        {
            float waitTime = Random.Range(3f, 20f);
            //yield return new WaitForSeconds(waitTime);

            float amplitude = Mathf.Lerp(minAmplitude, maxAmplitude, (float)fishSize / 10f);
            float frequency = Mathf.Lerp(minFrequency, maxFrequency, (float)fishSize / 10f);
            float duration = Mathf.Lerp(minDuration, maxDuration, 1f - (float)fishSize / 10f);

            fishingFloat.StartCoroutine(fishingFloat.FloatUpAndDown(duration, amplitude, frequency));

            audioSource.Stop();
            audioSource.PlayOneShot(fishBitingSound);
            Debug.Log(fishRod.GetComponent<FishingRod>().angle);

            // 开始鱼咬钩
            _isFishBiting = true;
            //biteCoroutine = StartCoroutine(FishBitingTimer());

            // 等待鱼咬钩期间
            yield return new WaitForSeconds(biteDuration);

            // 停止鱼咬钩
            _isFishBiting = false;

            //if (fishRod.GetComponent<FishingRod>().angle > 70)
            //{
            //    Debug.Log("收杆！！！");
            //    break;
            //}

            yield return new WaitForSeconds(duration); // wait for the fish to bite
        }
    }


    private void ReelInAndCheckIfCaughtTheFish()
    {
        PlayerStateManager.Instance.PlayerViewUnlock();
        Debug.Log("Finish the fishing game");

        // 播放抓到鱼的音效
        audioSource.Stop();
        audioSource.PlayOneShot(caughtFishSound);

        ShowTheResultOfFishingGame(_hasCaughtFish);

        FinishWholeGame();
    }

    private IEnumerator ShowTheResultOfFishingGame(bool hasCaughtFish)
    {
        // Stop specific coroutines
        if (startFishingCoroutine != null)
        {
            StopCoroutine(startFishingCoroutine);
            startFishingCoroutine = null;
        }

        if(hasCaughtFish)
            Message.text = caughtFishMessage;

        else
            Message.text = didntCatchFish;

        yield return new WaitForSeconds(3f);
        Message.text = "";
    }

    private void OnFinishWholeGame(InputAction.CallbackContext context)
    {
        FinishWholeGame();
    }

    private void FinishWholeGame()
    {
        if (!_isPlayingFishingGame)
            return;

        playerGrabItems.Grab(fishRod);
        PlayerStateManager.Instance.PlayerMoveUnlock();
        PlayerStateManager.Instance.PlayerViewUnlock("x");
        PauseAllSounds();
        fishRod.GetComponent<FishingRod>().EndFishingGame();
        _isPlayingFishingGame = false;
        _isFishBiting = false;

        TastList.SetActive(true);

        //StopAllCoroutines();
    }

    private void PauseAllSounds()
    {
        audioSource.Pause();
        audioSource.clip = null;
    }

    private void Update()
    {
        if (_isPlayingFishingGame && _isFishBiting)
        {
            if (fishRod.GetComponent<FishingRod>().angle > 70)
            {
                ReelInAndCheckIfCaughtTheFish();
            }
        }
    }


}
