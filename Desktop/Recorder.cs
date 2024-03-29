﻿using NAudio.Wave;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Xml.Serialization;

namespace AAM.Helpers.Desktop
{

    /// <summary>
    /// Helper recording class, used to record frames of a specific WindowScreen or FullScreen,
    /// And also specifying whether to record System, Microphone or Both Sounds into the video.
    /// </summary>
    public class Recorder
    {
        #region static helper members

        // Recording Condition
        static bool keepRecording = false;

        // Helper FullScreen Recorder Class, using SharpDX
        static ScreenStateLogger screenStateLogger = new ScreenStateLogger();

        // Microphone Event
        static WaveInEvent micWaveSource;
        // Speaker Event
        static WasapiLoopbackCapture sysWaveSource;
        // Microphone Output
        static WaveFileWriter micWaveFile;
        // Speaker Output
        static WaveFileWriter sysWaveFile;
        #endregion

        /// <summary>
        /// Starts the FullScreen recording process, Which records the whole Monitor using FFMpeg API Libraries.
        /// This method saves the recorded frames into the target path Provided.
        /// </summary>
        /// <param name="path">The path to save the file into.</param>
        public static void StartFullScreenSimpleRecording(string path)
        {
            keepRecording = true;
            Process ffmpegProc = new Process();
            ffmpegProc.StartInfo.FileName = "powershell";
            ffmpegProc.StartInfo.CreateNoWindow = true;
            ffmpegProc.StartInfo.UseShellExecute = false;
            ffmpegProc.StartInfo.RedirectStandardInput = true;
            ffmpegProc.StartInfo.Arguments
                =
                $".\\ffmpeg -init_hw_device d3d11va -filter_complex ddagrab=0 -c:v h264_nvenc -cq:v 20 {path}.mp4";
            ffmpegProc.Start();
            while (keepRecording)
            {
                Thread.Sleep(50);
            }
            using (StreamWriter sw = ffmpegProc.StandardInput)
            {
                sw.Write("q");
            }
        }

        /// <summary>
        /// Stops the WindowScreen recording process.
        /// </summary>
        public static void StopFullScreenSimpleRecording()
        {
            keepRecording = false;
        }

        /// <summary>
        /// Starts the FullScreen recording process, Which records the whole Monitor using SharpDX Libraries.
        /// This method saves the recorded frames (Images) into the provided MemoryStream.
        /// </summary>
        /// <remarks>Warning: Better to use <see cref="StartFullScreenSimpleRecording(string)"/>, This one has issues in recording frames.</remarks>
        /// <param name="imagesStreams">The stream on which the frames will be saved.</param>
        /// <param name="frameRatePerSecond">Number of frames to be taken, each Second. Default is 30</param>
        public static void StartFullScreenRecording(List<Bitmap> imagesStreams, int frameRatePerSecond = 30)
        {
            screenStateLogger = new ScreenStateLogger();
            screenStateLogger.ScreenRefreshed += (sender, data) =>
            {
                imagesStreams.Add((Bitmap)new ImageConverter().ConvertFrom(data));
                Thread.Sleep(1 / frameRatePerSecond);
            };
            screenStateLogger.Start();
        }

        /// <summary>
        /// Stops the FullScreen recording process, and Disposes the resources, You can then use the targeted
        /// MemoryStream at the Starting process.
        /// </summary>
        public static void StopFullScreenRecording()
        {
            screenStateLogger.Stop();
        }

