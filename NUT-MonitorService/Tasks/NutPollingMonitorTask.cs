using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUTMonitor.Tasks
{
    public class NutPollingMonitorTask : TimeredMonitorTask
    {
        public NutPollingMonitorTask(UPS.UPS upsEngine) : base(upsEngine)
        {

        }

        private Task<string> _launchUpscProcess()
        {
            ProcessStartInfo upscParams = new ProcessStartInfo();

            upscParams.ArgumentList.Add("ippon");
            upscParams.FileName = "upsc";
            upscParams.RedirectStandardError = true;
            upscParams.RedirectStandardInput = true;
            upscParams.RedirectStandardOutput = true;
            upscParams.CreateNoWindow = true;

            var taskHolder = new TaskCompletionSource<string>();

            using Process upscProcess = new Process()
            {
                StartInfo = upscParams,
                EnableRaisingEvents = true
            };

            using StreamReader streamReader = upscProcess.StandardOutput;

            upscProcess.Exited += (snd, e) =>
            {
                taskHolder.SetResult(streamReader.ReadToEnd());
            };

            upscProcess.Start();

            return taskHolder.Task; 
        }

        protected override Task OnLaunch()
        {
            return Task.CompletedTask;
        }

        protected override async Task Tick()
        {
            using StringReader streamReader = new StringReader(await _launchUpscProcess());

            UPS.UPSMeasure measure = new UPS.UPSMeasure();
            string cLine;
            while ((cLine = streamReader.ReadLine()) != null)
            {
                string[] components = cLine.Split(':');

                if (components.Length != 2)
                    continue;

                switch (components[0].ToLower())
                {
                    case "battery.voltage.high":
                        measure.BatteryHighVolt = decimal.Parse(components[1].Trim());
                        break;
                    case "battery.voltage.low":
                        measure.BatteryMinVolt = decimal.Parse(components[1].Trim());
                        break;
                    case "battery.charge":
                        break;
                    case "battery.voltage":
                        measure.BatteryVolt = decimal.Parse(components[1].Trim());
                        break;
                    case "input.voltage":
                        measure.InputVoltage = decimal.Parse(components[1].Trim());
                        break;
                    case "output.voltage":
                        measure.OutputVolatge = decimal.Parse(components[1].Trim());
                        break;
                    case "ups.temperature":
                        measure.Temperature = decimal.Parse(components[1].Trim());
                        break;
                    case "ups.status":
                        string statusName = components[1].Trim();

                        measure.Status = UPS.UPSStatuses.Statuses.Where(st => st.StringName == statusName).FirstOrDefault();

                        if (measure.Status == null)
                            measure.Status = new UPS.UPSStatus() { StringName = statusName };

                        break;
                }
            } //while ((cLine = streamReader.ReadLine()) != null)

            await UpsEngine.ProcessMeasure(measure);
        }
    }
}
