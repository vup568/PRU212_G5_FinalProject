using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    /// <summary>
    /// Event phát ra khi người chơi đọc hết tutorial (Level 1).
    /// UsernameWizard lắng nghe event này để hiện ô nhập tên.
    /// </summary>
    public static event Action OnTutorialCompleted;

    [Header("UI References")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Image playerPortraitImage;
    [SerializeField] private TextMeshProUGUI tutorialTextMesh;
    [SerializeField] private Button nextButton;

    [Header("Tutorial Data")]
    [SerializeField] private LevelTutorialData[] levelTutorials;

    private Queue<DialogueLine> currentDialogueQueue;
    private float typewriterSpeed = 0.04f;
    private Coroutine typewriterCoroutine;

    private void Awake()
    {
        currentDialogueQueue = new Queue<DialogueLine>();
        if (tutorialPanel != null) tutorialPanel.SetActive(false);

        // Đăng ký sự kiện lên level
        LevelManager.OnLevelUp += OnLevelChanged;

        if (nextButton != null)
            nextButton.onClick.AddListener(DisplayNextLine);
    }

    private void Start()
    {
        // CHỨC NĂNG MỚI: Kiểm tra level ngay khi vào game
        StartCoroutine(CheckLevelAtStart());
    }

    private IEnumerator CheckLevelAtStart()
    {
        // Đợi dữ liệu Firebase load xong (giống HouseManager)
        while (LoadDataManager.userInGame == null) yield return null;

        int currentLevel = LoadDataManager.userInGame.Level;
        Debug.Log($"[TutorialManager] Vào game ở Level {currentLevel}, đang kiểm tra hướng dẫn...");

        // Tìm và chạy hướng dẫn cho level hiện tại (Level 1)
        TryStartTutorial(currentLevel);
    }

    private void OnDestroy()
    {
        LevelManager.OnLevelUp -= OnLevelChanged;
        if (nextButton != null)
            nextButton.onClick.RemoveListener(DisplayNextLine);
    }

    private void OnLevelChanged(int newLevel)
    {
        // Khi lên level trong lúc đang chơi (Level 2)
        TryStartTutorial(newLevel);
    }

    private void TryStartTutorial(int level)
    {
        foreach (var tutorialData in levelTutorials)
        {
            if (tutorialData.targetLevel == level)
            {
                StartTutorial(tutorialData);
                break;
            }
        }
    }

    private void StartTutorial(LevelTutorialData tutorialData)
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);

        currentDialogueQueue.Clear();
        foreach (var line in tutorialData.dialogueLines)
        {
            currentDialogueQueue.Enqueue(line);
        }

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentDialogueQueue.Count == 0)
        {
            if (tutorialPanel != null) tutorialPanel.SetActive(false);

            // Thông báo tutorial đã hoàn thành
            Debug.Log("[TutorialManager] Tutorial hoàn thành! Phát event OnTutorialCompleted.");
            OnTutorialCompleted?.Invoke();
            return;
        }

        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);

        DialogueLine nextLine = currentDialogueQueue.Dequeue();

        if (playerPortraitImage != null)
            playerPortraitImage.sprite = nextLine.playerPortrait;

        if (tutorialTextMesh != null)
            typewriterCoroutine = StartCoroutine(TypewriterEffect(nextLine.dialogueText));
    }

    private IEnumerator TypewriterEffect(string fullText)
    {
        tutorialTextMesh.text = "";
        foreach (char letter in fullText.ToCharArray())
        {
            tutorialTextMesh.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }
        typewriterCoroutine = null;
    }
}