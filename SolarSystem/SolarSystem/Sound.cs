using Microsoft.Xna.Framework.Audio;

namespace SolarSystem
{
    public class Sound : GameEntity
    {
        public const float Radius = 0.2f;

        public SoundEffect SoundEffect;
        public SoundEffectInstance SoundEffectInstance;
        public string SoundName;

        public Sound()
        {
            SoundName = @"Sound\sound";
        }

        public override void LoadContent()
        {
            SoundEffect = Game.Content.Load<SoundEffect>(SoundName);
            SoundEffectInstance = SoundEffect.CreateInstance();
            SoundEffectInstance.Volume = 1.0f;
            SoundEffectInstance.IsLooped = true;
            SoundEffectInstance.Play();
        }
    }
}
