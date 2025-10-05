using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.EventLog.DataBase.Model;
using GS.EventLog.ViewModel;
using GS.Interfaces;

namespace GS.EventLog.DataBase.Dal
{
    public partial class EvlContext
    {
        public IQueryable<DbEventLogItem> SelectEventLogItems(long? eventLogId, string result, string subject, string search,
                                                                                bool? lastDayChecked)
        {
            IQueryable<DbEventLogItem> deals = null;

            // LastDayChecked
            var lastDate = new DateTime().Date;
            var isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;
            if (isLastDayChecked)
            {
                var firstOrDefault = EventLogItems.OrderByDescending(i => i.EventLogItemID).FirstOrDefault();

                if (firstOrDefault == null)
                {
                    return null;
                }
                lastDate = firstOrDefault.DT.Date;
            }

            //var isHaveSearch = search.HasValue();
            var isResultValid = !string.IsNullOrWhiteSpace(result);
            if (!isResultValid) result = "";
            var isSubjectValid = !string.IsNullOrWhiteSpace(subject);
            if (!isSubjectValid) subject = "";
            var isSearchValid = !string.IsNullOrWhiteSpace(search);
            if (!isSearchValid) search = "";
            var isIdValid = eventLogId != null && eventLogId != 0;
            if (!isIdValid)
                eventLogId = 0;


            //else if (result.HasValue() && subject.HasValue())
            //{
            deals = EventLogItems
                .Where(ei => !isIdValid || ei.EventLogID == eventLogId)
                .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
                .Where(ei => (!isResultValid || ei.ResultCode.ToString() == result) &&
                             (!isSubjectValid || ei.Subject.ToString() == subject))
                .Where(ei => !isSearchValid || ei.Source.Contains(search) || ei.Entity.Contains(search) || ei.Operation.Contains(search))
                //.OrderByDescending(i => i.EventLogItemID)
                ;
            return deals;

        }

        public IEnumerable <EventLogView> GetEventLogItemsGrouped(bool? evl, bool? results, bool? subjects)
        {
            var qry = default(IEnumerable<EventLogView>);
            var isResultValid = results.HasValue && results == true;
            var isSubjectValid = subjects.HasValue && subjects == true;

            if (isResultValid && isSubjectValid)
            {
                qry = (from ei in EventLogItems
                    group ei by new
                    {
                        EvlId = ei.EventLogID,
                        EvlName = ei.EventLogID.ToString(),
                        Result = ei.ResultCode.ToString(),
                        Subject = ei.Subject.ToString(),
                    }
                    into g
                    select new EventLogView
                    {
                        Id = g.Key.EvlId,
                        Name = g.Key.EvlName,
                        Result = g.Key.Result,
                        Subject = g.Key.Subject,
                        Count = g.Count(),
                        FirstDate = g.Min(ei => ei.DT),
                        LastDate = g.Max(ei => ei.DT)
                    }
                    )
                    //.OrderBy(p => p.Name)
                    //.ThenBy(p => p.Result)
                    //.ThenBy(p => p.Subject)
                    ;

            }
            if (isResultValid && !isSubjectValid)
            {
                qry = (from ei in EventLogItems
                       group ei by new
                       {
                           EvlId = ei.EventLogID,
                           EvlName = ei.EventLogID.ToString(),
                           Result = ei.ResultCode.ToString(),
                       }
                           into g
                           select new EventLogView
                           {
                               Id = g.Key.EvlId,
                               Name = g.Key.EvlName,
                               Result = g.Key.Result,
                               Subject = "All",
                               Count = g.Count(),
                               FirstDate = g.Min(ei => ei.DT),
                               LastDate = g.Max(ei => ei.DT)
                           }
                    )
                    //.OrderBy(p => p.Name)
                    //.ThenBy(p => p.Result)
                    ;
            }
            if (!isResultValid && isSubjectValid)
            {
                qry = (from ei in EventLogItems
                       group ei by new
                       {
                           EvlId = ei.EventLogID,
                           EvlName = ei.EventLogID.ToString(),
                           Subject = ei.Subject.ToString(),
                       }
                           into g
                           select new EventLogView
                           {
                               Id = g.Key.EvlId,
                               Name = g.Key.EvlName,
                               Result = "All",
                               Subject = g.Key.Subject,
                               Count = g.Count(),
                               FirstDate = g.Min(ei => ei.DT),
                               LastDate = g.Max(ei => ei.DT)
                           }
                    )
                    //.OrderBy(p => p.Name)
                    //.ThenBy(p => p.Subject)
                    ;
            }
            if (!isResultValid && !isSubjectValid)
            {
                qry = (from ei in EventLogItems
                       group ei by new
                       {
                           EvlId = ei.EventLogID,
                           EvlName = ei.EventLogID.ToString(),

                       }
                           into g
                           select new EventLogView
                           {
                               Id = g.Key.EvlId,
                               Name = g.Key.EvlName,
                               Result = "All",
                               Subject = "All",
                               Count = g.Count(),
                               FirstDate = g.Min(ei => ei.DT),
                               LastDate = g.Max(ei => ei.DT)
                           }
                    )
                    //.OrderBy(p => p.Name)
                    ;
            }

            var evls = EventLogs.
                        Select( e => new EventLogView
                        {
                            Id = e.EventLogID,
                            Code = e.Code,
                            Name = e.Name
                        }).ToList();
            var j = qry.ToList()
                .Join(
                evls,
                ei=>ei.Id,
                ev=>ev.Id,
                (ei, ev) => new EventLogView
                {
                    Id = ev.Id,
                    Code = ev.Code,
                    Name = ev.Name,
                    Result = ei.Result,
                    Subject = ei.Subject,
                    Count = ei.Count,
                    FirstDate = ei.FirstDate,
                    LastDate = ei.LastDate
                });
            return j;
            //return j.ToList();
        }

        public IQueryable<EvlResult> GetResultCodes()
        {
            IQueryable<EvlResult> resultsQry = from ei in EventLogItems
                select ei.ResultCode;
            return resultsQry.Distinct();
        }
        public IQueryable<EvlSubject> GetSubjectCodes()
        {
            IQueryable<EvlSubject> subjectsQry = from ei in EventLogItems
                                            select ei.Subject;
            return subjectsQry.Distinct();
        }
        public IQueryable<string> GetEventLogCodes()
        {
            IQueryable<string> qry = from evl in EventLogs select evl.Code;
            return qry.Distinct();
        }
    }
}
