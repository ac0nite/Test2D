using UnityEngine;

namespace Scriprs
{
    public class Eraser : MonoBehaviour
    {
        [SerializeField] private Erasable _erasable;
        [SerializeField] private Dragable _dragable;
        [SerializeField] LayerMask _mask;
        
        private RaycastHit2D[] _hits = new RaycastHit2D[10];
        private IErasing _erasing;

        private void Start()
        {
            _dragable.OnBeginPosition += TryToBeginErase;
            _dragable.OnChangedPosition += TryToErasing;
            
            _erasing = new Erasing();
            _erasing.Initialise(_erasable);
        }

        private void TryToBeginErase(Vector2 worldPosition)
        {
            if(CanErase(worldPosition))
                _erasing.BeginErase(_hits[0].point);
        }

        private void TryToErasing(Vector2 worldPosition)
        {
            if(CanErase(worldPosition))
                _erasing.Erase(_hits[0].point);
        }

        private bool CanErase(Vector2 worldPosition)
        {
            //var count = Physics2D.RaycastNonAlloc(worldPosition, Vector2.zero, _hits, 10f, _mask);
            if (Physics2D.RaycastNonAlloc(worldPosition, Vector2.zero, _hits, 10f, _mask) > 0)
            {
                //Debug.Log($"Count: {count} {worldPosition}");
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            _dragable.OnBeginPosition -= TryToBeginErase;
            _dragable.OnChangedPosition -= TryToErasing;
        }
    }
}