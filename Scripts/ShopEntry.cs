using Godot;
using System;

public partial class ShopEntry : Control {

    public Item res;

    public bool out_of_stock;

    public Tier curTier;

    public ShopEntry(Resource item) {
        res = (Item)item;
        clearItem();
    }

    private void OnBuyPressed() {

        GameState.GetGSInstance().buyItem(res, curTier);

        int index = res.Tiers.IndexOf(curTier);
        if (index >= res.Tiers.Count - 1) {
            out_of_stock = true;
        } else {
            curTier = res.Tiers[index];
        }
        updateItem();
    }

    public void clearItem() {
        out_of_stock = false;
        curTier = res.Tiers[0];
        updateItem();
    }

    public void updateItem() {
        GetNode<Label>("%Name").Text = curTier.Name;
        GetNode<Label>("%Description").Text = curTier.Description;
        if (res.Mutation == EMutator.BOON) {
            GetNode<Label>("%Cost").Text = "-" + curTier.Cost + " Tokens";
        } else {
            GetNode<Label>("%Cost").Text = "+" + curTier.Cost + " Wager";
        }

        GetNode<Button>("%Buy").Disabled = out_of_stock;
    }
}
