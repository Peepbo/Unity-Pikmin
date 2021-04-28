using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public int          index;          //오브젝트 번호 (같은 enum type의 오브젝트가 존재할 수 있음)
    [Space]
    [Space]
    public GameObject   prefToPool;     //복제할 프리펩
    public int          amount;         //처음에 생성할 오브젝트의 개수
    public bool         shouldExpend;   //해당 오브젝트를 모두 빌려줬을 때 크기를 확장할지 안할지? 
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    public List<ObjectPoolItem> colliderToPool; //COLLIDER pool
    public List<ObjectPoolItem> pikminToPool;   //PIKMIN pool
    public List<ObjectPoolItem> flowerToPool;   //FLOWER pool
    public List<ObjectPoolItem> enemyToPool;    //ENEMY pool

    /*
     * objectDictionary
     * 
     * key   : ObjectPoolType       (enum)
     * value : List<ObjectPoolItem> (list)
     * 빌려야 할 오브젝트가 부족할 때 shouldExpend가 true일 때 사용되는 딕셔너리다.
     * type으로 list를 찾아 prefToPool을 복제하는 식으로 사용된다.
     */
    private Dictionary<ObjectPoolType, List<ObjectPoolItem>> objectDictionary =
        new Dictionary<ObjectPoolType, List<ObjectPoolItem>>();

    /*
     * parentDictionary
     * 
     * key   : type + index         (string)
     * value : parent Object        (Transform)
     * 각각의 타입의 오브젝트를 담아두는 parent object를 반환하는 딕셔너리다.
     * BorrowObject(), ReturnObject() 를 호출할 때 주로 사용한다.
     */
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

    private void AddDictionary(ObjectPoolType type, List<ObjectPoolItem> list)
    {
        objectDictionary[type] = list;
    }

    private void MakePool(ObjectPoolType type, List<ObjectPoolItem> list)
    {
        foreach (ObjectPoolItem item in list)
        {
            GameObject newParent = new GameObject();
            newParent.name = type.ToString() + item.index;
            newParent.transform.parent = transform;

            parentDictionary.Add(type.ToString() + item.index, newParent.transform);

            for (int i = 0; i < item.amount; i++)
            {
                GameObject newObject = Instantiate(item.prefToPool);
                newObject.name = type.ToString() + item.index;
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
            borrowObject.transform.parent = null;

            return borrowObject;
        }

        //find list
        foreach (ObjectPoolItem item in objectDictionary[type])
        {
            if (item.index == index)
            {
                if (item.shouldExpend)
                {
                    Debug.Log("create a new item!");

                    GameObject newObject = Instantiate(item.prefToPool);
                    newObject.name = type.ToString() + index;

                    newObject.SetActive(true);
                    newObject.transform.parent = null;

                    return newObject;
                }

                else Debug.LogWarning("item shouldExpend is false!");
            }
        }

        Debug.LogWarning("borrowObject is return null!");
        return null;
    }

    public void ReturnObject(GameObject returnObj)
    {
        returnObj.SetActive(false);
        returnObj.transform.parent = parentDictionary[returnObj.name];
    }
}