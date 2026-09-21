using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ITSM.Web.Client.Models;

namespace ITSM.Web.Client.Services
{
    public class StatusesClient : IStatusesClient
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public StatusesClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<StatusDto>> GetAllAsync(CancellationToken ct = default)
        {
            var result = await _http.GetFromJsonAsync<IEnumerable<StatusDto>>("api/statuses", _jsonOptions, ct);
            return result ?? Array.Empty<StatusDto>();
        }

        public async Task<StatusDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _http.GetFromJsonAsync<StatusDto>($"api/statuses/{id}", _jsonOptions, ct);
        }

        public async Task<StatusDto> CreateAsync(StatusDto status, CancellationToken ct = default)
        {
            var response = await _http.PostAsJsonAsync("api/statuses", status, _jsonOptions, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<StatusDto>(_jsonOptions, ct) ?? status;
        }

        public async Task UpdateAsync(int id, StatusDto status, CancellationToken ct = default)
        {
            var response = await _http.PutAsJsonAsync($"api/statuses/{id}", status, _jsonOptions, ct);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var response = await _http.DeleteAsync($"api/statuses/{id}", ct);
            response.EnsureSuccessStatusCode();
        }
    }
}
