using ScriptableObjects;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(GridLayoutGroup))]
public class SigilManager : MonoBehaviour
{
    [SerializeField] private SigilPixel sigilPixelPrefab;
    [SerializeField] private int pixelsHorizontally = 30;
    [SerializeField] private Sigil activeSigilOverlay;

    private SigilPixel[,] _sigilPixels;
    private PlayerInput _input;

    private void OnEnable()
    {
        _input = FindFirstObjectByType<PlayerInput>();
        _input.OnMouseUp += HandleMouseUp;
    }
    
    private void OnDisable()
    {
        _input.OnMouseUp -= HandleMouseUp;
    }

    private void Start()
    {
        var gridLayoutGroup = GetComponent<GridLayoutGroup>();
        var canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        var width = canvasRectTransform.sizeDelta.x;
        var cellSize = width / pixelsHorizontally;
        var canvasHeight = canvasRectTransform.sizeDelta.y;
        var pixelsVertically = Mathf.CeilToInt(canvasHeight / cellSize);
        
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        
        var count = 0;
        _sigilPixels = new SigilPixel[pixelsVertically, pixelsHorizontally];
        var wasNull = activeSigilOverlay?.Pixels == null;
        for (var i = 0; i < _sigilPixels.GetLength(0); i++)
        {
            for (var j = 0; j < _sigilPixels.GetLength(1); j++)
            {
                var sigilPixel = Instantiate(sigilPixelPrefab, transform);
                sigilPixel.name = $"SigilPixel_{count++} ({i}, {j})";
                if (wasNull)
                {
                    sigilPixel.IsActivated = false;
                }
                else if (i >= 0 && i < activeSigilOverlay.Pixels.GetLength(0) && j >= 0 && j < activeSigilOverlay.Pixels.GetLength(1))
                {
                    sigilPixel.IsActivated = activeSigilOverlay.Pixels[i, j];
                }
                else
                {
                    sigilPixel.IsActivated = false;
                }
                
                _sigilPixels[i, j] = sigilPixel;
            }
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Return))
        {
            var sigil = ScriptableObject.CreateInstance<Sigil>();
            sigil.Pixels = new bool[_sigilPixels.GetLength(0), _sigilPixels.GetLength(1)];
            for (var i = 0; i < _sigilPixels.GetLength(0); i++)
            {
                for (var j = 0; j < _sigilPixels.GetLength(1); j++)
                {
                    sigil.Pixels[i, j] = _sigilPixels[i, j].IsActivated;
                }
            }
            
            var path = AssetDatabase.GenerateUniqueAssetPath("Assets/Scripts/ScriptableObjects/sigil.asset");
            AssetDatabase.CreateAsset(sigil, path);
            AssetDatabase.SaveAssets();
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = sigil;
        }
#endif
    }
    
    private void HandleMouseUp()
    {
        var (minX, maxX, minY, maxY) = FindBoundingBox();
        CenterBoundingBox(minX, maxX, minY, maxY);
    }
    
    private (int minX, int maxX, int minY, int maxY) FindBoundingBox()
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;

        for (var i = 0; i < _sigilPixels.GetLength(0); i++)
        {
            for (var j = 0; j < _sigilPixels.GetLength(1); j++)
            {
                if (!_sigilPixels[i, j].IsActivated) continue;
                
                minX = Mathf.Min(minX, j);
                maxX = Mathf.Max(maxX, j);
                minY = Mathf.Min(minY, i);
                maxY = Mathf.Max(maxY, i);
            }
        }

        return (minX, maxX, minY, maxY);
    }
    
    private void CenterBoundingBox(int minX, int maxX, int minY, int maxY)
    {
        var rows = _sigilPixels.GetLength(0);
        var cols = _sigilPixels.GetLength(1);

        var width = maxX - minX + 1;
        var height = maxY - minY + 1;
        var tempArray = new bool[height][];
        for (var i = 0; i < height; i++)
        {
            tempArray[i] = new bool[width];
        }

        for (var i = minY; i <= maxY; i++)
        {
            for (var j = minX; j <= maxX; j++)
            {
                if (_sigilPixels[i, j].IsActivated)
                {
                    tempArray[i - minY][j - minX] = true;
                }
            }
        }

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                _sigilPixels[i, j].IsActivated = false;
            }
        }

        var newMinX = (cols - width) / 2;
        var newMinY = (rows - height) / 2;
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                if (!tempArray[i][j]) continue;
                
                var newI = newMinY + i;
                var newJ = newMinX + j;
                if (newI >= 0 && newI < rows && newJ >= 0 && newJ < cols)
                {
                    _sigilPixels[newI, newJ].IsActivated = true;
                }
            }
        }
    }
}
