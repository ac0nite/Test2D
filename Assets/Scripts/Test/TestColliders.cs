using System;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Test
{
    public class TestColliders : MonoBehaviour
    {
        [SerializeField] private Circle _prefab;
        [SerializeField] private int _amount;
        
        private SimplePool<Circle> _pool;

        private void Awake()
        {
            _pool = new SimplePool<Circle>(_amount, _prefab, transform);
        }
        private void OnDestroy()
        {
            _pool.Release();
        }

        public void RunTest(Action callback = null)
        {
            Circle value;
            for (int i = 0; i < 7; i++)
            {
                LeanTween.delayedCall(0.1f * i, () =>
                {
                    value = _pool.GetPool();
                    value.transform.localPosition = new Vector3(Random.Range(-1.5f, 1.5f), 0, 0);
                    value.gameObject.SetActive(true);
                });
            }

            LeanTween.delayedCall(3f, () =>
            {
                Reset();
                callback?.Invoke();
            });
        }

        public void Reset()
        {
            var actives = _pool.Active();
            actives.ForEach(o => _pool.SetPool(o));
        }
    }
}