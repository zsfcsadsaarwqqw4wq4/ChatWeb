using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin;
[assembly:OwinStartup(typeof(ChatWeb.App_Start.Startupcs))]
namespace ChatWeb.App_Start
{
    public class Startupcs
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}