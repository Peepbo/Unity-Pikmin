using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimSetting : MonoBehaviour
{
    public Dictionary<string, Action> actions = new Dictionary<string, Action>();
    public void PlayAct(string name) => actions[name]();
}