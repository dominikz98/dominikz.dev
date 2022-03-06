namespace dominikz.Endpoints
{
    public abstract class Request<T>
    {
        public string Url { get; set; }

        public Request(string url)
        {
            Url = url;
        }
    }

    public class Get<T> : Request<T>
    {
        public Get(string url) : base(url) { }
    }

    public class GetByInt<T> : Request<T>
    {
        public GetByInt(string url, int value) : base($"{url}/{value}") { }
    }
}
