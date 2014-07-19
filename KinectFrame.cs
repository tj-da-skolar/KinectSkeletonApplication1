using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.IO;

namespace KinectSkeletonApplication1
{
    public class KinectFrame
    {
        private float[] xValues;
        private float[] yValues;
        private float[] zValues;

        private long timeFrame;
        private int frameNo;

        public KinectFrame(long timeFrame, int frameNo)
        {
            xValues = new float[12];
            yValues = new float[12];
            zValues = new float[12];

            this.timeFrame = timeFrame;
            this.frameNo = frameNo;
        }

        public void KinectValues(Joint value, int index)
        {
            xValues[index] = value.Position.X;
            yValues[index] = value.Position.Y;
            zValues[index] = value.Position.Z;

        }

        
        public void SaveValues()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Timjay Pc\Documents\FourthYearDLSU\THESIS\6-26-2014\Carl Young.csv", true))
            {
                file.Write("" + timeFrame + "," + frameNo);
                for(int i = 0; i < 12; i++)
                {
                    file.Write("," + xValues[i] + "," + yValues[i] + "," + zValues[i]);
                    if (i == 11)
                        file.WriteLine("");
                }
            }
        }
    }

}
