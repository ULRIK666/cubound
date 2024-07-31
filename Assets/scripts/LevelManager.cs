using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public GameObject boxPrefab;
    public GameObject player;
    public LevelsData levelsData;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI resetText;

    private int currentLevel = 0;
    private List<GameObject> obstacles = new List<GameObject>();
    private List<GameObject> boxes = new List<GameObject>();

    void Start()
    {
        GenerateLevel(currentLevel);
        UpdateLevelText();
        UpdateResetText();
    }

    void Update()
    {
        CheckPlayerPosition();

        // Check for spacebar press to reset level
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateLevel(currentLevel);
        }
    }

    void CheckPlayerPosition()
    {
        if (player.transform.position.x == 5 && player.transform.position.y == 6 && player.transform.position.z == -75)
        {
            NextLevel();
        }
    }

    public void GenerateLevel(int levelIndex)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        obstacles.Clear();
        boxes.Clear();

        foreach (ObstacleData obstacleData in levelsData.levels[levelIndex].obstacles)
        {
            GameObject obstacle = Instantiate(obstaclePrefab, obstacleData.position, obstacleData.rotation, transform);
            obstacle.transform.localScale = obstacleData.scale;
            obstacles.Add(obstacle);
        }

        foreach (Vector3 position in levelsData.levels[levelIndex].boxPositions)
        {
            GameObject box = Instantiate(boxPrefab, position, Quaternion.identity, transform);
            boxes.Add(box);
        }

        player.transform.position = new Vector3(5f, 6f, 5f); // Starting position
    }

    public void NextLevel()
    {
        currentLevel = (currentLevel + 1) % levelsData.levels.Length;
        GenerateLevel(currentLevel);
        UpdateLevelText();
    }

    private void UpdateLevelText()
    {
        levelText.text = "Level: " + (currentLevel + 1);
    }

    private void UpdateResetText()
    {
        resetText.text = "Press Spacebar to reset level";
    }

    public bool CanMove(Vector3 newPosition)
    {
        if (newPosition.x == 5 && newPosition.z == -75)
        {
            return true;
        }

        if (newPosition.x < -70 || newPosition.x > 75 || newPosition.z < -65 || newPosition.z > 85)
        {
            return false;
        }

        foreach (GameObject obstacle in obstacles)
        {
            Vector3 obstacleSize = obstacle.GetComponent<Renderer>().bounds.size;
            Vector3 obstaclePosition = obstacle.transform.position;

            if (newPosition.x >= obstaclePosition.x - obstacleSize.x / 2 && newPosition.x <= obstaclePosition.x + obstacleSize.x / 2 &&
                newPosition.z >= obstaclePosition.z - obstacleSize.z / 2 && newPosition.z <= obstaclePosition.z + obstacleSize.z / 2)
            {
                return false;
            }
        }

        foreach (GameObject box in boxes)
        {
            if (Vector3.Distance(newPosition, box.transform.position) < 0.1f)
            {
                return false;
            }
        }

        return true;
    }

    public GameObject GetBoxAtPosition(Vector3 position)
    {
        foreach (GameObject box in boxes)
        {
            if (Vector3.Distance(position, box.transform.position) < 0.1f)
            {
                return box;
            }
        }
        return null;
    }

    public void MoveBox(GameObject box, Vector3 newPosition)
    {
        box.transform.position = newPosition;
    }
}
