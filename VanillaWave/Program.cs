using Melanchall.DryWetMidi;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Windowing;

namespace VanillaWave;

internal class Program
{
    private static void Main(string[] args)
    {
        Game game = new();
        game.Run(args);
    }
}
