using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    int direction = 1; // Направление движения монеты
    bool isCollected = false; // Флаг, показывающий, была ли монета собрана

    float high = 1.6f; // Наивысшая точка
    float low = 1.2f; // Нисшая точка

    public GameObject coinSound; // Звук монеты
    public GameObject coinPrefab; // Префаб новой монеты

    void Update()
    {
        transform.Rotate(0f, 1f, 0f); // Монета с каждым кадром будет вращаться

        if (direction > 0) // Если направление больше нуля, то монета будет двигаться вверх
        {
            if (transform.position.y < high) // Пока не достигнет наивысшей точки
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z);
            }
            else
            {
                direction *= -1; // После этого направление изменится
            }
        }
        else // Иначе монета будет двигаться вниз
        {
            if (transform.position.y > low) // Пока не достигнет нижней точки
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.01f, transform.position.z);
            }
            else
            {
                direction *= -1; // После этого направление изменится
            }
        }
    }

    public void Delete() // Удаление монеты
    {
        // Запускаем корутину, чтобы постепенно уменьшить размер монеты
        StartCoroutine(ScaleDownOverTime());

        var sound = Instantiate(coinSound); //Добавление звука монеты

        Destroy(sound, 2f); //Уничтожение звука через две секунды
    }

    // Корутина для плавного уменьшения размера монеты
    IEnumerator ScaleDownOverTime()
    {
        float duration = 0.2f; // Продолжительность анимации изменения размера
        Vector3 originalScale = transform.localScale; // Изначальный размер монеты
        Vector3 targetScale = Vector3.zero; // Целевой размер (нулевой)

        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            // Интерполируем между начальным и целевым размером
            transform.localScale = Vector3.Lerp(originalScale, targetScale, timeElapsed / duration);

            timeElapsed += Time.deltaTime;
            yield return null; // Ждем следующего кадра
        }

        // После завершения анимации создаем новую монету
        CreateNewCoin();

        // Отключаем объект монеты
        gameObject.SetActive(false);
    }

    // Создание новой монеты
    void CreateNewCoin()
    {
        Instantiate(coinPrefab, transform.position, transform.rotation);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Coin trigger entered with: " + other.gameObject.name); // Выводим имя объекта, с которым произошло столкновение

        // Проверяем, столкнулась ли монета с игроком
        if (other.CompareTag("Player"))
        {
            // Если монета еще не была собрана
            if (!isCollected)
            {
                isCollected = true; // Устанавливаем флаг, что монета была собрана
                Delete(); // Вызываем метод удаления монеты
            }
        }
    }
}
