using Godot;
using System;

public class SettingsUIManager : Control {
    [Export(PropertyHint.File, "*.tscn")]
    public string SelectionScenePath;

    public void _OnRestartButtonPressed() {
        GetTree().ReloadCurrentScene();
    }
    public void _OnExitToSelectionButtonPressed() {
        GetTree().ChangeScene(SelectionScenePath);
    }
    public void _OnExitGameButtonPressed() {
        GetTree().Quit();
    }
}
