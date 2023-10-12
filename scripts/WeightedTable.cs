using System.Linq;
using Godot;
using Godot.Collections;

namespace DSurvivors.scripts;

public class WeightedTable
{
    private Array<Dictionary> _items = new Array<Dictionary>();
    private int _weightSum = 0;

    public Array<Dictionary> Items => _items;

    public void AddItem(Resource item, int weight)
    {
        _items.Add(new Dictionary{{"item", item}, {"weight", weight}});
        _weightSum += weight;
    }

    public void RemoveItem(Resource itemToRemove)
    {
        _items = new Array<Dictionary>(_items.Where(item => (Resource)item["item"] != itemToRemove));
        _weightSum = 0;
        foreach (var item in _items)
        {
            _weightSum += (int)item["weight"];
        }
    }

    public Resource PickItem(Array<Resource> exclude = null)
    {
        var adjustedItems = new Array<Dictionary>(_items);
        var adjustedItemsSum = _weightSum;
        if (exclude != null && exclude.Count > 0)
        {
            adjustedItems.Clear();
            adjustedItemsSum = 0;
            foreach (var item in _items)
            {
                foreach (var ex in exclude)
                {
                    if ((Resource)item["item"] == ex)
                    {
                        continue;
                    }
                    adjustedItems.Add(item);
                    adjustedItemsSum += (int)item["weight"];
                }
            }
        }
        
        var chosenWeight = GD.RandRange(1, adjustedItemsSum);
        var iterationSum = 0;
        foreach (var item in adjustedItems)
        {
            iterationSum += (int)item["weight"];
            if (chosenWeight <= iterationSum)
            {
                return (Resource)item["item"];
            }
        }

        return null;
    }
}