using System;
using System.IO;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public float speed = 0f; // Speed
    public float maxSpeed = 0.5f; // Maximum speed
    public float sideSpeed = 0f; // Side speed

    public float scores = 0f; // Scores
    public float highScore = 0f; // High score

    private string filePath = "D:\\Source\\reposUnity\\Coursework_KG\\Assets\\Scripts\\highscore.txt"; // File path to save high score

    public float accelerationRate = 0.0001f; // Rate at which the car accelerates
    public float decelerationRate = 0.0003f; // Rate at which the car decelerates

    void Start()
    {
        // Define the file path where high score will be saved
        filePath = Application.persistentDataPath + "/highscore.txt";

        // Load high score from file
        LoadHighScore();
    }

    void Update()
    {
        float moveSide = Input.GetAxis("Horizontal"); //Когда игрок будет нажимать на стрелочки влево или вправо, сюда будет добавляеться 1f или -1f
        float moveForward = Input.GetAxis("Vertical"); //То же само, но со стрелочками вверх и вниз

        if (moveSide != 0)
        {
            sideSpeed = moveSide * -0.3f; //Если игрок нажал на стрелочки влево или вправо, задаём боковую скорость
        }

        if (moveForward > 0)
        {
            speed += accelerationRate * moveForward; //Если игрок нажал вверх
        }
        else if (moveForward < 0)
        {
            speed += decelerationRate * moveForward; //Если игрок нажал вниз
        }
        else //Если игрок не нажал ни вверх, ни вниз, то скорость будет постепенно возвращаться к нулю
        {
            if (speed > 0)
            {
                speed -= decelerationRate;
            }
            else
            {
                speed += accelerationRate;
            }
        }

        if (speed > maxSpeed)
        {
            speed = maxSpeed; //Проверка на превышение максимальной скорости
        }
        else if (speed < -maxSpeed)
        {
            speed = -maxSpeed; // Проверка на превышение минимальной скорости (если нужно)
        }

        // Check if current score is greater than high score
        if (scores > highScore)
        {
            // Update high score
            highScore = scores;

            // Save high score to file
            SaveHighScore();
        }
    }

    // Function to load high score from file
    private void LoadHighScore()
    {
        // Check if the file exists
        if (File.Exists(filePath))
        {
            try
            {
                // Read high score from file
                string high = File.ReadAllText(filePath);
                highScore = Convert.ToSingle(high);
            }
            catch (Exception e)
            {
                Debug.Log("Error loading high score: " + e.ToString());
            }
        }
    }

    // Function to save high score to file
    private void SaveHighScore()
    {
        try
        {
            // Write high score to file
            File.WriteAllText(filePath, highScore.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Error saving high score: " + e.ToString());
        }
    }
}
