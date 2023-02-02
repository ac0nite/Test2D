
using UnityEngine;
using Utils;

public interface ILineFactory
{
    void Point(Vector2 point);
    void EndPoint();
    void AllDispose();
}
public class ColliderSpawner : MonoBehaviour, ILineFactory
{
    [SerializeField] private Line _linePrefab;
    [SerializeField] private int _amountPool = 30; 
    
    private SimplePool<Line> _pool;
    private Line _current;

    private void Awake()
    {
        _pool = new SimplePool<Line>(_amountPool, _linePrefab, transform);
    }
    private void OnDestroy()
    {
        _pool.Release();
    }

    public void Point(Vector2 point)
    {
        if (_current == null)
        {
            _current = _pool.GetPool();

            _current.SetRigidBodyType(RigidbodyType2D.Kinematic);
            _current.EnableCollider();
            _current.SimulateRigidBody();
            
            _current.gameObject.SetActive(true);
        }
        
        _current.AddPoint(point);
    }

    public void EndPoint()
    {
        _current = null;
    }

    public void AllDispose()
    {
        var active = _pool.Active();
        foreach (var line in active)
        {
            line.ReleaseCollider();
            _pool.SetPool(line);
        }
    }
}