using Godot;
using System;

public partial class GameUi : Control {

    public override void _EnterTree() {
        GameState.GetGSInstance().Connect(GameState.SignalName.TokenChange,
                                          Callable.From<int>(setTokenVal));

        GameState.GetGSInstance().Connect(GameState.SignalName.WagerChange,
                                          Callable.From<int>(setWagerVal));

        GameState.GetGSInstance().Connect(GameState.SignalName.KeyChange,
                                          Callable.From<int>(setKeyVal));

        GameState.GetGSInstance().Connect(
            GameState.SignalName.InteractionUpdate,
            Callable.From<bool, Node>(setInteraction));
    }

    public override void _Ready() {
        GameState gs = GameState.GetGSInstance();
        setTokenVal(gs.tokens);
        setWagerVal(gs.wager);
    }

    public void setTokenVal(int val) {
        GetNode<Label>("%Total").Text = "Power Tokens: " + val;
    }

    public void setWagerVal(int val) {
        GetNode<Label>("%Wager").Text = "Current Wager: " + val;
    }

    public void setKeyVal(int val) {
        GetNode<Label>("%Keys").Text = "Keys: " + val;
    }

    public void setInteraction(bool enabled, Node newItem) {
        Label prompt = GetNode<Label>("%InteractionPrompt");
        if (!enabled) {
            prompt.Hide();
            return;
        }
        IInteractable item = (IInteractable)newItem;
        prompt.Text = "Tap E to interact with : " + item.getName();
        prompt.Show();
    }
}
