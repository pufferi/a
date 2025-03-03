using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    private static PlayerStateManager _instance;
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
        Debug.Log("Player Move Unlock");
        PlayerMovement.Instance.playerStill = true;
    }

    public void PlayerMoveUnlock()
    {
        Debug.Log("PlayerMoveUnlock");
        PlayerMovement.Instance.playerStill = false;
    }

    public void PlayerViewLock(string axis = "all")
    {
        switch (axis.ToLower())
        {
            case "x":
                PlayerCamera.Instance.playerViewLockX = true;
                break;
            case "y":
                PlayerCamera.Instance.playerViewLockY = true;
                break;
            case "all":
            default:
                PlayerCamera.Instance.playerViewLockX = true;
                PlayerCamera.Instance.playerViewLockY = true;
                break;
        }
    }

    public void PlayerViewUnlock(string axis = "all")
    {
        switch (axis.ToLower())
        {
            case "x":
                PlayerCamera.Instance.playerViewLockX = false;
                break;
            case "y":
                PlayerCamera.Instance.playerViewLockY = false;
                break;
            case "all":
            default:
                PlayerCamera.Instance.playerViewLockX = false;
                PlayerCamera.Instance.playerViewLockY = false;
                break;
        }
    }
}
