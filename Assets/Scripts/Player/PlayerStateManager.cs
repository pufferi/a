using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public void PlayerStill()
    {
        PlayerMovement.Instance.playerStill=true;
    }

    public void PlayerMove()
    {
        PlayerMovement.Instance.playerStill = false;
    }

    public void PlayerViewLock()
    {
        PlayerCamera.Instance.playerViewLock = true;
    }
    public void PlayerViewUnlock()
    {
        PlayerCamera.Instance.playerViewLock=false;
    }

}
