using System.Collections;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    // Nesnenin hareket etme süresi
    public float moveTime = 0.1f;

    // Nesnenin engellerle çarpışıp çarpmadığını kontrol etmek için kullanılan katman
    public LayerMask blockingLayer;

    // Nesnenin BoxCollider2D ve Rigidbody2D bileşenleri
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2d;

    // Hareket süresinin tersini tutan değişken (hız için kullanılır)
    private float inverseMoveTime;

    // Başlangıçta yapılacak ayarlar
    protected virtual void Start()
    {
        // BoxCollider2D bileşenini alır
        boxCollider = GetComponent<BoxCollider2D>();

        // Rigidbody2D bileşenini alır
        rb2d = GetComponent<Rigidbody2D>();

        // Hareket süresinin tersini hesaplar (bu, hareketin hızını belirler)
        inverseMoveTime = 1f / moveTime;
    }

    // Bir yönü takip ederek nesneyi hareket ettirir
    protected bool Move(Vector2Int dir, out RaycastHit2D hit)
    {
        // Başlangıç ve hedef pozisyonlarını belirler
        Vector2 start = transform.position;
        Vector2 end = start + dir;

        // BoxCollider2D'yi geçici olarak devre dışı bırakır (çarpışma tespiti için)
        boxCollider.enabled = false;

        // Belirtilen engel katmanı ile çarpışma tespiti yapar
        hit = Physics2D.Linecast(start, end, blockingLayer);

        // BoxCollider2D'yi tekrar etkinleştirir
        boxCollider.enabled = true;

        // Eğer çarpışma yoksa, hareketi başlatır
        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end)); // Yavaş hareket için Coroutine başlatır
            return true;
        }

        // Çarpışma varsa, hareketi engeller
        return false;
    }

    // Hareket etme işlemini dener
    protected virtual void AttemptMove<T>(Vector2Int dir) where T : Component
    {
        // Çarpışma tespiti yapar
        RaycastHit2D hit;

        // Hareketin mümkün olup olmadığını kontrol eder
        bool canMove = Move(dir, out hit);

        // Eğer çarpışma varsa ve çarpışan nesne belirtilen tipte ise, çarpışmaya tepki verir
        if (hit.transform == null) return;

        T hitComponent = hit.transform.GetComponent<T>();

        // Eğer hareket mümkün değilse ve hedef nesne belirtilen türdeyse, çarpışmaya tepki verir
        if (!canMove && hitComponent != null) OnCantMove(hitComponent);
    }

    // Yavaş hareket işlemi (smooth movement)
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        // Kalan mesafeyi kareler cinsinden hesaplar
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        // Hedefe ulaşana kadar hareket etmeye devam eder
        while (sqrRemainingDistance > float.Epsilon)
        {
            // Yeni pozisyonu hedefe doğru hareket ettirir
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
            rb2d.MovePosition(newPosition); // Rigidbody2D'yi yeni pozisyona taşır

            // Kalan mesafeyi tekrar hesaplar
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null; // Bir sonraki frame'e geçer
        }
    }

    // Bu metod, çarpışma durumunda nasıl tepki verileceğini belirler (soyut metot, alt sınıflarda implement edilmelidir)
    protected abstract void OnCantMove<T>(T component) where T : Component;

}