using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    // Duvara verilen hasar miktarı
    public int wallDamage = 1;

    // Yiyecek başına kazanılan puan
    public int pointsPerFood = 10;

    // Soda başına kazanılan puan
    public int pointsPerSoda = 20;

    // Seviye yeniden başlatma gecikme süresi
    public float restartLevelDelay = 1f;

    // Yiyecek miktarını gösterecek UI metni
    public Text foodText;

    // Hareket, yeme ve içme ses efektleri
    public AudioClip[] moveSounds;
    public AudioClip[] eatSounds;
    public AudioClip[] drinkSounds;
    public AudioClip gameOverSound;

    // Animator bileşeni
    private Animator animator;

    // Oyuncunun yiyecek miktarı
    private int food;

    // Dokunmatik giriş başlangıç noktası
    private Vector2 touchOrigin = -Vector2.one;

    // Başlangıç ayarları
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.Instance.playerFoodPoints; // Başlangıçta yiyecek miktarını alır
        foodText.text = $"Food: {food}"; // Yiyecek miktarını UI'ye yansıtır

        base.Start();
    }

    // Yiyecek kaybı işlemi
    public void LossFood(int loss)
    {
        animator.SetTrigger("playerHit"); // Animasyonu oynatır
        food -= loss; // Yiyecek miktarını azaltır
        foodText.text = $"-{loss} Food: {food}"; // Yeni yiyecek miktarını UI'ye yansıtır
        CheckIfGameOver(); // Oyun bitti mi kontrol eder
    }

    // Hareket etme işlemi
    protected override void AttemptMove<T>(Vector2Int dir)
    {
        food -= 1; // Her hareket için yiyecek kaybeder
        foodText.text = $"Food: {food}"; // Yeni yiyecek miktarını UI'ye yansıtır
        base.AttemptMove<T>(dir); // Hareketi dener

        RaycastHit2D hit;
        if (Move(dir, out hit)) SoundManager.Instance.RandomizeSfx(moveSounds); // Hareket başarılıysa ses çalar

        CheckIfGameOver(); // Oyun bitti mi kontrol eder
        GameManager.Instance.playersTurn = false; // Oyuncunun sırası bitmiştir
    }

    // Çarpışma olduğunda yapılacak işlem (örneğin, duvara çarpma)
    protected override void OnCantMove<T>(T component)
    {
        var hitWall = component as Wall; // Çarpışan nesne duvar mı diye kontrol eder
        hitWall.DamageWall(wallDamage); // Duvara hasar verir
        animator.SetTrigger("playerChop"); // Duvara vurma animasyonunu oynatır
    }

    // Güncellenen her frame'de oyuncu hareketi
    private void Update()
    {
        if (!GameManager.Instance.playersTurn) return; // Eğer oyuncunun sırası değilse hareket etmez

        Vector2Int direction = Vector2Int.zero; // Yönü sıfırlar

#if UNITY_STANDALONE || UNITY_WEBGL

        // Bilgisayar girişleri (klavye ok tuşları ile)
        direction = new Vector2Int(
            (int)Input.GetAxisRaw("Horizontal"),
            (int)Input.GetAxisRaw("Vertical")
        );

#else

        // Mobil cihazlar için dokunmatik ekran girişleri
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0]; // İlk dokunuşu alır
            if (touch.phase == TouchPhase.Began)
            {
                touchOrigin = touch.position; // Dokunuş başlangıcı
            }
            else if (touch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = touch.position; // Dokunuş bitişi
                Vector2 swipeDir = touchEnd - touchOrigin; // Kaydırma yönünü hesaplar
                touchOrigin.x = -1;
                if (Mathf.Abs(swipeDir.x) > Mathf.Abs(swipeDir.y))
                    direction = new Vector2Int((int)(1f * Mathf.Sign(swipeDir.x)), 0); // Yatay kaydırma
                else
                    direction = new Vector2Int(0, (int)(1f * Mathf.Sign(swipeDir.y))); // Dikey kaydırma
            }
        }

#endif

        // Yalnızca yatay hareketi kabul et (dikey hareket yoksa)
        if (direction.x != 0) direction.y = 0;

        if (direction != Vector2Int.zero) AttemptMove<Wall>(direction); // Hareket etmeyi dener
    }

    // Etkileşimde bulunduğunda tetiklenen olaylar (örneğin, çıkış, yiyecek, soda)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit")) // Eğer çıkışa çarptıysa
        {
            StartCoroutine(RestartDelayed(restartLevelDelay)); // Seviye yeniden başlatılacak
            enabled = false; // Bu sınıfı devre dışı bırakır
        }
        else if (other.CompareTag("Food")) // Eğer yiyeceğe çarptıysa
        {
            food += pointsPerFood; // Yiyecek miktarını artırır
            other.gameObject.SetActive(false); // Yiyeceği yok eder
            foodText.text = $"+{pointsPerFood} Food: {food}"; // Yeni yiyecek miktarını UI'ye yansıtır
            SoundManager.Instance.RandomizeSfx(eatSounds); // Yiyecek yeme sesini çalar
        }
        else if (other.CompareTag("Soda")) // Eğer soda bulduysa
        {
            food += pointsPerSoda; // Yiyecek miktarını artırır
            other.gameObject.SetActive(false); // Sodayı yok eder
            foodText.text = $"+{pointsPerSoda} Food: {food}"; // Yeni yiyecek miktarını UI'ye yansıtır
            SoundManager.Instance.RandomizeSfx(drinkSounds); // İçme sesini çalar
        }
    }

    // Bu sınıf devre dışı kaldığında (oyuncu öldüğünde) yapılacak işlemler
    private void OnDisable()
    {
        GameManager.Instance.playerFoodPoints = food; // Son yiyecek miktarını kaydeder
    }

    // Oyuncu ölüm kontrolü
    private void CheckIfGameOver()
    {
        if (food <= 0) // Eğer yiyecek miktarı sıfır veya daha azsa
        {
            GameManager.Instance.GameOver(); // Oyunu bitirir
            SoundManager.Instance.PlaySingle(gameOverSound); // Oyun bitiş sesini çalar
            SoundManager.Instance.musicSource.Stop(); // Müzik çalmayı durdurur
            enabled = false; // Bu sınıfı devre dışı bırakır
        }
    }

    // Seviye yeniden başlatma gecikmesi
    private IEnumerator RestartDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Belirtilen süreyi bekler
        Restart(); // Seviye yeniden başlatılır
    }

    // Seviye sıfırlanır
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Aynı sahneyi yeniden yükler
    }
}