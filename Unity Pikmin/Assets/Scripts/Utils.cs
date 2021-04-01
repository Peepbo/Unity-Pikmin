using UnityEngine;

static class Utils
{
    //return PI(3.14...) * 2
    public static float PI2
    {
        get { return Mathf.PI * 2; }
    }

    //return -1 or 1
    public static int RandomSign
    {
        get { return Random.Range(0, 2) == 0 ? -1 : 1; }
    }

    public static Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        //define the distance x and y first
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        float Sy = distance.y;
        float Sxz = distanceXZ.magnitude;

        float Vxz = Sxz / time;
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;

        return result;
    }
}