using Godot;
using System;
using System.Threading.Tasks;

public partial class Pip : AnimatedSprite2D
{
    private const int Framerate = 12;

    private int frameCount;
    private int frame = 0;

    public override void _Ready()
    {
        GD.Print("Pip!");
        frameCount = SpriteFrames.GetFrameCount("default");
        Animate();
    }

    private async void Animate()
    {      
        await Task.Delay(1000 / Framerate);
        if (!IsInstanceValid(this))
        {
            return;
        }

        SetFrameAndProgress(frame, 0f);

        frame++;
        if (frame >= frameCount)
            frame = 0;

        Animate();
    }

}
