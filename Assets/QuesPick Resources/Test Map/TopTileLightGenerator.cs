using UnityEngine;
using UnityEngine.Tilemaps;

public class TopTileLightGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public SpriteRenderer overlayRenderer;

    private Texture2D lightMapTexture;
    private float[,] brightness;

    void Start()
    {
        if (tilemap == null || overlayRenderer == null)
        {
            Debug.LogError("필수 컴포넌트 미설정");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;
        brightness = new float[width, height];
        lightMapTexture = new Texture2D(width, height, TextureFormat.Alpha8, false);

        int yMin = bounds.yMin;
        int xMin = bounds.xMin;

        int[] topTileY = new int[width];
        for (int x = 0; x < width; x++)
        {
            topTileY[x] = -1;
            for (int y = height - 1; y >= 0; y--)
            {
                Vector3Int cellPos = new Vector3Int(xMin + x, yMin + y, 0);
                if (tilemap.HasTile(cellPos))
                {
                    topTileY[x] = y;
                    break;
                }
            }
        }

        int attenuationStep = 5; // 몇 칸까지 어두워질지

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (topTileY[x] == -1) // x열 전체 타일 없음, 모두 밝게 0
                {
                    brightness[x, y] = 0f;
                }
                else
                {
                    if (y > topTileY[x]) // topTile 위 빈 공간, 밝기 0 (밝음)
                    {
                        brightness[x, y] = 0f;
                    }
                    else
                    {
                        int distance = topTileY[x] - y;
                        float value = Mathf.Clamp01(distance / (float)attenuationStep);
                        brightness[x, y] = value; // 0(최상단)~1(완전 어두움) 그라데이션
                    }
                }
            }
        }

        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
            lightMapTexture.SetPixel(x, y, new Color(0f, 0f, 0f, brightness[x, y]));

        lightMapTexture.filterMode = FilterMode.Point;
        lightMapTexture.Apply();

        overlayRenderer.sprite = Sprite.Create(
            lightMapTexture,
            new Rect(0, 0, width, height),
            new Vector2(0f, 0f),
            1f
        );

        Vector3 overlayWorldPos = tilemap.CellToWorld(new Vector3Int(xMin, yMin, 0));
        overlayRenderer.transform.position = overlayWorldPos;

        Vector3 cellSize = tilemap.layoutGrid.cellSize;
        overlayRenderer.transform.localScale = new Vector3(cellSize.x, cellSize.y, 1);
    }
}