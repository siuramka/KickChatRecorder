using Cassandra;
using System.Net;
using ISession = Cassandra.ISession;

namespace KickChatRecorder.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddScoped<ICluster>(s => Cluster.Builder().AddContactPoint("localhost")
    .Build());

            builder.Services.AddScoped<ISession>(s => s.GetService<ICluster>().Connect("store"));

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}