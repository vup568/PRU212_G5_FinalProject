/// <summary>
/// Enum các loại quest có thể random.
/// </summary>
public enum QuestType
{
    HarvestCorn,    // Thu hoạch Corn
    HarvestFlower,  // Thu hoạch Flower (Level 2+)
    PlantAny,       // Trồng cây bất kỳ (Corn hoặc Flower)
    DigTiles,       // Đào đất
    SellGold,       // Bán hàng kiếm Gold
    PlantFlower     // Trồng Flower (Level 2+)
}

/// <summary>
/// Chứa thông tin 1 quest: loại, mô tả, mục tiêu, tiến độ, thưởng.
/// </summary>
[System.Serializable]
public class QuestData
{
    public QuestType questType;
    public string description;
    public int targetAmount;
    public int currentAmount;
    public int goldReward;

    /// <summary>
    /// Quest đã hoàn thành chưa?
    /// </summary>
    public bool IsCompleted => currentAmount >= targetAmount;

    public QuestData(QuestType type, string description, int targetAmount, int goldReward)
    {
        this.questType = type;
        this.description = description;
        this.targetAmount = targetAmount;
        this.currentAmount = 0;
        this.goldReward = goldReward;
    }

    /// <summary>
    /// Cộng tiến độ. Trả về true nếu vừa hoàn thành (chuyển từ chưa xong → xong).
    /// </summary>
    public bool AddProgress(int amount)
    {
        if (IsCompleted) return false; // Đã xong rồi thì bỏ qua

        currentAmount += amount;
        if (currentAmount > targetAmount)
            currentAmount = targetAmount;

        return IsCompleted;
    }

    /// <summary>
    /// Chuỗi tiến độ dạng "2/3".
    /// </summary>
    public string GetProgressText()
    {
        return $"{currentAmount}/{targetAmount}";
    }
}
