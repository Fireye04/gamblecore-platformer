using Godot;
using System;
using System.Runtime.Serialization;

public partial class GameState : Node {

    // Signal declaration
    [Signal]
    public delegate void TokenChangeEventHandler(int tokens);

    [Signal]
    public delegate void WagerChangeEventHandler(int wager);

    // Export Defaults
    [Export]
    public int StartTokens = 0;

    // Singleton Handler
    private static GameState instance;

    public static GameState GetGSInstance() { return instance; }

    private void SetGSInstance(GameState newGSInstance) {
        instance = newGSInstance;
    }
    public override void _EnterTree() { SetGSInstance(this); }

    public override void _Ready() { resetValues(); }

    // Reset
    public void resetValues() {
        tokens = StartTokens;
        wager = 0;
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

    private int Wager;

    public int wager {
        get { return Wager; }
        set {
            EmitSignal(SignalName.WagerChange, value);
            Wager = value;
        }
    }

    // Scene Handling
    public void changeScene(PackedScene scene) {
        GetTree().ChangeSceneToPacked(scene);
    }

    public void playAgain() {
        resetValues();
        changeScene(
            ResourceLoader.Load<PackedScene>("res://Scenes/platformer.tscn"));
    }
}
