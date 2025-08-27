using Godot;
using System;

public partial class GameUi : Control {

    public override void _EnterTree() {
        GameState.GetGSInstance().Connect(GameState.SignalName.TokenChange,
                                          Callable.From<int>(setTokenVal));

        GameState.GetGSInstance().Connect(GameState.SignalName.WagerChange,
                                          Callable.From<int>(setWagerVal));
    }

    public void setTokenVal(int val) {
        GetNode<Label>("%Total").Text = "Power Tokens: " + val;
    }

    public void setWagerVal(int val) {
        GetNode<Label>("%Wager").Text = "Current Wager: " + val;
    }
}
