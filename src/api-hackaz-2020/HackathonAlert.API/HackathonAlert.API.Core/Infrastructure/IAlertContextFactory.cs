namespace HackathonAlert.API.Core.Infrastructure
{
    public interface IAlertContextFactory
    {
        AlertApiContext AlertContext();
    }
}
