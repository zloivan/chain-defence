using System;

namespace ChainDefense.UI.ProgressBar
{
    public interface IHasProgress
    {
        event EventHandler<ProgressEventArgs> OnProgressUpdate;

        float GetNormalizedProgress();

        public class ProgressEventArgs : EventArgs
        {
            public ProgressEventArgs(float normalizedProgress) =>
                NormalizedProgress = normalizedProgress;

            public float NormalizedProgress { get; set; }
        }
    }
}