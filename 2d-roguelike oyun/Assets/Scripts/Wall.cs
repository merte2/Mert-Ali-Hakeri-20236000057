using UnityEngine;

public class Wall : MonoBehaviour
{
    // Duvarın zarar gördüğünde gösterilecek olan sprite
    public Sprite dmgSprite;
    // Duvarın başlangıçtaki sağlık değeri
    public int hp = 4;

    // Duvarı kesme ses efektleri (çalışırken rastgele seçilecek)
    public AudioClip[] chopSounds;

    // Duvarın sprite'ını değiştirecek olan SpriteRenderer bileşeni
    private SpriteRenderer spriteRenderer;

    // Wall sınıfı yüklendiğinde yapılacak işlemler
    void Awake()
    {
        // SpriteRenderer bileşenini alır
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Duvara zarar verme fonksiyonu
    public void DamageWall(int loss)
    {
        // Rastgele bir kesme sesi çalar
        SoundManager.Instance.RandomizeSfx(chopSounds);

        // Duvarın görselini değiştiren sprite'ı atar
        spriteRenderer.sprite = dmgSprite;

        // Duvarın sağlığını azaltır
        hp -= loss;

        // Eğer duvarın sağlığı sıfır ya da daha düşükse, duvarı devre dışı bırakır
        if (hp <= 0) gameObject.SetActive(false);
    }
}
