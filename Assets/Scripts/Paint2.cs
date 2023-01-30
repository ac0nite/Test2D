using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint2 : MonoBehaviour
{
    [Space]
    [SerializeField] private Texture2D _texture;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    

    [SerializeField] private Camera _camera;
    [SerializeField] private Collider2D _collider;

    [SerializeField] private Color _color = Color.black;
    [Range(0, 1)] [SerializeField] private float _alpha = 1f;
    [Range(0,1000)] [SerializeField] private int _brushSize = 10;
    [Range(0,1)] [SerializeField] private float _stiffness = 0.9f;
    
    private Color _paintColor;
    
    private Ray _ray;
    private RaycastHit2D _hit;
    private Color _oldColor;
    private int _drawPixelX;
    private int _drawPixelY;
    private int _oldRayX;
    private int _oldRayY;
    private float _x2;
    private float _y2;
    private float _r2;
    private float _rezult;
    private float _stiffnessInterpolate;
    private float _stiffnessR2;
    private List<int> _uvsIndex = new List<int>();
    private Vector2[] _uvs;
    private Color[] m_Colors;
    private bool _isDraw;
    private Vector3Int _start;
    private Vector3Int _end;

    private void Start()
    {
        var tex = _spriteRenderer.sprite.texture;
        _texture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        _texture.filterMode = FilterMode.Bilinear;
        _texture.wrapMode = TextureWrapMode.Clamp;
        m_Colors = tex.GetPixels();
        _texture.SetPixels(m_Colors);
        _texture.Apply();
        _spriteRenderer.sprite = Sprite.Create(_texture, _spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f));
    }


    private void Update()
    {
        _brushSize += (int) Input.mouseScrollDelta.y;
        
        if (Input.GetMouseButton(0))
        {
            _ray = _camera.ScreenPointToRay(Input.mousePosition);

            _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1f,
                LayerMask.GetMask("Erasable"));
            
            if (_hit.collider != null)
            {
                var p = PositionOnTheSprite(PercentageClickPosition(_hit.point));
                //if (_oldRayX != p.x || _oldRayY != p.y)
                {
                    DrawLine(p.x, p.y);
                    //DrawCircleWithStiffness(p.x, p.y);   
                    //_texture.SetPixels(m_Colors);
                    _texture.SetPixels(m_Colors);
                    _texture.Apply();
                    _spriteRenderer.sprite = Sprite.Create(_texture, _spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f));
                }
                _isDraw = true;
            }
            else
            {
                _isDraw = false;
            }
        }
    }

    private IEnumerator UpdateCoroutine()
    {
        yield return new WaitForEndOfFrame();
    }

    Vector2 PercentageClickPosition(Vector2 hitpoint)
    {
        Vector2 min = _spriteRenderer.bounds.min;
        Vector2 max = _spriteRenderer.bounds.max;
        //Mathf.Lerp (output.min, output.max, Mathf.InverseLerp (input.min, input.max, input));
        float x = Mathf.Lerp(0, 100, Mathf.InverseLerp(min.x, max.x, hitpoint.x));
        float y = Mathf.Lerp(0, 100, Mathf.InverseLerp(min.y, max.y, hitpoint.y));
        return new Vector2(x, y);
    }
        
    Vector2Int PositionOnTheSprite(Vector2 percentage)
    {
        Vector2 spriteSize = new Vector2(_texture.width, _texture.height);
        int x = Mathf.RoundToInt(Mathf.Lerp(0, _texture.width, Mathf.InverseLerp(0, 100, percentage.x)));
        int y = Mathf.RoundToInt(Mathf.Lerp(0, _texture.height, Mathf.InverseLerp(0, 100, percentage.y)));
        return new Vector2Int(x, y);
    }

    private void DrawForUVs(float coordX, float coordY)
    {
        
    }

    private void DrawQuad(int rayX, int rayY)
    {
        for (int x = 0; x < _brushSize; x++)
        {
            for (int y = 0; y < _brushSize; y++)
            {
                _texture.SetPixel(rayX + x - _brushSize / 2, rayY + y - _brushSize / 2, _paintColor);
            }
        }
    }
    
    private void DrawCircle(int rayX, int rayY)
    {
        for (int x = 0; x < _brushSize; x++)
        {
            for (int y = 0; y < _brushSize; y++)
            {
                float x2 = Mathf.Pow(x  - _brushSize / 2f, 2);
                float y2 = Mathf.Pow(y - _brushSize / 2f, 2);
                float r2 = Mathf.Pow(_brushSize / 2f - 0.5f, 2);

                if (x2 + y2 < r2)
                {
                    _drawPixelX = rayX + x - _brushSize / 2;
                    _drawPixelY = rayY + y - _brushSize / 2;

                    //if (_drawPixelX >= 0 && _drawPixelX < _texture.height && _drawPixelY >= 0 && _drawPixelY < _texture.width)
                    {
                        // _oldColor = _texture.GetPixel(_drawPixelX, _drawPixelY);
                        // _paintColor = Color.Lerp(_oldColor, _color, _alpha);
                        // _texture.SetPixel(_drawPixelX, _drawPixelY, _paintColor);

                        _oldColor = m_Colors[_drawPixelX + _drawPixelY * _texture.width];
                        _paintColor = Color.Lerp(_oldColor, Color.clear, _alpha);
                        m_Colors[_drawPixelX + _drawPixelY * _texture.width] = _paintColor;
                    }
                }
            }
        }
    }
    
    private void EraseCircle(int rayX, int rayY)
    {
        for (int x = 0; x < _brushSize; x++)
        {
            for (int y = 0; y < _brushSize; y++)
            {
                float x2 = Mathf.Pow(x  - _brushSize / 2f, 2);
                float y2 = Mathf.Pow(y - _brushSize / 2f, 2);
                float r2 = Mathf.Pow(_brushSize / 2f - 0.5f, 2);

                if (x2 + y2 < r2)
                {
                    _drawPixelX = rayX + x - _brushSize / 2;
                    _drawPixelY = rayY + y - _brushSize / 2;

                    if (_drawPixelX >= 0 && _drawPixelX < _texture.height && _drawPixelY >= 0 && _drawPixelY < _texture.width)
                    {
                        _oldColor = _texture.GetPixel(_drawPixelX, _drawPixelY);
                        // _paintColor = Color.Lerp(_oldColor, _color, _alpha);
                    
                        _texture.SetPixel(_drawPixelX, _drawPixelY, Color.clear);   
                    }
                }
            }
        }
    }

    private void DrawLine(int endX, int endY)
    {
        if (!_isDraw)
        {
            _oldRayX = endX;
            _oldRayY = endY;
        }

        Debug.Log($"[OLD] [{_oldRayX};{_oldRayY}] [{endX};{endY}]");
        
        PlotLine(_oldRayX, _oldRayY, endX, endY);
        
        //drawline(_oldRayX, _oldRayY, endX, endY);
        
        //DrawCircleWithStiffness(x, y);

        _oldRayX = endX;
        _oldRayY = endY;
    }
    
    
    //http://members.chello.at/~easyfilter/bresenham.html
    private void PlotLine(int x0, int y0, int x1, int y1)
    {
        //Debug.Log($"IN: {x1} {y1}");
        
        int step = 1;
        
        // x1 = (x1 % step) == 0 ? x1 : x1 + step - x1 % step;
        // y1 = (y1 % step) == 0 ? y1 : y1 + step - y1 % step;
        
        x1 = (Mathf.Abs(x1 - x0) % step) == 0 ? x1 : x1  - Mathf.Abs(x1 - x0) % step;
        y1 = (Mathf.Abs(y1 - y0) % step) == 0 ? y1 : y1  - Mathf.Abs(y1 - y0) % step;
        
        //Debug.Log($"OUT: {x1} {y1}");
        
        int dx =  Mathf.Abs(x1-x0), sx = x0<x1 ? step : -step;
        int dy = -Mathf.Abs(y1-y0), sy = y0<y1 ? step : -step; 
        int err = dx+dy, e2; /* error value e_xy */
        
        for(;;){  /* loop */
            //DrawCircleWithStiffness(x0, y0);
            DrawCircle(x0, y0);
            if (x0==x1 && y0==y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; } /* e_xy+e_x > 0 */
            if (e2 <= dx) { err += dx; y0 += sy; } /* e_xy+e_y < 0 */
        }
    }

    private void DrawCircleWithStiffness(int rayX, int rayY)
    {
        for (int x = 0; x < _brushSize; x++)
        {
            for (int y = 0; y < _brushSize; y++)
            {
                _x2 = Mathf.Pow(x  - _brushSize / 2f, 2);
                _y2 = Mathf.Pow(y - _brushSize / 2f, 2);
                _r2 = Mathf.Pow(_brushSize / 2f - 0.5f, 2);
                _rezult = _x2 + _y2;
                
                _stiffnessR2 = Mathf.Pow(_stiffness * _brushSize / 2f - 0.5f, 2);
                _stiffnessInterpolate = Mathf.InverseLerp(_stiffnessR2, _r2, _rezult);

                if (_rezult < _r2)
                {
                    _drawPixelX = rayX + x - _brushSize / 2;
                    _drawPixelY = rayY + y - _brushSize / 2;

                    // _drawPixelX = rayX;
                    // _drawPixelY = rayY;

                    //if (_drawPixelX >= 0 && _drawPixelX < _resolution && _drawPixelY >= 0 && _drawPixelY < _resolution)
                    {
                        _oldColor = _texture.GetPixel(_drawPixelX, _drawPixelY);
                        //_oldColor = m_Colors[x + y * _texture.width];
                        //_paintColor = Color.Lerp(_oldColor, _color, _alpha);
                        _paintColor = Color.Lerp(_oldColor, Color.clear, _alpha);

                        if (_rezult > _stiffnessR2 && _rezult < _r2)
                        {
                            _paintColor = Color.Lerp(_paintColor, _oldColor, _stiffnessInterpolate);
                        }

                        //Debug.Log($"dX:{_drawPixelX} dY:{_drawPixelY}");
                        
                        _texture.SetPixel(_drawPixelX, _drawPixelY, _paintColor);
                        //m_Colors[x + y * _texture.width] = _paintColor;
                    }
                }
            }
        }
    }
}
