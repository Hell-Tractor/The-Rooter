using Godot;
using System;

public class StartSceneManager : Control {
    public void _OnStartButtonPressed() {
        // change current scene to stage1
        GetTree().ChangeScene("res://Scenes/Stages/Stage1.tscn");
    }

    public void _OnExitButtonPressed() {
        GetTree().Quit();
    }
}
