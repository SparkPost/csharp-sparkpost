using System.Threading.Tasks;

namespace SparkPost
{
    public interface IMessageEvents
    {
        Task<ListMessageEventsResponse> List();
        Task<ListMessageEventsResponse> List(MessageEventsQuery query);
        Task<ListMessageEventsResponse> List(string url);
        Task<MessageEventSampleResponse> SamplesOf(string events);
    }
}