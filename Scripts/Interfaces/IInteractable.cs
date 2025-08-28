using Godot;
using System;

public interface IInteractable {
    string getName();
    void interact();
    bool canInteract();
}
