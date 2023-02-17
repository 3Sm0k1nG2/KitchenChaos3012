using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgressBarUIParentCounter : BaseCounter, IProgressBarUIParent
{
    public virtual event EventHandler<IProgressBarUIParent.OnProgressChangedEventArgs> OnProgressChanged;
}
