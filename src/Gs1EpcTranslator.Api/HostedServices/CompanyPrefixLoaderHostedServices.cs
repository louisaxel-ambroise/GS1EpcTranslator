using GS1CompanyPrefix;
using Microsoft.Extensions.Options;
using System.Xml;

namespace Gs1EpcTranslator.Api.HostedServices;

public sealed class CompanyPrefixLoaderHostedServices : IHostedService
{
    private readonly TimeSpan _refreshDelay;
    private readonly GS1CompanyPrefixProvider _gcpProvider;
    private readonly HttpClient _httpClient;
    private readonly Timer _timer;
    private readonly ILogger<CompanyPrefixLoaderHostedServices> _logger;
    private string? _lastEtag;

    private static readonly XmlReaderSettings settings = new()
    {
        IgnoreComments = true,
        IgnoreWhitespace = true,
        CloseInput = true
    };

    public CompanyPrefixLoaderHostedServices(
        GS1CompanyPrefixProvider gcpProvider, 
        IOptions<CompanyPrefixOptions> options,
        ILogger<CompanyPrefixLoaderHostedServices> logger)
    {
        _refreshDelay = TimeSpan.FromMinutes(options.Value.RefreshDelayInMinutes);
        _gcpProvider = gcpProvider;
        _httpClient = new HttpClient { BaseAddress = new Uri(options.Value.Url) };
        _timer = new(Execute, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        _logger = logger;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer.Change(TimeSpan.Zero, _refreshDelay);

        return Task.CompletedTask;
    }

    private void Execute(object? _)
    {
        if (_lastEtag is null || ShouldLoadCompanyPrefixes(_lastEtag))
        {
            _logger.LogInformation("GCP list is out of date with source");
            _logger.LogInformation("Reloading GCP list from {AbsoluteUri}", _httpClient.BaseAddress?.AbsoluteUri);

            LoadCompanyPrefixes();
        }
        else
        {
            _logger.LogInformation("GCP list up to date with source");
        }
    }

    private void LoadCompanyPrefixes()
    {
        using var response = _httpClient.Send(new(HttpMethod.Get, string.Empty), HttpCompletionOption.ResponseHeadersRead);
        using var reader = XmlReader.Create(response.Content.ReadAsStream(), settings);

        while (reader.ReadToFollowing("entry"))
        {
            var prefix = reader.GetAttribute("prefix");
            var length = int.Parse(reader.GetAttribute("gcpLength") ?? "-1");

            _gcpProvider.SetPrefix(prefix, length);
        }
        _lastEtag = response.Headers.ETag?.Tag;
    }

    private bool ShouldLoadCompanyPrefixes(string lastEtag)
    {
        using var response = _httpClient.Send(new(HttpMethod.Head, string.Empty), HttpCompletionOption.ResponseHeadersRead);

        return !string.Equals(response.Headers.ETag?.Tag, lastEtag);
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