        /// <summary>
        /// Starts the WindowScreen recording process, Which records a specific application using FFMpeg Libraries
        /// to create video of target Main application window, determined by it's title.
        /// </summary>
        /// <param name="processWindowTitle">The full Main Window Title of the application to be recorded.</param>
        public static void StartWindowScreenSimpleRecording(string processWindowTitle)
        {
            keepRecording = true;
            Process ffmpegProc = new Process();
            ffmpegProc.StartInfo.FileName = "powershell";
            ffmpegProc.StartInfo.CreateNoWindow = true;
            ffmpegProc.StartInfo.UseShellExecute = false;
            ffmpegProc.StartInfo.RedirectStandardInput = true;
            ffmpegProc.StartInfo.Arguments
                =
                $".\\ffmpeg -f gdigrab -i title='{processWindowTitle}' -b:v 3M -pix_fmt yuv420p 1.mp4";
            ffmpegProc.Start();
            while (keepRecording)
            {
                Thread.Sleep(50);
            }
            using (StreamWriter sw = ffmpegProc.StandardInput)
            {
                sw.Write("q");
            }
        }

        /// <summary>
        /// Starts the WindowScreen recording process, Which records a specific application using Windows APIs
        /// Calls to take image of the current main application window, Saves the images to the provided ListBitmap.
        /// </summary>
        /// <remarks>Warning: Better to use <see cref="StartWindowScreenSimpleRecording(string)"/>, This one has issues in recording frames.</remarks>
        /// <param name="windowHandl">The main window handle of the application to be recorded.</param>
        /// <param name="screens">The List of bitmaps object to store onto.</param>
        /// <param name="frameRatePerSecond">Number of frames to be taken, each Second. Default is 30.</param>
        public static void StartWindowScreenRecording(IntPtr windowHandl, List<Bitmap> screens, int frameRatePerSecond = 30)
        {
            keepRecording = true;
            while (keepRecording)
            {
                screens.Add(PrintWindow(windowHandl));
                Thread.Sleep(1 / frameRatePerSecond);
            }
        }

        /// <summary>
        /// Stops the WindowScreen recording process.
        /// </summary>
        public static void StopWindowScreenRecording()
        {
            keepRecording = false;
        }

        /// <summary>
        /// Starts the System Sounds Recording process, Which records any sound generated by the Windows
        /// APIs using NAudio Libraries.
        /// </summary>
        /// <param name="fileName">The file path which will be written to (example: wav1.wave - extension WAV is important)</param>
        public static void StartSysSoundRecording(string fileName)
        {
            sysWaveSource = new WasapiLoopbackCapture();
            sysWaveSource.WaveFormat = new NAudio.Wave.WaveFormat(44100, 1);

            sysWaveSource.DataAvailable += new EventHandler<WaveInEventArgs>(sysWaveSource_DataAvailable);
            sysWaveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(sysWaveSource_RecordingStopped);

            sysWaveFile = new WaveFileWriter(fileName, sysWaveSource.WaveFormat);
            sysWaveSource.StartRecording();
        }

        /// <summary>
        /// Starts the Microphone Sounds Recording process, Which records any sound generated by the Microphone
        /// Drivers using NAudio Libraries.
        /// </summary>
        /// <param name="fileName">The file path which will be written to (example: wav1.wave - extension WAV is important)</param>
        public static void StartMicSoundRecording(string fileName)
        {
            micWaveSource = new WaveInEvent();
            micWaveSource.WaveFormat = new NAudio.Wave.WaveFormat(44100, 1);

            micWaveSource.DataAvailable += new EventHandler<WaveInEventArgs>(micWaveSource_DataAvailable);
            micWaveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(micWaveSource_RecordingStopped);

            micWaveFile = new WaveFileWriter(fileName, micWaveSource.WaveFormat);
            micWaveSource.StartRecording();
        }

        /// <summary>
        /// Stops the system sounds recording process.
        /// </summary>
        public static void StopSysSoundRecording()
        {
            if (sysWaveSource != null)
                sysWaveSource.StopRecording();
        }

        /// <summary>
        /// Stops the microphone sounds recording process.
        /// </summary>
        public static void StopMicSoundRecording()
        {
            if (micWaveSource != null)
                micWaveSource.StopRecording();
        }

