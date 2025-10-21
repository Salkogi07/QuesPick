using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

public class TopTileLightFalloffGenerator : MonoBehaviour
{
    [Tooltip("최상단 타일을 감지할 타일맵")]
    public Tilemap tilemap;

    [Tooltip("생성된 마스크 텍스처를 적용할 2D 라이트")]
    public Light2D lightToApplyMask;

    [Tooltip("빛이 아래로 몇 칸까지 스며들지 결정합니다.")]
    [Range(1, 10)]
    public int falloffDepth = 4; // 최상단 타일 포함 총 4칸

    void Start()
    {
        if (tilemap == null || lightToApplyMask == null)
        {
            Debug.LogError("필수 컴포넌트(Tilemap 또는 Light2D)가 설정되지 않았습니다.");
            return;
        }

        GenerateFalloffLightMask();
    }

    void GenerateFalloffLightMask()
    {
        // 1. 타일맵의 경계와 크기를 가져옵니다.
        BoundsInt bounds = tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;

        // 2. 라이트 마스크로 사용할 텍스처를 생성합니다.
        Texture2D lightMaskTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        
        // 텍스처를 완전히 투명한 상태로 초기화합니다.
        Color[] clearPixels = new Color[width * height];
        lightMaskTexture.SetPixels(clearPixels);

        // 3. 각 열의 최상단 타일을 찾고, 그 아래로 빛 감쇠 효과를 그립니다.
        for (int x = 0; x < width; x++)
        {
            int topTileTextureY = -1; // 텍스처 좌표계 기준 y값

            // y축 위에서부터 아래로 스캔하여 최상단 타일 찾기
            for (int y = height - 1; y >= 0; y--)
            {
                Vector3Int cellPos = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                if (tilemap.HasTile(cellPos))
                {
                    topTileTextureY = y;
                    break;
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
                    // j=0 (최상단): 밝기 1.0
                    // j=1: 밝기 0.75
                    // j=2: 밝기 0.5
                    // j=3: 밝기 0.25
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
            1f
        );
        lightCookieSprite.name = "TopTile_FalloffMask_Sprite";

        // 5. Light2D 컴포넌트의 위치와 크기를 타일맵에 맞춥니다.
        lightToApplyMask.transform.position = tilemap.CellToWorld(bounds.min);
        lightToApplyMask.transform.localScale = tilemap.layoutGrid.cellSize;

        // 6. Light2D에 생성된 스프라이트를 할당합니다.
        lightToApplyMask.lightType = Light2D.LightType.Sprite;
        lightToApplyMask.lightCookieSprite = lightCookieSprite;
    }
}