using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimSetting : MonoBehaviour
{
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    public void PlayAct(string functionName) => actions[functionName]();
    public void AddAct(string functionName, Action action) => actions.Add(functionName, action);
}