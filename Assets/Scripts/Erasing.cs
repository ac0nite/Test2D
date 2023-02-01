using UnityEngine;

namespace Scriprs
{
    public interface IErasing
    {
        void Initialise(IErasable erasable);
        void Erase(Vector2 worldPosition);
        void Nothing();
        void DebugDraw();
    }
    
    public class Erasing : IErasing 
    {
        private IErasable _erasable;
        private Vector2Int? _oldPosition = null;
        private int x0, y0, x1, y1;
        private Color _oldColor;
        private int _drawPixelX;
        private int _drawPixelY;
        private Color _paintColor;
        private float _x2;
        private float _y2;

        private int _brushSize;
        private const float Alpha = 0.2f;
        private int _halfBrushSize;
        private float _r2;
        
        private int _textureHeight;
        private int _textureWidth;
        private int _changedPixel;

        public void Initialise(IErasable erasable)
        {
            _erasable = erasable;
            _erasable.Reset();

            _textureHeight = _erasable.Texture.height;
            _textureWidth = _erasable.Texture.width;
            _brushSize = erasable.BrushSize;
            _halfBrushSize = _brushSize / 2;
            _r2 = Mathf.Pow(_brushSize / 2f - 0.5f, 2);
        }
        

        public void DebugDraw()
        {
            Debug.Log($"DEBUG DRAW!");
            _oldPosition = Vector2Int.zero;
            DrawLine(new Vector2Int(300, 400));
            _erasable.UpdateTexture();
        }

        public void Erase(Vector2 hitPoint)
        {
            if (_oldPosition == null)
            {
                _oldPosition = PositionOnTheSprite(PercentageClickPosition(hitPoint));
            }
            else
            {
                DrawLine(PositionOnTheSprite(PercentageClickPosition(hitPoint)));
                _erasable.UpdateTexture();
            }
        }
        
        public void Nothing()
        {
            _oldPosition = null;
        }

        Vector2 PercentageClickPosition(Vector2 hitpoint)
        {
            Vector2 min = _erasable.Bounds.min;
            Vector2 max = _erasable.Bounds.max;
            float x = Mathf.Lerp(0, 100, Mathf.InverseLerp(min.x, max.x, hitpoint.x));
            float y = Mathf.Lerp(0, 100, Mathf.InverseLerp(min.y, max.y, hitpoint.y));
            return new Vector2(x, y);
        }
        
        Vector2Int PositionOnTheSprite(Vector2 percentage)
        {
            Vector2 spriteSize = new Vector2(_erasable.Texture.width, _erasable.Texture.height);
            int x = Mathf.RoundToInt(Mathf.Lerp(0, _erasable.Texture.width, Mathf.InverseLerp(0, 100, percentage.x)));
            int y = Mathf.RoundToInt(Mathf.Lerp(0, _erasable.Texture.height, Mathf.InverseLerp(0, 100, percentage.y)));
            return new Vector2Int(x, y);
        }
        
        //http://members.chello.at/~easyfilter/bresenham.html
        private void DrawLine(Vector2Int endPosition)
        {
            x0 = _oldPosition.Value.x;
            y0 = _oldPosition.Value.y;
            x1 = endPosition.x;
            y1 = endPosition.y;

            int step = 1;

            int dx =  Mathf.Abs(x1-x0), sx = x0<x1 ? step : -step;
            int dy = -Mathf.Abs(y1-y0), sy = y0<y1 ? step : -step; 
            int err = dx+dy, e2; /* error value e_xy */

            var count = 0;
            for(;;){  /* loop */
                
                if(count % 2 == 0)
                    DrawCircle(x0, y0);
                
                if (x0==x1 && y0==y1) break;
                e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; } /* e_xy+e_x > 0 */
                if (e2 <= dx) { err += dx; y0 += sy; } /* e_xy+e_y < 0 */

                count++;
            }

            _oldPosition = endPosition;
        }

        private void DrawCircle(int pointX, int pointY)
        {
            for (int x = 0; x < _brushSize; x++)
            {
                for (int y = 0; y < _brushSize; y++)
                {
                    _x2 = (x - _halfBrushSize) * (x - _halfBrushSize);
                    _y2 = (y - _halfBrushSize) * (y - _halfBrushSize);

                    if (_x2 + _y2 < _r2)
                    {
                        _drawPixelX = pointX + x - _halfBrushSize;
                        _drawPixelY = pointY + y - _halfBrushSize;
                        
                        if (_drawPixelX >= 0 && _drawPixelX < _textureWidth && _drawPixelY >= 0 && _drawPixelY < _textureHeight)
                        {
                            _changedPixel = _drawPixelX + _drawPixelY * _textureWidth;
                            _erasable.Colors[_changedPixel] = Color.Lerp(_erasable.Colors[_changedPixel], Color.clear, Alpha);

                            // _erasable.Colors[_drawPixelX + _drawPixelY * _textureWidth] = Color.clear;
                        }
                    }
                }
            }
        }
    }
}