namespace KickChatRecorder.Api.Paging
{
    public abstract class RequestParameters
    {
        private string? _pageinfo;
        private int _pageSize = 2;
        public int PageSize {
            get { return _pageSize; }
            set { _pageSize = value; }
        }
        public string? PagingInfo
        {
            get { return _pageinfo; }
            set { _pageinfo = value; }
        }

    }
}
