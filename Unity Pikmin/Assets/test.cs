using UnityEngine;

public class test : Interaction
{
    public override void Arrangement(Transform trans)
    {
        Debug.Log("Arrangement");
    }

    public override void Expansion()
    {
        Debug.Log("Expansion");
    }

    public override void FinishWork()
    {
        Debug.Log("FinishWork");
    }

    public override void Reduction()
    {
        Debug.Log("Reduction");
    }
}