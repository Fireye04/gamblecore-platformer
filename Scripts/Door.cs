using Godot;
using System;

public partial class Door : Node2D, IInteractable {

    [Export]
    public bool locked = false;

    public AnimatedSprite2D sprite;

    public override void _Ready() {
        sprite = GetNode<AnimatedSprite2D>("%Sprite");
        if (locked) {
            sprite.Play("locked");
        } else {
            sprite.Play("open");
        }
    }

    public string getPrompt() {
        if (locked) {

            GameState gs = GameState.GetGSInstance();
            if (gs.keys > 0) {

                return "Door is locked. Press E to unlock.";
            }
            return "Door is locked.";
        }
        return "Press E to exit through unlocked door";
    }

    public void interact() {
        GameState gs = GameState.GetGSInstance();

        if (locked) {
            if (gs.keys > 0) {
                gs.keys -= 1;
                locked = false;
                sprite.Play("unlock");
            }
            // TODO: Give user feedback!
            return;
        }
        gs.winRound();
    }
    public bool canInteract() { return true; }
}
