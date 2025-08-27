using Godot;
using System;

[GlobalClass]
public partial class Tier : Resource {

    [Export]
    public string Name { get; set; }

    [Export]
    public int Cost {
        get; set;
    }

    [Export]
    public string Description {
        get; set;
    }

    public Tier() : this(null, 0, null) {}

    public Tier(string name, int cost, string description) {
        Name = name;
        Cost = cost;
        Description = description;
    }
}
