
using Google.Apis.Upload;
using static Google.Apis.Drive.v3.FilesResource;

namespace MoveSharp.GoogleDrive
{
    public class GoogleDriveUploadStatus : Models.UploadStatus
    {
        private IUploadProgress _progress;
        private CreateMediaUpload _mediaUpload;

        internal ResumableUpload MediaUpload => _mediaUpload;

        public GoogleDriveUploadStatus(string uri, CreateMediaUpload mediaUpload)
        {
            Id = uri;
            _mediaUpload = mediaUpload;
            _progress = _mediaUpload.GetProgress();
            Status = _progress.Status.ToString();
        }

        public override bool IsCompleted
        {
            get => _progress.Status == UploadStatus.Completed;
            protected set { }
        }

        public string ActivityUrl
        {
            get
            {
                return IsCompleted ? _mediaUpload.ResponseBody.WebViewLink : "";
            }
        }
    }
}
