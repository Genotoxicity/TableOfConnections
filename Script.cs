using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KSPE3Lib;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;


namespace KSP.E3.TableOfConnections
{
    class Script
    {
        private E3ApplicationInfo applicationInfo;
        private string configurationFile;

        public UI UI;

        public Script()
        {
            applicationInfo = new E3ApplicationInfo();
            UI = new UI(applicationInfo, new Action<ScriptType>(Start));
            configurationFile = Assembly.GetExecutingAssembly().GetName().Name + ".scf";
            //FontSettingsWindow w = new FontSettingsWindow(applicationInfo.ProcessId);
            //w.Show();
        }

        private void Start(ScriptType scriptType)
        {
            UI.Cursor = System.Windows.Input.Cursors.Wait;
            Settings settings = new Settings();
            settings.electricSchemeTypeCode = 0;
            settings.firstPageFormat = "Формат А3 лист 1";
            settings.firstPageHeaderLineX = 0;
            settings.firstPageHeaderLineY = -12;
            settings.firstPageUttermostPositionY = -225;
            settings.subsequentPageFormat = "Формат А3_каб.журнал";
            settings.subsequentPageHeaderLineX = 0;
            settings.subsequentPageHeaderLineY = 287;
            settings.subsequentPageUttermostPositionY = 20;
            settings.lineFont = new E3Font();
            settings.headerFont = new E3Font(height: 3.5);
            settings.separatingSymbols = "?!.:, -";
            settings.verticalLineWidth = 0.5;
            settings.horizontalLineWidth = 0.2;
            settings.headerVerticalLineWidth = 0.5;
            settings.headerHorizontalLineWidth = 0.5;
            settings.bottomLineWidth = 0.5;
            settings.sheetTypeAttribute = "marka2";
            SaveSettings(settings);
            
            E3Project project = new E3Project(applicationInfo.ProcessId);
            Sheet sheet = project.GetSheetById(0);

            List<ConnectionInfo> connectionInfos;

            if (scriptType == ScriptType.ByConnections)
                connectionInfos = GetConnectionInfosByConnections(project, sheet, settings.electricSchemeTypeCode);
            else
                connectionInfos = GetConnectionInfosByCoresAndWires(project);

            connectionInfos.RemoveAll(ci => ci.assignmentFrom != ci.assignmentTo || String.IsNullOrEmpty(ci.assignmentFrom)); // удаляем все соединения, не идущие внутри одного шкафа
            ConnectionInfoComparer comparer = new ConnectionInfoComparer();
            connectionInfos.Sort(comparer);

            StringSeparator separator = new StringSeparator(settings.separatingSymbols.ToCharArray(), settings.lineFont, project.GetTextById(0));
            LineTemplate lineTemplate = new LineTemplate(new List<double>() { 35, 45, 45, 45, 45, 45, 45, 45, 45 }, settings.lineFont, 8, settings.verticalLineWidth, settings.horizontalLineWidth);
            LineTemplate headerLineTemplate = new LineTemplate(new List<double>() { 35, 45, 45, 45, 45, 45, 45, 45, 45 }, settings.headerFont, 8, settings.headerVerticalLineWidth, settings.headerHorizontalLineWidth);
            List<string> headerColumnNames = new List<string>(9){"Имя цепи", "От \"Места\"", "От \"Поз. обозначения\"", "От \"Вывода\"", "К \"Месту\"", "К \"Поз. обозначению\"", "К \"Выводу\"", "Тип провода", "Примечание"};
            Dictionary<string, Table> assignmentTables = new Dictionary<string, Table>();
            foreach (ConnectionInfo info in connectionInfos)
            {
                if (!assignmentTables.ContainsKey(info.assignmentFrom))
                {
                    HeaderText headerText = new HeaderText("Таблица соединений шкафа "+info.assignmentFrom, settings.headerFont, settings.headerFont.height + 1);
                    TablePageTemplate firstPageTemplate = new TablePageTemplate(settings.firstPageFormat, settings.firstPageHeaderLineX, settings.firstPageHeaderLineY, settings.firstPageUttermostPositionY, headerText);
                    TablePageTemplate subsequentPageTemplate = new TablePageTemplate(settings.subsequentPageFormat, settings.subsequentPageHeaderLineX, settings.subsequentPageHeaderLineY, settings.subsequentPageUttermostPositionY);
                    assignmentTables.Add(info.assignmentFrom, new Table(project, headerLineTemplate, headerColumnNames, lineTemplate, settings.bottomLineWidth, separator, firstPageTemplate, subsequentPageTemplate));
                }
                assignmentTables[info.assignmentFrom].AddLine(new List<string>(8) { info.signal, info.locationFrom, info.deviceFrom, info.pinFrom, info.locationTo, info.deviceTo, info.pinTo, info.type });
            }
            foreach (string key in assignmentTables.Keys)
            {
                assignmentTables[key].AddFinalGraphicLine();
                foreach (int id in assignmentTables[key].SheetIds)
                {
                    sheet.Id = id;
                    sheet.SetAttribute(settings.sheetTypeAttribute, key);
                }
            }
            project.Release();
            UI.Cursor = System.Windows.Input.Cursors.Arrow;                                                                                                                                                                                                                                                
        }

