using Godot;
using System;

public partial class Gate : Node2D, IInteractable {

    public bool locked = true;

    public AnimatedSprite2D sprite;

    public override void _Ready() {
        sprite = GetNode<AnimatedSprite2D>("%Sprite");
        sprite.Play("locked");
    }

    public string getPrompt() {
        if (locked) {

            GameState gs = GameState.GetGSInstance();
            if (gs.keys > 0) {

                return "Gate is locked. Press E to unlock.";
            }
            return "Gate is locked.";
        }
        return "Gate Is Unlocked";
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
        GetNode<StaticBody2D>("%Collision").Hide();
    }
    public bool canInteract() { return true; }
}
