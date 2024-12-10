using UnityEngine;

public class Loader : MonoBehaviour
{
    // Oyun yöneticisinin GameObject referansı
    public GameObject gameManager;

    // Awake fonksiyonu, nesne oluşturulmadan önce çağrılır
    void Awake()
    {
        // Eğer GameManager örneği yoksa, yeni bir tane oluştur
        if (GameManager.Instance == null)
            Instantiate(gameManager); // GameManager nesnesini oluşturur
    }
}