using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestUIManager : MonoBehaviour
{
    public static QuestUIManager instance;

    public GameObject questPanelPrefab;
    public Transform questPanelParent;
    private Dictionary<Quest, GameObject> questUIDictionary = new Dictionary<Quest, GameObject>();

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
    }

    void Start()
    {
        InitializeQuestUI();
    }

    void InitializeQuestUI()
    {
        foreach (Quest quest in QuestManager.instance.activeQuests)
        {
            AddQuestToUI(quest);
        }
    }

    public void AddQuestToUI(Quest quest)
    {
        GameObject questPanel = Instantiate(questPanelPrefab, questPanelParent);
        UpdateQuestUI(quest, questPanel);
        questUIDictionary.Add(quest, questPanel);
    }

    public void UpdateQuestUI(Quest quest)
    {
        if (questUIDictionary.ContainsKey(quest))
        {
            GameObject questPanel = questUIDictionary[quest];
            UpdateQuestUI(quest, questPanel);
        }
    }

    void UpdateQuestUI(Quest quest, GameObject questPanel)
    {
        TMP_Text[] texts = questPanel.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text text in texts)
        {
            if (text.name == "QuestDescription")
            {
                text.text = GetQuestDescription(quest);
            }
            else if (text.name == "QuestProgress")
            {
                if (quest.isCompleted)
                {
                    text.text = "Quest Completed!";
                }
                else
                {
                    text.text = GetQuestProgress(quest);
                }
            }
        }
    }

    string GetQuestDescription(Quest quest)
    {
        switch (quest.questType)
        {
            case QuestType.Kills:
                return $"Kill {quest.targetValue} enemies";
            case QuestType.DamageDone:
                return $"Deal {quest.targetValue} damage";
            case QuestType.DPS:
                return $"Reach {quest.targetValue} DPS";
            default:
                return "Unknown quest";
        }
    }

    string GetQuestProgress(Quest quest)
    {
        int currentValue = 0;
        switch (quest.questType)
        {
            case QuestType.Kills:
                currentValue = GameManager.instance.killCount;
                break;
            case QuestType.DamageDone:
                currentValue = (int)GameManager.instance.totalDamageDone;
                break;
            case QuestType.DPS:
                currentValue = (int)GameManager.instance.dps;
                break;
        }
        return $"{currentValue}/{quest.targetValue}";
    }

    public void CloseQuestUI()
    {
        StartCoroutine(CloseQuestUICoroutine());
    }

    IEnumerator CloseQuestUICoroutine()
    {
        // Change all quest texts to "Quest Completed!"
        foreach (var questPanel in questUIDictionary.Values)
        {
            TMP_Text[] texts = questPanel.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text text in texts)
            {
                if (text.name == "QuestProgress")
                {
                    text.text = "Quest Completed!";
                }
            }
        }

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Deactivate the quest UI
        questPanelParent.gameObject.SetActive(false);
    }
}
