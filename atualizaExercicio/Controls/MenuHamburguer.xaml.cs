using atualizaExercicio.Views;

namespace atualizaExercicio.Controls
{
    public partial class MenuHamburguer : ContentView
    {
        public Page ParentPage { get; set; }
       
        bool menuAberto = false;
        public MenuHamburguer()
        {
            InitializeComponent();

            MenuLateral.IsVisible = false;
            OverlayFundo.IsVisible = false;
        }

        private async void OnMenuClicked(object sender, EventArgs e)
        {
            if (menuAberto)
            {
                await FecharMenuAnimado();
            }
            else
            {
                await AbrirMenuAnimado();
            }
        }
        
        private async void Sobre_Clicked(object sender, EventArgs e)
        {
            await FecharMenuAnimado();

            var page = Application.Current.MainPage as NavigationPage;
            var currentPage = page?.CurrentPage ?? ParentPage;

            if (currentPage != null)
            {
                await currentPage.DisplayAlert("Sobre", "Você clicou em Sobre", "OK");
                //await ParentPage.Navigation.PushAsync(new Home());
            }

            
        }


        private async void Contato_Clicked(object sender, EventArgs e)
        {
            await FecharMenuAnimado();

            var page = Application.Current.MainPage as NavigationPage;
            var currentPage = page?.CurrentPage ?? ParentPage;

            if (ParentPage != null)
            {
                await ParentPage.DisplayAlert("Contato", "Você clicou em Contato", "OK");
                // await ParentPage.Navigation.PushAsync(new ContatoPage());
            }

        }
        private async void Logout_Clicked(object sender, EventArgs e)
        {
            await FecharMenuAnimado();

            var page = Application.Current.MainPage as NavigationPage;
            var currentPage = page?.CurrentPage ?? ParentPage;

            if (ParentPage != null)
            {

                bool confirmar = await ParentPage.DisplayAlert("Logout", "Deseja realmente sair?", "Sim", "Cancelar");
                MenuLateral.IsVisible = false;

                if (!confirmar)
                    return;

                try
                {
                    SecureStorage.Remove("usuario_id");

                    await ParentPage.DisplayAlert("Logout", "Você saiu da conta.", "OK");

                    // volta à tela inicial
                    Application.Current.MainPage = new NavigationPage(new MainPage());
                }
                catch (Exception ex)
                {
                    await ParentPage.DisplayAlert("Erro", $"Falha ao sair: {ex.Message}", "OK");
                }
            }
        }

        private async Task AbrirMenuAnimado()
        {
            MenuLateral.IsVisible = true;

            OverlayFundo.IsVisible = true;
           // OverlayFundo.InputTransparent = false; // captura toques apenas quando o menu está aberto

            await MenuLateral.TranslateTo(0, 0, 200, Easing.SinOut);
            menuAberto = true;
        }

        private async Task FecharMenuAnimado()
        {
            await MenuLateral.TranslateTo(-MenuLateral.Width, 0, 200, Easing.SinIn);
            MenuLateral.IsVisible = false;

            OverlayFundo.IsVisible = false;        // some visualmente
            //OverlayFundo.InputTransparent = true;
            menuAberto = false;
        }
        private async void OnOverlayTapped(object sender, EventArgs e)
        {
            //await ParentPage.DisplayAlert("Teste", "Overlay clicado!", "OK"); // debug
            await FecharMenuAnimado();
        }

        private async void OnLogoTapped(object sender, EventArgs e)
        {
            await FecharMenuAnimado(); // fecha o menu se estiver aberto

            var page = Application.Current.MainPage as NavigationPage;
            var currentPage = page?.CurrentPage ?? ParentPage;

            if (ParentPage != null)
            {
                try
                {
                    // Navega para a Home
                    await ParentPage.Navigation.PushAsync(new Views.Home());
                }
                catch (Exception ex)
                {
                    await ParentPage.DisplayAlert("Erro", $"Falha ao abrir Home: {ex.Message}", "OK");
                }
            }
        }

    }
}
