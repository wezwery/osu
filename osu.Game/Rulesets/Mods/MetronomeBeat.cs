// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osu.Game.Audio;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Containers;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Mods
{
    public partial class MetronomeBeat : BeatSyncedContainer, IAdjustableAudioComponent
    {
        private readonly double firstHitTime;
        private readonly bool modernSounds;

        private PausableSkinnableSound sampleTick;
        private PausableSkinnableSound sampleTickDownbeat;
        private AudioContainer audioContainer;

        /// <param name="firstHitTime">Start time of the first hit object, used for providing a count down.</param>
        /// <param name="modernSounds">Use the modern metronome sound effects.</param>
        public MetronomeBeat(double firstHitTime, bool modernSounds = false)
        {
            this.firstHitTime = firstHitTime;
            this.modernSounds = modernSounds;
            AllowMistimedEventFiring = false;
            InternalChild = audioContainer = new AudioContainer()
            {
                Children = new Drawable[]
                {
                    sampleTick = new PausableSkinnableSound(new SampleInfo(modernSounds ? @"UI/metronome-tick" : "Gameplay/catch-banana")),
                    sampleTickDownbeat = new PausableSkinnableSound(new SampleInfo(@"UI/metronome-tick-downbeat"))
                }
            };
        }

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes)
        {
            base.OnNewBeat(beatIndex, timingPoint, effectPoint, amplitudes);

            if (!IsBeatSyncedWithTrack) return;

            int timeSignature = timingPoint.TimeSignature.Numerator;

            // play metronome from one measure before the first object.
            if (BeatSyncSource.Clock.CurrentTime < firstHitTime - timingPoint.BeatLength * timeSignature)
                return;

            if (modernSounds)
            {
                var channel = beatIndex % timingPoint.TimeSignature.Numerator == 0 ? sampleTickDownbeat : sampleTick;

                channel.Frequency.Value = RNG.NextDouble(0.98f, 1.02f);
                channel.Play();
            }
            else
            {
                sampleTick.Frequency.Value = beatIndex % timeSignature == 0 ? 1 : 0.5f;
                sampleTick.Play();
            }
        }

        #region IAdjustableAudioComponent

        public IBindable<double> AggregateVolume => audioContainer.AggregateVolume;

        public IBindable<double> AggregateBalance => audioContainer.AggregateBalance;

        public IBindable<double> AggregateFrequency => audioContainer.AggregateFrequency;

        public IBindable<double> AggregateTempo => audioContainer.AggregateTempo;

        public BindableNumber<double> Volume => audioContainer.Volume;

        public BindableNumber<double> Balance => audioContainer.Balance;

        public BindableNumber<double> Frequency => audioContainer.Frequency;

        public BindableNumber<double> Tempo => audioContainer.Tempo;

        public void BindAdjustments(IAggregateAudioAdjustment component)
        {
            audioContainer.BindAdjustments(component);
        }

        public void UnbindAdjustments(IAggregateAudioAdjustment component)
        {
            audioContainer.UnbindAdjustments(component);
        }

        public void AddAdjustment(AdjustableProperty type, IBindable<double> adjustBindable)
        {
            audioContainer.AddAdjustment(type, adjustBindable);
        }

        public void RemoveAdjustment(AdjustableProperty type, IBindable<double> adjustBindable)
        {
            audioContainer.RemoveAdjustment(type, adjustBindable);
        }

        public void RemoveAllAdjustments(AdjustableProperty type)
        {
            audioContainer.RemoveAllAdjustments(type);
        }

        #endregion
    }
}
