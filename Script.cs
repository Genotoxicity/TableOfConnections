using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KSPE3Lib;
using System.Text.RegularExpressions;

namespace KSP.E3.TableOfConnections
{
    class Script
    {
        private Project project; 

        public UI UI {get; private set;}

        public Script()
        {
            project = new Project();
            UI = new UI(project, new Action(Start));
        }

        [STAThread]
        private void Start()
        {
            project.Seize();
            List<CableDevice> cables = project.GetCables();
            List<WireCore> wires = project.GetWires();
            List<ConnectionInfo> infos = new List<ConnectionInfo>();
            foreach (CableDevice cable in cables)
                foreach (CableCore core in cable.Cores)
                    infos.Add(new ConnectionInfo(project, core, cable.Name, cable.Component));  
            foreach (WireCore wire in wires)
                infos.Add(new ConnectionInfo(project, wire, wire.Name, wire.WireType));
            infos.RemoveAll(ci => ci.assignmentFrom != ci.assignmentTo || String.IsNullOrEmpty(ci.assignmentFrom)); // удаляем все соединения, не идущие внутри одного шкафа
            string firstFormt = "Формат А3 лист 1";
            string subsequentFormat = "Формат А3_каб.журнал";
            E3Font lineFont = new E3Font();
            StringSeparator separator = new StringSeparator("?!.:, -".ToCharArray(), lineFont, project.GetTextById(0));
            LineTemplate lineTemplate = new LineTemplate(new List<double>() { 35, 45, 45, 45, 45, 45, 45, 45, 45 }, lineFont, 8, 0.5, 0.2);
            E3Font headerFont = new E3Font(height: 3.5);
            LineTemplate headerLineTemplate = new LineTemplate(new List<double>() { 35, 45, 45, 45, 45, 45, 45, 45, 45 }, headerFont, 8, 0.5, 0.5);
            List<string> headerColumnNames = new List<string>(9){"Имя цепи", "От \"Места\"", "От \"Поз. обозначения\"", "От \"Вывода\"", "К \"Месту\"", "К \"Поз. обозначению\"", "К \"Выводу\"", "Тип провода", "Примечание"};
            Dictionary<string, Table> assignmentTables = new Dictionary<string, Table>();
            ConnectionInfoComparer comparer = new ConnectionInfoComparer();
            infos.Sort(comparer);
            foreach (ConnectionInfo info in infos)
            {
                if (!assignmentTables.ContainsKey(info.assignmentFrom))
                {
                    HeaderText headerText = new HeaderText("Таблица соединений шкафа "+info.assignmentFrom, headerFont, headerFont.height + 1);
                    TablePageTemplate firstPageTemplate = new TablePageTemplate(firstFormt, 0, -12, -225, headerText);
                    TablePageTemplate subsequentPageTemplate = new TablePageTemplate(subsequentFormat, 0, 287, 20);
                    assignmentTables.Add(info.assignmentFrom, new Table(project, headerLineTemplate, headerColumnNames, lineTemplate, 0.5, separator, firstPageTemplate, subsequentPageTemplate));
                }
                assignmentTables[info.assignmentFrom].AddLine(new List<string>(8) { info.signal, info.locationFrom, info.deviceFrom, info.pinFrom, info.locationTo, info.deviceTo, info.pinTo, info.type });
            }
            string markaAttribute = "marka2";
            foreach (string key in assignmentTables.Keys)
            {
                assignmentTables[key].AddFinalGraphicLine();
                foreach (int id in assignmentTables[key].SheetIds)
                {
                    Sheet sheet = project.GetSheetById(id);
                    sheet.SetAttribute(markaAttribute, key);
                }
            }
            project.Release();                                                                                                                                                                                                                                                                                                              
        }

    }
}
