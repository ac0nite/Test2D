using Gameplay.Core.Camera;
using Scriprs;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private UIHelper _uiHelper;
    [SerializeField] private Model _model;
    [SerializeField] private InputHandler _input;
    
    private CircularBuffer<Erasable> _levels;

    private void Start()
    {
        _levels = new CircularBuffer<Erasable>(_model.Levels);
        _model.Eraser.Initialize(_levels.Value);
        _levels.Value.Enabled = true;
        
        _model.Eraser.OnSuccessfulErasing += SuccessfulErasing;
        _uiHelper.OnNextButtonPressed += InitNextLevel;
    }

    private void OnDestroy()
    {
        _model.Eraser.OnSuccessfulErasing -= SuccessfulErasing;
        _uiHelper.OnNextButtonPressed -= InitNextLevel;
    }

    private void SuccessfulErasing()
    {
        _input.Lock = true;
        _uiHelper.OnWin();
    }

    private void InitNextLevel()
    {
        _levels.Value.Enabled = false;
        _levels.Value.Reset();
        
        _levels.Next();
        
        _model.Eraser.Initialize(_levels.Value);
        _levels.Value.Enabled = true;
        _input.Lock = false;
    }
}