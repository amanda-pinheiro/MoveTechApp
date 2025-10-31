using Microsoft.Extensions.Logging;

namespace atualizaExercicio
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Quicksand-Bold.ttf", "QuicksandBold");
                    fonts.AddFont("Quicksand-Light.ttf", "QuicksandLight");
                    fonts.AddFont("Quicksand-Regular.ttf", "QuicksandRegular");
                    fonts.AddFont("Quicksand-Semibold.ttf", "QuicksandSemibold");
                    fonts.AddFont("Quicksand-Medium.ttf", "QuicksandMedium");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
