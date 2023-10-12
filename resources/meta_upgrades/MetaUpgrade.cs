using Godot;

namespace DSurvivors.resources.meta_upgrades;

public partial class MetaUpgrade : Resource
{
    [Export] private string _id;
    [Export] private int _maxQuantity = 1;
    [Export] private int _experienceCost = 10;
    [Export] private string _title;
    [Export(PropertyHint.MultilineText)] private string _description;

    public string Id => _id;
    public int MaxQuantity => _maxQuantity;
    public int ExperienceCost => _experienceCost;
    public string Title => _title;
    public string Description => _description;
}