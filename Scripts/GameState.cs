using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

public partial class GameState : Node {

    // Signal declaration
    [Signal]
    public delegate void TokenChangeEventHandler(int tokens);

    [Signal]
    public delegate void WagerChangeEventHandler(int wager);

    [Signal]
    public delegate void KeyChangeEventHandler(int keys);

    [Signal]
    public delegate void ShopUpdateEventHandler(int wagerMod);

    [Signal]
    public delegate void InteractionUpdateEventHandler(Node item);

    // Export Defaults
    [Export]
    public int StartTokens = 0;

    [Export]
    public int DefaultTime = 90;

    // Singleton Handler
    private static GameState instance;

    public static GameState GetGSInstance() { return instance; }

    private void SetGSInstance(GameState newGSInstance) {
        instance = newGSInstance;
    }
    public override void _EnterTree() {
        SetGSInstance(this);
        boons = new HashSet<String>();
        banes = new HashSet<String>();
    }

    // Reset
    public void resetValues(bool full) {
        // only full reset tokens if it's a new game
        if (full) {
            tokens = StartTokens;
        }
        tempTokens = tokens;
        wager = 0;
        wagerMod = 0;
        boons = new HashSet<String>();
        banes = new HashSet<String>();
        timeLimit = DefaultTime;
        timeLeft = timeLimit;
        keys = 0;
    }

    // Values

    private int Tokens;

    public int tokens {
        get { return Tokens; }
        set {
            EmitSignal(SignalName.TokenChange, value);
            Tokens = value;
        }
    }

    public int tempTokens;

    private int Wager;

    public int wager {
        get { return Wager; }
        set {
            EmitSignal(SignalName.WagerChange, value);
            Wager = value;
        }
    }

    public int wagerMod;

    public HashSet<String> boons;

    public HashSet<String> banes;

    // Seconds
    public double timeLimit;

    public double timeLeft;

    private int Keys;

    public int keys {
        get { return Keys; }
        set {
            EmitSignal(SignalName.KeyChange, value);
            Keys = value;
        }
    }

    // Functions

    public void buyItem(Item res, Tier tier) {
        // Exit early if boon insufficient funds or already purchased.
        if (boons.Contains(tier.Name) || banes.Contains(tier.Name) ||
            (res.Mutation == EMutator.BOON && tier.Cost > tokens)) {
            return;
        }
        if (res.Mutation == EMutator.BOON) {
            tokens -= tier.Cost;
            boons.Add(tier.Name);
        } else {
            wagerMod += tier.Cost;
            banes.Add(tier.Name);
        }
        EmitSignal(SignalName.ShopUpdate);
        // GD.Print(string.Join("", boons));
        // GD.Print(string.Join("", banes));
    }

    public void clearItem(Item item) {
        if (item.Mutation == EMutator.BOON) {
            foreach (Tier tier in item.Tiers) {
                boons.Remove(tier.Name);
            }
        } else {
            foreach (Tier tier in item.Tiers) {
                banes.Remove(tier.Name);
            }
        }
    }

    // Game Flow
    public void winRound() {
        tokens += wager * 2;
        resetValues(false);
        CallDeferred(MethodName.play);
    }

    public void loseRound() {
        resetValues(false);
        CallDeferred(MethodName.play);
    }

    // Scene Handling
    public void changeScene(PackedScene scene) {
        GetTree().ChangeSceneToPacked(scene);
    }

    public void play() {
        changeScene(ResourceLoader.Load<PackedScene>("res://Scenes/shop.tscn"));
    }

    public void playRound() {
        changeScene(
            ResourceLoader.Load<PackedScene>("res://Scenes/platformer.tscn"));
    }
}
