using MyLeasing.Common.Models;
using System.Threading.Tasks;

namespace MyLeasing.Common.Services
{
    public interface IApiService
    {
        Task<Response> GetOwnerByEmail(string urlBase, string servicePrefix, string controller, string tokenType, string accessToken, string email);
        Task<Response> GetTokenAsync(string urlBase, string servicePrefix, string controller, TokenRequest request);
        
        Task<bool> CheckConnectionAsync(string url);
    }
}