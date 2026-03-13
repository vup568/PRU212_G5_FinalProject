using UnityEngine;

/// <summary>
/// Dữ liệu cho một loại building có thể đặt.
/// Gán trong Inspector của BuildingSystem.
/// </summary>
[System.Serializable]
public class BuildingData
{
    [Tooltip("Tên hiển thị")]
    public string displayName;

    [Tooltip("Prefab để Instantiate")]
    public GameObject prefab;

    [Tooltip("Giá Gold để đặt")]
    public int goldCost;

    [Tooltip("Level tối thiểu để mở khóa")]
    public int requiredLevel = 1;

    [Tooltip("Trạng thái TilemapState tương ứng")]
    public TilemapState tilemapState;

    [Tooltip("Icon hiển thị trong Building UI (tùy chọn)")]
    public Sprite icon;
}
