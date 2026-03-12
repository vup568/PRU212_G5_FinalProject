using UnityEngine;

[System.Serializable] // Để có thể hiển thị trong Inspector
public struct DialogueLine
{
    [Tooltip("Ảnh đại diện nhân vật (kéo Sprite vào)")]
    public Sprite playerPortrait;

    [Tooltip("Nội dung hướng dẫn")]
    [TextArea(3, 10)] // Cho phép nhập nhiều dòng văn bản
    public string dialogueText;
}

[System.Serializable]
public struct LevelTutorialData
{
    [Tooltip("Cấp độ sẽ kích hoạt hướng dẫn này")]
    public int targetLevel;

    [Tooltip("Danh sách các câu hướng dẫn cho cấp độ này")]
    public DialogueLine[] dialogueLines;
}