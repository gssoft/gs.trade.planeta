namespace GS.WorkTasks
{
    public enum WorkTaskStatus
    {
        Unknown = 0,
        Started = 1,
        Starting = 2,
        Stopped = 3,
        Stopping = 4,
        TryToStart = 5,
        TryToStop = 6,
        TryToCreate = 7,
        Completed = 8,
        Working = 9,
        Created = 10,
        Finished = 11
    }
}
