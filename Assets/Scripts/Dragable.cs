using System;
using Scriprs;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dragable : MonoBehaviour
{
    [SerializeField] private InputHandler _input;
    [SerializeField] private Vector2 _offset = Vector2.zero;
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
    }

    private void TryToDrag(PointerEventData pointerEventData)
    {
        _worldPosition = Camera.main.ScreenToWorldPoint(pointerEventData.position);

        if (_isDraggable)
        {
            transform.position = _worldPosition + _offset;
            OnChangedPosition?.Invoke(_worldPosition + _offset);
        }
    }
    
    private void TryToEndDrag(PointerEventData pointerEventData)
    {
        if(!_isDraggable)
            return;
        
        _isDraggable = false;
        
        Debug.Log($"TryToEndDrag");

        LeanTween.move(gameObject, _defaultPosition, 1f)
            .setSpeed(20f)
            .setOnStart(() => _collider.enabled = false)
            .setOnComplete(() => _collider.enabled = true);
    }

    private void OnDestroy()
    {
        _input.OnInputBeginDrag -= TryToBeginDrag;
        _input.OnInputDrag -= TryToDrag;
        _input.OnInputEndDrag -= TryToEndDrag;
    }
}
