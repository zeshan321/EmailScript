using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EmailScript.Extensions
{
    public static class StringExtensions
    {
        private const string StartScript = "<script type=\"c#\">";
        private const string EndScript = "<.script>";
        private const string EndScriptReplace = "</script>";
        
        private static readonly Regex ScriptRegex = new($"{StartScript}(.|\n)*?{EndScript}", RegexOptions.Compiled);
        private static readonly Regex InlineReturnRegex = new("@{.+}", RegexOptions.Compiled);
        private static readonly Regex TemplateRegex = new($"<template type=\".+\">", RegexOptions.Compiled);
        private static readonly Regex TemplateWithModelRegex = new($"<template type=\".+\" model=\".+\">", RegexOptions.Compiled);

        public static IEnumerable<string> ExtractScriptsString(this string source)
        {
            var results = new List<string>();

            foreach (Match match in ScriptRegex.Matches(source).Concat(InlineReturnRegex.Matches(source)))
            {
                results.Add(match.Groups[0].Value);
            }

            return results;
        }

        public static string ReplaceScriptTags(this string source)
        {
            return source.Replace(StartScript, string.Empty).Replace(EndScriptReplace, string.Empty);
        }
        
        public static string ModifyInlineReturn(this string source)
        {
            var cleaned = source.ReplaceFirst("@{", string.Empty);  
            return $"return {cleaned.Remove(cleaned.Length - 1)};";
        }
        
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        
        public static IEnumerable<string> ExtractTemplatesString(this string source)
        {
            var results = new List<string>();

            foreach (Match match in TemplateWithModelRegex.Matches(source).Concat(TemplateRegex.Matches(source)))
            {
                results.Add(match.Groups[0].Value);
            }

            return results;
        }
        
        public static string? ExtractValue(this string source, string start = "", string end = "")
        {
            if (!source.Contains(start))
            {
                return null;
            }
            
            var rest = source.Split(start)[1];
            return rest.Contains(end) ? rest.Split(end)[0] : rest;
        }
    }
}
