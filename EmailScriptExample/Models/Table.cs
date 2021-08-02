using System.Collections.Generic;

namespace EmailScriptExample.Models
{
    public class Table
    {
        public string[] Columns { get; set; }
        public List<string[]> Rows { get; set; }
    }
}