using GS1CompanyPrefix;

namespace Gs1EpcTranslator.Api;

public sealed class CompanyPrefixBackgroundLoader(GS1CompanyPrefixProvider companyPrefixProvider) : IHostedService, IDisposable
{
    private Timer? _timer;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri("https://www.gs1.org/") };
        var request = httpClient.GetAsync("sites/default/files/docs/gcp_length/gcpprefixformatlist.json");
        var response = request.Result;

        using var responseStream = response.Content.ReadAsStream();

        GS1CompanyPrefixLoader.LoadFromJsonStream(companyPrefixProvider, responseStream);
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}