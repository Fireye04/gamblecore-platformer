using Godot;
using System;

public partial class Door : Node2D, IInteractable {

    [Export]
    public bool locked = false;

    public string getPrompt() { return "exit through unlocked door"; }

    public void interact() {
        GameState gs = GameState.GetGSInstance();

        if (locked) {
            if (gs.keys > 0) {
                gs.keys -= 1;
                locked = false;
            }
            // TODO: Give user feedback!
            return;
        }
        gs.winRound();
    }
    public bool canInteract() { return true; }
}
