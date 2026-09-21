namespace TARge25;

public partial class ValgusfoorPage : ContentPage
{
    private bool isTrafficLightOn = false;
    private bool isNightMode = false;

    private CancellationTokenSource? nightModeCancellation;

    public ValgusfoorPage()
    {
        InitializeComponent();

        SetAllLightsGray();
        SetLightsClickable(false);
    }


    // SISSE
    private void OnTurnOnClicked(object? sender, EventArgs e)
    {
        isTrafficLightOn = true;

        if (isNightMode)
        {
            SetLightsClickable(false);

            StatusLabel.Text = "Öörežiim – kollane vilgub";

            StartNightBlinking();
            return;
        }

        // Näitame kõiki valgusfoori värve
        RedLight.BackgroundColor = Colors.Red;
        YellowLight.BackgroundColor = Colors.Yellow;
        GreenLight.BackgroundColor = Colors.LimeGreen;

        StatusLabel.Text = "Vali valgus";

        SetLightsClickable(true);
    }


    // VÄLJA
    private void OnTurnOffClicked(object? sender, EventArgs e)
    {
        isTrafficLightOn = false;

        StopNightBlinking();

        SetAllLightsGray();
        SetLightsClickable(false);

        StatusLabel.Text = "Lülita esmalt foor sisse";
    }


    // PUNANE
    private async void OnRedTapped(object? sender, TappedEventArgs e)
    {
        if (!isTrafficLightOn || isNightMode)
            return;

        SetActiveLight(RedLight, Colors.Red);

        StatusLabel.Text = "Seisa";

        await AnimateLightAsync(RedLight);
    }


    // KOLLANE
    private async void OnYellowTapped(object? sender, TappedEventArgs e)
    {
        if (!isTrafficLightOn || isNightMode)
            return;

        SetActiveLight(YellowLight, Colors.Yellow);

        StatusLabel.Text = "Valmista";

        await AnimateLightAsync(YellowLight);
    }


    // ROHELINE
    private async void OnGreenTapped(object? sender, TappedEventArgs e)
    {
        if (!isTrafficLightOn || isNightMode)
            return;

        SetActiveLight(GreenLight, Colors.LimeGreen);

        StatusLabel.Text = "Sõida";

        await AnimateLightAsync(GreenLight);
    }


    // ÖÖREŽIIM
    private void OnNightModeClicked(object? sender, EventArgs e)
    {
        if (isNightMode)
        {
            DisableNightMode();
        }
        else
        {
            EnableNightMode();
        }
    }


    private void EnableNightMode()
    {
        isNightMode = true;
        isTrafficLightOn = true;

        NightOverlay.Opacity = 0.55;

        StatusLabel.TextColor = Colors.White;
        StatusLabel.Text = "Öörežiim – kollane vilgub";

        TrafficLightBody.BackgroundColor =
            Color.FromArgb("#101010");

        NightModeButton.Text = "PÄEVAREŽIIM";

        SetAllLightsDarkGray();

        // Öörežiimis tulesid käsitsi vajutada ei saa
        SetLightsClickable(false);

        StartNightBlinking();
    }


    private void DisableNightMode()
    {
        isNightMode = false;

        StopNightBlinking();

        NightOverlay.Opacity = 0;

        StatusLabel.TextColor = Colors.Black;

        TrafficLightBody.BackgroundColor =
            Color.FromArgb("#202020");

        NightModeButton.Text = "ÖÖREŽIIM";

        if (isTrafficLightOn)
        {
            RedLight.BackgroundColor = Colors.Red;
            YellowLight.BackgroundColor = Colors.Yellow;
            GreenLight.BackgroundColor = Colors.LimeGreen;

            StatusLabel.Text = "Vali valgus";

            SetLightsClickable(true);
        }
        else
        {
            SetAllLightsGray();

            StatusLabel.Text = "Lülita esmalt foor sisse";

            SetLightsClickable(false);
        }
    }


    // Kollase tule vilkumine öörežiimis
    private void StartNightBlinking()
    {
        StopNightBlinking();

        nightModeCancellation =
            new CancellationTokenSource();

        _ = BlinkYellowAsync(
            nightModeCancellation.Token);
    }


    private void StopNightBlinking()
    {
        if (nightModeCancellation == null)
            return;

        nightModeCancellation.Cancel();
        nightModeCancellation.Dispose();

        nightModeCancellation = null;
    }


    private async Task BlinkYellowAsync(
        CancellationToken token)
    {
        try
        {
            while (isNightMode &&
                   isTrafficLightOn &&
                   !token.IsCancellationRequested)
            {
                Color darkGray =
                    Color.FromArgb("#303030");

                RedLight.BackgroundColor = darkGray;
                GreenLight.BackgroundColor = darkGray;

                // Kollane sisse
                YellowLight.BackgroundColor =
                    Colors.Gold;

                await AnimateLightAsync(
                    YellowLight);

                await Task.Delay(
                    500,
                    token);

                // Kollane välja
                YellowLight.BackgroundColor =
                    darkGray;

                await Task.Delay(
                    500,
                    token);
            }
        }
        catch (TaskCanceledException)
        {
            // See on normaalne,
            // kui öörežiim peatatakse.
        }
    }


    // Aktiivseks jääb ainult valitud tuli
    private void SetActiveLight(
        Border activeLight,
        Color activeColor)
    {
        SetAllLightsGray();

        activeLight.BackgroundColor =
            activeColor;
    }


    // Väike animatsioon
    private async Task AnimateLightAsync(
        Border light)
    {
        await Task.WhenAll(
            light.ScaleToAsync(1.12, 140),
            light.FadeToAsync(0.65, 140)
        );

        await Task.WhenAll(
            light.ScaleToAsync(1.0, 140),
            light.FadeToAsync(1.0, 140)
        );
    }


    // Kõik tuled halliks
    private void SetAllLightsGray()
    {
        RedLight.BackgroundColor =
            Colors.Gray;

        YellowLight.BackgroundColor =
            Colors.Gray;

        GreenLight.BackgroundColor =
            Colors.Gray;
    }


    // Öörežiimi tumedad tuled
    private void SetAllLightsDarkGray()
    {
        Color darkGray =
            Color.FromArgb("#303030");

        RedLight.BackgroundColor =
            darkGray;

        YellowLight.BackgroundColor =
            darkGray;

        GreenLight.BackgroundColor =
            darkGray;
    }


    // Kas tulesid saab vajutada
    private void SetLightsClickable(
        bool clickable)
    {
        RedLight.InputTransparent =
            !clickable;

        YellowLight.InputTransparent =
            !clickable;

        GreenLight.InputTransparent =
            !clickable;
    }


    protected override void OnDisappearing()
    {
        StopNightBlinking();

        base.OnDisappearing();
    }
}