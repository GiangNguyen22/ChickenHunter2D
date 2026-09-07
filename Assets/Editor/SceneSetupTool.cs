using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSetupTool
{
    private const string SplashScenePath = "Assets/Scenes/SplashScene.unity";
    private const string GameScenePath = "Assets/Scenes/GameScene.unity";
    private const string BulletPrefabPath = "Assets/Prefabs/Bullet.prefab";
    private const string ChickenPrefabPath = "Assets/Prefabs/Chicken.prefab";
    private const string EggPrefabPath = "Assets/Prefabs/Egg.prefab";

    private const string ImagesDir = "Assets/Resources/Images";
    private const string ExplosionDir = "Assets/Resources/Explosion";
    private const string AudioDir = "Assets/Resources/Audio";
    private static readonly string FontPath = "Assets/Resources/Fonts/font.ttf";

    private static Font uiFont;

    [MenuItem("Tools/Chicken Hunter/Setup All Scenes")]
    public static void SetupAllScenes()
    {
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        PrepareSpriteFolders();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        uiFont = AssetDatabase.LoadAssetAtPath<Font>(FontPath);

        BuildSplashScene();
        BuildGameScene();
        ConfigureBuildSettings();
        AssetDatabase.SaveAssets();

        Debug.Log("[Chicken Hunter] Setup complete! Scenes: SplashScene + GameScene (" + uiFont.name + ")");
    }

    private static void PrepareSpriteFolders()
    {
        PrepareSpriteFolder(ImagesDir);
        PrepareSpriteFolder(ExplosionDir);
    }

    private static void PrepareSpriteFolder(string folder)
    {
        if (!AssetDatabase.IsValidFolder(folder)) return;

        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp == null) continue;

            imp.textureType = TextureImporterType.Sprite;
            imp.spritePixelsPerUnit = 100;
            imp.alphaIsTransparency = true;
            imp.SaveAndReimport();
        }
    }

    // ===== HELPERS LOAD ASSETS =====

    private static Sprite LoadSprite(string name)
    {
        string path = ImagesDir + "/" + name;
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        return sprite;
    }

    private static AudioClip LoadAudio(string name)
    {
        string path = AudioDir + "/" + name;
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        if (clip == null)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        }
        return clip;
    }

    private static Sprite[] LoadExplosionFrames()
    {
        if (!AssetDatabase.IsValidFolder(ExplosionDir)) return new Sprite[0];

        string[] paths = Directory.GetFiles(ExplosionDir, "explosion_*.png");
        System.Array.Sort(paths);
        var sprites = new System.Collections.Generic.List<Sprite>();
        foreach (string path in paths)
        {
            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (s == null)
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
            if (s != null) sprites.Add(s);
        }
        return sprites.ToArray();
    }

    // ===== CONFIGURE =====

    private static void ConfigureBuildSettings()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(SplashScenePath, true),
            new EditorBuildSettingsScene(GameScenePath, true)
        };
    }

    // ===== SPLASH SCENE =====

    private static void BuildSplashScene()
    {
        Scene scene = NewSceneOrClearCurrent();

        Camera cam = GetOrCreateMainCamera(scene).GetComponent<Camera>();
        cam.orthographicSize = 5f;
        cam.backgroundColor = new Color(0.05f, 0.05f, 0.1f, 1f);

        // Sound + managers persist into game scene
        BuildAudioManager(new Vector3(0.5f, 0.2f, 0f), true);
        BuildGameManager();

        // ---- Canvas ----
        GameObject canvasGo = new GameObject("SplashCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // ---- Background image ----
        Sprite bgSprite = LoadSprite("background2.png");
        if (bgSprite == null) bgSprite = LoadSprite("background1.png");
        GameObject bg = CreateUIRect(canvasGo, "Background", new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        SetImage(bg, bgSprite, Color.white);

        Sprite white = CreateWhiteSprite();

        // ---- Logo ----
        GameObject logoGroup = CreateUIRect(canvasGo, "LogoGroup", new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f), new Vector2(1400, 500), Vector2.zero);
        CanvasGroup logoCG = logoGroup.AddComponent<CanvasGroup>();
        logoCG.alpha = 0f;

        GameObject logoPanel = CreateUIRect(logoGroup, "LogoPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(1400, 300), Vector2.zero);
        SetImage(logoPanel, white, new Color(0f, 0f, 0f, 0.45f));

        Text title = CreateText(logoGroup, "Title", "CHICKEN HUNTER", 96, new Color(1f, 0.85f, 0.2f, 1f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(1300, 160), new Vector2(0, 60));
        title.alignment = TextAnchor.MiddleCenter;
        title.fontStyle = FontStyle.Bold;

        Text subtitle = CreateText(logoGroup, "Subtitle", "Bao ve trang trai", 38, new Color(1f, 1f, 1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(1000, 80), new Vector2(0, -90));
        subtitle.alignment = TextAnchor.MiddleCenter;

        // ---- Loading bar ----
        GameObject loadGroup = CreateUIRect(canvasGo, "LoadingGroup", new Vector2(0.5f, 0.3f), new Vector2(0.5f, 0.3f), new Vector2(700, 90), Vector2.zero);
        CanvasGroup loadCG = loadGroup.AddComponent<CanvasGroup>();
        loadCG.alpha = 0f;

        GameObject barBg = CreateUIRect(loadGroup, "BarBackground", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(700, 32), Vector2.zero);
        SetImage(barBg, white, new Color(0.2f, 0.2f, 0.2f, 0.9f));

        GameObject barFill = CreateUIRect(loadGroup, "BarFill", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(700, 32), Vector2.zero);
        Image fill = SetImage(barFill, white, new Color(0.15f, 0.85f, 0.45f, 1f));
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillAmount = 0f;

        // ---- Play button ----
        GameObject playGroup = CreateUIRect(canvasGo, "PlayGroup", new Vector2(0.5f, 0.16f), new Vector2(0.5f, 0.16f), new Vector2(420, 120), Vector2.zero);
        CanvasGroup playCG = playGroup.AddComponent<CanvasGroup>();

        Sprite startSprite = LoadSprite("start1.png");
        if (startSprite == null) startSprite = LoadSprite("play.png");
        Button playBtn = CreateIconButton(playGroup, "PlayButton", startSprite, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(380, 110), Vector2.zero);

        // ---- Wire splash ----
        SplashScreen splash = canvasGo.AddComponent<SplashScreen>();
        splash.logoCanvasGroup = logoCG;
        splash.loadingBar = fill;
        splash.loadingBarCanvasGroup = loadCG;
        splash.playButton = playBtn;
        splash.playButtonCanvasGroup = playCG;

        AddEventSystem(scene);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, SplashScenePath);
    }

    // ===== GAME SCENE =====

    private static void BuildGameScene()
    {
        Scene scene = NewSceneOrClearCurrent();

        Camera cam = GetOrCreateMainCamera(scene).GetComponent<Camera>();
        cam.orthographicSize = 5f;
        cam.backgroundColor = new Color(0.03f, 0.03f, 0.06f, 1f);

        // ---- Static background (background_1, no seam) ----
        GameObject bg = new GameObject("Background");
        Sprite bgSprite = LoadSprite("background_1.png");
        if (bgSprite == null)
            UnityEngine.Debug.LogWarning("[Chicken Hunter] Khong tim thay background_1.png - nen transparent!");
        SpriteRenderer bgSr = bg.AddComponent<SpriteRenderer>();
        bgSr.sprite = bgSprite;
        bgSr.sortingOrder = 0;
        FitBackground(bgSr, cam, 20f);

        // ---- Player (Object A) at center-bottom, nose up ----
        Sprite playerSprite = LoadSprite("shipMain.png");
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        SpriteRenderer playerSr = player.AddComponent<SpriteRenderer>();
        playerSr.sprite = playerSprite;
        playerSr.sortingOrder = 5;
        NormalizeHeight(playerSr, 1.4f);
        player.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        BoxCollider2D playerCol = player.AddComponent<BoxCollider2D>();
        playerCol.isTrigger = true;
        player.AddComponent<PlayerMovement>();

        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(player.transform, false);
        firePoint.transform.localPosition = new Vector3(0f, -0.5f, 0f);

        ShootOnClick shooter = player.AddComponent<ShootOnClick>();

        // ---- Chicken prefab (Object B) ----
        GameObject chickenPrefab = BuildChickenPrefab();

        // main chicken instance appears at top-center
        GameObject mainChicken = GameObject.Instantiate(chickenPrefab);
        mainChicken.name = "Chicken";
        ChickenMovement mainMover = mainChicken.GetComponent<ChickenMovement>();
        mainMover.snapToStartPosition = true; // places at top-center in Start
        mainChicken.GetComponent<SpriteRenderer>().sprite = LoadSprite("bird1.png");

        // ---- Bullet prefab (Object C) ----
        GameObject bulletPrefab = BuildBulletPrefab();
        shooter.bulletPrefab = bulletPrefab;
        shooter.firePoint = firePoint.transform;
        shooter.bulletSpeed = 12f;

        // ---- Egg prefab (enemy projectile) ----
        GameObject eggPrefab = BuildEggPrefab();

        // ---- ChickenSpawner ----
        GameObject spawnerGo = new GameObject("ChickenSpawner");
        ChickenSpawner spawner = spawnerGo.AddComponent<ChickenSpawner>();
        spawner.chickenPrefab = chickenPrefab;
        spawner.eggPrefab = eggPrefab;
        spawner.chickenSprites = new[]
        {
            LoadSprite("bird1.png"),
            LoadSprite("bird2.png"),
            LoadSprite("bird3.png"),
            LoadSprite("bird4.png")
        };

        // ---- AudioManager ----
        BuildAudioManager(new Vector3(-8f, -8f, 0f), false);

        // ---- GameManager + effects ----
        BuildGameManager();

        // ---- Camera note: managers marked DontDestroy at runtime persist across scenes

        // ---- HUD ----
        BuildHUD();

        AddEventSystem(scene);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, GameScenePath);
    }

    // ===== MANAGERS =====

    private static void BuildAudioManager(Vector3 pos, bool inSplash)
    {
        GameObject mgr = new GameObject("AudioManager");
        mgr.transform.position = pos;
        AudioManager audio = mgr.AddComponent<AudioManager>();
        audio.musicClip = LoadAudio("music.mp3");
        audio.shootClip = inSplash ? LoadAudio("click.ogg") : LoadAudio("click.ogg");
        audio.explosionClip = LoadAudio("explosion.wav");
        audio.clickClip = LoadAudio("click.ogg");
        audio.gameOverClip = LoadAudio("gameover.wav");
    }

    private static void BuildGameManager()
    {
        GameObject mgr = new GameObject("GameManager");
        GameManager gm = mgr.AddComponent<GameManager>();
        gm.explosionFrames = LoadExplosionFrames();
    }

    private static void BuildHUD()
    {
        GameObject hud = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        hud.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = hud.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        Sprite white = CreateWhiteSprite();
        Sprite pauseSprite = LoadSprite("pause1.png");
        Sprite resumeSprite = LoadSprite("resume1.png");
        Sprite restartSprite = LoadSprite("restart1.png");
        Sprite menuSprite = LoadSprite("menu.png");
        Sprite gameOverSprite = LoadSprite("game_over.png");
        Sprite starSprite = LoadSprite("star1.png");
        Sprite healthSprite = LoadSprite("health.png");

        // ---- Info backdrop panel (darker, behind icons/text) ----
        SetImage(CreateUIRect(hud, "InfoPanel", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(360, 240), new Vector2(180, -130)),
            white, new Color(0f, 0f, 0f, 0.55f));

        // ---- Info text + icons ----
        Image scoreIcon = SetImage(CreateUIRect(hud, "ScoreIcon", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(48, 48), new Vector2(50, -40)), starSprite, Color.white);
        scoreIcon.preserveAspect = true;

        Text scoreText = CreateText(hud, "ScoreText", "Score: 0", 42, Color.white, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(600, 60), new Vector2(100, -40));
        scoreText.alignment = TextAnchor.MiddleLeft;
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.rectTransform.pivot = new Vector2(0f, 0.5f);

        Text bestText = CreateText(hud, "BestText", "Best: 0", 30, new Color(1f, 0.9f, 0.3f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(600, 50), new Vector2(100, -105));
        bestText.alignment = TextAnchor.MiddleLeft;
        bestText.rectTransform.pivot = new Vector2(0f, 0.5f);

        Image livesIcon = SetImage(CreateUIRect(hud, "LivesIcon", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(48, 48), new Vector2(50, -165)), healthSprite, Color.white);
        livesIcon.preserveAspect = true;

        Text livesText = CreateText(hud, "LivesText", "Lives: 3", 34, new Color(1f, 0.5f, 0.4f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(400, 50), new Vector2(105, -165));
        livesText.alignment = TextAnchor.MiddleLeft;
        livesText.rectTransform.pivot = new Vector2(0f, 0.5f);

        Text gunText = CreateText(hud, "GunText", "GUN LV.1", 30, new Color(0.45f, 0.85f, 1f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(320, 50), new Vector2(105, -215));
        gunText.alignment = TextAnchor.MiddleLeft;
        gunText.rectTransform.pivot = new Vector2(0f, 0.5f);

        // ---- Boss HP bar (top-center) ----
        GameObject bossBarRoot = CreateUIRect(hud, "BossBar", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(900, 44), new Vector2(0, -120));
        bossBarRoot.SetActive(false);

        Text bossLabel = CreateText(bossBarRoot, "BossLabel", "BOSS", 26, new Color(1f, 0.4f, 0.3f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(200, 30), new Vector2(0, -4));
        bossLabel.alignment = TextAnchor.MiddleCenter;
        bossLabel.fontStyle = FontStyle.Bold;

        Image bossBarBg = SetImage(CreateUIRect(bossBarRoot, "BossBarBG", new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0, 34), Vector2.zero), white, new Color(0f, 0f, 0f, 0.75f));

        GameObject bossFillGo = CreateUIRect(bossBarRoot, "BossBarFill", new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0, 34), Vector2.zero);
        bossFillGo.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
        Image bossBarFill = SetImage(bossFillGo, white, new Color(0.85f, 0.2f, 0.15f, 1f));
        bossBarFill.type = Image.Type.Filled;
        bossBarFill.fillMethod = Image.FillMethod.Horizontal;
        bossBarFill.fillAmount = 0f;

        // ---- Level banner (center) ----
        Text bannerText = CreateText(hud, "BannerText", "LEVEL 1", 76, new Color(1f, 0.9f, 0.2f, 1f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(1400, 120), new Vector2(0, 150));
        bannerText.alignment = TextAnchor.MiddleCenter;
        bannerText.fontStyle = FontStyle.Bold;
        bannerText.color = new Color(1f, 0.9f, 0.2f, 0f);

        // ---- Pause button ----
        Button pauseBtn = CreateIconButton(hud, "PauseButton", pauseSprite, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(90, 90), new Vector2(-50, -40));

        // ---- Ready panel ----
        GameObject readyPanel = CreateUIRect(hud, "ReadyPanel", new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        SetImage(readyPanel, white, new Color(0f, 0f, 0f, 0.55f));

        Text readyTitle = CreateText(readyPanel, "ReadyTitle", "CHICKEN HUNTER", 84, new Color(1f, 0.85f, 0.2f, 1f), new Vector2(0.5f, 0.6f), new Vector2(0.5f, 0.6f), new Vector2(1200, 140), Vector2.zero);
        readyTitle.alignment = TextAnchor.MiddleCenter;
        readyTitle.fontStyle = FontStyle.Bold;

        Text readyHint = CreateText(readyPanel, "ReadyHint", "CHAM MAN HINH DE BAT DAU", 36, Color.white, new Vector2(0.5f, 0.45f), new Vector2(0.5f, 0.45f), new Vector2(1000, 70), Vector2.zero);
        readyHint.alignment = TextAnchor.MiddleCenter;

        // ---- Pause overlay ----
        GameObject pauseOverlay = CreateUIRect(hud, "PauseOverlay", new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        SetImage(pauseOverlay, white, new Color(0f, 0f, 0f, 0.72f));
        pauseOverlay.SetActive(false);

        Text pauseTitle = CreateText(pauseOverlay, "PauseTitle", "PAUSED", 72, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(700, 120), new Vector2(0, 220));
        pauseTitle.alignment = TextAnchor.MiddleCenter;
        pauseTitle.fontStyle = FontStyle.Bold;

        Button resumeBtn = CreateIconButton(pauseOverlay, "ResumeButton", resumeSprite, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(340, 90), new Vector2(0, 90));
        Button restartBtn = CreateIconButton(pauseOverlay, "RestartButton", restartSprite, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(340, 90), new Vector2(0, -20));
        Button menuBtn = CreateIconButton(pauseOverlay, "MenuButton", menuSprite, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(340, 90), new Vector2(0, -130));

        // ---- Game Over panel ----
        GameObject gameOverPanel = CreateUIRect(hud, "GameOverPanel", new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        SetImage(gameOverPanel, white, new Color(0f, 0f, 0f, 0.8f));
        gameOverPanel.SetActive(false);

        Image goBanner = SetImage(CreateUIRect(gameOverPanel, "GoBanner", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(700, 160), new Vector2(0, 190)),
            gameOverSprite == null ? white : gameOverSprite,
            Color.white);

        Text finalScoreText = CreateText(gameOverPanel, "FinalScore", "Score: 0", 50, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(700, 70), new Vector2(0, 40));
        finalScoreText.alignment = TextAnchor.MiddleCenter;
        finalScoreText.fontStyle = FontStyle.Bold;

        Text finalBestText = CreateText(gameOverPanel, "FinalBest", "Best: 0", 36, new Color(1f, 0.9f, 0.3f, 1f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(700, 60), new Vector2(0, -35));
        finalBestText.alignment = TextAnchor.MiddleCenter;

        Button retryBtn = CreateIconButton(gameOverPanel, "RetryButton", restartSprite, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(340, 90), new Vector2(0, -120));
        Button menuGoButton = CreateIconButton(gameOverPanel, "MenuGameOverButton", menuSprite, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(340, 90), new Vector2(0, -220));

        // ---- Wire HUD ----
        HUD hudScript = hud.AddComponent<HUD>();
        hudScript.scoreText = scoreText;
        hudScript.livesText = livesText;
        hudScript.bestText = bestText;
        hudScript.scoreIcon = scoreIcon;
        hudScript.livesIcon = livesIcon;
        hudScript.gunText = gunText;
        hudScript.bossBarRoot = bossBarRoot;
        hudScript.bossBarFill = bossBarFill;
        hudScript.bannerText = bannerText;
        hudScript.readyPanel = readyPanel;
        hudScript.pauseButton = pauseBtn;
        hudScript.pauseOverlay = pauseOverlay;
        hudScript.resumeButton = resumeBtn;
        hudScript.restartButton = restartBtn;
        hudScript.menuButton = menuBtn;
        hudScript.gameOverPanel = gameOverPanel;
        hudScript.finalScoreText = finalScoreText;
        hudScript.finalBestText = finalBestText;
        hudScript.retryButton = retryBtn;
        hudScript.menuGameOverButton = menuGoButton;
    }

    // ===== PREFABS =====

    private static GameObject BuildBulletPrefab()
    {
        Sprite bulletSprite = LoadSprite("bullet1.png");
        if (bulletSprite == null) bulletSprite = LoadSprite("bullet2.png");

        GameObject bullet = new GameObject("Bullet");
        SpriteRenderer sr = bullet.AddComponent<SpriteRenderer>();
        sr.sprite = bulletSprite;
        sr.sortingOrder = 10;

        NormalizeHeight(sr, 0.45f);

        Rigidbody2D rb = bullet.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D col = bullet.AddComponent<CircleCollider2D>();
        col.isTrigger = true;

        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.speed = 12f;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(bullet, BulletPrefabPath);
        Object.DestroyImmediate(bullet);
        return prefab;
    }

    private static GameObject BuildChickenPrefab()
    {
        Sprite chickenSprite = LoadSprite("bird1.png");
        if (chickenSprite == null) chickenSprite = LoadSprite("bird2.png");

        GameObject chicken = new GameObject("Chicken");
        SpriteRenderer sr = chicken.AddComponent<SpriteRenderer>();
        sr.sprite = chickenSprite;
        sr.sortingOrder = 5;
        NormalizeHeight(sr, 1.2f);

        Rigidbody2D rb = chicken.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        BoxCollider2D col = chicken.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        ChickenMovement mover = chicken.AddComponent<ChickenMovement>();
        mover.speed = 2.2f;
        mover.hp = 1;
        mover.maxHp = 1;
        mover.points = 10;

        chicken.AddComponent<ChickenEggShooter>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(chicken, ChickenPrefabPath);
        Object.DestroyImmediate(chicken);
        return prefab;
    }

    private static GameObject BuildEggPrefab()
    {
        Sprite eggSprite = LoadSprite("bomb1.png");

        GameObject egg = new GameObject("Egg");
        SpriteRenderer sr = egg.AddComponent<SpriteRenderer>();
        sr.sprite = eggSprite;
        sr.sortingOrder = 10;
        NormalizeHeight(sr, 0.5f);

        Rigidbody2D rb = egg.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CircleCollider2D col = egg.AddComponent<CircleCollider2D>();
        col.isTrigger = true;

        Egg eggScript = egg.AddComponent<Egg>();
        eggScript.speed = 6f;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(egg, EggPrefabPath);
        Object.DestroyImmediate(egg);
        return prefab;
    }

    // ===== UI HELPERS =====

    private static GameObject CreateUIRect(GameObject parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Vector2 pos)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;
        return go;
    }

    private static Image SetImage(GameObject go, Sprite sprite, Color color)
    {
        Image img = go.AddComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        return img;
    }

    private static Text CreateText(GameObject parent, string name, string text, int fontSize, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Vector2 pos)
    {
        GameObject go = CreateUIRect(parent, name, anchorMin, anchorMax, size, pos);
        Text txt = go.AddComponent<Text>();
        txt.text = text;
        txt.fontSize = fontSize;
        txt.color = color;
        if (uiFont != null)
            txt.font = uiFont;
        else
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.horizontalOverflow = HorizontalWrapMode.Overflow;
        txt.verticalOverflow = VerticalWrapMode.Overflow;
        return txt;
    }

    private static Button CreateIconButton(GameObject parent, string name, Sprite sprite, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Vector2 pos)
    {
        GameObject go = CreateUIRect(parent, name, anchorMin, anchorMax, size, pos);
        Image img = SetImage(go, sprite, Color.white);
        img.preserveAspect = false;
        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        SpriteState state = new SpriteState();
        btn.transition = Selectable.Transition.SpriteSwap;
        btn.spriteState = state;
        return btn;
    }

    // ===== WORLDD HELPERS =====

    private static Sprite CreateWhiteSprite()
    {
        string path = "Assets/Sprites/WhiteSquare.png";
        if (File.Exists(path))
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);

        Texture2D tex = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();

        if (!AssetDatabase.IsValidFolder("Assets/Sprites"))
            AssetDatabase.CreateFolder("Assets", "Sprites");

        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);

        TextureImporter imp = (TextureImporter)AssetImporter.GetAtPath(path);
        imp.textureType = TextureImporterType.Sprite;
        imp.spritePixelsPerUnit = 100;
        imp.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static void NormalizeHeight(SpriteRenderer sr, float targetHeight)
    {
        float h = sr.bounds.size.y;
        if (h > 0.01f)
        {
            float factor = targetHeight / h;
            if (factor > 0.01f)
                sr.transform.localScale = new Vector3(
                    sr.transform.localScale.x * factor,
                    sr.transform.localScale.y * factor,
                    1f);
        }
    }

    private static void FitBackground(SpriteRenderer sr, Camera cam, float targetHeight)
    {
        float h = sr.bounds.size.y;
        float w = sr.bounds.size.x;
        if (h <= 0.01f) return;

        float scaleY = targetHeight / h;
        float camHalfW = cam.orthographicSize * cam.aspect;
        float scaleX = Mathf.Max(scaleY, (camHalfW * 2f) / w);
        sr.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    // ===== SCENE HELPERS =====

    private static Scene NewSceneOrClearCurrent()
    {
        return EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }

    private static GameObject GetOrCreateMainCamera(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.CompareTag("MainCamera"))
                return root;
        }
        GameObject camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        Camera cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        camGo.AddComponent<AudioListener>();
        camGo.transform.position = new Vector3(0f, 0f, -10f);
        return camGo;
    }

    private static void AddEventSystem(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.GetComponent<EventSystem>() != null)
                return;
        }
        GameObject esGo = new GameObject("EventSystem");
        esGo.AddComponent<EventSystem>();
        InputSystemUIInputModule module = esGo.AddComponent<InputSystemUIInputModule>();
        module.AssignDefaultActions();
    }
}