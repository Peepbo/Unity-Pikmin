using UnityEngine;
using UnityEditor;

public abstract class Interaction : MonoBehaviour
{
    public abstract void Arrangement(Transform trans);

    public abstract void Expansion();

    public abstract void FinishWork();

    public abstract void Reduction();
}