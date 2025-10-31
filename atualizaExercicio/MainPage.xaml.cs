using atualizaExercicio.Views;
using atualizaExercicio.Views.Login;
using atualizaExercicio.Views.Cadastro;


namespace atualizaExercicio
{
    public partial class MainPage : ContentPage
    {

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
    }

}
