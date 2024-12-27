using GS1CompanyPrefix;
using Microsoft.Extensions.Options;

namespace Gs1EpcTranslator.Api.HostedServices;

public sealed class CompanyPrefixLoaderHostedServices : IHostedService
{
    private readonly TimeSpan _refreshDelay;
    private readonly GS1CompanyPrefixProvider _gcpProvider;
    private readonly HttpClient _httpClient;
    private readonly Timer _timer;

    public CompanyPrefixLoaderHostedServices(GS1CompanyPrefixProvider gcpProvider, IOptions<CompanyPrefixOptions> options)
    {
        _refreshDelay = TimeSpan.FromMinutes(options.Value.RefreshDelayInMinutes);
        _gcpProvider = gcpProvider;
        _httpClient = new HttpClient { BaseAddress = new Uri(options.Value.Url) };
        _timer = new(LoadCompanyPrefixes, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer.Change(TimeSpan.Zero, _refreshDelay);

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
        _timer.Dispose();

        return Task.CompletedTask;
    }

    public sealed class CompanyPrefixOptions
    {
        public required int RefreshDelayInMinutes { get; init; }
        public required string Url { get; init; }
    }
}