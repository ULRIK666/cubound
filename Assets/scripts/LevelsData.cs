using UnityEngine;

[System.Serializable]
public class Level
{
    public ObstacleData[] obstacles;
    public Vector3[] boxPositions;
}

[System.Serializable]
public class ObstacleData
{
    public Vector3 position;
    public Vector3 scale = Vector3.one;
    public Quaternion rotation = Quaternion.identity;
}

[CreateAssetMenu(fileName = "LevelsData", menuName = "ScriptableObjects/LevelsData", order = 1)]
public class LevelsData : ScriptableObject
{
    public Level[] levels;
}
