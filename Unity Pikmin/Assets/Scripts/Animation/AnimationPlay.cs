using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlay : MonoBehaviour
{
    public string animationName;
    // Start is called before the first frame update
    void Start()=>GetComponent<Animator>().Play(animationName);   
}
