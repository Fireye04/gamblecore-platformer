using Godot;
using System;

public partial class GameUi : Control {

    // Signal handling
    public override void _EnterTree() {
        GameState.GetGSInstance().Connect(GameState.SignalName.TokenChange,
                                          Callable.From<int>(setTokenVal));

        GameState.GetGSInstance().Connect(GameState.SignalName.WagerChange,
                                          Callable.From<int>(setWagerVal));

        GameState.GetGSInstance().Connect(GameState.SignalName.KeyChange,
                                          Callable.From<int>(setKeyVal));

        GameState.GetGSInstance().Connect(GameState.SignalName.LivesChange,
                                          Callable.From<int>(setLivesVal));

        GameState.GetGSInstance().Connect(
            GameState.SignalName.InteractionUpdate,
            Callable.From<Node>(setInteraction));
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

    public void setLivesVal(int val) {
        GetNode<Label>("%Lives").Text = "Lives: " + val;
    }

    // Interaction Prompt
    public void setInteraction(Node newItem) {
        // DO NOT TRY TO PASS A NON-IINTERACTABLE NODE WITH ENABLED=TRUE
        Label prompt = GetNode<Label>("%InteractionPrompt");
        if (newItem is not IInteractable) {
            prompt.Hide();
            return;
        }
        IInteractable item = (IInteractable)newItem;
        prompt.Text = item.getPrompt();
        prompt.Show();
    }

    // Variabes

    public Label clock;
    private double TimeLeft;

    public double timeLeft {
        get { return TimeLeft; }
        set {
            // Update the global timer every integer
            if (Double.IsInteger(value)) {
                GameState.GetGSInstance().timeLeft = value;
            }
            TimeLeft = value;
        }
    }

    public override void _Ready() {
        GameState gs = GameState.GetGSInstance();
        setTokenVal(gs.tokens);
        setWagerVal(gs.wager);
        clock = GetNode<Label>("%Clock");
        if (gs.banes.Contains("Time Limit 2")) {
            timeLeft = 30;

        } else if (gs.banes.Contains("Time Limit 1")) {
            timeLeft = 60;
        } else {
            timeLeft = gs.timeLimit;
        }
    }

    private void OnTimerTimeout() {
        // I am a kind being
        if (timeLeft < 0) {
            GameState.GetGSInstance().loseRound();
        }
        timeLeft -= 0.05;
        clock.Text = timeLeft.ToString("0.00");
    }
}
