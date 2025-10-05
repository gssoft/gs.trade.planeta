using System;
using System.Collections.Generic;
using System.Linq;
using DDEInfo;
using FTFHelper;
using GS.Extension;
using GS.Interfaces.Dde;

namespace GS.Trade.Dde.TpSbscr
{
    public partial class Dde
    {
        private void DataPoked(object sender, DataPokeddEventArgs args)
        {
            if(args == null) return;
            if(IsProcessTaskInUse) ProcessTaskDataPoked.EnQueue(args);
            else DataPoked2(args);                   
        }
        // Mode = Table
        private void DataPoked2(DataPokeddEventArgs e)
        {
            try
            {
                var list = new List<string>();
                int ndxRow;
                var ndxRowMax = e.Cells.Length;
                for (ndxRow = 0; ndxRow < ndxRowMax; ndxRow++)
                {
                    var ndxCollMax = e.Cells[ndxRow].Length;
                    var strTemp = string.Empty;
                    int ndxColl;
                    for (ndxColl = 0; ndxColl < ndxCollMax; ndxColl++)
                    {
                        if (e.Cells[ndxRow][ndxColl] == null)
                        // Если значение ячейки типа "tdtBlank" - неопределенное (пустое) значение
                        {
                            //strTemp += "<null>; ";
                            continue;
                        }
                        //else 
                        if (e.Cells[ndxRow][ndxColl] is PREVIEWVALUE)
                        // Если значение ячейки типа "tdtSkip" - пропущенное значение
                        {
                            //strTemp += "<preview>; ";
                            continue;
                        }
                        if (e.Cells[ndxRow][ndxColl].GetType().Name == "Double")
                        {
                            var d = (double)e.Cells[ndxRow][ndxColl];
                            var dstr = d.ToString("G").Trim().Replace(" ", "").Replace(",", ".");
                            if (strTemp.HasValue())
                                strTemp += ";" + dstr;
                            else
                                strTemp = dstr;
                        }
                        else
                        {
                            if (strTemp.HasValue())
                                strTemp += ";" + e.Cells[ndxRow][ndxColl];
                            else
                                strTemp = e.Cells[ndxRow][ndxColl].ToString();
                        }
                    }
                    if (!strTemp.HasValue()) continue;

                    if (IsNeedTopicName && e.Topic.HasValue())
                        strTemp = string.Concat(e.Topic, ";", strTemp);

                    if (Mode == ChangesSendMode.Line)
                    {
                        if (IsEventHubInUse)
                        {   
                            var s = new MessageStr(e.Topic, strTemp);
                            EventHubStr.EnQueue(s);
                        }
                        else
                            LineChangesSendAction?.Invoke(strTemp);
                    }
                    else list.Add(strTemp);
                }
                if (list.Count <= 0) return;

                if (Mode == ChangesSendMode.Table && IsNeedTopicNameInTable && e.Topic.HasValue())
                    list.Insert(0, $"{Code}.{e.Topic}");
                if (IsEventHubInUse)
                {
                    var s = new MessageListStr(list);
                    EventHubLstStr.EnQueue(s);
                }
                else
                    TableChangesSendAction.Invoke(list);
            }
            catch (Exception ex)
            {
                SendException(ex);
            }
        }
    }
}
