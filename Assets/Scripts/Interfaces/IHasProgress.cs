using System;

public interface IHasProgress
{
    public event EventHandler<ProgressEventArgs> OnProgressChanged;
    public class ProgressEventArgs : EventArgs
    {
        public float ProgressNormalized;
    }
}
