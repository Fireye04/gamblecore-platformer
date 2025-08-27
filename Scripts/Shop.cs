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

            ShopEntry se =
                ResourceLoader.Load<PackedScene>("res://Scenes/shop_entry.tscn")
                    .Instantiate() as ShopEntry;
            se.init(item);
            entries.Add(se);

            if (item.Mutation == EMutator.BOON) {
                GetNode<VBoxContainer>("%Boons").AddChild(se);
            } else {
                GetNode<VBoxContainer>("%Banes").AddChild(se);
            }
        }
        updateShop();
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
        setTokenVal(gs.tokens);
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
        updateShop();
    }

    private void OnBeginPressed() {
        gs = GameState.GetGSInstance();
        int wager = getWager();
        if (wager < 0) {
            // TODO: Display error message
            return;
        }
        gs.tokens -= wager;
        gs.wager = wager + gs.wagerMod;
        gs.playRound();
    }
}
