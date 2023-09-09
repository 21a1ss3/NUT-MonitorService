using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUTMonitor.Tasks
{
    public class DatabaseMonitorTask : MonitorTask
    {
        public DatabaseMonitorTask(UPS.UPS upsEngine) : base(upsEngine)
        {
        }

        private static bool _walletInit = true;


        protected override async Task InternalLaunch()
        {
            if (_walletInit)
            {
                OracleConfiguration.TnsAdmin = System.IO.Path.Combine(AppContext.BaseDirectory, "Sensetive/DB/Wallet");
                OracleConfiguration.WalletLocation = System.IO.Path.Combine(AppContext.BaseDirectory, "Sensetive/DB/Wallet");

                _walletInit = false;
            }

            UpsEngine.BeginNewMonitorSession();

            DB.UpsStatus.MonitorSession dbSession = new DB.UpsStatus.MonitorSession();
            dbSession.SessionId = UpsEngine.MonitorSessionId;
            dbSession.UPSId = UpsEngine.UpsId;

            dbSession.ConfiguredInterval = Settings.SettingsHolder.Settings.UPS.ProbingInterval;

            {
                using (var dbModel = new DB.UpsStatus.UPSMonDBModel())
                {
                    dbModel.Add(dbSession);
                    await dbModel.SaveChangesAsync();
                }
            }

            while (IsRun)
            {
                await UpsEngine.DbProcessEvent.WaitAsync();

                dbSession = null;

                foreach (var measure in UpsEngine.MeasuresToProcess)
                {
                    using (var dbModel = new DB.UpsStatus.UPSMonDBModel())
                    {
                        if (!measure.ProcessingStatus.FixedInDb)
                        {
                            try
                            {
                                if (dbSession == null)
                                    dbSession = dbModel.MonitorSessions.Where(ms => ms.SessionId == UpsEngine.MonitorSessionId).First();

                                measure.ProcessingStatus.DbProcessingAttempts++;

                                //TODO: treshold number checkout

                                UPS.UPSMeasureDifference diff;

                                if (UpsEngine.LastDbFixedMeasure != null)
                                    diff = UpsEngine.LastDbFixedMeasure.GetDifferenceWith(measure);
                                else
                                {
                                    diff = new UPS.UPSMeasureDifference();

                                    diff.InputVoltage = measure.InputVoltage;
                                    diff.OutputVolatge = measure.OutputVolatge;
                                    diff.Status = measure.Status;
                                    diff.Temperature = measure.Temperature;
                                    diff.BatteryCharge = measure.BatteryCharge;
                                    diff.BatteryVolt = measure.BatteryVolt;

                                    dbSession.Launched = measure.Timestamp;
                                }

                                if (dbSession.LastUpdate < measure.Timestamp)
                                    dbSession.LastUpdate = measure.Timestamp;

                                if (dbSession.BatteryHighVolt != measure.BatteryHighVolt)
                                    dbSession.BatteryHighVolt = measure.BatteryHighVolt;

                                if (dbSession.BatteryMinVolt != measure.BatteryMinVolt)
                                    dbSession.BatteryMinVolt = measure.BatteryMinVolt;

                                if (!diff.IsEmpty)
                                {
                                    DB.UpsStatus.UpsMesurement dbUpsMesurement = new DB.UpsStatus.UpsMesurement();

                                    dbUpsMesurement.Timestamp = DateTime.UtcNow;

                                    dbUpsMesurement.BatteryCharge = diff.BatteryCharge;
                                    dbUpsMesurement.BatteryVolt = diff.BatteryVolt;
                                    dbUpsMesurement.InputVoltage = diff.InputVoltage;
                                    dbUpsMesurement.OutputVolatge = diff.OutputVolatge;
                                    dbUpsMesurement.Temperature = diff.Temperature;

                                    dbUpsMesurement.MonitorSession = dbSession;

                                    dbModel.UpsMesurements.Add(dbUpsMesurement);
                                }

                                await dbModel.SaveChangesAsync();

                                await UpsEngine.MarkAsDBProcessed(measure);
                            }
                            catch (Exception ex)
                            {
                                measure.ProcessingStatus.ProcessDBFailure(ex);
                                break;
                            }
                        } // if (!measure.ProcessingStatus.FixedInDb)
                    } //using (var dbModel = new DB.UpsStatus.UPSMonDBModel())
                } // foreach (var measure in UpsEngine.MeasuresToProcess)
            }
        }
    }
}
