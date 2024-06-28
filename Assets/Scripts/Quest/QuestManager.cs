using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public List<Quest> allQuests = new List<Quest>();
    public List<Quest> activeQuests = new List<Quest>();
    public Transform rewardSpawnPoint;
    public List<GameObject> rewardPrefabs;

    private const int totalQuests = 20;
    private const int maxActiveQuests = 3;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GenerateRandomQuests();
        ActivateRandomQuests();
    }

    void GenerateRandomQuests()
    {
        QuestType[] questTypes = (QuestType[])System.Enum.GetValues(typeof(QuestType));
        for (int i = 0; i < totalQuests; i++)
        {
            QuestType randomType = questTypes[Random.Range(0, questTypes.Length)];
            int targetValue = 0;

            switch (randomType)
            {
                case QuestType.Kills:
                    targetValue = Random.Range(10, 51);
                    break;
                case QuestType.DamageDone:
                    targetValue = Random.Range(100, 501);
                    break;
                case QuestType.DPS:
                    targetValue = Random.Range(10, 51);
                    break;
            }

            Quest newQuest = new Quest(randomType, targetValue);
            allQuests.Add(newQuest);
        }
    }

    void ActivateRandomQuests()
    {
        for (int i = 0; i < maxActiveQuests; i++)
        {
            int randomIndex = Random.Range(0, allQuests.Count);
            Quest quest = allQuests[randomIndex];
            allQuests.RemoveAt(randomIndex);
            activeQuests.Add(quest);
            QuestUIManager.instance.AddQuestToUI(quest);
        }
    }

    public void UpdateQuests(QuestType type, int currentValue)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.questType == type && !quest.isCompleted)
            {
                quest.CheckCompletion(currentValue);
                QuestUIManager.instance.UpdateQuestUI(quest);
            }
        }

        CheckAllQuestsCompleted();
    }

    void CheckAllQuestsCompleted()
    {
        foreach (Quest quest in activeQuests)
        {
            if (!quest.isCompleted)
            {
                return;
            }
        }

        QuestUIManager.instance.CloseQuestUI();
        ActivateRandomQuests(); // Activate new quests when all current quests are completed
    }

    public void SpawnReward()
    {
        if (rewardPrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, rewardPrefabs.Count);
            Instantiate(rewardPrefabs[randomIndex], rewardSpawnPoint.position, rewardSpawnPoint.rotation);
        }
    }
}
