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
        // Sprite pixel_per_unit
        int PPU = 16;
        // Distance between items in pixels
        float pixelMargin = 5; // FIX THIS
        // Inner slot margin in pixels
        float innerPixelMargin = 2;

        float worldMargin = pixelMargin / PPU;
        float innerWorldMargin = innerPixelMargin / PPU;

        float slotHalfW = slot.Size.x / 2f;
        float slotHalfH = slot.Size.y / 2f;

        for (int i = 0; i < items.Length; i++)
        {
            SpriteRenderer sr = items[i].GetComponent<SpriteRenderer>();

            // Random rotation
            float rot = (Random.Range(0, 100) > 60) ? 90f * Random.Range(1, 4) : 0f;
            items[i].transform.rotation = Quaternion.Euler(0, 0, rot);
            items[i].SetIsAxisRotated(rot == 90f || rot == 270f);

            // Sprite world size
            float halfW = sr.bounds.size.x / 2f;
            float halfH = sr.bounds.size.y / 2f;

            // Swap if rotated
            if (items[i].GetIsAxisRotated())
            {
                float t = halfW;
                halfW = halfH;
                halfH = t;
            }

            // Allowed placement region
            float maxX = slotHalfW - halfW - worldMargin - innerWorldMargin;
            float maxY = slotHalfH - halfH - worldMargin - innerWorldMargin;

            Vector2 pos = Vector2.zero;
            bool valid = false;
            int attempts = 0;

            while (!valid && attempts < 200)
            {
                pos = new Vector2(Random.Range(-maxX, maxX),
                                  Random.Range(-maxY, maxY));

                valid = true;

                // Check overlap with previous items
                for (int j = 0; j < i; j++)
                {
                    SpriteRenderer otherSR = items[j].GetComponent<SpriteRenderer>();

                    float otherHalfW = otherSR.bounds.size.x / 2f;
                    float otherHalfH = otherSR.bounds.size.y / 2f;

                    if (items[j].GetIsAxisRotated())
                    {
                        float t = otherHalfW;
                        otherHalfW = otherHalfH;
                        otherHalfH = t;
                    }

                    // Separation test with margin
                    if (Mathf.Abs(pos.x - items[j].transform.localPosition.x) < (halfW + otherHalfW + worldMargin) &&
                        Mathf.Abs(pos.y - items[j].transform.localPosition.y) < (halfH + otherHalfH + worldMargin))
                    {
                        valid = false;
                        break;
                    }
                }

                attempts++;
            }

            // Assign final position
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
