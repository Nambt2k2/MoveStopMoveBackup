using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour {
    public T objInPool;
    public List<T> list_pooledObjects = new List<T>();

    public T GetPooledObject() {
        for (int i = 0; i < list_pooledObjects.Count; i++)
            if (!list_pooledObjects[i].gameObject.activeInHierarchy)
                return list_pooledObjects[i];

        T tmp = Object.Instantiate(objInPool);
        list_pooledObjects.Add(tmp);
        tmp.gameObject.SetActive(false);
        return tmp;
    }

    public T GetPooledObjectNotInstantiate() {
        for (int i = 0; i < list_pooledObjects.Count; i++)
            if (!list_pooledObjects[i].gameObject.activeInHierarchy)
                return list_pooledObjects[i];

        return null;
    }
}