        private static List<ConnectionInfo> GetConnectionInfosByConnections(E3Project project, Sheet sheet, int electricShemeTypeCode)
        {
            List<ConnectionInfo> connectionInfos = new List<ConnectionInfo>();
            Core core = project.GetCoreById(0);
            WireCore wire = project.GetWireCoreById(0);
            NormalDevice normalDevice = project.GetDeviceById(0);
            DevicePin devicePin = project.GetDevicePinById(0);
            Connection connection = project.GetConnectionById(0);
            HashSet<int> electricSchemeSheetIds = GetElectricSchemeSheetIds(project, sheet, electricShemeTypeCode);
            List<int> connectionIds = project.ConnectionIds;
            foreach (int connectionId in connectionIds)
            {
                connection.Id = connectionId;

                List<int> pinIds = new List<int>(connection.PinIds.Count);
                foreach (int pinId in connection.PinIds)
                {
                    devicePin.Id = pinId;
                    pinIds.Add(devicePin.Id);
                }

                pinIds = pinIds.Distinct<int>().ToList<int>();
                pinIds.RemoveAll(id => { devicePin.Id = id; return !electricSchemeSheetIds.Contains(devicePin.SheetId); });

                Dictionary<string, List<int>> pinsBySignals = new Dictionary<string, List<int>>();
                foreach(int pinId in pinIds)
                {
                    devicePin.Id = pinId;
                    string signal = String.Intern(devicePin.SignalName);
                    if (!pinsBySignals.ContainsKey(signal))
                        pinsBySignals.Add(signal, new List<int>());
                    pinsBySignals[signal].Add(pinId);
                }

                foreach (string signal in pinsBySignals.Keys)
                {
                    pinIds = pinsBySignals[signal];
                    if (pinIds.Count > 1)
                        for(int i=1; i<pinIds.Count; i++)
                            connectionInfos.Add(new ConnectionInfo(pinIds[i - 1], pinIds[i], signal, normalDevice, devicePin, core, wire));
                }
            }
            return connectionInfos;
        }

        private static HashSet<int> GetElectricSchemeSheetIds(E3Project project, Sheet sheet, int electricShemeTypeCode)
        {
            List<int> sheetIds = project.SheetIds;
            HashSet<int> electricSchemeSheetIds = new HashSet<int>();
            foreach (int sheetId in sheetIds)
            {
                sheet.Id = sheetId;
                if (sheet.IsTypeOf(electricShemeTypeCode))
                    electricSchemeSheetIds.Add(sheetId);
            }
            return electricSchemeSheetIds;
        }

        private static List<ConnectionInfo> GetConnectionInfosByCoresAndWires(E3Project project)
        {
            List<ConnectionInfo> connectionInfos = new List<ConnectionInfo>();
            CableDevice cable = project.GetCableById(0);
            Core core = project.GetCoreById(0);
            WireCore wire = project.GetWireCoreById(0);
            NormalDevice normalDevice = project.GetDeviceById(0);
            DevicePin devicePin = project.GetDevicePinById(0);
            List<int> cableIds = project.CableIds;
            List<int> wireIds = project.WireIds;
            foreach (int cableId in cableIds)
            {
                cable.Id = cableId;
                foreach (int coreId in cable.CoreIds)
                {
                    core.Id = coreId;
                    connectionInfos.Add(new ConnectionInfo(core, normalDevice, devicePin, cable.Name, cable.ComponentName));
                }
            }
            foreach (int wireId in wireIds)
            {
                wire.Id = wireId;
                connectionInfos.Add(new ConnectionInfo(wire, normalDevice, devicePin, wire.Name, wire.WireType));
            }
            return connectionInfos;
        }

        private Settings GetSettings()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(configurationFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            Settings settings = (Settings)formatter.Deserialize(stream);
            stream.Close();
            return settings;
        }

        private void SaveSettings(Settings settings)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(configurationFile, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, settings);
            stream.Close();
        }
    }
}
