﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Linq;
using System.IO;

namespace KinectSkeletonApplication1
{
    public partial class MainWindow : Window
    {
        private int framenumber = 0;
        private long firsttick = -1;
        //Instantiate the Kinect runtime. Required to initialize the device.
        //IMPORTANT NOTE: You can pass the device ID here, in case more than one Kinect device is connected.
        KinectSensor sensor = KinectSensor.KinectSensors[0];
        byte[] pixelData;
        Skeleton[] skeletons;

        KinectRecordOptions kro = new KinectRecordOptions();
        KinectRecorder recorder;
        ColorImageFrame cFrame;
        ImageFrame imgFrame;

        int ctr = 0;

        public MainWindow()
        {
            InitializeComponent();

            //Runtime initialization is handled when the window is opened. When the window
            //is closed, the runtime MUST be unitialized.
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.Unloaded += new RoutedEventHandler(MainWindow_Unloaded);

            sensor.ColorStream.Enable();
            sensor.SkeletonStream.Enable();

            

            recorder = new KinectRecorder(kro, File.Open(@"C:\Users\Timjay Pc\Documents\videos.avi", FileMode.Create));
        }

        void runtime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool receivedData = false;

            using (SkeletonFrame SFrame = e.OpenSkeletonFrame())
            {
                if (SFrame == null)
                {
                    // The image processing took too long. More than 2 frames behind.
                }
                else
                {
                    skeletons = new Skeleton[SFrame.SkeletonArrayLength];
                    SFrame.CopySkeletonDataTo(skeletons);
                    receivedData = true;
                }
            }

            if (receivedData)
            {

                Skeleton currentSkeleton = (from s in skeletons
                                            where s.TrackingState == SkeletonTrackingState.Tracked
                                            select s).FirstOrDefault();

                if (currentSkeleton != null)
                {
                    SetEllipsePosition(head, currentSkeleton.Joints[JointType.Head]);
                    SetEllipsePosition(leftHand, currentSkeleton.Joints[JointType.HandLeft]);
                    SetEllipsePosition(rightHand, currentSkeleton.Joints[JointType.HandRight]);
                    SetEllipsePosition(rightKnee, currentSkeleton.Joints[JointType.KneeRight]);
                    SetEllipsePosition(leftKnee, currentSkeleton.Joints[JointType.KneeLeft]);
                    SetEllipsePosition(rightShoulder, currentSkeleton.Joints[JointType.ShoulderRight]);
                    SetEllipsePosition(leftShoulder, currentSkeleton.Joints[JointType.ShoulderLeft]);
                    SetEllipsePosition(leftFoot, currentSkeleton.Joints[JointType.FootLeft]);
                    SetEllipsePosition(rightFoot, currentSkeleton.Joints[JointType.FootRight]);
                    SetEllipsePosition(leftElbow, currentSkeleton.Joints[JointType.ElbowLeft]);
                    SetEllipsePosition(rightElbow, currentSkeleton.Joints[JointType.ElbowRight]);
                    SetEllipsePosition(spine, currentSkeleton.Joints[JointType.Spine]);
                    framenumber++;
                    if (firsttick == -1)
                    {
                        firsttick = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                    }
                    //call kinectframe constructor here.

                    KinectFrame kf = new KinectFrame(DateTime.Now.Ticks/TimeSpan.TicksPerSecond - firsttick, framenumber);

                    kf.KinectValues(currentSkeleton.Joints[JointType.Head], 0);
                    kf.KinectValues(currentSkeleton.Joints[JointType.ShoulderLeft], 1);
                    kf.KinectValues(currentSkeleton.Joints[JointType.ShoulderRight], 2);
                    kf.KinectValues(currentSkeleton.Joints[JointType.KneeLeft], 3);
                    kf.KinectValues(currentSkeleton.Joints[JointType.KneeRight], 4);
                    kf.KinectValues(currentSkeleton.Joints[JointType.ElbowLeft], 5);
                    kf.KinectValues(currentSkeleton.Joints[JointType.ElbowRight], 6);
                    kf.KinectValues(currentSkeleton.Joints[JointType.Spine], 7);
                    kf.KinectValues(currentSkeleton.Joints[JointType.HandLeft], 8);
                    kf.KinectValues(currentSkeleton.Joints[JointType.HandRight], 9);
                    kf.KinectValues(currentSkeleton.Joints[JointType.FootLeft], 10);
                    kf.KinectValues(currentSkeleton.Joints[JointType.FootRight], 11);
                    
                   //kf.SaveValues(); //Uncomment this line AFTER you've finished the setup!
                    
                }
            }
        }


        //This method is used to position the ellipses on the canvas
        //according to correct movements of the tracked joints.

        //IMPORTANT NOTE: Code for vector scaling was imported from the Coding4Fun Kinect Toolkit
        //available here: http://c4fkinect.codeplex.com/
        //I only used this part to avoid adding an extra reference.
        private void SetEllipsePosition(Ellipse ellipse, Joint joint)
        {
            Microsoft.Kinect.SkeletonPoint vector = new Microsoft.Kinect.SkeletonPoint();
            vector.X = ScaleVector(800, joint.Position.X);
            vector.Y = ScaleVector(600, -joint.Position.Y);
            vector.Z = joint.Position.Z;

            Joint updatedJoint = new Joint();
            updatedJoint = joint;
            updatedJoint.TrackingState = JointTrackingState.Tracked;
            updatedJoint.Position = vector;

            Canvas.SetLeft(ellipse, updatedJoint.Position.X);
            Canvas.SetTop(ellipse, updatedJoint.Position.Y);
        }

        private float ScaleVector(int length, float position)
        {
            float value = (((((float)length) / 1f) / 2f) * position) + (length / 2);
            if (value > length)
            {
                return (float)length;
            }
            if (value < 0f)
            {
                return 0f;
            }
            return value;
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            sensor.Stop();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensor.SkeletonFrameReady += runtime_SkeletonFrameReady;
            sensor.ColorFrameReady += runtime_VideoFrameReady;
            sensor.Start();
        }

        void runtime_VideoFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            bool receivedData = false;

            using (ColorImageFrame CFrame = e.OpenColorImageFrame())
            {
                if (CFrame == null)
                {
                    // The image processing took too long. More than 2 frames behind.
                }
                else
                {
                    pixelData = new byte[CFrame.PixelDataLength];
                    CFrame.CopyPixelDataTo(pixelData);
                    receivedData = true;
                    //cFrame = CFrame;
                }
            }

            if (receivedData)
            {
                BitmapSource source = BitmapSource.Create(640, 480, 96, 96,
                        PixelFormats.Bgr32, null, pixelData, 640 * 4);

                videoImage.Source = source;

            }
        }
        private void SaveImage(BitmapSource image)
        {
            System.IO.FileStream stream = new System.IO.FileStream((@"C:\Users\Timjay Pc\Documents\FourthYearDLSU\THESIS\Images\" + ctr + ".jpg"), System.IO.FileMode.Create);
            ctr++;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            encoder.FlipHorizontal = true;
            encoder.FlipVertical = false;
            encoder.QualityLevel = 30;
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(stream);
            stream.Close();
        }

        /*void nui_ColorFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            using (Microsoft.Kinect.)
            // 32-bit per pixel, RGBA image
            PlanarImage Image = e.ImageFrame.Image;
            BitmapSource image = BitmapSource.Create(
                Image.Width, Image.Height, 96, 96, PixelFormats.Bgr32, null, Image.Bits, Image.Width * Image.BytesPerPixel);
            video.Source = image;
            SaveImage(image);
        }*/
    }
}
