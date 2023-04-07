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
        public static BitmapSource GenerateCode(string link)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return Bitmap2BitmapImage(qrCodeImage);
        }
        private static BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
            var hBitmap = bitmap.GetHbitmap();
            BitmapSource? retval = null;

            try
            {
                retval = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return retval;
        }

    }
}
