using UI;
using UnityEngine;
using Utils;

public class GameController : MonoBehaviour
{
    [SerializeField] private UIHelper _uiView;
    [SerializeField] private Model _model;
    [SerializeField] private InputHandler _input;
    
    private CircularBuffer<Erasable> _levels;

    private void Start()
    {
        _levels = new CircularBuffer<Erasable>(_model.Levels);
        _model.Eraser.Initialize(_levels.Value);
        _levels.Value.Enabled = true;
        _uiView.SetActiveTestButton(true);
        
        _model.Eraser.OnSuccessfulErasing += SuccessfulErasing;
        _uiView.OnNextButtonPressed += InitNextLevel;
        _uiView.OnTestButtonPressed += TryToRunTest;
    }

    private void OnDestroy()
    {
        _model.Eraser.OnSuccessfulErasing -= SuccessfulErasing;
        _uiView.OnNextButtonPressed -= InitNextLevel;
        _uiView.OnTestButtonPressed -= TryToRunTest;
    }

    private void TryToRunTest()
    {
        _uiView.SetActiveTestButton(false);
        _model.Test.RunTest(() => _uiView.SetActiveTestButton(true));
    }

    private void SuccessfulErasing()
    {
        _input.Lock = true;
        _levels.Value.Clear();
        _uiView.Win();
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