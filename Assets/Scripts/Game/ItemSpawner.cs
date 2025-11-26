using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{

    public GameObject itemPrefab;
    private int numItems = 5;
    private Sprite[] itemSprites;
    private ClickeableItem[] leftItems;
    private ClickeableItem[] rightItems;
    private Slot leftSlot;
    private Slot rightSlot;

    public void Awake()
    {
        IconPack[] auxIconPacks = DataManager.Instance.allIconPacks;
        bool[] boughtIconPacks = DataManager.Instance.userData.boughtIconPacks;

        int totalAvailableIcons = 0;
        for (int i = 0; i < auxIconPacks.Length; i++)
        {
            if (boughtIconPacks[i])
            {
                totalAvailableIcons += auxIconPacks[i].iconSprites.Length;
            }
        }

        itemSprites = new Sprite[totalAvailableIcons];

        int index = 0;
        for (int i = 0; i < auxIconPacks.Length; i++)
        {
            if (boughtIconPacks[i])
            {
                for (int j = 0; j < auxIconPacks[i].iconSprites.Length; j++)
                {
                    itemSprites[index] = auxIconPacks[i].iconSprites[j];
                    index++;
                }
            }
        }
    }

    public void InitializeItems(Slot l, Slot r)
    {
        leftSlot = l;
        rightSlot = r;
        leftItems = new ClickeableItem[numItems];
        rightItems = new ClickeableItem[numItems];

        for (int i = 0; i < numItems; i++)
        {
            GameObject aux = Instantiate(itemPrefab);
            aux.GetComponent<SpriteRenderer>().sortingOrder = 1;

            leftItems[i] = aux.GetComponent<ClickeableItem>();
            leftItems[i].SetAssignedSlot(0);
        }

        for (int i = 0; i < numItems; i++)
        {
            GameObject aux = Instantiate(itemPrefab);
            aux.GetComponent<SpriteRenderer>().sortingOrder = 1;

            rightItems[i] = aux.GetComponent<ClickeableItem>();
            rightItems[i].SetAssignedSlot(1);
        }
    }

    public void SpawnItems()
    {
        AssignItemsDesign();
        AssignItemsPosition(leftSlot, leftItems);
        AssignItemsPosition(rightSlot, rightItems);
    }

    public void ChangeItemsPosition()
    {
        AssignItemsPosition(leftSlot, leftItems);
        AssignItemsPosition(rightSlot, rightItems);
    }

    void AssignItemsPosition(Slot slot, ClickeableItem[] items)
    {
        int PPU = 16;
        float pixelMargin = 2f;
        float innerPixelMargin = 2f;

        float worldMargin = pixelMargin / PPU;
        float innerWorldMargin = innerPixelMargin / PPU;

        float slotHalfW = slot.Size.x / 2f;
        float slotHalfH = slot.Size.y / 2f;

        float[] halfW = new float[items.Length];
        float[] halfH = new float[items.Length];
        Vector2[] placedPos = new Vector2[items.Length]; // ← KEY FIX

        // Pre-calc sizes
        for (int i = 0; i < items.Length; i++)
        {
            SpriteRenderer sr = items[i].GetComponent<SpriteRenderer>();

            float rot = (Random.Range(0, 100) > 60) ? 90f * Random.Range(1, 4) : 0f;
            items[i].transform.rotation = Quaternion.Euler(0, 0, rot);
            items[i].SetIsAxisRotated(rot == 90f || rot == 270f);

            float w = sr.sprite.rect.width / PPU;
            float h = sr.sprite.rect.height / PPU;

            if (items[i].GetIsAxisRotated())
                (w, h) = (h, w);

            halfW[i] = w / 2f;
            halfH[i] = h / 2f;
        }

        // Place items
        for (int i = 0; i < items.Length; i++)
        {
            float maxX = slotHalfW - halfW[i] - worldMargin - innerWorldMargin;
            float maxY = slotHalfH - halfH[i] - worldMargin - innerWorldMargin;

            Vector2 pos = Vector2.zero;
            bool validPosition = false;

            for (int attempts = 0; attempts < 200 && !validPosition; attempts++)
            {
                pos = new Vector2(Random.Range(-maxX, maxX),
                                  Random.Range(-maxY, maxY));

                validPosition = true;

                for (int j = 0; j < i; j++)
                {
                    float minDist = Mathf.Max(halfW[i], halfH[i]) +
                                    Mathf.Max(halfW[j], halfH[j]) +
                                    worldMargin;

                    if (Vector2.Distance(pos, placedPos[j]) < minDist)
                    {
                        validPosition = false;
                        break;
                    }
                }
            }

            // Save the final position BEFORE assigning
            placedPos[i] = pos;

            items[i].transform.SetParent(slot.transform, false);
            items[i].transform.localPosition = pos;
        }
    }

    public void AssignItemsDesign()
    {
        int uniqueSpritesNeeded = (numItems * 2) - 1;

        HashSet<int> chosenSpritesSet = new HashSet<int>();

        // Chooses numItems*2 - 1 unique sprite indexes
        while (chosenSpritesSet.Count < uniqueSpritesNeeded)
        {
            int aux = Random.Range(0, itemSprites.Length);
            chosenSpritesSet.Add(aux);
        }

        List<int> chosenIndex = new List<int>(chosenSpritesSet);

        // Assigns all the unique sprites and the duplicates in each slot
        for (int i = 0; i < numItems - 1; i++)
        {
            // 0, ..., numItems - 2
            leftItems[i].SetDesign(itemSprites[chosenIndex[i]], false);

            // numItems - 1, ..., 2*numItems - 3
            rightItems[i].SetDesign(itemSprites[chosenIndex[i + numItems - 1]], false);
        }

        leftItems[numItems - 1].SetDesign(itemSprites[chosenIndex[uniqueSpritesNeeded - 1]], true);
        rightItems[numItems - 1].SetDesign(itemSprites[chosenIndex[uniqueSpritesNeeded - 1]], true);
    }
}
