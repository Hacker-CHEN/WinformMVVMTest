using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UW.Common
{
    public class ExcelSheet
    {
        public string SheetName { get; set; }

        public string DataTableName { get; set; }

        public int FirstRowNum { get; set; }

        public List<string> LstColumnName { get; set; }
    }
}
