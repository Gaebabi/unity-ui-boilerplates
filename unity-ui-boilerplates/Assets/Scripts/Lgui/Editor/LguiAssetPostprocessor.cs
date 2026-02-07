using UnityEditor;
using UnityEngine;

namespace Lgui.Editor
{
    /// <summary>
    /// UI 에셋들이 유니티로 임포트될 때 자동으로 설정을 최적화해주는 포스트 프로세서입니다.
    /// </summary>
    public class LguiAssetPostprocessor : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            // 특정 경로(Icons 등)에 포함된 이미지는 자동으로 Sprite (2D and UI)로 설정
            if (assetPath.Contains("Resources/Icons") || assetPath.Contains("UI/Sprites"))
            {
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                
                // Texture Type을 Sprite로 설정 (이미 설정되어 있지 않은 경우에만)
                if (textureImporter.textureType != TextureImporterType.Sprite)
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.spriteImportMode = SpriteImportMode.Single;
                    textureImporter.mipmapEnabled = false;
                    textureImporter.alphaIsTransparency = true;
                    
                    Debug.Log($"[Lgui] Auto-configured Texture Type to Sprite for: {assetPath}");
                }
            }
        }
    }
}
