using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSetting : MonoBehaviour
{
    PlayerController controller;

    private void Awake()
    {
        controller = transform.parent.GetComponent<PlayerController>();
    }

    public void ChangeAnim()
    {
        controller.ChangeState(PlayerController.PlayerState.Idle);
    }

    public void ThrowAnim()
    {
        controller.ThrowPik();
    }
}
