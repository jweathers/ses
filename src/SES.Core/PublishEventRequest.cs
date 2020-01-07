public class PublishEventRequest<T> where T:class
{
    public PublishEventRequest(T eventData)
    {
        
    }  
    internal T eventData{get;set;}
    internal string QueueName{get;set;}  
}
