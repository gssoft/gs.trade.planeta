using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDEInfo;
using FTFHelper;
using GS.Extension;
using GS.Interfaces.Dde;

namespace GS.Trade.Dde
{
    public partial class Dde
    {
        private void DataPoked1(object sender, DataPokeddEventArgs e)
        {
            try
            {
                int ndxRow;
                var ndxRowMax = e.Cells.Length;
                for (ndxRow = 0; ndxRow < ndxRowMax; ndxRow++)
                {
                    var ndxCollMax = e.Cells[ndxRow].Length;
                    var strTemp = String.Empty;
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
                        // else
                        // Для остальных типов 
                        //{
                        if (strTemp.HasValue())
                            strTemp += ";" + e.Cells[ndxRow][ndxColl];
                        else
                            strTemp = (string)e.Cells[ndxRow][ndxColl];
                        //}
                    }
                    if (!strTemp.HasValue())
                        continue;

                    if (IsNeedTopicName && e.Topic.HasValue())
                        strTemp = string.Concat(e.Topic, ";", strTemp);

                    if (IsProcessTaskInUse)
                        ProcessTask1?.EnQueue(strTemp);
                    else
                        LineChangesSendAction?.Invoke(strTemp);
                }
            }
            catch (Exception ex)
            {
                SendException(ex);
            }
        }
        // Mode = Table
        private void DataPoked2(object sender, DataPokeddEventArgs e)
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
                            // Evlm2(EvlResult.INFO, EvlSubject.DIAGNOSTIC, ParentTypeName, TypeName,
                            //"OnPoked2", $"{(e.Cells[ndxRow][ndxColl]).GetType().Name} = {d}" ,
                            //(e.Cells[ndxRow][ndxColl]).GetType().FullName);

                            if (strTemp.HasValue())
                                strTemp += ";" + d.ToString("G").Trim().Replace(" ", "").Replace(",", "."); //.Replace(@"\t", "");
                            else
                                strTemp = d.ToString("G").Trim().Replace(" ", "").Replace(",", "."); //.Replace(@"\t","");
                        }
                        else
                        {
                            if (strTemp.HasValue())
                                strTemp += ";" + e.Cells[ndxRow][ndxColl];
                            else
                                strTemp = e.Cells[ndxRow][ndxColl].ToString();
                        }
                        //if (strTemp.HasValue())
                        //    strTemp += ";" + e.Cells[ndxRow][ndxColl];
                        //else
                        //    strTemp = (string)e.Cells[ndxRow][ndxColl];
                    }
                    if (!strTemp.HasValue())
                        continue;

                    if (IsNeedTopicName && e.Topic.HasValue())
                        strTemp = string.Concat(e.Topic, ";", strTemp);

                    list.Add(strTemp);
                }
                if (list.Count <= 0)
                    return;
                // Topic Name at the Top of the List
                if (Mode == ChangesSendMode.Table && IsNeedTopicNameInTable && e.Topic.HasValue())
                    list.Insert(0, $"{Code}.{e.Topic}");

                if (IsProcessTaskInUse)
                    ProcessTask2?.EnQueue(list);
                else
                    TableChangesSendAction?.Invoke(list);
            }
            catch (Exception ex)
            {
                SendException(ex);
            }
        }
        private void OnPokedTopic(string topic, string data)
        {
            var top = Collection.GetByKey(topic);
            if (top != null)
            {
                if (top.Action != null)
                    top.Action(data);
                else
                {
                    if (DefaultCallBack != null)
                        top.Action = DefaultCallBack;
                    else
                        throw new NullReferenceException("Dde DefaultCallBack is Null");
                }
            }
            else
                RegisterTopic(topic);
        }
        private void OnPokedTopic2(string topic, string data)
        {
            var top = Collection.GetByKey(topic);
            if (top != null)
                top.Action(data);
            else
            {
                top = RegisterTopic(topic);
                top.Action(data);
            }
        }
        private void CheckCallBacks()
        {
            if (DefaultCallBack == null)
                throw new NullReferenceException("DefaultCallBack == Null");

            foreach (var i in Collection.Items.Where(i => i.Action == null))
            {
                i.Action = DefaultCallBack;
            }
        }
    }
}
