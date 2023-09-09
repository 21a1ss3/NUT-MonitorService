using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NUTMonitor.Esxi
{
    public abstract class EsxiCliProvider : EsxiProvider
    {

        protected abstract string ExecuteCommand(string command);

        public override bool[] GetActualStatuses<T>(T vm)
        {
            if (vm == null)
                throw new ArgumentNullException(nameof(vm));

            if (vm.Count == 0)
                return new bool[0];

            bool[] results = new bool[vm.Count];


            string commandOutput = ExecuteCommand("vim-cmd vmsvc/getallvms");

            StringReader reader = new StringReader(commandOutput);
            reader.ReadLine();//skip header

            //state-machine for parse
            {
                int status = 0;
                int field = 0;
                string cBuffer = string.Empty;
                string cLine;

                string vmVersion;

                //statuses:
                // 0 - skipping spaces
                // 1 - reading field

                while ((cLine = reader.ReadLine()) != null)
                {
                    int vmId = 0;
                    string vmName = string.Empty;
                    string vmDatastore = string.Empty;
                    string vmFilePath = string.Empty;
                    string vmGuestOs = string.Empty;

                    foreach (char pChar in cLine)
                    {
                        switch (status)
                        {
                            case 0:
                                if (!char.IsWhiteSpace(pChar))
                                {
                                    cBuffer = new string(pChar, 1);
                                    status = 1;
                                }
                                break;
                            case 1:
                                if (char.IsWhiteSpace(pChar))
                                {
                                    status = 0;

                                    switch (field)
                                    {
                                        case 0:
                                            vmId = int.Parse(cBuffer);
                                            break;
                                        case 1:
                                            vmName = cBuffer;
                                            break;
                                        case 2:
                                            vmDatastore = cBuffer;
                                            break;
                                        case 3:
                                            vmFilePath = cBuffer;
                                            break;
                                        case 4:
                                            vmGuestOs = cBuffer;
                                            break;
                                        case 5:
                                            vmVersion = cBuffer;
                                            break;
                                    }

                                    field++;
                                    cBuffer = string.Empty;
                                }
                                else                                
                                    cBuffer += pChar;
                                
                                break;
                        }
                    }

                    foreach (var vmDesc in vm)
                        if (vmDesc.VMName == vmName)
                        {
                            vmDesc.EsxiId = vmId;
                            vmDesc.Datastore = vmDatastore;
                            vmDesc.Path = vmFilePath;
                        }
                } //while ((cLine = reader.ReadLine()) != null)

            }

            return results;
        }//public override bool[] GetActualStatuses<T>(T vm)
    }
}
