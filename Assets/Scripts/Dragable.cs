using System;
using Scriprs;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dragable : MonoBehaviour
{
    [SerializeField] private InputHandler _input;

    public event Action<Vector2> OnBeginPosition;
    public event Action<Vector2> OnChangedPosition;
    
    private Vector2 _worldPosition;
    private Vector3 _defaultPosition;
    private Collider2D _collider;
    private bool _isDraggable;

    private void Awake()
    {
        _input.OnInputBeginDrag += TryToBeginDrag;
        _input.OnInputDrag += TryToDrag;
        _input.OnInputEndDrag += TryToEndDrag;

        _collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        _defaultPosition = transform.position;
    }
    
    private void TryToBeginDrag(PointerEventData pointerEventData)
    {
        _worldPosition = Camera.main.ScreenToWorldPoint(pointerEventData.position);

        _isDraggable = _collider.OverlapPoint(_worldPosition);
        
        Debug.Log($"_isDraggable: {_isDraggable}");
            
        if (_isDraggable)
        {
            OnBeginPosition?.Invoke(_worldPosition);
        }
    }

    private void TryToDrag(PointerEventData pointerEventData)
    {
        _worldPosition = Camera.main.ScreenToWorldPoint(pointerEventData.position);

        if (_isDraggable)
        {
            transform.position = _worldPosition;
            OnChangedPosition?.Invoke(_worldPosition);
        }
    }
    
    private void TryToEndDrag(PointerEventData pointerEventData)
    {
        _isDraggable = false;
        transform.position = _defaultPosition;
    }

    private void OnDestroy()
    {
        _input.OnInputBeginDrag -= TryToBeginDrag;
        _input.OnInputDrag -= TryToDrag;
        _input.OnInputEndDrag -= TryToEndDrag;
    }
}
