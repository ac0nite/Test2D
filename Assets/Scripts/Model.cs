using System.Collections.Generic;
using Test;
using UnityEngine;

public interface IModel
{
    List<Erasable> Levels { get; }
    Eraser Eraser { get; }
    TestColliders Test { get; }
}
public class Model : MonoBehaviour, IModel
{
    [SerializeField] private Eraser _eraser;
    [SerializeField] private List<Erasable> _levels;
    [SerializeField] private TestColliders _test;
    public List<Erasable> Levels => _levels;
    public Eraser Eraser => _eraser;
    public TestColliders Test => _test;
}
