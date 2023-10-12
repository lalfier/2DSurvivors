using Godot;

namespace DSurvivors.resources.upgrades;

public partial class AbilityUpgrade : Resource
{
    [Export] private string _id;
    [Export] private int _maxQuantity;
    [Export] private string _name;
    [Export(PropertyHint.MultilineText)] private string _description;

    public string Id => _id;
    public int MaxQuantity => _maxQuantity;
    public string Name => _name;
    public string Description => _description;
}