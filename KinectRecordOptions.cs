﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectSkeletonApplication1
{
    [FlagsAttribute]
    public enum KinectRecordOptions
    {
        Color = 1,
        Depth = 2,
        Skeletons = 4
    }
}
