using UnityEngine;

namespace Scriprs
{
    public interface IErasable
    {
        void Reset();
        void UpdateTexture();
        Texture2D Texture { get; }
        Color[] Colors { get; }
        Bounds Bounds { get; }
        int BrushSize { get; }
        bool Enabled { get; set; }
    }
    public class Erasable : MonoBehaviour, IErasable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private int _brushSize = 42;
        
        private Color[] _defaultColors;
        private Texture2D _texture = null;
        private Color[] _colors;
        public void Reset()
        {
            if (_texture == null)
            {
                _texture = _spriteRenderer.sprite.texture;
                _defaultColors = _texture.GetPixels();

                _texture = new Texture2D(_texture.width, _texture.height, TextureFormat.ARGB32, false);
                _texture.filterMode = FilterMode.Bilinear;
                _texture.wrapMode = TextureWrapMode.Clamp;
                
            }
            
            _texture.SetPixels(_defaultColors);
            _texture.Apply();
            _spriteRenderer.sprite = Sprite.Create(_texture, _spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f), 100f);
            _colors = _spriteRenderer.sprite.texture.GetPixels();
        }

        public void UpdateTexture()
        {
            _texture.SetPixels(_colors);
            _texture.Apply();
            //_spriteRenderer.sprite = Sprite.Create(_texture, _spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f));
        }
        
        public Texture2D Texture => _texture;
        public Color[] Colors => _colors;
        public Bounds Bounds => _spriteRenderer.bounds;
        public int BrushSize => _brushSize;

        public bool Enabled
        {
            get => gameObject.activeInHierarchy;
            set => gameObject.SetActive(value);
        }
    }
}