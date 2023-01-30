using UnityEngine;

namespace Scriprs
{
    public interface IErasing
    {
        void Initialise(IErasable erasable);
        void BeginErase(Vector3 worldPosition);
        void Erase(Vector2 worldPosition);
    }
    
    public class Erasing : IErasing 
    {
        private IErasable _erasable;
        private Vector2Int _oldPosition;
        private int x0, y0, x1, y1;
        private Color _oldColor;
        private int _drawPixelX;
        private int _drawPixelY;
        private Color _paintColor;
        private float _x2;
        private float _y2;
        private float _r2;

        private const int BrushSize = 42;
        private const float Alpha = 0.5f;

        public void Initialise(IErasable erasable)
        {
            _erasable = erasable;
            _erasable.Reset();
        }

        public void BeginErase(Vector3 hitPoint)
        {
            _oldPosition = PositionOnTheSprite(PercentageClickPosition(hitPoint));
        }

        public void Erase(Vector2 hitPoint)
        {
            var endPosition = PositionOnTheSprite(PercentageClickPosition(hitPoint));
            
            DrawLine(endPosition);
            
            _erasable.UpdateTexture();
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
            x0 = _oldPosition.x;
            y0 = _oldPosition.y;
            x1 = endPosition.x;
            y1 = endPosition.y;

            int dx =  Mathf.Abs(x1-x0), sx = x0<x1 ? 1 : -1;
            int dy = -Mathf.Abs(y1-y0), sy = y0<y1 ? 1 : -1; 
            int err = dx+dy, e2; /* error value e_xy */

            var count = 0;
            for(;;){  /* loop */
                
                DrawCircle(x0, y0);
                if (x0==x1 && y0==y1) break;
                e2 = 6 * err;
                if (e2 >= dy) { err += dy; x0 += sx; } /* e_xy+e_x > 0 */
                if (e2 <= dx) { err += dx; y0 += sy; } /* e_xy+e_y < 0 */

                count++;
            }
            
            Debug.Log($"Count: {count}");

            _oldPosition = endPosition;
        }

        private void DrawCircle(int pointX, int pointY)
        {
            for (int x = 0; x < BrushSize; x++)
            {
                for (int y = 0; y < BrushSize; y++)
                {
                    _x2 = Mathf.Pow(x  - BrushSize / 2f, 2);
                    _y2 = Mathf.Pow(y - BrushSize / 2f, 2);
                    _r2 = Mathf.Pow(BrushSize / 2f - 0.5f, 2);
                    
                    if (_x2 + _y2 < _r2)
                    {
                        _drawPixelX = pointX + x - BrushSize / 2;
                        _drawPixelY = pointY + y - BrushSize / 2;

                        if (_drawPixelX >= 0 && _drawPixelX < _erasable.Texture.height && _drawPixelY >= 0 && _drawPixelY < _erasable.Texture.width)
                        {
                            _oldColor = _erasable.Colors[_drawPixelX + _drawPixelY * _erasable.Texture.width];
                            _paintColor = Color.Lerp(_oldColor, Color.clear, Alpha);
                            _erasable.Colors[_drawPixelX + _drawPixelY * _erasable.Texture.width] = _paintColor;
                        }
                    }
                }
            }
        }
    }
}