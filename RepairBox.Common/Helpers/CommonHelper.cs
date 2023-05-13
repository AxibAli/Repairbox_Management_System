using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.Common.Helpers
{
    public class CommonHelper
    {
        public static double TotalPagesforPagination(int total, int pageSize)
        {
            double.TryParse(pageSize.ToString(), out double newPageSize);
            double.TryParse(total.ToString(), out double newTotal);

            return Math.Ceiling(newTotal/newPageSize);
        }
    }
}
