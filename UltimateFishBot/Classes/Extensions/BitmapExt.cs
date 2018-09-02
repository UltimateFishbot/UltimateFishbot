using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace UltimateFishBot.Extensions
{
    internal static class BitmapExt
    {
        [DllImport("msvcrt.dll")]
        private static extern int memcmp(IntPtr b1, IntPtr b2, long count);

        public static bool ImageCompare(this Bitmap b1, Bitmap b2)
        {
            if (b1 == null || b2 == null) return false;
            if (b1.Size != b2.Size || !b1.PixelFormat.Equals(b2.PixelFormat)) return false;
            if (ReferenceEquals(b1, b2)) return true;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                var bd1Scan0 = bd1.Scan0;
                var bd2Scan0 = bd2.Scan0;

                var stride = bd1.Stride;
                var len = stride * b1.Height;

                return memcmp(bd1Scan0, bd2Scan0, len) == 0;
            }
            finally
            {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }
    }
}
