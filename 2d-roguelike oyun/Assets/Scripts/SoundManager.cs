using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton örneği
    public static SoundManager Instance { get; private set; }

    // Ses kaynakları (efektler ve müzik için)
    public AudioSource efxSource;
    public AudioSource musicSource;

    // Efekt seslerinin pitch aralığı (düşük ve yüksek)
    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    // Varsayılan pitch değeri
    private float defaultPitchRange;

    // SoundManager yüklendiğinde yapılacak işlemler
    void Awake()
    {
        // Singleton desenini uygular
        if (Instance == null) Instance = this;
        if (Instance != this)
        {
            Destroy(gameObject); // Eğer başka bir SoundManager varsa, bunu yok et
            return;
        }

        // Ses yöneticisinin sahneler arasında korunmasını sağlar
        DontDestroyOnLoad(gameObject);
        defaultPitchRange = efxSource.pitch; // Varsayılan pitch değerini saklar
    }

    // Tek bir ses parçası çalar
    public void PlaySingle(AudioClip clip)
    {
        // Efekt sesinin pitch değerini varsayılana ayarlar
        efxSource.pitch = defaultPitchRange;
        // Verilen ses parçasını atar
        efxSource.clip = clip;
        // Ses parçasını çalar
        efxSource.Play();
    }

    // Rasgele bir ses efekti çalar (sesin pitch'ini rastgele değiştirir)
    public void RandomizeSfx(params AudioClip[] clips)
    {
        // Rasgele bir ses parçası seçer
        int randomIndex = Random.Range(0, clips.Length);
        // Rasgele bir pitch değeri seçer
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        // Ses kaynağının pitch değerini ayarlar
        efxSource.pitch = randomPitch;
        // Rasgele seçilen ses parçasını atar
        efxSource.clip = clips[randomIndex];
        // Ses parçasını çalar
        efxSource.Play();
    }
}