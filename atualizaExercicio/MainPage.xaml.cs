using atualizaExercicio.Views;
using atualizaExercicio.Views.Menu;
using atualizaExercicio.Views.Login;
using atualizaExercicio.Views.Cadastro;


namespace atualizaExercicio
{
    public partial class MainPage : ContentPage
    {
        private bool menuAberto = false;

        public MainPage()
        {
            InitializeComponent();
        }
        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new LoginPage1());
            }
            catch
            {
                await DisplayAlert("Ops!", "Não foi possível abrir a tela de login. Tente novamente", "Ok");
            }
        }
        private async void CadastroButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new CadastroPage1());
            }
            catch
            {
                await DisplayAlert("Ops!", "Não foi possível abrir a tela de cadastro. Tente novamente", "Ok");
            }
        }

        private async void Info_Clicked(object sender, EventArgs e)
        {
            menuAberto = !menuAberto;

            if (menuAberto)
            {
                MenuOpcoes.IsVisible = true;
                await MenuOpcoes.FadeTo(1, 200, Easing.BounceIn);
            }
            else
            {
                await MenuOpcoes.FadeTo(0, 150, Easing.BounceOut);
                MenuOpcoes.IsVisible = false;
            }
        }

        private async void Sobre_Page(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SobrePage1());
            FecharMenu();
        }

        private async void Contato_Page(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ContatoPage1());
            FecharMenu();
        }

        private void FecharMenu()
        {
            menuAberto = false;
            MenuOpcoes.IsVisible = false;
        }
    }


}