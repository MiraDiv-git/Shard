using System.IO;
using Engine;

namespace Game;

public class TextureTest : Script
{
    private Texture? _texture;

    public override void Ready()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Test", "texture.png");
        _texture = new Texture(path);
        Logger.Info("TextureTest", $"Loaded texture: {_texture.Width}x{_texture.Height}");
    }

    public override void Draw(SpriteBatch batch)
    {
        if (_texture is null) return;

        // Center on screen (1280x720)
        float screenW = 1280;
        float screenH = 720;
        float scale = 0.3f;
        float w = _texture.Width * scale;
        float h = _texture.Height * scale;
        float x = (screenW - w) / 2f;
        float y = (screenH - h) / 2f;

        batch.Draw(_texture, x, y, w, h);
    }
}