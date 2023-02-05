using Godot;
using System;

public class StartSceneManager : Control {
    public void _OnStartButtonPressed() {
        GD.Print("Start button pressed");
    }

    public void _OnExitButtonPressed() {
        GetTree().Quit();
    }
}
