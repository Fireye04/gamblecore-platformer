using Godot;
using System;
using System.Collections.Generic;

public partial class InteractionBox : Area2D {

    public Godot.Collections.Array<Node2D> interactablesInRange =
        new Godot.Collections.Array<Node2D>();

    public override void _Ready() { interactablesInRange.Clear(); }

    private void OnAreaShapeEntered(Godot.Rid rid, Node2D body, int shape_index,
                                    int local_shape_index) {
        if (body is IInteractable) {
            interactablesInRange.Add(body);
            GameState.GetGSInstance().EmitSignal(
                GameState.SignalName.InteractionUpdate,
                find_nearest_interactable());
        }
    }

    private void OnAreaShapeExited(Godot.Rid rid, Node2D body, int shape_index,
                                   int local_shape_index) {
        if (body is IInteractable) {
            if (interactablesInRange.Contains(body)) {
                interactablesInRange.Remove(body);
                if (interactablesInRange.Count == 0) {
                    GameState.GetGSInstance().EmitSignal(
                        GameState.SignalName.InteractionUpdate, new Node());
                }
            }
        }
    }

    public Node find_nearest_interactable() {
        GD.Print(interactablesInRange.Count);
        if (interactablesInRange.Count == 1) {
            return interactablesInRange[0];

        } else if (interactablesInRange.Count > 1) {

            (Node2D, float)closest = default;

            for (int i = 0; i < interactablesInRange.Count; i++) {
                Godot.Vector2 itemPos = interactablesInRange[i].GlobalPosition;

                float distance = GlobalPosition.DistanceTo(itemPos);

                if (closest.Item1 is null) {
                    closest = (interactablesInRange[i], distance);
                } else {
                    if (distance <= closest.Item2) {
                        closest = (interactablesInRange[i], distance);
                    }
                }
            }

            return closest.Item1;

        } else {
            return null;
        }
    }
}
