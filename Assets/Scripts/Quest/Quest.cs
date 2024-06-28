using UnityEngine;

public enum QuestType { Kills, DamageDone, DPS }

[System.Serializable]
public class Quest
{
    public QuestType questType;
    public int targetValue;
    public bool isCompleted;

    public Quest(QuestType type, int value)
    {
        questType = type;
        targetValue = value;
        isCompleted = false;
    }

    public void CheckCompletion(int currentValue)
    {
        if (currentValue >= targetValue)
        {
            isCompleted = true;
            OnQuestCompleted();
        }
    }

    public void OnQuestCompleted()
    {
        Debug.Log($"Quest of type {questType} completed!");
        QuestManager.instance.SpawnReward();
    }
}
