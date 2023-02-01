using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Scriprs
{
    public interface IEraser
    {
        void Initialize(IErasable erasable);
        event Action OnSuccessfulErasing;
    }
    public class Eraser : MonoBehaviour, IEraser
    {
        [SerializeField] private Dragable _dragable;
        [SerializeField] private ColliderSpawner _colliderSpawner;
        [SerializeField] LayerMask _mask;

        private const float ThresholdRemainingPixels = 0.08f;
        
        private RaycastHit2D[] _hits = new RaycastHit2D[1];
        private IErasing _erasing = new Erasing();
        private ILineFactory _line;
        private List<int> _remainingPixels = new List<int>();
        private Coroutine _coroutine = null;
        private IErasable _erasable;
        
        public event Action OnSuccessfulErasing;

        private void Awake()
        {
            _line = _colliderSpawner.GetComponent<ILineFactory>();
            _dragable.OnChangedPosition += TryToErasing;

            //StartCoroutine(DrawDebugCoroutine());
            //StartCoroutine(CalculateRemainingPixelsCoroutine());
        }

        public void Initialize(IErasable erasable)
        {
            _erasable = erasable;
            _erasing.Initialise(erasable);
            
            _line.AllDispose();
        }

        private void TryToErasing(Vector2 worldPosition)
        {
            if (CanErase(worldPosition))
            {
                _erasing.Erase(_hits[0].point);
                _line.Point(_hits[0].point);
                StartCalculateRemainingPixels();
            }
            else
            {
                _erasing.Nothing();
                _line.EndPoint();
            }
        }

        private bool CanErase(Vector2 worldPosition)
        {
            return Physics2D.RaycastNonAlloc(worldPosition, Vector2.zero, _hits, 10f, _mask) > 0;
        }

        private void OnDestroy()
        {
            _dragable.OnChangedPosition -= TryToErasing;
        }
        
        private void StartCalculateRemainingPixels()
        {
            if (_coroutine == null)
            {
                _coroutine = StartCoroutine(CalculateRemainingPixelsCoroutine());   
            }
        }
        private void StopCalculateRemainingPixels()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }
        private IEnumerator CalculateRemainingPixelsCoroutine()
        {
            var amount = _erasable.Colors.Length;
            
            _remainingPixels.Clear();
            
            for (int i = 0; i < amount; i++)
                _remainingPixels.Add(i);

            while (true)
            {
                yield return new WaitForSeconds(2f);
                _remainingPixels.RemoveAll(index => _erasable.Colors[index].a < 0.01f);
                
                Debug.Log($"_remainingPixels: {(float) _remainingPixels.Count / amount}");

                if ((float) _remainingPixels.Count / amount < ThresholdRemainingPixels)
                {
                    OnSuccessfulErasing?.Invoke();
                    break;
                }
            }

            _coroutine = null;
            _erasing.Nothing();
        }  

        #region DEBUG

#if UNITY_EDITOR
        static readonly ProfilerMarker ProfilerMarker = new ProfilerMarker("XXX_DRAW_XXX");
        static readonly ProfilerMarker CalculateMarker = new ProfilerMarker("XXX_CALCULATE_XXX");
        private IEnumerator DrawDebugCoroutine()
        {
            yield return new WaitForSeconds(2f);
            
            ProfilerMarker.Begin();
            _erasing.DebugDraw();
            ProfilerMarker.End();
        }  
#endif

        #endregion
    }
}