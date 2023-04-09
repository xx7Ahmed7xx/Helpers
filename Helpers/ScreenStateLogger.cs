using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace AAM.Helpers.Desktop
{
    internal class ScreenStateLogger : IDisposable
    {

        private bool _run, _init;

        public int Size { get; private set; }

        public ScreenStateLogger()
        {

        }

        //public void Start(List<MemoryStream> allStreams)
        public void Start()
        {
            _run = true;
            var factory = new Factory1();
            //Get first adapter
            var adapter = factory.GetAdapter1(0);
            //Get device from adapter
            var device = new SharpDX.Direct3D11.Device(adapter);
            //Get front buffer of the adapter
            var output = adapter.GetOutput(0);
            var output1 = output.QueryInterface<Output1>();

            // Width/Height of desktop to capture
            int width = output.Description.DesktopBounds.Right;
            int height = output.Description.DesktopBounds.Bottom;

            // Create Staging texture CPU-accessible
            var textureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };
            var screenTexture = new Texture2D(device, textureDesc);

            Task.Factory.StartNew(() =>
            {
                // Duplicate the output
                using (var duplicatedOutput = output1.DuplicateOutput(device))
                {
                    while (_run)
                    {
                        var tempMS = new MemoryStream();
                        try
                        {
                            SharpDX.DXGI.Resource screenResource;
                            OutputDuplicateFrameInformation duplicateFrameInformation;

                            // Try to get duplicated frame within given time is ms
                            var result = duplicatedOutput.TryAcquireNextFrame(1, out duplicateFrameInformation, out screenResource);
                            if (result == Result.Ok)
                            {
                                // copy resource into memory that can be accessed by the CPU
                                using (var screenTexture2D = screenResource.QueryInterface<Texture2D>())
                                    device.ImmediateContext.CopyResource(screenTexture2D, screenTexture);

                                // Get the desktop capture texture
                                var mapSource = device.ImmediateContext.MapSubresource(screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

                                // Create Drawing.Bitmap
                                using (var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                                {
                                    var boundsRect = new Rectangle(0, 0, width, height);

                                    // Copy pixels from screen capture Texture to GDI bitmap
                                    var mapDest = bitmap.LockBits(boundsRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                    var sourcePtr = mapSource.DataPointer;
                                    var destPtr = mapDest.Scan0;
                                    for (int y = 0; y < height; y++)
                                    {
                                        // Copy a single line 
                                        Utilities.CopyMemory(destPtr, sourcePtr, width * 4);

                                        // Advance pointers
                                        sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
                                        destPtr = IntPtr.Add(destPtr, mapDest.Stride);
                                    }

                                    // Release source and dest locks
                                    bitmap.UnlockBits(mapDest);
                                    device.ImmediateContext.UnmapSubresource(screenTexture, 0);

                                    ScreenRefreshed?.Invoke(this, converterDemo(bitmap));
                                    _init = true;
                                }
                                screenResource.Dispose();
                                duplicatedOutput.ReleaseFrame();
                            }

                        }
                        catch (SharpDXException e)
                        {
                            if (e.ResultCode.Code != SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code)
                            {
                                Trace.TraceError(e.Message);
                                Trace.TraceError(e.StackTrace);
                            }
                        }
                    }
                }
            });
            while (!_init) ;
        }

        public static byte[] converterDemo(Image x)
        {
            ImageConverter _imageConverter = new ImageConverter();
            return (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
        }

        public void Stop()
        {
            _run = false;
        }

        public void Dispose()
        {

        }

        public EventHandler<byte[]> ScreenRefreshed;
    }
}
