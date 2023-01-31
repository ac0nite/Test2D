using System;
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
    }
    public class Erasable : MonoBehaviour, IErasable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private Color[] _defaultColors;
        private Texture2D _texture;
        private Color[] _colors;

        private void Awake()
        {
            _texture = _spriteRenderer.sprite.texture;
            _defaultColors = _texture.GetPixels();

            Reset();
        }
        public void Reset()
        {
            _texture = new Texture2D(_texture.width, _texture.height, TextureFormat.ARGB32, false);
            _texture.filterMode = FilterMode.Bilinear;
            _texture.wrapMode = TextureWrapMode.Clamp;
            _texture.SetPixels(_defaultColors);
            _texture.Apply();
            
            _spriteRenderer.sprite = Sprite.Create(_texture, _spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f));
            _colors = _spriteRenderer.sprite.texture.GetPixels();
        }

        public void UpdateTexture()
        {
            _texture.SetPixels(_colors);
            _texture.Apply();
            _spriteRenderer.sprite = Sprite.Create(_texture, _spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f));
        }
        
        public Texture2D Texture => _texture;
        public Color[] Colors => _colors;
        public Bounds Bounds => _spriteRenderer.bounds;
    }
}