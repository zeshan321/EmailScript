using System.Collections.Generic;
using System.Threading.Tasks;
using EmailScriptExample.Models;

namespace EmailScriptExample
{
    public static class Program
    {
        private static readonly EmailScript.EmailScript EmailScript = new();
        
        private static async Task Main()
        {
            await EmailScript.RegisterTemplateAsync("TableHeader", "EmailTemplates/TableHeader.html");
            await EmailScript.RegisterTemplateAsync("Table", "EmailTemplates/Table.html");
            await EmailScript.RegisterTemplateAsync("Sample", "EmailTemplates/Sample.html", typeof(BaseModel<ModelExample>));
                
            var test = await EmailScript.GetTemplateAsync("Sample", new BaseModel<ModelExample>
            {
                Model = new ModelExample
                {
                    Name = "Hello world",
                    TableA = new Table
                    {
                        Columns = new []{ "A", "B"},
                        Rows = new List<string[]>
                        {
                            new []{ "Test", "Best" },
                            new []{ "Wah", "Bah" }
                        }
                    },
                    TableB = new Table
                    {
                        Columns = new []{ "C", "D"},
                        Rows = new List<string[]>
                        {
                            new []{ "Jello", "Bello" },
                            new []{ "Wass", "Duup" }
                        }
                    }
                }
            });
        }
    }
}