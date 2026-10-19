
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questText;
    private Quest currentQuest;

    void Start()
    {
        questText.text = "진행 중인 퀘스트가 없습니다.";
    }

    private void UpdateQuestText()
    {
        if (currentQuest.state == QuestState.InProgress)
        {
            questText.text =
                currentQuest.questName + "\n" +
                currentQuest.targetItemName + " " +
                currentQuest.currentAmount + " / " +
                currentQuest.targetAmount;
        }
        else if (currentQuest.state == QuestState.ReadyToComplete)
        {
            questText.text = "수집 완료! \nNPC에게 돌아가세요";
        }
        else if (currentQuest.state == QuestState.Completed)
        {
            questText.text = currentQuest.questName + "\n퀘스트 완료";
        }
    }

    void StartQuest(Quest newQuest)
    {
        currentQuest = newQuest;
        currentQuest.currentAmount = 0;
        currentQuest.state = QuestState.InProgress;

        Debug.Log(currentQuest.questName + "퀘스트 시작!");
        UpdateQuestText();
    }
    public void AddQuestItem(string itemName, int amount)
    {
        if (currentQuest == null) return;
        if (currentQuest.state != QuestState.InProgress) return;
        if (currentQuest.targetItemName != itemName) return;
        if (amount <= 0) return; // 0 이하의 수량 추가 방지 예외 처리

        currentQuest.currentAmount += amount;

        // 목표 수량을 초과하더라도 목표치까지만 채우고 완료 대기 상태로 변경
        if (currentQuest.currentAmount >= currentQuest.targetAmount)
        {
            currentQuest.currentAmount = currentQuest.targetAmount;
            currentQuest.state = QuestState.ReadyToComplete;
        }

        UpdateQuestText();
    }
    public void CompleteQuest()
    {
        // 퀘스트가 없거나 아직 완료 대기 상태가 아니라면 실행 중지
        if (currentQuest == null) return;
        if (currentQuest.state != QuestState.ReadyToComplete) return;

        currentQuest.state = QuestState.Completed;
        Debug.Log(currentQuest.questName + " 퀘스트 완료!");
        UpdateQuestText();
    }

    public void TalkToNPC(Quest npcQuest)
    {
        // 1. NPC 퀘스트가 없는 경우
        if (npcQuest == null)
        {
            Debug.Log("퀘스트 정보가 없습니다.");
            return;
        }

        // 2. 이미 완료한 퀘스트인지 확인
        if (npcQuest.state == QuestState.Completed)
        {
            Debug.Log("이미 완료한 퀘스트입니다.");
            return;
        }

        // 3. 진행 중인 퀘스트가 없거나 이전 퀘스트가 완료된 경우
        if (currentQuest == null ||
            currentQuest.state == QuestState.Completed)
        {
            StartQuest(npcQuest);
        }

        // 4. 다른 퀘스트를 진행 중인지 확인
        if (currentQuest != npcQuest)
        {
            Debug.Log("이미 다른 퀘스트를 진행 중입니다.");
            return;
        }

        // 5. 퀘스트 상태에 따라 처리
        if (currentQuest.state == QuestState.InProgress)
        {
            Debug.Log("아직 아이템이 부족합니다.");
        }
        else if (currentQuest.state == QuestState.ReadyToComplete)
        {
            CompleteQuest();
        }
    }
}
