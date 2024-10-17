namespace VanillaWave;

public static partial class Utils
{
    public static float DbToVolume(this float db)
    {
        return MathF.Pow(10f, db / 20);
    }

    public static float VolumeToDb(this float volume)
    {
        return 20f * MathF.Log10(volume);
    }
}
