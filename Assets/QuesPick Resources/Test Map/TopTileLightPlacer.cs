using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class TopTileLightPlacer : MonoBehaviour
{
    [Tooltip("빛을 감지할 타일맵 배열")]
    public Tilemap[] tilemaps;

    [Tooltip("생성된 마스크 텍스처를 적용할 2D 라이트")]
    public Light2D lightToApplyMask;

    [Tooltip("빛이 아래 및 옆으로 몇 칸까지 스며들지 결정합니다.")]
    [Range(1, 10)]
    public int falloffDepth = 4;

    void Start()
    {
        if (tilemaps == null || tilemaps.Length == 0 || lightToApplyMask == null)
        {
            Debug.LogError("필수 컴포넌트(Tilemap 배열 또는 Light2D)가 설정되지 않았습니다.");
            return;
        }

        GenerateFalloffLightMask();
    }

    private bool HasTileAtPosition(Vector3Int cellPos)
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.HasTile(cellPos))
            {
                return true;
            }
        }
        return false;
    }

    void GenerateFalloffLightMask()
    {
        // 1. 모든 타일맵을 포함하는 전체 경계를 계산합니다.
        BoundsInt totalBounds = tilemaps[0].cellBounds;
        foreach (var tilemap in tilemaps)
        {
            totalBounds.xMin = Mathf.Min(totalBounds.xMin, tilemap.cellBounds.xMin);
            totalBounds.yMin = Mathf.Min(totalBounds.yMin, tilemap.cellBounds.yMin);
            totalBounds.xMax = Mathf.Max(totalBounds.xMax, tilemap.cellBounds.xMax);
            totalBounds.yMax = Mathf.Max(totalBounds.yMax, tilemap.cellBounds.yMax);
        }

        int width = totalBounds.size.x;
        int height = totalBounds.size.y;

        // 2. BFS를 위한 자료구조를 초기화합니다.
        float[,] brightnessMap = new float[width, height];
        int[,] distanceMap = new int[width, height]; // 빛으로부터의 거리를 저장
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // distanceMap을 방문하지 않은 상태(-1)로 초기화합니다.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                distanceMap[x, y] = -1;
            }
        }

        // 3. 빛이 시작될 지점(타일에 인접한 빈 공간)을 찾아 큐에 추가합니다.
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPos = new Vector3Int(totalBounds.xMin + x, totalBounds.yMin + y, 0);

                if (!HasTileAtPosition(cellPos)) // 현재 위치가 빈 공간인 경우
                {
                    // 상하좌우 인접한 곳에 타일이 있는지 확인
                    if (HasTileAtPosition(cellPos + Vector3Int.up) ||
                        HasTileAtPosition(cellPos + Vector3Int.down) ||
                        HasTileAtPosition(cellPos + Vector3Int.left) ||
                        HasTileAtPosition(cellPos + Vector3Int.right))
                    {
                        distanceMap[x, y] = 0; // 빛의 시작점이므로 거리는 0
                        brightnessMap[x, y] = 1.0f;
                        queue.Enqueue(new Vector2Int(x, y));
                    }
                }
            }
        }

        // 4. BFS를 사용하여 빛을 전파시킵니다.
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int currentDist = distanceMap[current.x, current.y];

            if (currentDist >= falloffDepth - 1) continue; // 빛이 더 이상 퍼지지 않음

            for (int i = 0; i < 4; i++)
            {
                int nx = current.x + dx[i];
                int ny = current.y + dy[i];

                // 텍스처 범위 내에 있고 아직 방문하지 않았다면
                if (nx >= 0 && nx < width && ny >= 0 && ny < height && distanceMap[nx, ny] == -1)
                {
                    int newDist = currentDist + 1;
                    distanceMap[nx, ny] = newDist;
                    brightnessMap[nx, ny] = 1.0f - (newDist / (float)falloffDepth);
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }

        // 5. 밝기 맵을 기반으로 라이트 마스크 텍스처를 생성합니다.
        Texture2D lightMaskTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixelColors = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float brightness = brightnessMap[x, y];
                // 밝기 값을 알파 값으로 사용하여 부드러운 투명도를 만듭니다.
                pixelColors[y * width + x] = new Color(1f, 1f, 1f, brightness);
            }
        }
        lightMaskTexture.SetPixels(pixelColors);
        lightMaskTexture.filterMode = FilterMode.Point;
        lightMaskTexture.Apply();

        // 6. 텍스처로 스프라이트를 생성하고 Light2D에 적용합니다.
        Sprite lightCookieSprite = Sprite.Create(
            lightMaskTexture,
            new Rect(0, 0, width, height),
            new Vector2(0f, 0f), // 피벗을 좌측 하단으로 설정
            tilemaps[0].cellSize.x,
            0,
            SpriteMeshType.FullRect
        );
        lightCookieSprite.name = "Natural_FalloffMask_Sprite";

        // Light2D 컴포넌트의 위치를 타일맵 경계의 좌측 하단에 맞춥니다.
        lightToApplyMask.transform.position = tilemaps[0].CellToWorld(totalBounds.min);
        lightToApplyMask.transform.localScale = Vector3.one;

        lightToApplyMask.lightType = Light2D.LightType.Sprite;
        lightToApplyMask.lightCookieSprite = lightCookieSprite;
    }
}