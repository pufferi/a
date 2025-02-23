using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    private static PlayerStateManager _instance;

    public bool AAplayerLockView = false;

    public static PlayerStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerStateManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<PlayerStateManager>();
                    singletonObject.name = typeof(PlayerStateManager).ToString() + " (Singleton)";
                }
            }
            return _instance;
        }
    }

    private void Update()
    {
        PlayerCamera.Instance.playerViewLock = AAplayerLockView;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlayerMoveLock()
    {
        PlayerMovement.Instance.playerStill = true;
    }

    public void PlayerMoveUnlock()
    {
        PlayerMovement.Instance.playerStill = false;
    }

    public void PlayerViewLock()
    {
        PlayerCamera.Instance.playerViewLock = true;
    }

    public void PlayerViewUnlock()
    {
        PlayerCamera.Instance.playerViewLock = false;
    }
}
