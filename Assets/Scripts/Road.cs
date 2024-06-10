using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public List<GameObject> blocks; // Коллекция всех блоков дороги
    public GameObject player; // Игрок
    public GameObject roadPrefab; // Префаб блока дороги
    public GameObject carPrefab; // Префаб машины NPC
    public GameObject coinPrefab; // Префаб монеты

    private System.Random rand = new System.Random(); // Генератор случайных чисел
    private List<Vector3> carPositions = new List<Vector3>(); // Список всех позиций машин

    private float minDistanceBetweenCars = 40.0f; // Минимальное расстояние между машинами по горизонтали
    private float laneZLeft = -0.80f; // Координата Z для левой полосы
    private float laneZRight = -3.40f; // Координата Z для правой полосы
    private int maxCarsPerBlock = 1; // Максимальное количество машин на блок
    private float initialSpawnOffset = 15.0f; // Начальное смещение для спавна машин

    void Start()
    {
        if (player == null || roadPrefab == null || carPrefab == null || coinPrefab == null)
        {
            Debug.LogError("Один или несколько необходимых GameObject отсутствуют!");
        }

        if (blocks == null)
        {
            blocks = new List<GameObject>();
            Debug.LogError("Список блоков не инициализирован!");
        }
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Ссылка на игрока равна null!");
            return;
        }

        Moving moving = player.GetComponent<Moving>();
        if (moving == null || moving.rb == null)
        {
            Debug.LogError("Компонент Moving или Rigidbody на игроке отсутствует!");
            return;
        }

        float x = moving.rb.position.x;

        if (blocks.Count == 0)
        {
            Debug.LogWarning("В списке нет блоков для проверки.");
            return;
        }

        var last = blocks[blocks.Count - 1];

        if (x > last.transform.position.x - 24.69f * 10f)
        {
            var block = Instantiate(roadPrefab, new Vector3(last.transform.position.x + 24.69f, last.transform.position.y, last.transform.position.z), Quaternion.identity);
            block.transform.SetParent(gameObject.transform);
            blocks.Add(block);

            // Ensure cars are spawned far enough from the player
            float carStartX = Mathf.Max(player.transform.position.x + initialSpawnOffset, last.transform.position.x + 24.69f);

            // Track the last used lane to alternate lanes for the checkerboard pattern
            bool useLeftLane = (carPositions.Count % 2 == 0);

            for (int i = 0; i < maxCarsPerBlock; i++)
            {
                float laneZ = useLeftLane ? laneZLeft : laneZRight;
                Vector3 newCarPosition = new Vector3(carStartX + (i * minDistanceBetweenCars), 0.96f, laneZ);

                // Check for minimum distance to previous car in adjacent lane
                if (i > 0 && Mathf.Abs(newCarPosition.x - carPositions[carPositions.Count - 1].x) < minDistanceBetweenCars)
                {
                    // Adjust x position to ensure enough space for player to maneuver
                    newCarPosition.x += minDistanceBetweenCars;
                }

                var car = Instantiate(carPrefab, newCarPosition, Quaternion.Euler(new Vector3(0f, 90f, 0f)));
                car.transform.SetParent(gameObject.transform);
                carPositions.Add(newCarPosition);

                Debug.Log($"Car spawned at: {newCarPosition}");

                useLeftLane = !useLeftLane; // Alternate lane for the next car
            }

            // Clean up the list to remove positions that are too far back
            carPositions.RemoveAll(pos => pos.x < player.transform.position.x - 50f); // Adjust as needed

            // Coin creation logic
            if (rand.Next(0, 100) > 90)
            {
                float coinLaneZ = rand.Next(2) == 0 ? laneZLeft : laneZRight;
                var coin = Instantiate(coinPrefab, new Vector3(carStartX + initialSpawnOffset, 0.96f, coinLaneZ), Quaternion.identity);
                coin.transform.SetParent(gameObject.transform);
                Debug.Log($"Coin spawned at: {coin.transform.position}");
            }
        }

        for (int i = blocks.Count - 1; i >= 0; i--)
        {
            GameObject block = blocks[i];
            RoadBlock roadBlock = block.GetComponent<RoadBlock>();
            if (roadBlock == null)
            {
                Debug.LogError("Компонент RoadBlock отсутствует на блоке!");
                continue;
            }

            bool fetched = roadBlock.Fetch(x);

            if (fetched)
            {
                blocks.RemoveAt(i);
                roadBlock.Delete();
            }
        }
    }
}
