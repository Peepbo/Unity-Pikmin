using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem0
{
    public int index;
    [Space]
    public int amount;
    public GameObject prefToPool;
    public bool shouldExpend;
}

public class test : MonoBehaviour
{
    public static test instance;

    public List<ObjectPoolItem0> colliderToPool;
    public List<ObjectPoolItem0> pikminToPool;
    public List<ObjectPoolItem0> flowerToPool;
    public List<ObjectPoolItem0> enemyToPool;

    private Dictionary<ObjectPoolType, List<ObjectPoolItem0>> objectDictionary = 
        new Dictionary<ObjectPoolType, List<ObjectPoolItem0>>();

    private Dictionary<string, Transform> parentDictionary =
        new Dictionary<string, Transform>();

    private void Awake()
    {
        instance = this;

        AddDictionary(ObjectPoolType.COLLIDER, colliderToPool);
        AddDictionary(ObjectPoolType.PIKMIN, pikminToPool);
        AddDictionary(ObjectPoolType.FLOWER, flowerToPool);
        AddDictionary(ObjectPoolType.ENEMY, enemyToPool);

        MakePool(ObjectPoolType.COLLIDER, colliderToPool);
        MakePool(ObjectPoolType.PIKMIN, pikminToPool);
        MakePool(ObjectPoolType.FLOWER, flowerToPool);
        MakePool(ObjectPoolType.ENEMY, enemyToPool);
    }

    private void AddDictionary(ObjectPoolType type, List<ObjectPoolItem0> list)
    {
        objectDictionary[type] = list;
    }

    private void MakePool(ObjectPoolType type, List<ObjectPoolItem0> list)
    {
        foreach(ObjectPoolItem0 item in list)
        {
            GameObject newParent = new GameObject();
            newParent.name = type.ToString() + item.index;
            newParent.transform.parent = transform;

            parentDictionary.Add(type.ToString() + item.index, newParent.transform);

            for(int i = 0; i < item.amount; i++)
            {
                GameObject newObject = Instantiate(item.prefToPool);
                newObject.SetActive(false);
                newObject.transform.parent = newParent.transform;
            }
        }
    }

    public GameObject BorrowObject(ObjectPoolType type, int index = 0)
    {
        Transform borrowParent = parentDictionary[type.ToString() + index];

        if (borrowParent.childCount > 0)
        {
            GameObject borrowObject = borrowParent.GetChild(0).gameObject;

            borrowObject.SetActive(true);
            return borrowObject;
        }

        return null;
    }

    public void ReturnObject(GameObject returnObj, int index = 0)
    {
        returnObj.SetActive(false);
        returnObj.transform.parent = parentDictionary[returnObj.tag + index];
    }
}