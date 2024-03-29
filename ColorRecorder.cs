﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Kinect;

namespace KinectSkeletonApplication1
{
    class ColorRecorder
    {
        DateTime referenceTime;
        readonly BinaryWriter writer;

        internal ColorRecorder(BinaryWriter writer)
        {
            this.writer = writer;
            referenceTime = DateTime.Now;
        }

        public void Record(ColorImageFrame frame)
        {
            // Header
            writer.Write((int)KinectRecordOptions.Color);

            // Data
            TimeSpan timeSpan = DateTime.Now.Subtract(referenceTime);
            referenceTime = DateTime.Now;
            writer.Write((long)timeSpan.TotalMilliseconds);
            writer.Write(frame.BytesPerPixel);
            writer.Write((int)frame.Format);
            writer.Write(frame.Width);
            writer.Write(frame.Height);

            writer.Write(frame.FrameNumber);

            // Bytes
            writer.Write(frame.PixelDataLength);
            byte[] bytes = new byte[frame.PixelDataLength];
            frame.CopyPixelDataTo(bytes);
            writer.Write(bytes);
        }
    }
}
