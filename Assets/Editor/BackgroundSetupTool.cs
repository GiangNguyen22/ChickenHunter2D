using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BackgroundSetupTool
{
    private const string ImagesDir = "Assets/Resources/Images";
    private const string BackgroundFileName = "background_1.png";
    private const string GameScenePath = "Assets/Scenes/GameScene.unity";

    [MenuItem("Tools/Chicken Hunter/Set Background (background_1)")]
    public static void SetBackground()
    {
        // 1. Make sure background_1.png exists in the Images folder
        string srcPath = "Resources/Background/background_1.png";
        string destPath = ImagesDir + "/" + BackgroundFileName;
        if (File.Exists(destPath) == false)
        {
            if (File.Exists(srcPath) == false)
            {
                Debug.LogError("[Background] Khong tim thay " + srcPath);
                return;
            }
            File.Copy(srcPath, destPath, true);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        // 2. Import it as a Sprite
        TextureImporter imp = AssetImporter.GetAtPath(destPath) as TextureImporter;
        if (imp != null)
        {
            imp.textureType = TextureImporterType.Sprite;
            imp.spritePixelsPerUnit = 100;
            imp.alphaIsTransparency = true;
            imp.SaveAndReimport();
        }
        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(destPath);
        if (bgSprite == null)
        {
            Debug.LogError("[Background] Khong the import sprite: " + destPath);
            return;
        }

        // 3. Open GameScene and assign sprite to the two scrolling tiles
        UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError("[Background] Khong mo duoc " + GameScenePath);
            return;
        }

        bool anyAssigned = false;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            ScrollingBackground scroller = root.GetComponentInChildren<ScrollingBackground>(true);
            if (scroller == null) continue;

            Camera cam = Camera.main;
            float targetHeight = 20f;

            AssignTile(scroller.tileA, bgSprite, cam, targetHeight, ref anyAssigned);
            AssignTile(scroller.tileB, bgSprite, cam, targetHeight, ref anyAssigned);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log(anyAssigned
            ? "[Background] Da gan background_1.png vao hai tile cua GameScene!"
            : "[Background] Khong tim thay tile nao de gan sprite (da co san?).");
    }

    private static void AssignTile(Transform tile, Sprite sprite, Camera cam, float targetHeight, ref bool anyAssigned)
    {
        if (tile == null) return;
        SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = tile.gameObject.AddComponent<SpriteRenderer>();
        }
        sr.sprite = sprite;
        sr.sortingOrder = 0;
        sr.transform.localScale = Vector3.one;

        // Fit: height = targetHeight, width at least camera width
        float h = sr.bounds.size.y;
        float w = sr.bounds.size.x;
        if (h > 0.01f && w > 0.01f && cam != null)
        {
            float camHalfW = cam.orthographicSize * cam.aspect;
            float scaleY = targetHeight / h;
            float scaleX = Mathf.Max(scaleY, (camHalfW * 2f) / w);
            sr.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
        anyAssigned = true;
    }
}
