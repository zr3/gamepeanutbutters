using System.Collections.Generic;
using UnityEngine;

public class GobPool : MonoBehaviour {
    private static GobPool _instance;
    private Dictionary<string, Stack<GameObject>> pool;

    private void Awake()
    {
        _instance = this.CheckSingleton(_instance);
        pool = new Dictionary<string, Stack<GameObject>>();
    }

	public static GameObject Instantiate(GameObject o)
    {
        bool usePooledGob = _instance.pool.ContainsKey(o.tag) && _instance.pool[o.tag].Count > 0;
        GameObject newObject = usePooledGob ?
            _instance.pool[o.tag].Pop() :
            GameObject.Instantiate(o);
        newObject.SetActive(true);
        if (!usePooledGob)
        {
            newObject.AddComponent<PooledGob>();
        }
        return newObject;
    }

    public static void Destroy(GameObject o)
    {
        if (!_instance.pool.ContainsKey(o.tag))
        {
            _instance.pool.Add(o.tag, new Stack<GameObject>());
        }
        _instance.pool[o.tag].Push(o);
        o.SetActive(false);
    }

    public static void Clear()
    {
        foreach (var p in _instance.pool.Keys) {
            ClearPool(p);
        }
        _instance = null;
    }

    public static void ClearPool(string pool)
    {
        if (_instance == null) return;

        var p = _instance.pool[pool];
        while (p.Count > 0)
        {
            var gob = p.Pop();
            gob.GetComponent<PooledGob>().WasSafelyDestroyed = true;
            Destroy(gob);
        }
        _instance.pool.Clear();
    }
}
