using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmailScript.Extensions;
using EmailScript.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace EmailScript
{
    public class EmailScript : IEmailScript
    {
        private const int MaxDepth = 9;
        private readonly ConcurrentDictionary<string, TemplateData> _templates = new();
        
        public async Task RegisterTemplate(string key, string filePath, Type? model = null)
        {
            var scriptOptions = ScriptOptions.Default.WithImports("System");
            var text = await File.ReadAllTextAsync(filePath);
            var template = PopulateTemplate(text);

            var scripts = new Dictionary<string, Script<object>>();
            foreach (var rawScript in template.ExtractScriptsString())
            {
                var script = rawScript.ReplaceScriptTags();
                if (script.Contains("@{"))
                {
                    script = script.ModifyInlineReturn();
                }
                
                if (model == null)
                {
                    scripts[rawScript] = CSharpScript.Create(script, scriptOptions);
                }
                else
                {
                    scripts[rawScript] = CSharpScript.Create(script, scriptOptions, model);
                }
            }
            
            _templates.TryAdd(key, new TemplateData()
            {
                Template = template,
                Scripts = scripts
            });
        }

        public Task<string?> GetTemplate<T>(string templateKey, T model)
        {
            var template = _templates[templateKey];
            return Generate(template, model);
        }

        private string PopulateTemplate(string template)
        {
            var depth = 0;
            while (true)
            {
                depth++;
                if (depth > MaxDepth)
                {
                    throw new OperationCanceledException("Max depth reached");
                }
                
                var subTemplatesData = template.ExtractTemplatesString().ToList();

                foreach (var subTemplateData in subTemplatesData)
                {
                    var type = subTemplateData.ExtractValue("type=\"", "\"");
                    var modelName = subTemplateData.ExtractValue("model=\"", "\"");
                    if (type == null) continue;

                    var subTemplate = _templates[type].Template;
                    if (subTemplate == null)
                    {
                        continue;
                    }
                    
                    if (modelName != null)
                    {
                        subTemplate = subTemplate.Replace("Model.", $"{modelName}.");
                    }
                    
                    template = template.Replace($"{subTemplateData}</template>", subTemplate);
                }

                if (template.ExtractTemplatesString().Any())
                {
                    continue;
                }

                return template;
            }
        }

        private static async Task<string?> Generate<T>(TemplateData data, T model)
        {
            var template = data.Template;
            if (template == null)
            {
                return null;
            }

            foreach (var (key, script) in data.Scripts)
            {
                template = template.Replace(key, (await script.RunAsync(model)).ReturnValue.ToString());
            }

            return template;
        }
    }
}