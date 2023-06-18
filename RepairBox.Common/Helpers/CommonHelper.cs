using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronBarCode;

namespace RepairBox.Common.Helpers
{
    public class CommonHelper
    {
        private static Random random = new Random();
        public static double TotalPagesforPagination(int total, int pageSize)
        {
            double.TryParse(pageSize.ToString(), out double newPageSize);
            double.TryParse(total.ToString(), out double newTotal);

            return Math.Ceiling(newTotal/newPageSize);
        }
        public static long RandomLongValueGenerator(long minValue, long maxValue)
        {
            long value;

            byte[] buffer = new byte[8];
            random.NextBytes(buffer);

            long longRand = BitConverter.ToInt64(buffer, 0);
            value = Math.Abs(longRand % (maxValue - minValue)) + minValue;

            return value;
        }
        public static void GenerateQRCode(string data, string path)
        {
            QRCodeWriter.CreateQrCode(data, 500, QRCodeWriter.QrErrorCorrectionLevel.Medium).SaveAsPng(path);
        }
    }
}
