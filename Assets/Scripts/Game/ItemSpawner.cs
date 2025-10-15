using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public Sprite[] itemSprites;
    public GameObject itemPrefab;
    private int numItems = 6;
    private ClickeableItem[] leftItems;
    private ClickeableItem[] rightItems;
    private Slot leftSlot;
    private Slot rightSlot;

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
        float margin = 0.25f;
        float slotHalfWidth = slot.Size.x / 2f;
        float slotHalfHeight = slot.Size.y / 2f;

        for (int i = 0; i < items.Length; i++)
        {
            SpriteRenderer sr = items[i].GetComponent<SpriteRenderer>();

            // Random rotation
            float rotation = (Random.Range(0, 100) > 60) ? 90f * Random.Range(1, 4) : 0f;
            items[i].transform.rotation = Quaternion.Euler(0f, 0f, rotation);
            items[i].SetIsAxisRotated(rotation == 90f || rotation == 270f);

            // Sprite size
            float halfWidth = sr.bounds.size.x / 2f;
            float halfHeight = sr.bounds.size.y / 2f;

            // Swap if rotated
            if (items[i].GetIsAxisRotated())
            {
                float tmp = halfWidth;
                halfWidth = halfHeight;
                halfHeight = tmp;
            }

            // Allowed placement area
            float maxX = slotHalfWidth - halfWidth - margin;
            float maxY = slotHalfHeight - halfHeight - margin;

            Vector2 pos = Vector2.zero;
            bool valid = false;
            int attempts = 0;

            while (!valid && attempts < 200)
            {
                pos = new Vector2(Random.Range(-maxX, maxX), Random.Range(-maxY, maxY));
                valid = true;

                // Check overlap with previous items
                for (int j = 0; j < i; j++)
                {
                    ClickeableItem other = items[j];
                    SpriteRenderer otherSR = other.GetComponent<SpriteRenderer>();
                    float otherHalfW = otherSR.bounds.size.x / 2f;
                    float otherHalfH = otherSR.bounds.size.y / 2f;
                    if (other.GetIsAxisRotated())
                    {
                        float tmp = otherHalfW;
                        otherHalfW = otherHalfH;
                        otherHalfH = tmp;
                    }

                    bool overlapX = Mathf.Abs(pos.x - other.transform.localPosition.x) < (halfWidth + otherHalfW + margin);
                    bool overlapY = Mathf.Abs(pos.y - other.transform.localPosition.y) < (halfHeight + otherHalfH + margin);

                    if (overlapX && overlapY)
                    {
                        valid = false;
                        break;
                    }
                }

                attempts++;
            }

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