        /// <summary>
        /// Saves all files created by the older methods, If there is a provided Bitmap lists, It will use it to write
        /// temporary memory files onto disk to save them as a video using the FFMpeg API.
        /// <br /> Otherwise, It will save just files created by the FFMpeg directly, from Simple methods onto the target path.
        /// </summary>
        /// <param name="videoPath">The video relative path, you don't need to put file extension (example: filename ONLY).</param>
        /// <param name="frameRate">Target framerate of the video, currently unused. Default is 30 FPS.</param>
        /// <param name="Bitmaps1">First bitmap lists to be written to disk (provided you used older methods).</param>
        /// <param name="Bitmaps2">Second bitmap lists to be written to disk (provided you used older methods)</param>
        public static void SaveFullVideo(string videoPath, int frameRate = 30, List<Bitmap> Bitmaps1 = null, List<Bitmap> Bitmaps2 = null)
        {
            string parentPath = Directory.GetParent(Environment.ProcessPath).FullName + "\\";
            Thread.Sleep(500);
            if (Bitmaps1 != null && Bitmaps2 != null)
            {
                for (int i = 0; i < Bitmaps1.Count; i++)
                {
                    Bitmaps1[i].Save(parentPath + $"{i}.png", ImageFormat.Png);
                    Bitmaps1[i].Dispose();
                }
                Bitmaps1.Clear();

                for (int i = 0; i < Bitmaps2.Count; i++)
                {
                    Bitmaps2[i].Save(parentPath + $"{i}.png", ImageFormat.Png);
                    Bitmaps2[i].Dispose();
                }
                Bitmaps2.Clear();
            }


            Process ffmpegProc = new Process();
            ffmpegProc.StartInfo.FileName = "powershell";
            ffmpegProc.StartInfo.CreateNoWindow = true;
            // Concat all wav files, incase we are recording both audios.
            string[] wavFiles = Directory.GetFiles(parentPath, "*.wav");
            if (wavFiles.Length == 1)
            {
                foreach (var file in wavFiles)
                {
                    File.Move(file, Path.Combine(parentPath, "output.wav"));
                }
            }
            else
            {
                ffmpegProc.StartInfo.Arguments
                    =
                    $".\\ffmpeg -i 1.wav -i 2.wav -filter_complex amix=inputs=2:duration=longest output.wav";
                ffmpegProc.Start();
                ffmpegProc.WaitForExit();
            }

            // Now check if there is a created mp4 video file, because of simple recording for example.
            string[] mp4Files = Directory.GetFiles(parentPath, "*.mp4");
            if (mp4Files.Length == 1)
            {
                ffmpegProc.StartInfo.Arguments
                =
                $".\\ffmpeg -i 1.mp4 -i output.wav -c:v copy -c:a aac {videoPath}.mp4";
                ffmpegProc.Start();
                ffmpegProc.WaitForExit();
            }
            else if (mp4Files.Length == 0)
            {
                ffmpegProc.StartInfo.Arguments
                =
                $".\\ffmpeg -framerate {frameRate} -pattern_type sequence -i '%d.png' -i 'output.wav' -c:v libx264 -pix_fmt yuv420p '{videoPath}.mp4'";
                ffmpegProc.Start();
                ffmpegProc.WaitForExit();
            }

            // Delete previous recorded files, If found.
            string[] directoryFiles1 = Directory.GetFiles(parentPath, "*.png");
            foreach (string directoryFile in directoryFiles1)
            {
                File.Delete(directoryFile);
            }
            string[] directoryFiles2 = Directory.GetFiles(parentPath, "*.wav");
            foreach (string directoryFile in directoryFiles2)
            {
                File.Delete(directoryFile);
            }
            string[] directoryFiles3 = Directory.GetFiles(parentPath, "*.mp4");
            foreach (string directoryFile in directoryFiles3)
            {
                if (new FileInfo(directoryFile).Name.Contains("1"))
                    File.Delete(directoryFile);
            }
            if (ffmpegProc.ExitCode == 0)
            {
                MessageBox.Show("Video saved successfully!");
            }
        }

        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        private static Bitmap PrintWindow(IntPtr hwnd)
        {
            RECT rc;
            GetWindowRect(hwnd, out rc);

            Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
            using (Graphics gfxBmp = Graphics.FromImage(bmp))
            {
                IntPtr hdcBitmap = gfxBmp.GetHdc();

                // 2 Value for DirectComposition windows..
                PrintWindow(hwnd, hdcBitmap, 2);

                gfxBmp.ReleaseHdc(hdcBitmap);

                return bmp;
            }
        }

