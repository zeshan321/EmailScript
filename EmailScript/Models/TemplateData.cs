using System.Collections.Generic;
using Microsoft.CodeAnalysis.Scripting;

namespace EmailScript.Models
{
    public class TemplateData
    {
        public string? Template { get; set; }
        public Dictionary<string, Script<object>> Scripts { get; set; } = new();
    }
}