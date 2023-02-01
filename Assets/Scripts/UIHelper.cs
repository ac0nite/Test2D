using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public interface IUIHelper
{
    event Action OnNextButtonPressed;
    void OnWin();
}

public class UIHelper : MonoBehaviour, IUIHelper
{
    [SerializeField] private Button _nextButton;
    [SerializeField] private TMP_Text _winText;
    
    private Canvas _canvas;
    
    public event Action OnNextButtonPressed;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        
        _nextButton.onClick.AddListener(NextButtonPressed);
        SetControlsDefault();
    }

    private void OnDestroy()
    {
        _nextButton.onClick.RemoveListener(NextButtonPressed);
    }

    private void SetControlsDefault()
    {
        _canvas.enabled = false;
        
        _winText.enabled = false;
        _nextButton.gameObject.SetActive(false);
    }
    

    private void NextButtonPressed()
    {
        SetControlsDefault();
        OnNextButtonPressed?.Invoke();
    }

    public void OnWin()
    {
        _nextButton.gameObject.SetActive(false);
        _canvas.enabled = true;
        
        LeanTween.scale(_winText.gameObject, Vector2.one * 1.25f, 0.5f)
            .setOnStart(() => _winText.enabled = true)
            .setLoopPingPong(3)
            .setOnComplete(() =>
            {
                _winText.enabled = false;
                _nextButton.gameObject.SetActive(true);
            });
    }
}