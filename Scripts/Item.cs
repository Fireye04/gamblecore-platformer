using Godot;
using System;

[GlobalClass]
public partial class Item : Resource {

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

    [Export]
    public EMutator Mutation {
        get; set;
    }

    // Make sure you provide a parameterless constructor.
    // In C#, a parameterless constructor is different from a
    // constructor with all default values.
    // Without a parameterless constructor, Godot will have problems
    // creating and editing your resource via the inspector.
    public Item() : this(null, 0, null, EMutator.BOON) {}

    public Item(string name, int cost, string description, EMutator mutation) {
        Name = name;
        Cost = cost;
        Description = description;
        Mutation = mutation;
    }
}
