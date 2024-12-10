using UnityEngine;

public class Enemy : MovingObject
{
    // Oyuncuya vereceği hasar
    public int playerDamage;

    // Animator, hedef (oyuncu) ve hareketi atlama durumu için değişkenler
    private Animator animator;
    private Transform target;
    private bool skipMove;

    // Düşman saldırı sesleri
    public AudioClip[] attackClips;

    // Başlangıçta yapılacak işlemler
    protected override void Start()
    {
        // Düşmanı oyun yöneticisine ekler
        GameManager.Instance.AddEnemyToList(this);

        // Animator bileşenini alır
        animator = GetComponent<Animator>();

        // Oyuncunun transform bileşenini bulur
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // Üst sınıfın başlangıç fonksiyonunu çağırır
        base.Start();
    }

    // Düşmanı hareket ettirme fonksiyonu
    public void MoveEnemy()
    {
        Vector2Int dir = Vector2Int.zero; // Hareket yönünü belirleyecek değişken

        // Eğer oyuncu ile düşman yatayda aynı hizadaysa, düşman oyuncuya dikey olarak yaklaşır
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            dir.y = target.position.y > transform.position.y ? 1 : -1; // Yukarı ya da aşağı hareket
        else
            dir.x = target.position.x > transform.position.x ? 1 : -1; // Sağ ya da sol hareket

        // Hareketi gerçekleştirmek için base sınıfındaki AttemptMove fonksiyonunu çağırır
        AttemptMove<Player>(dir);
    }

    // Hareket etme işlemi (Override edilmiş fonksiyon)
    protected override void AttemptMove<T>(Vector2Int dir)
    {
        // Eğer hareket atlanacaksa, fonksiyondan çık
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        // Base sınıfın hareket fonksiyonunu çağırarak oyuncuyu hareket ettirir
        base.AttemptMove<T>(dir);

        // Bir sonraki hareketi atlamak için skipMove değişkenini true yapar
        skipMove = true;
    }

    // Eğer bir şeyle çarpışma gerçekleşirse (bu durumda oyuncu), bu fonksiyon çalışır
    protected override void OnCantMove<T>(T component)
    {
        // Çarpışılan nesne Player ise, saldırı animasyonunu oynatır
        Player hitPlayer = component as Player;
        animator.SetTrigger("enemyAttack"); // Saldırı animasyonunu tetikler
        SoundManager.Instance.RandomizeSfx(attackClips); // Rastgele bir saldırı sesi çalar

        // Oyuncuya hasar verir
        hitPlayer.LossFood(playerDamage);
    }
}