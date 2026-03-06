using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("UI")]
    [Tooltip("UI Image GameObjects that represent collected badges. They should start disabled.")]
    [SerializeField] private Image[] itemImages = new Image[3];

    [Tooltip("Names used to identify items. Comparison is case-insensitive. Must correspond 1:1 to itemImages.")]
    [SerializeField] private string[] itemNames = new string[3];

    [Header("Completion")]
    [Tooltip("GameObjects to activate when all items are collected (e.g. enable story objects).")]
    [SerializeField] private GameObject[] completionActivators;

    [Tooltip("If true the world item GameObject passed to Collect will be SetActive(false).")]
    [SerializeField] private bool deactivateWorldItemOnCollect = true;

    private bool[] collected;
    private int collectedCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (itemImages == null)
            itemImages = new Image[0];

        if (itemNames == null)
            itemNames = new string[0];

        if (completionActivators == null)
            completionActivators = new GameObject[0];

        // Ensure itemNames and itemImages arrays match in length
        if (itemNames.Length != itemImages.Length)
        {
            var newNames = new string[itemImages.Length];
            for (int i = 0; i < newNames.Length; i++)
            {
                if (i < itemNames.Length && !string.IsNullOrWhiteSpace(itemNames[i]))
                    newNames[i] = itemNames[i];
                else if (itemImages[i] != null)
                    newNames[i] = itemImages[i].gameObject.name; // fallback to image GameObject name
                else
                    newNames[i] = string.Empty;
            }
            itemNames = newNames;
        }

        collected = new bool[itemImages.Length];
        collectedCount = 0;

        // Ensure UI images start disabled
        for (int i = 0; i < itemImages.Length; i++)
        {
            if (itemImages[i] != null)
                itemImages[i].gameObject.SetActive(false);
        }

        // Make sure completion activators start disabled (optional)
        for (int i = 0; i < completionActivators.Length; i++)
        {
            if (completionActivators[i] != null)
                completionActivators[i].SetActive(false);
        }
    }

    /// <summary>
    /// Mark the item with the given name as collected (case-insensitive). Enables corresponding UI image.
    /// If the name isn't found it logs a warning and falls back to destroying or deactivating the world item.
    /// </summary>
    public void CollectByName(string itemName, GameObject worldItem)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            return;

        // Match name case-insensitively
        int index = -1;
        for (int i = 0; i < itemNames.Length; i++)
        {
            if (string.Equals(itemNames[i], itemName, System.StringComparison.OrdinalIgnoreCase))
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Debug.LogWarning($"ItemManager: collected item name '{itemName}' not found in configured itemNames.");
            if (deactivateWorldItemOnCollect && worldItem != null)
                Destroy(worldItem);
            return;
        }

        if (collected[index])
            return; // already collected this item

        collected[index] = true;
        collectedCount++;

        if (itemImages[index] != null)
            itemImages[index].gameObject.SetActive(true);

        if (deactivateWorldItemOnCollect && worldItem != null)
            worldItem.SetActive(false);

        if (collectedCount == collected.Length)
        {
            // Activate all configured completion activators
            for (int i = 0; i < completionActivators.Length; i++)
            {
                if (completionActivators[i] != null)
                    completionActivators[i].SetActive(true);
            }
        }
    }

    /// <summary>
    /// Optional helper to reset collection state (editor/testing).
    /// </summary>
    public void ResetAll()
    {
        for (int i = 0; i < collected.Length; i++)
        {
            collected[i] = false;
            if (itemImages[i] != null)
                itemImages[i].gameObject.SetActive(false);
        }

        collectedCount = 0;

        for (int i = 0; i < completionActivators.Length; i++)
        {
            if (completionActivators[i] != null)
                completionActivators[i].SetActive(false);
        }
    }
}
