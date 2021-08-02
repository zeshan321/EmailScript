using System;
using System.Threading.Tasks;

namespace EmailScript
{
    public interface IEmailScript
    {
        Task RegisterTemplate(string key, string filePath, Type? model = null);
        Task<string?> GetTemplate<T>(string templateKey, T model);
    }
}