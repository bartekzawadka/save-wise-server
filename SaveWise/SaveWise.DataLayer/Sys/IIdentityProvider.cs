namespace SaveWise.DataLayer.Sys
{
    public interface IIdentityProvider
    {
        bool IsAuthenticated { get; }
        
        string GetUserId();
    }
}