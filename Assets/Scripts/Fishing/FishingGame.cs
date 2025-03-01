using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;


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
    int fishSize=0;
    public Transform playerTransform;

    private GrabableObjectComponent fishRod;


    //flags
    private bool _isPlayingFishingGame = false;//1
    private bool _isFishBiting = false;//2
    public bool CanShowInAreaMessage = true;//3



    private float biteDuration = 1.5f; // am i going to torture my player?

    // Coroutine references
    private Coroutine startFishingCoroutine;


    private FishGenerator fishGenerator;


    private void Start()
    {
        var FishGameMap = inputActions.FindActionMap("FishGame");
        fishAction = FishGameMap.FindAction("Fishing");
        finishWholeGame = FishGameMap.FindAction("FinishWholeFishGame");

        fishAction.Enable();
        finishWholeGame.Enable();
        fishAction.performed += OnFishingGameStart;

        finishWholeGame.performed += OnFinishWholeGame;

        audioSource = GetComponent<AudioSource>();
        fishGenerator = GetComponent<FishGenerator>();
    }







    /// ///////////////////////////////////////////////

    private void OnFishingGameStart(InputAction.CallbackContext context)
    {
        Debug.Log(playerGrabItems.grabbedObject);
        if (!infishArea.IsInArea())
            return;
        if (playerGrabItems.grabbedObject == null || playerGrabItems.grabbedObject.objID != -2)
            return;

        CanShowInAreaMessage = false;
        tip0.SetActive(false);
        tip1.SetActive(false);
        TastList.SetActive(false);
        _fishingFloat.SetActive(true);

        PlayerStateManager.Instance.PlayerMoveLock();
        PlayerStateManager.Instance.PlayerViewLock("all");
        playerTransform.position = playerStillPos;
        PlayerCamera.Instance.LookAtSomeWhere(Vector3.zero); // point to the center of the pool

        //place the fish rod
        fishRod = playerGrabItems.grabbedObject;
        playerGrabItems.Release();
        fishRod.GetComponent<FishingRod>().StartFishingGame_PlacingTheFishRod();

        fishSize = Random.Range(1, 30);


        //sound
        audioSource.clip = backGroundWaterSound;
        audioSource.loop = true;
        audioSource.Play();

        _isPlayingFishingGame = true;
        startFishingCoroutine = StartCoroutine(StartFishing(fishSize));

    }
    //-----------------------

    private string didntCatchFish = "You didn't catch anything!";
    private string caughtFishMessage = "You caught a garbage!";
    public TextMeshProUGUI Message;



    private IEnumerator StartFishing(int fishSize)
    {
        while (true)
        {
            float waitTime = Random.Range(3f, 20f);
            yield return new WaitForSeconds(waitTime);

            float amplitude = Mathf.Lerp(minAmplitude, maxAmplitude, (float)fishSize / 10f);
            float frequency = Mathf.Lerp(minFrequency, maxFrequency, (float)fishSize / 10f);
            float duration = Mathf.Lerp(minDuration, maxDuration, 1f - (float)fishSize / 10f);

            fishingFloat.StartCoroutine(fishingFloat.FloatUpAndDown(duration, amplitude, frequency));

            audioSource.Stop();
            audioSource.PlayOneShot(fishBitingSound);

            float startFishingTims = Time.time;
            _isFishBiting = true;

            while (Time.time - startFishingTims < biteDuration)
            {
                if (fishRod.GetComponent<FishingRod>().angle > 70)
                {
                    ReelInAndCheckIfCaughtTheFish();
                    yield break;
                }
                yield return null;
            }

            _isFishBiting = false;

            yield return new WaitForSeconds(duration); // wait for the fish to bite
        }
    }


    private void ReelInAndCheckIfCaughtTheFish()
    {
        PlayerStateManager.Instance.PlayerViewUnlock();
        Debug.Log("Finish the fishing game");

        audioSource.Stop();
        audioSource.PlayOneShot(caughtFishSound);

        //ShowTheResultOfFishingGame(_hasCaughtFish);

        if (startFishingCoroutine != null)
        {
            Debug.Log("START FISHING coroutine  exsist and i  STOPPED  it");
            StopCoroutine(startFishingCoroutine);
            startFishingCoroutine = null;
        }

        StartCoroutine(ShowTheResultOfFishingGame(_isFishBiting));
        FinishWholeGame();
    }

    private IEnumerator ShowTheResultOfFishingGame(bool hasCaughtFish)
    {
        if (hasCaughtFish)
        {
            Message.text = caughtFishMessage;
            GenerateGarbage(fishSize);

            audioSource.Stop();
            audioSource.PlayOneShot(caughtFishSound);
        }
        else
            Message.text = didntCatchFish;

        yield return new WaitForSeconds(1.5f);
        Message.text = "";
        CanShowInAreaMessage = true;
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
        PlayerStateManager.Instance.PlayerViewUnlock("all");
        fishRod.GetComponent<FishingRod>().EndFishingGame();
        _isPlayingFishingGame = false;
        _isFishBiting = false;
        //Message.text = "";
        TastList.SetActive(true);
        fishingFloat.gameObject.SetActive(false);
        fishingFloat.transform.position = fishingFloat.initialPosition;
        fishRod.GetComponent<FishingRod>().angle = 0;//this is important,.,
    }

    private void GenerateGarbage(int fishSize)
    {
        fishGenerator.GenerateFish(fishSize);
    }

}
