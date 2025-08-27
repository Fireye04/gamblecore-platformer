using Godot;
using System;

[GlobalClass]
public partial class Item : Resource {

    [Export]
    public EMutator Mutation { get; set; }

    [Export]
    public Godot.Collections.Array<Tier> Tiers {
        get; set;
    }

    // Make sure you provide a parameterless constructor.
    // In C#, a parameterless constructor is different from a
    // constructor with all default values.
    // Without a parameterless constructor, Godot will have problems
    // creating and editing your resource via the inspector.
    public Item() : this(EMutator.BOON, null) {}

    public Item(EMutator mutation, Godot.Collections.Array<Tier> tiers) {
        Mutation = mutation;
        Tiers = tiers;
    }
}
