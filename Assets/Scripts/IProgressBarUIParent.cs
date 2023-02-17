using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProgressBarUIParent
{
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }
}
