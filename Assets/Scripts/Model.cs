using System.Collections.Generic;
using Scriprs;
using UnityEngine;

public interface IModel
{
    List<Erasable> Levels { get; }
    Eraser Eraser { get; }
}
public class Model : MonoBehaviour, IModel
{
    [SerializeField] private Eraser _eraser;
    [SerializeField] private List<Erasable> _levels;
    public List<Erasable> Levels => _levels;
    public Eraser Eraser => _eraser;
}
