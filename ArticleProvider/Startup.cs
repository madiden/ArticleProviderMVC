using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ArticleProvider.Startup))]
namespace ArticleProvider
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
