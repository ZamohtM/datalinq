using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using E.DataLinq.Core.Reflection;
using E.DataLinq.Web.Services.Abstraction;
using Newtonsoft.Json;

public class MonacoSnippetService : IMonacoSnippetService
{
    private readonly Type _targetType;

    public MonacoSnippetService(Type targetType)
    {
        _targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
    }

    public string BuildSnippetJson()
    {
        var snippets = new List<object>();

        var methods = _targetType
                            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                            .Where(m => m.GetCustomAttribute<ExcludeFromSnippetsAttribute>() == null)
                            .ToArray();

        foreach (var method in methods)
        {
            var methodDescription = method.GetCustomAttribute<HelpDescriptionAttribute>()?.Description ?? "No description available.";
            var parameters = method.GetParameters();

            var insertTextLines = parameters.Select((p, i) =>
            {
                var defaultVal = GetDefaultValueString(p);
                var comma = (i < parameters.Length - 1) ? "," : "";
                return $"    ${{{i + 1}:{defaultVal}}}{comma} //{p.Name}";
            }).ToList();

            var insertText = new StringBuilder();
            insertText.AppendLine($"{method.Name}(");
            insertText.AppendLine(string.Join(",\n", insertTextLines));
            insertText.Append(")");

            var snippet = new
            {
                label = method.Name,
                kind = 3,
                insertText = insertText.ToString(),
                insertTextRules = 4,
                documentation = methodDescription
            };

            snippets.Add(snippet);
        }

        return JsonConvert.SerializeObject(snippets, Formatting.Indented);
    }


    private static string GetDefaultValueString(ParameterInfo p)
    {
        if (p.HasDefaultValue)
        {
            if (p.DefaultValue == null) return "null";
            if (p.ParameterType == typeof(string)) return $"\"{p.DefaultValue}\"";
            if (p.ParameterType == typeof(bool)) return p.DefaultValue.ToString().ToLower();
            return p.DefaultValue.ToString();
        }

        return p.ParameterType == typeof(string) ? "\"\"" : "null";
    }
}
