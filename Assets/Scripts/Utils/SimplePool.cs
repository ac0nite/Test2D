using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public interface ISimplePool<T> where T : Component
    {
        T GetPool();
        void SetPool(T value);
        void Release();
        List<T> Active();
    }
    public class SimplePool<T> : ISimplePool<T> where T : Component
    {
        private Transform _parent;
        private List<T> _pool = new List<T>();
        private T _prefab;

        public SimplePool(int count, T prefab, Transform parent)
        {
            _parent = parent;
            _prefab = prefab;

            Spawn(count);
        }

        public T GetPool()
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                if (!_pool[i].gameObject.activeInHierarchy)
                {
                    return _pool[i];
                }
            }
        
            return null;
        }

        public void SetPool(T value)
        {
            value.gameObject.SetActive(false);
            value.transform.localPosition = Vector3.zero;
            value.transform.rotation = Quaternion.identity;
        }

        public void Release()
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                GameObject.Destroy(_pool[i].gameObject);
            }
            _pool.Clear();
        }

        public List<T> Active()
        {
            return _pool.Where(value => value.gameObject.activeInHierarchy).ToList();
        }

        private void Spawn(int count)
        {
            GameObject value;
            for (int i = 0; i < count; i++)
            {
                value = GameObject.Instantiate(_prefab.gameObject, _parent);
            
                value.transform.localPosition = Vector3.zero;
                value.transform.rotation = Quaternion.identity;
            
                value.SetActive(false);
                _pool.Add(value.GetComponent<T>());
            }
        }
    }
}