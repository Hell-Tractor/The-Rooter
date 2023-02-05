using Godot;

public class AudioManager : Node2D {
    public static AudioManager Instance { get; private set; } = null;
    [Export]
    public NodePath BGMPlayerPath;
    public AudioStreamPlayer2D BGMPlayer { get; private set; }
    [Export]
    public NodePath SEPlayerPath;
    public AudioStreamPlayer2D SEPlayer { get; private set; }

    public override void _EnterTree() {
        if (Instance != null)
            throw new System.Exception("Duplicate AudioManager exists.");
        Instance = this;

        this.BGMPlayer = this.GetNode<AudioStreamPlayer2D>(this.BGMPlayerPath);
        this.SEPlayer = this.GetNode<AudioStreamPlayer2D>(this.SEPlayerPath);
    }

    public void PlayBGM(string path) {
        this.BGMPlayer.Stream = GD.Load<AudioStream>(path);
        this.BGMPlayer.Play();
    }

    public void PlaySE(string path) {
        this.SEPlayer.Stream = GD.Load<AudioStream>(path);
        this.SEPlayer.Play();
    }
}