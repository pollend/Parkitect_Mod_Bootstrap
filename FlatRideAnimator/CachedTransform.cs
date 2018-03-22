using System;
using System.Collections.Generic;
using UnityEngine;

public class CachedTransform : MonoBehaviour
{
    [SerializeField] public List<String> Key = new List<string>();
    [SerializeField] public List<Transform> Transforms = new List<Transform>();

    private readonly Dictionary<string, Transform> _cache = new Dictionary<string, Transform>();

    private void Start()
    {
        for (int x = 0; x < Key.Count; x++)
        {
            if (ContainsKey(Key[x]))
            {
                _cache[Key[x]] = Transforms[x];
            }
            else
            {
                _cache.Add(Key[x], Transforms[x]);
            }
        }
    }

    public void SetTransform(String key, Transform tr)
    {
        if (tr == null)
            return;
        Start();

        if (ContainsKey(key))
        {
            _cache[key] = tr;
        }
        else
        {
            _cache.Add(key, tr);
        }

        Key.Clear();
        Transforms.Clear();

        foreach (var entry in _cache)
        {
            Key.Add(entry.Key);
            Transforms.Add(entry.Value);
        }
    }

    public bool ContainsKey(String key)
    {
        if (key == null)
            return false;
        return _cache.ContainsKey(key);
    }

    public Transform GetCachedTransform(string key)
    {
        if (ContainsKey(key))
        {
            return _cache[key];
        }

        return null;
    }
}