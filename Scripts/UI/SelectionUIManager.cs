using System.Diagnostics;
using Godot;
using System;

public class SelectionUIManager : Control {
    public void _OnStageSelectionButtonPressed(int stage) {
        GD.Print("Stage " + stage + " selected");
        GetTree().ChangeScene("Scenes/Stages/Stage" + stage + ".tscn");
    }
}
