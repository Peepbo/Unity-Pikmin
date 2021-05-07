using UnityEngine;
using System.Collections.Generic;

static class Utils
{
    //return float, get PI(3.14...) * 2
    public static float PI2
    {
        get { return Mathf.PI * 2; }
    }

    //return int, get random number (-1 or 1)
    public static int RandomSign
    {
        get { return Random.Range(0, 2) == 0 ? -1 : 1; }
    }

    //return List<float>, get random n floats between a and b
    public static List<float> RandomFloats(float a, float b, int n)
    {
        List<float> fList = new List<float>();

        while(n > 0)
        {
            fList.Add(Random.Range(a, b));
            n--;
        }

        return fList;
    }

    //return vector3, get random Vector between a and b
    public static Vector3 RandomVector(Vector3 a, Vector3 b)
    {
        return new Vector3(Random.Range(a.x, b.x), Random.Range(a.y, b.y), Random.Range(a.z, b.z));
    }

    //return bool, abs(value0 - value1) < errorValue (default errorValue = 0.001f)
    public static bool AlmostSame (float myValue, float targetValue, float errorValue = 0.001f)
    {
        return Mathf.Abs(targetValue - myValue) < errorValue;
    }

    public static float DecimalRoundDown(float Value, int saveCount)
    {
        return Value % Time.frameCount * (int)Mathf.Pow(10, saveCount) / (int)Mathf.Pow(10, saveCount);
    }

    //return vector3, get vector to fly with a parabolic
    public static Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time)
    {
        //벡터의 뺄샘을 하여 거리, 방향을 저장
        Vector3 distance = target - origin;

        //y를 0으로 두고, xz의 거리, 방향을 저장
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        //높이
        float Sy = distance.y;

        //높이를 무시한 두 벡터간의 거리
        float Sxz = distanceXZ.magnitude;

        /*
         * xz 속도
         * x = VelocityXZ * time
         * VelocityXZ = x / time
         */
        float Vxz = Sxz / time;

        /*
         * y 속도
         * y = Vy * t - 0.5f * g * t²
         * Vy * t = y + 0.5f * g * t²
         * Vy = y/t + 0.5f * g * t
         */
        float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        //result에 distance를 정규화 한 방향
        Vector3 result = distanceXZ.normalized;

        //xy 속도를 result에 곱
        result *= Vxz;
        //y 속도를 result의 y에 대입
        result.y = Vy;

        //속도를 담은 vector를 반환
        return result;
    }
}