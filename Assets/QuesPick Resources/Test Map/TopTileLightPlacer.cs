using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

public class TopTileLightPlacer : MonoBehaviour
{
    [Tooltip("최상단 타일을 감지할 타일맵 배열")]
    public Tilemap[] tilemaps;

    [Tooltip("생성된 마스크 텍스처를 적용할 2D 라이트")]
    public Light2D lightToApplyMask;

    [Tooltip("빛이 아래로 몇 칸까지 스며들지 결정합니다.")]
    [Range(1, 10)]
    public int falloffDepth = 4; // 최상단 타일 포함 총 4칸

    void Start()
    {
        if (tilemaps == null || tilemaps.Length == 0 || lightToApplyMask == null)
        {
            Debug.LogError("필수 컴포넌트(Tilemap 배열 또는 Light2D)가 설정되지 않았습니다.");
            return;
        }

        GenerateFalloffLightMask();
    }

    void GenerateFalloffLightMask()
    {
        // 1. 모든 타일맵을 포함하는 전체 경계를 계산합니다.
        BoundsInt totalBounds = tilemaps[0].cellBounds;
        for (int i = 1; i < tilemaps.Length; i++)
        {
            // 각 타일맵의 경계를 포함하도록 totalBounds를 확장합니다.
            totalBounds.xMin = Mathf.Min(totalBounds.xMin, tilemaps[i].cellBounds.xMin);
            totalBounds.yMin = Mathf.Min(totalBounds.yMin, tilemaps[i].cellBounds.yMin);
            totalBounds.xMax = Mathf.Max(totalBounds.xMax, tilemaps[i].cellBounds.xMax);
            totalBounds.yMax = Mathf.Max(totalBounds.yMax, tilemaps[i].cellBounds.yMax);
        }

        int width = totalBounds.size.x;
        int height = totalBounds.size.y;

        // 2. 라이트 마스크로 사용할 텍스처를 생성합니다.
        Texture2D lightMaskTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        
        // 텍스처를 완전히 투명한 상태로 초기화합니다.
        Color[] clearPixels = new Color[width * height];
        for (int i = 0; i < clearPixels.Length; i++)
        {
            clearPixels[i] = Color.clear;
        }
        lightMaskTexture.SetPixels(clearPixels);

        // 3. 각 열의 최상단 타일을 찾고, 그 아래로 빛 감쇠 효과를 그립니다.
        for (int x = 0; x < width; x++)
        {
            int topTileTextureY = -1; // 텍스처 좌표계 기준 y값

            // y축 위에서부터 아래로 스캔하여 모든 타일맵에서 최상단 타일 찾기
            for (int y = height - 1; y >= 0; y--)
            {
                bool tileFound = false;
                Vector3Int cellPos = new Vector3Int(totalBounds.xMin + x, totalBounds.yMin + y, 0);

                // 현재 셀 위치에 타일이 있는지 모든 타일맵을 확인합니다.
                foreach (Tilemap tilemap in tilemaps)
                {
                    if (tilemap.HasTile(cellPos))
                    {
                        topTileTextureY = y;
                        tileFound = true;
                        break; // 타일을 찾았으므로 더 이상 이 셀의 다른 타일맵을 확인할 필요가 없습니다.
                    }
                }

                if (tileFound)
                {
                    break; // 해당 x열의 최상단 타일을 찾았으므로 y축 스캔을 중단합니다.
                }
            }

            // 최상단 타일을 찾았다면, 그 위치부터 아래로 falloffDepth만큼 픽셀을 칠합니다.
            if (topTileTextureY != -1)
            {
                for (int j = 0; j < falloffDepth; j++)
                {
                    int currentY = topTileTextureY - j;

                    // 텍스처 범위를 벗어나지 않도록 확인
                    if (currentY < 0) break;

                    // 거리가 멀어질수록 밝기를 점차 감소시킵니다.
                    float brightness = 1.0f - (j / (float)falloffDepth);

                    // Color.white에 밝기를 곱하면 회색조 색상이 됩니다.
                    Color pixelColor = Color.white * brightness;
                    lightMaskTexture.SetPixel(x, currentY, pixelColor);
                }
            }
        }

        // 4. 텍스처 변경사항을 적용하고 스프라이트로 변환합니다.
        lightMaskTexture.filterMode = FilterMode.Point;
        lightMaskTexture.Apply();

        Sprite lightCookieSprite = Sprite.Create(
            lightMaskTexture,
            new Rect(0, 0, width, height),
            new Vector2(0f, 0f),
            tilemaps[0].cellSize.x // PPU(Pixels Per Unit)를 타일 크기에 맞게 설정
        );
        lightCookieSprite.name = "TopTile_FalloffMask_Sprite";

        // 5. Light2D 컴포넌트의 위치와 크기를 전체 타일맵 경계에 맞춥니다.
        // 기준 타일맵(tilemaps[0])을 사용하여 월드 좌표와 스케일을 설정합니다.
        lightToApplyMask.transform.position = tilemaps[0].CellToWorld(totalBounds.min);
        lightToApplyMask.transform.localScale = Vector3.one; // 스케일은 스프라이트 크기로 제어되므로 1로 설정

        // 6. Light2D에 생성된 스프라이트를 할당합니다.
        lightToApplyMask.lightType = Light2D.LightType.Sprite;
        lightToApplyMask.lightCookieSprite = lightCookieSprite;
    }
}