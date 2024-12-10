using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    // Sütun ve satır sayıları
    public int columns = 8;
    public int rows = 8;

    // Duvar ve yiyecek için minimum ve maksimum değerler
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);

    // Çıkış, zemin, duvar, yiyecek ve düşman nesneleri
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    // Tahtanın tutacağı nesne ve pozisyonlar listesi
    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    // Sahneyi başlatan fonksiyon
    public void SetupScene(int level)
    {
        BoardSetup(); // Tahtayı kurar
        InitialiseList(); // Pozisyon listesini başlatır
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum); // Rastgele duvar yerleştirir
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum); // Rastgele yiyecek yerleştirir

        // Düşman sayısını hesaplar ve rastgele düşman yerleştirir
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        // Çıkışı sağ alt köşeye yerleştirir
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }

    // Pozisyonlar listesini başlatan fonksiyon
    void InitialiseList()
    {
        gridPositions.Clear(); // Listeyi temizler

        // Tahtadaki her geçerli pozisyonu gridPositions listesine ekler
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // Tahtayı kuran fonksiyon
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform; // Yeni bir Board nesnesi oluşturur

        // Tahtadaki tüm hücrelere zemin ve dış duvar yerleştirir
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate;

                // Dış sınırlar için dış duvar yerleştirir, iç kısımlar için zemin
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                else
                    toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                // Nesneyi sahneye ekler ve boardHolder'a ekler
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    // Rastgele bir pozisyon döndüren fonksiyon
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count); // Rastgele bir indeks seçer

        Vector3 randomPosition = gridPositions[randomIndex]; // Pozisyonu alır
        gridPositions.RemoveAt(randomIndex); // Seçilen pozisyonu listeden çıkarır
        return randomPosition; // Pozisyonu döndürür
    }

    // Nesneleri rastgele bir şekilde yerleştiren fonksiyon
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        // Rastgele nesne sayısını belirler
        int objectCount = Random.Range(minimum, maximum + 1);

        // Belirtilen nesne sayısı kadar yerleştirir
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition(); // Rastgele bir pozisyon alır
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)]; // Rastgele bir nesne seçer
            Instantiate(tileChoice, randomPosition, Quaternion.identity); // Seçilen nesneyi sahneye yerleştirir
        }
    }

    // Minimum ve maksimum değerler için bir sınıf
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        // Sınıfın yapıcı fonksiyonu
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }
}