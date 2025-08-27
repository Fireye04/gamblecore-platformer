using Godot;
using System;

public partial class ShopEntry : Control {

    public Item res;

    public bool out_of_stock;

    public Tier curTier;

    public void init(Resource item) {
        res = (Item)item;
        clearItem();
    }

    private void OnBuyPressed() {

        GameState.GetGSInstance().buyItem(res, curTier);

        int index = res.Tiers.IndexOf(curTier);

        if (curTier.Cost > GameState.GetGSInstance().tokens) {

        } else if (index >= res.Tiers.Count - 1) {
            out_of_stock = true;
        } else {
            curTier = res.Tiers[index + 1];
        }
        updateItem();
    }

    public void clearItem() {
        out_of_stock = false;
        curTier = res.Tiers[0];
        GameState gs = GameState.GetGSInstance();
        gs.clearItem(res);
        updateItem();
    }

    public void updateItem() {
        GetNode<Label>("%Name").Text = curTier.Name;
        GetNode<RichTextLabel>("%Description").Text = curTier.Description;
        if (res.Mutation == EMutator.BOON) {
            GetNode<Label>("%Cost").Text = "-" + curTier.Cost + " Tokens";
        } else {
            GetNode<Label>("%Cost").Text = "+" + curTier.Cost + " Wager";
        }

        // Disables buy button if out of stock or insufficient funds
        GetNode<Button>("%Buy").Disabled =
            out_of_stock || curTier.Cost > GameState.GetGSInstance().tokens;
    }
}
