
using System.Threading;
using System.Threading.Tasks;
using LinkVault.Api;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace LinkVault.Services
{
    public class ServerService
    {
        public ServerService()
        {

        }

        private string serverStatus;

        private IWebHost server = null;
        public void RestartServer()
        {
            StopServer();

            this.server = WebHost.CreateDefaultBuilder().UseKestrel(x =>
             {
                 x.ListenLocalhost(3000);

             }).UseStartup<Startup>().UseDefaultServiceProvider((b, o) =>
             {

             })
             .Build();

            serverStatus = "Starting";

            // this.messageBusService.Emit("serverstatuschanged", serverStatus);

            Task.Run(() =>
            {
                Thread.Sleep(3000);
                server.RunAsync();
                serverStatus = "Started";
                // this.messageBusService.Emit("serverstatuschanged", serverStatus);

            });
        }

        public void StopServer()
        {
            if (this.server != null)
            {
                serverStatus = "Shutting down";
                // this.messageBusService.Emit("serverstatuschanged", serverStatus);
                this.server.StopAsync().Wait();

            }
            Thread.Sleep(3000);
            serverStatus = "Down";
            // this.messageBusService.Emit("serverstatuschanged", serverStatus);

        }

        public string GetServerStatus()
        {
            return serverStatus;
        }
    }
}