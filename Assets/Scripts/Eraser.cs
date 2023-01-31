using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Scriprs
{
    public class Eraser : MonoBehaviour
    {
        [SerializeField] private Erasable _erasable;
        [SerializeField] private Dragable _dragable;
        [SerializeField] private ColliderSpawner _colliderSpawner;
        [SerializeField] LayerMask _mask;
        
        private RaycastHit2D[] _hits = new RaycastHit2D[1];
        private IErasing _erasing;
        private ILineFactory _line;

        private void Start()
        {
            _line = _colliderSpawner.GetComponent<ILineFactory>();
            _dragable.OnChangedPosition += TryToErasing;
            
            _erasing = new Erasing();
            _erasing.Initialise(_erasable);

            //StartCoroutine(DrawDebugCoroutine());
            StartCoroutine(CalculateCoroutine());
        }


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
        
        private IEnumerator CalculateCoroutine()
        {
            var amount = _erasable.Colors.Length;
            List<int> NotCleared = new List<int>();

            for (int i = 0; i < amount; i++)
                NotCleared.Add(i);

            while (true)
            {
                yield return new WaitForSeconds(5f);
                CalculateMarker.Begin();
                NotCleared.RemoveAll(index => _erasable.Colors[index].a == 0);
                CalculateMarker.End();
                
                //Debug.Log($"CLEARED: {(float) (amount - NotCleared.Count)/amount * 100f}");
            }
        }  

        private void TryToErasing(Vector2 worldPosition)
        {
            if (CanErase(worldPosition))
            {
                _erasing.Erase(_hits[0].point);
                _line.Point(_hits[0].point);
            }
            else
            {
                _erasing.Nothing();
                //_line.EndPoint();
            }
        }

        private bool CanErase(Vector2 worldPosition)
        {
            if (Physics2D.RaycastNonAlloc(worldPosition, Vector2.zero, _hits, 10f, _mask) > 0)
            {
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            _dragable.OnChangedPosition -= TryToErasing;
        }
    }
}