using System;
using System.Collections.Generic;
using Godot;
using GC = Godot.Collections;

public class UndoManager : Node {
    [Export]
    public string GroupName = "Undoable";
    private List<List<GC.Dictionary<string, object>>> _undoStack = new List<List<GC.Dictionary<string, object>>>();
    [Export]
    public NodePath RoundManagerPath;
    private RoundManager _roundManager;

    public override void _Ready() {
        this._roundManager = this.GetNode<RoundManager>(this.RoundManagerPath);
    }

    public void Save() {
        List<GC.Dictionary<string, object>> saveData = new List<GC.Dictionary<string, object>>();
        foreach (Node saveNode in this.GetTree().GetNodesInGroup(this.GroupName)) {
            // Check the node is an instanced scene so it can be instanced again during load.
            if (saveNode.Filename.Empty()) {
                GD.Print(String.Format("persistent node '{0}' is not an instanced scene, skipped", saveNode.Name));
                continue;
            }

            // Check the node has a save function.
            if (!saveNode.HasMethod("Save")) {
                GD.Print(String.Format("persistent node '{0}' is missing a Save() function, skipped", saveNode.Name));
                continue;
            }

            // Call the node's save function.
            var nodeData = saveNode.Call("Save") as GC.Dictionary<string, object>;

            // Store the save dictionary as a new line in the save file.
            saveData.Add(nodeData);
        }
        this._undoStack.Add(saveData);
    }

    public void Load() {
        if (this._undoStack.Count == 0)
            return;
        // We need to revert the game state so we're not cloning objects during loading.
        // This will vary wildly depending on the needs of a project, so take care with
        // this step.
        // For our example, we will accomplish this by deleting saveable objects.
        var saveNodes = GetTree().GetNodesInGroup(this.GroupName);
        foreach (Node saveNode in saveNodes)
            saveNode.QueueFree();
        this._roundManager.Clear();
        PlantBase.Count = 0;

        foreach (var nodeData in this._undoStack[this._undoStack.Count - 1]) {
            // Firstly, we need to create the object and add it to the tree and set its position.
            var newObjectScene = (PackedScene)ResourceLoader.Load(nodeData["Filename"].ToString());
            var newObject = (Node)newObjectScene.Instance();
            GetNode(nodeData["Parent"].ToString()).AddChild(newObject);

            // Now we set the remaining variables.
            foreach (KeyValuePair<string, object> entry in nodeData) {
                string key = entry.Key.ToString();
                if (key == "Filename" || key == "Parent")
                    continue;
                newObject.Set(key, entry.Value);
            }
        }
    }
}