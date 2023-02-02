using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public interface IUIHelper
    {
        event Action OnNextButtonPressed;
        event Action OnTestButtonPressed;
        void Win();
        void SetActiveTestButton(bool active);
    }

    public class UIHelper : MonoBehaviour, IUIHelper
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _testButton;
        [SerializeField] private TMP_Text _winText;
    
        private Canvas _canvas;
    
        public event Action OnNextButtonPressed;
        public event Action OnTestButtonPressed;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();

            _nextButton.onClick.AddListener(NextButtonPressed);
            _testButton.onClick.AddListener(TestButtonPressed);

            SetControlsDefault();
        }

        private void OnDestroy()
        {
            _nextButton.onClick.RemoveListener(NextButtonPressed);
            _testButton.onClick.RemoveListener(TestButtonPressed);
        }

        private void SetControlsDefault()
        {
            // _canvas.enabled = false;
            _winText.enabled = false;
            _nextButton.gameObject.SetActive(false);
            SetActiveTestButton(false);
        }
    

        private void NextButtonPressed()
        {
            SetControlsDefault();
            SetActiveTestButton(true);
            OnNextButtonPressed?.Invoke();
        }
    
        private void TestButtonPressed()
        {
            OnTestButtonPressed?.Invoke();
        }

        public void Win()
        {
            SetActiveTestButton(false);
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

        public void SetActiveTestButton(bool active)
        {
            _testButton.gameObject.SetActive(active);
        }
    }
}