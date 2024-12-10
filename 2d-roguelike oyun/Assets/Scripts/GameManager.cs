using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Singleton pattern: Oyun yöneticisinin tek bir örneğini tutar
    public static GameManager Instance { get; private set; }

    // Seviyenin başlangıç gecikmesi ve her dönüş için gecikme süreleri
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;

    // Oyuncunun başlangıçta sahip olduğu yiyecek puanı
    public int playerFoodPoints = 100;

    // Oyuncunun sırası mı, yoksa düşmanların sırası mı olduğunu kontrol eder
    [HideInInspector] public bool playersTurn = true;

    // Seviye metni ve seviyenin gösterileceği görüntü
    private Text levelText;
    private GameObject levelImage;

    // BoardManager referansı, oyun tahtasının kontrol edilmesini sağlar
    private BoardManager boardScript;

    // Oyun kurulumu yapılırken geçici bir bayrak
    private bool doingSetup;

    // Şu anki seviye, düşmanlar ve düşmanların hareket durumunu tutar
    private int level = 0;
    private List<Enemy> enemies;
    private bool enemiesMoving;

    // Oyun başlatıldığında sadece bir kez çağrılır
    void Awake()
    {
        // Eğer Instance zaten varsa, bu nesne yok edilir
        if (Instance == null) Instance = this;

        // Eğer Instance başka bir nesne ise, bu nesneyi yok eder
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Oyun başladığında bu nesnenin yok edilmemesini sağlar
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
    }

    // Oyun bittiğinde çağrılan fonksiyon
    public void GameOver()
    {
        // Oyun bittiğinde seviyeyi gösterir ve oyuncuyu bilgilendirir
        levelText.text = $"After {level} days, you starved.";
        levelImage.SetActive(true); // Oyun bitti ekranını göster
        enabled = false; // Oyun yöneticisi artık çalışmasın
    }

    // Düşman eklemek için kullanılan fonksiyon
    public void AddEnemyToList(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    // Scene yüklendikçe oyunun yapılacak ayarları
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading; // Seviye bittiğinde yapılacak işlemi ayarlıyoruz
    }

    // Scene silindiğinde, etkinlikleri iptal eder
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    // Her frame'de kontrol edilen fonksiyon
    void Update()
    {
        // Eğer oyuncunun sırası, düşmanlar hareket ediyorsa veya kurulum yapılıyorsa işlemi yapma
        if (playersTurn || enemiesMoving || doingSetup) return;

        // Düşmanları hareket ettirmek için Coroutine başlatır
        StartCoroutine(MoveEnemies());
    }

    // Oyunu başlatan fonksiyon
    void InitGame()
    {
        doingSetup = true;

        // Bu adım gerçek projelerde yapılmamalıdır, sadece bu örnek için
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = $"Day: {level}"; // Seviye gününü ayarlama
        levelImage.SetActive(true); // Seviyeyi başlatma ekranını göster

        // Seviye başlama gecikmesini bekler
        Invoke("HideLevelImage", levelStartDelay);

        // Düşmanları temizle
        enemies.Clear();

        // Tahtayı kurar
        boardScript.SetupScene(level);
    }

    // Seviye başlama ekranını gizler
    void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    // Yeni seviye yüklendiğinde çağrılır
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        level += 1; // Seviyeyi bir arttır
        InitGame(); // Yeni seviyeyi başlat
    }

    // Düşmanları sırayla hareket ettiren Coroutine
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;

        // Düşman hareketi için bir gecikme süresi bekler
        yield return new WaitForSecondsRealtime(turnDelay);

        // Eğer hiç düşman yoksa, hareket etmeyi bekler
        if (enemies.Count == 0)
        {
            yield return new WaitForSecondsRealtime(turnDelay);
        }

        // Her düşmanı sırayla hareket ettirir
        for (int i = 0; i < enemies.Count; ++i)
        {
            enemies[i].MoveEnemy(); // Düşmanı hareket ettir
            yield return new WaitForSecondsRealtime(enemies[i].moveTime); // Düşman hareketini bitirmesi için bekler
        }

        // Oyuncunun sırası geldiğinde tekrar oyuncuyu bekler
        playersTurn = true;
        enemiesMoving = false;
    }
}