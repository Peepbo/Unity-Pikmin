using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public int amount;
    public int index;
    public GameObject prefToPool;
    public bool shouldExpend;
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    public List<ObjectPoolItem> itemToPool;
    private Dictionary<string, Transform> parentDictionary = new Dictionary<string, Transform>();

    private void Awake()
    {
        instance = this;

        foreach (ObjectPoolItem item in itemToPool)
        {
            GameObject newParent = new GameObject();
            newParent.name = item.prefToPool.tag + item.index + "[objectPool]";
            parentDictionary.Add(item.prefToPool.tag + item.index, newParent.transform);

            for (int i = 0; i < item.amount; i++)
            {
                GameObject _obj = Instantiate(item.prefToPool);
                _obj.SetActive(false);
                _obj.transform.parent = newParent.transform;
            }
        }
    }

    public GameObject BorrowObject(string tag, int index = 0)
    {
        if (parentDictionary[tag + index].childCount != 0)
        {
            parentDictionary[tag + index].GetChild(0).gameObject.SetActive(true);
            return parentDictionary[tag + index].GetChild(0).gameObject;
        }

        foreach (ObjectPoolItem item in itemToPool)
        {
            if (item.prefToPool.CompareTag(tag))
            {
                if (item.shouldExpend)
                {
                    GameObject _obj = Instantiate(item.prefToPool);
                    _obj.SetActive(true);
                    _obj.transform.parent = parentDictionary[_obj.tag];
                    return _obj;
                }
            }
        }

        return null;
    }

    public void ReturnObject(GameObject returnObj, int index = 0)
    {
        returnObj.SetActive(false);
        returnObj.transform.parent = parentDictionary[returnObj.tag + index];
    }
}