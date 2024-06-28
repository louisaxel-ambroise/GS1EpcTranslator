using GS1CompanyPrefix;
using Microsoft.Extensions.Options;

namespace Gs1EpcTranslator.Api;

public sealed class CompanyPrefixBackgroundLoader : IHostedService, IDisposable
{
    private readonly GS1CompanyPrefixProvider _gcpProvider;
    private readonly HttpClient _httpClient;
    private readonly Timer _timer;

    public CompanyPrefixBackgroundLoader(GS1CompanyPrefixProvider gcpProvider, IOptions<CompanyPrefixOptions> options)
    {
        var refreshDelay = TimeSpan.FromMinutes(options.Value.RefreshDelayInMinutes);

        _gcpProvider = gcpProvider;
        _httpClient = new HttpClient { BaseAddress = new Uri(options.Value.Url) };
        _timer = new(LoadCompanyPrefixes, null, TimeSpan.Zero, refreshDelay);
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    private void LoadCompanyPrefixes(object? _)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
        var response = _httpClient.Send(request, HttpCompletionOption.ResponseHeadersRead);

        using var responseStream = response.Content.ReadAsStream();

        GS1CompanyPrefixLoader.LoadFromJsonStream(_gcpProvider, responseStream, response.Content.Headers.ContentLength ?? 0);
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        return Task.CompletedTask;
    }

    public void Dispose() => _timer.Dispose();

    public sealed class CompanyPrefixOptions
    {
        public required int RefreshDelayInMinutes { get; init; }
        public required string Url { get; init; }
    }
}