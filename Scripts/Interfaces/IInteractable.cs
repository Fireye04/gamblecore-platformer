using Godot;
using System;

public interface IInteractable {
    // Tap E to + PROMPT GOES HERE
    string getPrompt();
    void interact();
    bool canInteract();
}
