using Godot;
using System;

public partial class Shop : Control {

    [Export]
    public Godot.Collections.Array<Item> Items;

    public Godot.Collections.Array<ShopEntry> entries;

    GameState gs;

    public override void _EnterTree() {
        GameState.GetGSInstance().Connect(GameState.SignalName.TokenChange,
                                          Callable.From<int>(setTokenVal));

        GameState.GetGSInstance().Connect(GameState.SignalName.ShopUpdate,
                                          Callable.From(updateShop));
    }

    public override void _Ready() {

        entries = new Godot.Collections.Array<ShopEntry>();
        gs = GameState.GetGSInstance();
        gs.resetValues(false);

        foreach (Item item in Items) {
            ShopEntry se = new ShopEntry(item);
            entries.Add(se);
            if (item.Mutation == EMutator.BOON) {
                GetNode<TextEdit>("%Boons").AddChild(se);
            } else {
                GetNode<TextEdit>("%Banes").AddChild(se);
            }
        }
    }

    public int getWager() {

        gs = GameState.GetGSInstance();
        int wager;
        bool success =
            int.TryParse(GetNode<TextEdit>("%WagerIn").Text, out wager);

        // non integer provided
        if (!success) {
            return -1;
        }
        // Ingteger out of bounds
        if (wager < 0) {
            return -2;
        }
        if (wager > gs.tokens) {
            return -3;
        }
        return wager;
    }

    public void setTokenVal(int val) {
        GetNode<Label>("%Tokens").Text = "Power Tokens: " + val;
    }

    public void updateShop() {
        gs = GameState.GetGSInstance();
        GetNode<Label>("%WagerMod").Text =
            "+" + gs.wagerMod + " Wager Modifier";
        foreach (ShopEntry entry in entries) {
            entry.updateItem();
        }
    }

    private void OnClearPressed() {
        gs = GameState.GetGSInstance();
        gs.tokens = gs.tempTokens;
        gs.wagerMod = 0;
        GetNode<Label>("%WagerMod").Text =
            "+" + gs.wagerMod + " Wager Modifier";
        foreach (ShopEntry entry in entries) {
            entry.clearItem();
        }
    }

    private void OnBeginPressed() {
        gs = GameState.GetGSInstance();
        int wager = getWager();
        if (wager < 0) {
            // TODO: Display error message
            return;
        }
        gs.wager = wager + gs.wagerMod;
        gs.tokens -= gs.wager;
        gs.play();
    }
}
