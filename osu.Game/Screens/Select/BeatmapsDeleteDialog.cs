// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Game.Beatmaps;
using osu.Game.Localisation;
using osu.Game.Overlays.Dialog;

namespace osu.Game.Screens.Select
{
    public partial class BeatmapsDeleteDialog : DeletionDialog
    {
        private readonly List<BeatmapSetInfo> beatmapSets;

        public BeatmapsDeleteDialog(List<BeatmapSetInfo> beatmapSets)
        {
            this.beatmapSets = beatmapSets;
            BodyText = DeleteConfirmationContentStrings.Beatmaps;
        }

        [BackgroundDependencyLoader]
        private void load(BeatmapManager beatmapManager)
        {
            DangerousAction = () => beatmapManager.Delete(beatmapSets, true);
        }
    }
}
