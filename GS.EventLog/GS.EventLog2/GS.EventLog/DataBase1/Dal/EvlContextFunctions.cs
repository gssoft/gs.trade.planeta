using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GS.EventLog.DataBase1.Model;
using GS.EventLog.ViewModel;
using GS.Interfaces;

namespace GS.EventLog.DataBase1.Dal
{
    public partial class EvlContext1
    {
        public async Task<KeyValuePair<bool, DateTime>> GetLastDateAsync(long? evlId)
        {
            var dt = default (DateTime);

            if (! await EventLogItems.AnyAsync())
                return new KeyValuePair<bool,DateTime>(false, dt.Date);
            
            if (evlId == null)
            {
                dt = await EventLogItems
                    .AsNoTracking()
                    .Select(i=>i.DT)
                    .MaxAsync();
            }
            else
            {
                dt = await EventLogItems
                    .Where(i=>i.EventLogID == evlId)
                    .AsNoTracking()
                    .Select(i=>i.DT)
                    .MaxAsync();
            }
            return new KeyValuePair<bool, DateTime>(true, dt.Date);
        }

        public IQueryable<DbEventLogItem> SelectEventLogItems(long? eventLogId, string result, string subject, string search,
                                                                                bool? lastDayChecked)
        {
            var deals = default(IQueryable<DbEventLogItem>);

            // LastDayChecked
            var lastDate = new DateTime().Date;
            var isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;

            var hasItems = EventLogItems.Any();
            if (isLastDayChecked && hasItems)
            {
                var firstOrDefault = EventLogItems.AsNoTracking().
                    OrderByDescending(i => i.EventLogItemID).FirstOrDefault();

                //if (firstOrDefault == null)
                //{
                //    return null;
                //}
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
                .AsNoTracking()
                //.OrderByDescending(i => i.EventLogItemID)
                ;
            return deals;
        }

        public async Task<IQueryable<DbEventLogItem>> SelectEventLogItemsAsync(long? eventLogId, string result, string subject, string search,
                                                                               bool? lastDayChecked)
        {
            var deals = default(IQueryable<DbEventLogItem>);

            // LastDayChecked
            var isLastDayChecked = lastDayChecked.HasValue && lastDayChecked == true;

            var dt = await GetLastDateAsync(eventLogId);

            if (!dt.Key)
                return EventLogItems.AsNoTracking();

            var lastDate = dt.Value;

            var isResultValid = !string.IsNullOrWhiteSpace(result);
            if (!isResultValid) result = "";
            var isSubjectValid = !string.IsNullOrWhiteSpace(subject);
            if (!isSubjectValid) subject = "";
            var isSearchValid = !string.IsNullOrWhiteSpace(search);
            if (!isSearchValid) search = "";
            var isIdValid = eventLogId != null && eventLogId != 0;
            if (!isIdValid)
                eventLogId = 0;

            deals = EventLogItems
                .Where(ei => !isIdValid || ei.EventLogID == eventLogId)
                .Where(p => !isLastDayChecked || DbFunctions.TruncateTime(p.DT) == lastDate)
                .Where(ei => (!isResultValid || ei.ResultCode.ToString() == result) &&
                             (!isSubjectValid || ei.Subject.ToString() == subject))
                .Where(ei => !isSearchValid || ei.Source.Contains(search) || ei.Entity.Contains(search) || ei.Operation.Contains(search))
                .AsNoTracking()
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
                qry = (from ei in EventLogItems.AsNoTracking()
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
                qry = (from ei in EventLogItems.AsNoTracking()
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
                qry = (from ei in EventLogItems.AsNoTracking()
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
                qry = (from ei in EventLogItems.AsNoTracking()
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
        public IQueryable<EventLogView> GetEventLogItemsGroupedQ(bool? evl, bool? results, bool? subjects)
        {
            var qry = default(IQueryable<EventLogView>);

            var isResultValid = results.HasValue && results == true;
            var isSubjectValid = subjects.HasValue && subjects == true;

            if (isResultValid && isSubjectValid)
            {
                qry = (from ei in EventLogItems.AsNoTracking()
                       group ei by new
                       {
                           EvlId = ei.EventLogID,
                           EvlName = ei.DbEventLog.Code,
                           Result = ei.ResultCode.ToString(),
                           Subject = ei.Subject.ToString(),
                       }
                           into g
                           select new EventLogView
                           {
                               Id = g.Key.EvlId,
                               Code = g.Key.EvlName,
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
                qry = (from ei in EventLogItems.AsNoTracking()
                       group ei by new
                       {
                           EvlId = ei.EventLogID,
                           EvlName = ei.DbEventLog.Code,
                           Result = ei.ResultCode.ToString(),
                       }
                           into g
                           select new EventLogView
                           {
                               Id = g.Key.EvlId,
                               Code = g.Key.EvlName,
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
                qry = (from ei in EventLogItems.AsNoTracking()
                       group ei by new
                       {
                           EvlId = ei.EventLogID,
                           EvlName = ei.DbEventLog.Code,
                           Subject = ei.Subject.ToString(),
                       }
                           into g
                           select new EventLogView
                           {
                               Id = g.Key.EvlId,
                               Code = g.Key.EvlName,
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
                qry = (from ei in EventLogItems.AsNoTracking()
                       group ei by new
                       {
                           EvlId = ei.EventLogID,
                           EvlName = ei.DbEventLog.Code,

                       }
                           into g
                           select new EventLogView
                           {
                               Id = g.Key.EvlId,
                               Code = g.Key.EvlName,
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
            return qry;

            //var evls = EventLogs.
            //    Select(e => new EventLogView
            //    {
            //        Id = e.EventLogID,
            //        Code = e.Code,
            //        Name = e.Name
            //    }); //.ToList();
            //var j = qry // .ToList()
            //    .Join(
            //    evls,
            //    ei => ei.Id,
            //    ev => ev.Id,
            //    (ei, ev) => new EventLogView
            //    {
            //        Id = ev.Id,
            //        Code = ev.Code,
            //        Name = ev.Name,
            //        Result = ei.Result,
            //        Subject = ei.Subject,
            //        Count = ei.Count,
            //        FirstDate = ei.FirstDate,
            //        LastDate = ei.LastDate
            //    });
            //return j;
            //return j.ToList();
        }
        public IQueryable<EventLogView> GetEventLogItemsGroupedQ(int? evlId, bool? evl, bool? results, bool? subjects)
        {
            var qry = default(IQueryable<EventLogView>);

            var isResultValid = results.HasValue && results == true;
            var isSubjectValid = subjects.HasValue && subjects == true;

            IQueryable<DbEventLogItem> qryDbItems = evlId != null 
                ? EventLogItems.Where(i => i.EventLogID == evlId).AsNoTracking() 
                : EventLogItems.AsNoTracking();

            if (isResultValid && isSubjectValid)
            {
                qry = (from ei in qryDbItems   // from ei in EventLogItems
                       group ei by new
                       {
                           EvlId = ei.EventLogID,
                           EvlName = ei.DbEventLog.Code,
                           Result = ei.ResultCode.ToString(),
                           Subject = ei.Subject.ToString(),
                       }
                           into g
                           select new EventLogView
                           {
                               Id = g.Key.EvlId,
                               Code = g.Key.EvlName,
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
                qry = ( from ei in qryDbItems          //      from ei in EventLogItems
                       group ei by new
                       {
                           EvlId = ei.EventLogID,
                           EvlName = ei.DbEventLog.Code,
                           Result = ei.ResultCode.ToString(),
                       }
                           into g
                           select new EventLogView
                           {
                               Id = g.Key.EvlId,
                               Code = g.Key.EvlName,
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
                qry = (from ei in qryDbItems   // from ei in EventLogItems
                       group ei by new
                       {
                           EvlId = ei.EventLogID,
                           EvlName = ei.DbEventLog.Code,
                           Subject = ei.Subject.ToString(),
                       }
                           into g
                           select new EventLogView
                           {
                               Id = g.Key.EvlId,
                               Code = g.Key.EvlName,
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
                qry = ( from ei in qryDbItems // from ei in EventLogItems
                       group ei by new
                       {
                           EvlId = ei.EventLogID,
                           EvlName = ei.DbEventLog.Code,

                       }
                           into g
                           select new EventLogView
                           {
                               Id = g.Key.EvlId,
                               Code = g.Key.EvlName,
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
            return qry;
       }

        // To Remove
        #region ToRemove
        public IQueryable<EvlResult> GetResultCodes()
        {
            IQueryable<EvlResult> resultsQry = from ei in EventLogItems.AsNoTracking()
                select ei.ResultCode;
            return resultsQry.Distinct();
        }
        public IQueryable<EvlSubject> GetSubjectCodes()
        {
            IQueryable<EvlSubject> subjectsQry = from ei in EventLogItems.AsNoTracking()
                                            select ei.Subject;
            return subjectsQry.Distinct();
        }
        public IQueryable<string> GetEventLogCodes()
        {
            IQueryable<string> qry = from evl in EventLogs.AsNoTracking() select evl.Code;
            return qry.Distinct();
        }
        #endregion


        public async Task<IEnumerable<string>> GetEventLogCodesAsync()
        {
            return await EventLogs.AsNoTracking().Select(e => e.Code + "@" + e.EventLogID).ToListAsync();
        }
        public async Task<IEnumerable<KeyValuePair<long,string>>> GetEventLogPairAsync()
        {
            return await EventLogs.AsNoTracking()
                .Select(e => new KeyValuePair<long, string>(e.EventLogID,e.Code)).ToListAsync();
        }
        public async Task<IEnumerable<dynamic>> GetEventLogDinamicAsync()
        {
            return await EventLogs.AsNoTracking()
                .Select(e => new {ID = e.EventLogID, Code = e.Code}).ToListAsync(); }
        }
    
}
