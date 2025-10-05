namespace GS.Ftp
{
    public enum FtpOperationStatus
    {
        FileNotFound = -1,
        BadHash = -2,
        Exception = -3,
        NullContent = -4,
        DeleteFileFailure = -5,
        UpLoadFailure = -6,
        MakeDirectoryFailure = -7,
        ListDirectoryDetailsFailure = -8,
        RemoveDirectoryFailure = -9,
        ExtraFileDetected = -10,
        ExtraDirDetected = -11,
        DirNotFound = -12,
        RemoveDirectoryTreeFailure = -13,
        GetDirectoryFailure = -14,
        GetDirectoryListFailure = -15,

        UnDefined = 0,

        Ok = 1
    }
    public enum FtpOperation
    {
        Undefined = 0,
        DownloadFile = 1, UploadFile = 2, DeleteFile = 3, GetFile = 8,
        GetDirectoryList = 4, RemoveDirectory = 5, MakeDirectory = 6, RemoveDirectoryTree = 7, GetDirectory = 9,
        ExtraFileScan = 10, ExtraDirScan = 11
    }

    // Parsing FtpGetDirectoryDetailsList
    public enum DirMemberEnum { File = 0, Dir = 1, CurrentDir = 2, ParentDir = 3 }

    public enum HashStatus
    {
        Undefined = 0, Bad = -1, Ok = 1
    }
}