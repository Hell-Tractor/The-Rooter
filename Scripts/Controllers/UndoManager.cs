using System;
using System.Collections.Generic;
using Godot;

public class UndoManager : Node {
    [Export]
    public string GroupName = "Undoable";
    private List<List<Dictionary<string, object>>> _undoStack = new List<List<Dictionary<string, object>>>();
    public static UndoManager Instance { get; private set; } = null;

    public override void _EnterTree() {
        if (Instance != null)
            throw new Exception("Duplicate UndoManagerExists.");
        Instance = this;
    }

    public void Save() {
        List<Dictionary<string, object>> saveData = new List<Dictionary<string, object>>();
        foreach (Node saveNode in this.GetTree().GetNodesInGroup(this.GroupName)) {
            // Check the node is an instanced scene so it can be instanced again during load.
            if (saveNode.Filename.Empty()) {
                GD.Print(String.Format("persistent node '{0}' is not an instanced scene, skipped", saveNode.Name));
                continue;
            }

            // Check the node has a save function.
            if (!(saveNode is ISave)) {
                GD.Print(String.Format("persistent node '{0}' is not implenting ISave interface, skipped", saveNode.Name));
                continue;
            }

            GD.Print(String.Format("Saving node {0}...", saveNode.Name));

            // Call the node's save function.
            Dictionary<string, object> nodeData = (saveNode as ISave).Save();

            // Store the save dictionary as a new line in the save file.
            saveData.Add(nodeData);
        }
        this._undoStack.Add(saveData);
    }

    public void Load() {
        GD.Print("Loading..");
        if (this._undoStack.Count == 0)
            return;
        // We need to revert the game state so we're not cloning objects during loading.
        // This will vary wildly depending on the needs of a project, so take care with
        // this step.
        // For our example, we will accomplish this by deleting saveable objects.
        var saveNodes = GetTree().GetNodesInGroup(this.GroupName);
        foreach (Node saveNode in saveNodes)
            saveNode.Free();
        RoundManager.Instance.Clear();
        PlantBase.Count = 0;
        GD.Print(this._undoStack.Count);

        foreach (var nodeData in this._undoStack[this._undoStack.Count - 1]) {
            // Firstly, we need to create the object and add it to the tree and set its position.
            var newObjectScene = (PackedScene)ResourceLoader.Load(nodeData["Filename"].ToString());
            var newObject = (Node)newObjectScene.Instance();
            GetNode(nodeData["Parent"].ToString()).AddChild(newObject);

            // Now we set the remaining variables.
            (newObject as ISave).Load(nodeData);
        }
        this._undoStack.RemoveAt(this._undoStack.Count - 1);
        GD.Print(this._undoStack.Count);
    }
}