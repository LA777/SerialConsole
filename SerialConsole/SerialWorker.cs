using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SerialConsole.Models;
using SerialConsole.Services;
using System.IO.Ports;

namespace SerialConsole
{
    public class SerialWorker : BackgroundService
    {
        private readonly IConsoleSpinner _consoleSpinner;
        private readonly ILogger<SerialWorker> _logger;
        private readonly SerialPort _serialPort;

        public SerialWorker(IConsoleSpinner consoleSpinner, ILogger<SerialWorker> logger, IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _consoleSpinner = consoleSpinner ?? throw new ArgumentNullException(nameof(consoleSpinner));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var optionsMonitorData = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
            var appSettings = optionsMonitorData.CurrentValue;

            var parity = (Parity)appSettings.Parity;
            var stopBits = (StopBits)appSettings.StopBits;

            _serialPort = new SerialPort(appSettings.PortName, appSettings.BaudRate, parity, appSettings.DataBits, stopBits);

            _logger.LogInformation($"SerialPort setup: PortName {appSettings.PortName}; BaudRate {appSettings.BaudRate}; Parity {parity}; DataBits {appSettings.DataBits}; StopBits {stopBits}");
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SerialConsoleWorker ExecuteAsync started");
            _logger.LogInformation($"Working directory is {Directory.GetCurrentDirectory()}");

            Subscribe();
            _serialPort.Open();

            _logger.LogInformation("Listening to port...");

            while (true)
            {
                Thread.Sleep(int.MaxValue);
            }
        }

        public void Subscribe()
        {
            _serialPort.DataReceived += port_DataReceived;
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            var line = _serialPort.ReadLine();
            _logger.LogInformation(line);
        }
    }
}