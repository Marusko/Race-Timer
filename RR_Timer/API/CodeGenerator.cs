using QRCoder;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace RR_Timer.API
{
    internal static class CodeGenerator
    {
        /// <summary>
        /// Method will encode link or any text and returns QR code
        /// </summary>
        /// <param name="link">Link that will be encoded</param>
        /// <returns>QR code image as BitmapSource</returns>
        public static BitmapSource? GenerateCode(string link)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return Bitmap2BitmapImage(qrCodeImage);
        }
        /// <summary>
        /// Converts Bitmap to BitmapSource, copied from https://stackoverflow.com/a/71676333
        /// </summary>
        /// <param name="bitmap">Bitmap to convert</param>
        /// <returns></returns>
        private static BitmapSource? Bitmap2BitmapImage(Bitmap bitmap)
        {
            var hBitmap = bitmap.GetHbitmap();
            BitmapSource? bitmapSource = null;

            try
            {
                bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return bitmapSource;
        }

    }
}
