using CashDriver.ViewModels;
using CashDriver.Views;

namespace CashDriver
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //registrando as rotas acessadas pelas views
            Routing.RegisterRoute(nameof(CriarMetaPage), typeof(CriarMetaPage));
            Routing.RegisterRoute(nameof(CriarDespesaPage), typeof(CriarDespesaPage));
            Routing.RegisterRoute(nameof(MetasListPage), typeof(MetasListPage));

            Routing.RegisterRoute(nameof(JornadaPage), typeof(JornadaPage));
            //Routing.RegisterRoute(nameof(IniciarJornadaPage), typeof(IniciarJornadaPage));
            Routing.RegisterRoute(nameof(DespesasListPage), typeof(DespesasListPage));


            Routing.RegisterRoute(nameof(CriarPlataformaPage), typeof(CriarPlataformaPage));
            Routing.RegisterRoute(nameof(CriarPlataformaViewModel), typeof(CriarPlataformaViewModel));
        }
    }
}
