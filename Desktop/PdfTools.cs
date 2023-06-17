using Ghostscript.NET.Rasterizer;
using System.Drawing.Imaging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace AAM.Helpers.Desktop
{
    /// <summary>
    /// PDF Automation helper class to manipulate PDFs. <br></br>
    /// Warning: Requires GhostScript to be installed on the machine! <br></br>
    /// <a href="https://ghostscript.com/releases/gsdnld.html">https://ghostscript.com/releases/gsdnld.html</a>
    /// </summary>
    public static class PdfTools
    {
        /// <summary>
        /// Reads a given PDF and converts it's pages into text file per each.
        /// </summary>
        /// <param name="inputPDF">Path to the PDF file.</param>
        public static void ConvertPdf2TextPages(string inputPDF)
        {
            PdfReader reader = new PdfReader(inputPDF);
            PdfDocument pdfDoc = new PdfDocument(reader);
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var text = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i)).Split("\n");
                File.WriteAllLines($"TempTextsFromlastPDF\\page-{i}.txt", text);
            }
        }

        /// <summary>
        /// Reads a given PDF and converts it's pages into images per each.
        /// </summary>
        /// <param name="inputPDF">Path to the PDF file.</param>
        /// <returns>The number of pages in the given PDF file.</returns>
        public static int ConvertPdf2PngImages(string inputPDF)
        {
            int desired_dpi = 96;

            string inputPdfPath = inputPDF;
            string outputPath = Directory.CreateDirectory("TempImagesFromLastPDF").Name;

            // Use File Stream to open the input pdf files, to suppress Security exception in IO.
            using (var inputStream = File.Open(inputPdfPath, FileMode.Open))
            {
                using (var rasterizer = new GhostscriptRasterizer())
                {
                    rasterizer.Open(inputStream);

                    for (var pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                    {
                        var pageFilePath = Path.Combine(outputPath, string.Format("Page-{0}.png", pageNumber));

                        var img = rasterizer.GetPage(desired_dpi, pageNumber);
                        img.Save(pageFilePath, ImageFormat.Png);
                    }
                    return rasterizer.PageCount;
                }
            }
        }
    }
}
