using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AAM.Helpers.Desktop
{
    /// <summary>
    /// Simple files manipulation class.
    /// </summary>
    public class FilesSimpler
    {
        /// <summary>
        /// Loads an image as ImageSource object into memory.
        /// </summary>
        /// <param name="relativePath">Path to the Image file.</param>
        /// <returns>ImageSource (specifically BitmapImage) Object.</returns>
        public static ImageSource LoadImageSourceFromPath(string relativePath)
        {
            var bmp = new BitmapImage();
            using (var stream = File.OpenRead(relativePath))
            {
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = stream;
                bmp.EndInit();
            }
            return bmp;
        }

        /// <summary>
        /// Loads an image as ImageSource object into memory.
        /// </summary>
        /// <param name="img">Image object to be converted.</param>
        /// <returns>ImageSource (specifically BitmapImage) Object.</returns>
        public static ImageSource LoadImageSourceFromImage(Image img)
        {
            using (var memory = new MemoryStream())
            {
                img.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        /// <summary>
        /// Launches a given OpenFileDialog object and waits for it to exit.
        /// </summary>
        /// <param name="ofd">The OpenFileDialog object to be started.</param>
        /// <returns>A boolean value, which shows whether it succeeded in selecting a file or not.</returns>
        public static bool OpenFileDialogLauncher(Microsoft.Win32.OpenFileDialog ofd)
        {
            if (ofd == null)
            {
                ofd = new Microsoft.Win32.OpenFileDialog();
            }
            return ofd.ShowDialog().Value;
        }
    }
}
