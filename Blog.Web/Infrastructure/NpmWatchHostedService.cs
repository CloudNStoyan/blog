using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Blog.Web.Infrastructure;
#if DEBUG
public class NpmWatchHostedService : IHostedService, IDisposable
{
    private bool Enabled { get; }
    private ILogger<NpmWatchHostedService> Logger { get; }

    private Process Process { get; set; }

    public NpmWatchHostedService(bool enabled, ILogger<NpmWatchHostedService> logger)
    {
        this.Enabled = enabled;
        this.Logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (this.Enabled)
        {
            this.StartProcess();
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (this.Process == null)
        {
            return Task.CompletedTask;
        }

        this.Process.Close();
        this.Process.Dispose();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        this.Process?.Dispose();
        this.Process = null;
    }

    private void StartProcess()
    {
        this.Process = new Process
        {
            EnableRaisingEvents = true,
            StartInfo = new ProcessStartInfo
            {
                FileName = Path.Join(Directory.GetCurrentDirectory(), "node_modules/.bin/sass.cmd"),
                Arguments = "--watch Sass:wwwroot/sass_compiled",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Directory.GetCurrentDirectory()
            }
        };

        this.Process.EnableRaisingEvents = true;

        this.Process.OutputDataReceived += (_, args) =>
        {
            if (!string.IsNullOrWhiteSpace(args.Data))
            {
                this.Logger.LogInformation(args.Data);
            }
        };
        this.Process.ErrorDataReceived += (_, args) =>
        {
            if (!string.IsNullOrWhiteSpace(args.Data))
            {
                this.Logger.LogError(args.Data);
            }
        };

        this.Process.Exited += this.HandleProcessExit;

        this.Process.Start();
        this.Process.BeginOutputReadLine();
        this.Process.BeginErrorReadLine();

        this.Logger.LogInformation("Started NPM watch");
    }

    private async void HandleProcessExit(object sender, object args)
    {
        this.Process.Dispose();
        this.Process = null;

        this.Logger.LogWarning("npm watch exited, restarting in 1 second.");

        await Task.Delay(1000);
        this.StartProcess();
    }
}
#endif