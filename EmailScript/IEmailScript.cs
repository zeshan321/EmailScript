using System;
using System.Threading.Tasks;

namespace EmailScript
{
    public interface IEmailScript
    {
        Task RegisterTemplateAsync(string key, string filePath, Type? model = null);
        Task<string?> GetTemplateAsync<T>(string templateKey, T model);
    }
}