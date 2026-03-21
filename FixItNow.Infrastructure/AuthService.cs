using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace FixItNow.Infrastructure
{
    public class AuthService
    {
        private readonly IJSRuntime _js;

        public AuthService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<UserDto?> GetUser()
        {
            var json = await _js.InvokeAsync<string>("localStorage.getItem", "user");

            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<UserDto>(json);
        }
        public async Task Logout()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "user");
        }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }   
}
