using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using OpinionesExtraccion.Data.Interfaces;
using OpinionesExtraccion.Data.Models.Api;

namespace OpinionesExtraccion.Data.Repositories.Api;

public class SocialCommentApiRepository : ISocialCommentApiRepository
{
    private readonly HttpClient _httpClient;

    public SocialCommentApiRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<SocialCommentRecord>> GetSocialCommentsAsync(CancellationToken cancellationToken = default)
    {
        var result = new List<SocialCommentRecord>();

        var response = await _httpClient.GetAsync("/api/socialcomments", cancellationToken);
        response.EnsureSuccessStatusCode();

        var records = await response.Content.ReadFromJsonAsync<List<SocialCommentRecord>>(cancellationToken: cancellationToken);
        if (records != null)
            result.AddRange(records);

        return result;
    }
}