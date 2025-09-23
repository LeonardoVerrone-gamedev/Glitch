using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int initialSize;
    }

    [SerializeField] private List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        Debug.Log("Entrou em SpawnFromPool");
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        // Se a pool estiver vazia, cria um novo objeto
        if (poolDictionary[tag].Count == 0)
        {
            Debug.Log("Pool vazia, Instanciando novo");
            GameObject newObj = Instantiate(GetPrefabByTag(tag));
            poolDictionary[tag].Enqueue(newObj);
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        Debug.Log("Setou como ativo");
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        Debug.Log("Setou transform");

        // Chama OnSpawn em componentes que implementam a interface
        IPoolable[] poolables = objectToSpawn.GetComponents<IPoolable>();
        foreach (IPoolable poolable in poolables)
        {
            poolable.OnSpawn();
        }

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject obj, string tag)
    {
        Debug.Log("Entrou em ReturnToPool");
        obj.SetActive(false);
        
        if (poolDictionary.ContainsKey(tag))
        {
            poolDictionary[tag].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning($"Trying to return object to non-existent pool: {tag}");
            Destroy(obj);
        }
    }

    private GameObject GetPrefabByTag(string tag)
    {
        foreach (Pool pool in pools)
        {
            if (pool.tag == tag)
            {
                return pool.prefab;
            }
        }
        return null;
    }
}

public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
}