        static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            XmlSerializer xs = new XmlSerializer(typeof(object));
            using (MemoryStream ms = new MemoryStream())
            {
                xs.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        #region Sound Recorders Stopping and Data events

        static void sysWaveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (sysWaveFile != null)
            {
                sysWaveFile.Write(e.Buffer, 0, e.BytesRecorded);
                sysWaveFile.Flush();
            }
        }

        static void sysWaveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (sysWaveFile != null)
            {
                sysWaveFile.Dispose();
                sysWaveFile = null;
            }

            if (sysWaveFile != null)
            {
                sysWaveFile.Dispose();
                sysWaveFile = null;
            }

        }

        static void micWaveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (micWaveFile != null)
            {
                micWaveFile.Write(e.Buffer, 0, e.BytesRecorded);
                micWaveFile.Flush();
            }
        }

        static void micWaveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (micWaveFile != null)
            {
                micWaveFile.Dispose();
                micWaveFile = null;
            }

            if (micWaveFile != null)
            {
                micWaveFile.Dispose();
                micWaveFile = null;
            }

        }

        #endregion

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

    }

    /// <summary>
    /// Helper struct that represents a rectangular shape, according to monitor dimensions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        private int _Left;
        private int _Top;
        private int _Right;
        private int _Bottom;

        public RECT(RECT Rectangle) : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
        {
        }
        public RECT(int Left, int Top, int Right, int Bottom)
        {
            _Left = Left;
            _Top = Top;
            _Right = Right;
            _Bottom = Bottom;
        }

        public int X
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Y
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Left
        {
            get { return _Left; }
            set { _Left = value; }
        }
        public int Top
        {
            get { return _Top; }
            set { _Top = value; }
        }
        public int Right
        {
            get { return _Right; }
            set { _Right = value; }
        }
        public int Bottom
        {
            get { return _Bottom; }
            set { _Bottom = value; }
        }
        public int Height
        {
            get { return _Bottom - _Top; }
            set { _Bottom = value + _Top; }
        }
        public int Width
        {
            get { return _Right - _Left; }
            set { _Right = value + _Left; }
        }
        public System.Drawing.Point Location
        {
            get { return new System.Drawing.Point(Left, Top); }
            set
            {
                _Left = value.X;
                _Top = value.Y;
            }
        }
        public System.Drawing.Size Size
        {
            get { return new System.Drawing.Size(Width, Height); }
            set
            {
                _Right = value.Width + _Left;
                _Bottom = value.Height + _Top;
            }
        }

        public static implicit operator System.Drawing.Rectangle(RECT Rectangle)
        {
            return new System.Drawing.Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
        }
        public static implicit operator RECT(System.Drawing.Rectangle Rectangle)
        {
            return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
        }
        public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
        {
            return Rectangle1.Equals(Rectangle2);
        }
        public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
        {
            return !Rectangle1.Equals(Rectangle2);
        }

        public override string ToString()
        {
            return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public bool Equals(RECT Rectangle)
        {
            return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
        }

        public override bool Equals(object Object)
        {
            if (Object is RECT)
            {
                return Equals((RECT)Object);
            }
            else if (Object is System.Drawing.Rectangle)
            {
                return Equals(new RECT((System.Drawing.Rectangle)Object));
            }

            return false;
        }
    }
}