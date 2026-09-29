namespace TARge25;

public partial class ValgusfoorPage : ContentPage
{
    private bool isTrafficLightOn = false;
    private bool isNightMode = false;
    private bool isAutoMode = false;

    private CancellationTokenSource? nightCancellation;
    private CancellationTokenSource? autoCancellation;

    private readonly Color offColor =
        Color.FromArgb("#404040");


    public ValgusfoorPage()
    {
        InitializeComponent();

        SetAllLightsGray();

        SetLightsClickable(false);
    }


    // SISSE

    private void OnTurnOnClicked(
        object? sender,
        EventArgs e)
    {
        StopNightMode();
        StopAutoMode();

        isTrafficLightOn = true;

        // Alguses näitame kõiki kolme värvi
        ShowNormalColors();

        SetLightsClickable(true);

        StatusLabel.Text =
            "Vali valgus";

        StatusLabel.TextColor =
            Color.FromArgb("#1F2937");
    }


    // VÄLJA

    private void OnTurnOffClicked(
        object? sender,
        EventArgs e)
    {
        isTrafficLightOn = false;

        StopNightMode();
        StopAutoMode();

        SetAllLightsGray();

        SetLightsClickable(false);

        StatusLabel.Text =
            "Lülita esmalt foor sisse";

        StatusLabel.TextColor =
            Color.FromArgb("#C62828");
    }


    // PUNANE

    private async void OnRedTapped(
        object? sender,
        TappedEventArgs e)
    {
        if (!CanSelectLight())
            return;

        SetSelectedLight(
            RedLight,
            Colors.Red);

        StatusLabel.Text =
            "Seisa";

        StatusLabel.TextColor =
            Colors.Red;

        await AnimateLightAsync(
            RedLight);
    }


    // KOLLANE

    private async void OnYellowTapped(
        object? sender,
        TappedEventArgs e)
    {
        if (!CanSelectLight())
            return;

        SetSelectedLight(
            YellowLight,
            Colors.Gold);

        StatusLabel.Text =
            "Valmista";

        StatusLabel.TextColor =
            Color.FromArgb("#D49B00");

        await AnimateLightAsync(
            YellowLight);
    }


    // ROHELINE

    private async void OnGreenTapped(
        object? sender,
        TappedEventArgs e)
    {
        if (!CanSelectLight())
            return;

        SetSelectedLight(
            GreenLight,
            Colors.LimeGreen);

        StatusLabel.Text =
            "Sõida";

        StatusLabel.TextColor =
            Color.FromArgb("#2E7D32");

        await AnimateLightAsync(
            GreenLight);
    }


    // AINULT ÜKS TULI PÕLEB

    private void SetSelectedLight(
        BoxView selectedLight,
        Color selectedColor)
    {
        // Kõik tuled kustuvad
        RedLight.Color = offColor;
        YellowLight.Color = offColor;
        GreenLight.Color = offColor;

        RedLight.Opacity = 1;
        YellowLight.Opacity = 1;
        GreenLight.Opacity = 1;

        RedLight.Scale = 1;
        YellowLight.Scale = 1;
        GreenLight.Scale = 1;

        // Valitud tuli süttib
        selectedLight.Color =
            selectedColor;
    }


    // KAS TULD SAAB VAJUTADA?

    private bool CanSelectLight()
    {
        return isTrafficLightOn
               && !isNightMode
               && !isAutoMode;
    }

    // ANIMATSIOON

    private async Task AnimateLightAsync(
        VisualElement light)
    {
        await Task.WhenAll(

            light.ScaleToAsync(
                1.15,
                150,
                Easing.CubicOut),

            light.FadeToAsync(
                0.65,
                150)
        );


        await Task.WhenAll(

            light.ScaleToAsync(
                1,
                150,
                Easing.CubicIn),

            light.FadeToAsync(
                1,
                150)
        );
    }


    // KÕIK VÄRVID

    private void ShowNormalColors()
    {
        RedLight.Color =
            Colors.Red;

        YellowLight.Color =
            Colors.Gold;

        GreenLight.Color =
            Colors.LimeGreen;


        RedLight.Opacity = 1;
        YellowLight.Opacity = 1;
        GreenLight.Opacity = 1;


        RedLight.Scale = 1;
        YellowLight.Scale = 1;
        GreenLight.Scale = 1;
    }


    // KÕIK HALLIKS

    private void SetAllLightsGray()
    {
        RedLight.Color =
            Colors.Gray;

        YellowLight.Color =
            Colors.Gray;

        GreenLight.Color =
            Colors.Gray;


        RedLight.Opacity = 1;
        YellowLight.Opacity = 1;
        GreenLight.Opacity = 1;


        RedLight.Scale = 1;
        YellowLight.Scale = 1;
        GreenLight.Scale = 1;
    }


    // KLIKKIMINE

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


    // ÖÖREŽIIM

    private void OnNightModeClicked(
        object? sender,
        EventArgs e)
    {
        if (!isTrafficLightOn)
        {
            StatusLabel.Text =
                "Lülita esmalt foor sisse";

            StatusLabel.TextColor =
                Color.FromArgb("#C62828");

            return;
        }


        // Kui öörežiim juba töötab,
        // siis lülitame selle välja
        if (isNightMode)
        {
            StopNightMode();

            ShowNormalColors();

            SetLightsClickable(true);

            StatusLabel.Text =
                "Vali valgus";

            StatusLabel.TextColor =
                Color.FromArgb("#1F2937");

            return;
        }


        StopAutoMode();

        isNightMode = true;

        SetLightsClickable(false);

        NightModeButton.Text =
            "PÄEVAREŽIIM";


        StatusLabel.Text =
            "Öörežiim – kollane vilgub";

        StatusLabel.TextColor =
            Color.FromArgb("#C69200");


        // Punane ja roheline kustuvad
        RedLight.Color =
            offColor;

        GreenLight.Color =
            offColor;


        StartNightBlinking();
    }


    // KOLLANE VILGUB ÖÖSEL

    private void StartNightBlinking()
    {
        nightCancellation =
            new CancellationTokenSource();

        CancellationToken token =
            nightCancellation.Token;

        _ = BlinkYellowAsync(token);
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
                YellowLight.Color =
                    Colors.Gold;

                YellowLight.Opacity =
                    1;


                await Task.Delay(
                    550,
                    token);


                YellowLight.Color =
                    offColor;

                YellowLight.Opacity =
                    1;


                await Task.Delay(
                    550,
                    token);
            }
        }
        catch (TaskCanceledException)
        {
        }
    }


    // ÖÖREŽIIM STOP

    private void StopNightMode()
    {
        isNightMode = false;


        if (nightCancellation is not null)
        {
            nightCancellation.Cancel();

            nightCancellation.Dispose();

            nightCancellation =
                null;
        }


        NightModeButton.Text =
            "ÖÖREŽIIM";
    }


    // AUTOMAATREŽIIM

    private void OnAutoModeClicked(
        object? sender,
        EventArgs e)
    {
        if (!isTrafficLightOn)
        {
            StatusLabel.Text =
                "Lülita esmalt foor sisse";

            StatusLabel.TextColor =
                Color.FromArgb("#C62828");

            return;
        }


        // Kui automaatrežiim juba töötab
        if (isAutoMode)
        {
            StopAutoMode();

            ShowNormalColors();

            SetLightsClickable(true);


            StatusLabel.Text =
                "Vali valgus";

            StatusLabel.TextColor =
                Color.FromArgb("#1F2937");

            return;
        }


        StopNightMode();

        isAutoMode = true;

        SetLightsClickable(false);


        AutoModeButton.Text =
            "PEATA AUTOMAATREŽIIM";


        StatusLabel.Text =
            "Automaatrežiim";

        StatusLabel.TextColor =
            Color.FromArgb("#0277BD");


        autoCancellation =
            new CancellationTokenSource();


        _ = RunAutomaticModeAsync(
            autoCancellation.Token);
    }


    // AUTOMAATNE TSÜKKEL

    private async Task RunAutomaticModeAsync(
        CancellationToken token)
    {
        try
        {
            while (isAutoMode &&
                   isTrafficLightOn &&
                   !token.IsCancellationRequested)
            {
                // 1. PUNANE

                SetSelectedLight(
                    RedLight,
                    Colors.Red);


                StatusLabel.Text =
                    "Seisa";


                StatusLabel.TextColor =
                    Colors.Red;


                await AnimateLightAsync(
                    RedLight);


                await Task.Delay(
                    2000,
                    token);


                // 2. KOLLANE

                SetSelectedLight(
                    YellowLight,
                    Colors.Gold);


                StatusLabel.Text =
                    "Valmista";


                StatusLabel.TextColor =
                    Color.FromArgb("#D49B00");


                await AnimateLightAsync(
                    YellowLight);


                await Task.Delay(
                    2000,
                    token);


                // 3. ROHELINE

                SetSelectedLight(
                    GreenLight,
                    Colors.LimeGreen);


                StatusLabel.Text =
                    "Sõida";


                StatusLabel.TextColor =
                    Color.FromArgb("#2E7D32");


                await AnimateLightAsync(
                    GreenLight);


                await Task.Delay(
                    2000,
                    token);


                // 4. ROHELINE VILGUB ENNE KOLLAST

                StatusLabel.Text =
                    "Roheline vilgub";


                StatusLabel.TextColor =
                    Color.FromArgb("#2E7D32");


                await BlinkGreenAsync(
                    token);


                // 5. KOLLANE

                SetSelectedLight(
                    YellowLight,
                    Colors.Gold);


                StatusLabel.Text =
                    "Valmista";


                StatusLabel.TextColor =
                    Color.FromArgb("#D49B00");


                await AnimateLightAsync(
                    YellowLight);


                await Task.Delay(
                    2000,
                    token);


                // Pärast seda algab tsükkel uuesti punasest
            }
        }
        catch (TaskCanceledException)
        {
        }
    }


    // ROHELINE VILGUB

    private async Task BlinkGreenAsync(
        CancellationToken token)
    {
        // Punane ja kollane on kustunud
        RedLight.Color =
            offColor;

        YellowLight.Color =
            offColor;


        // Vilgub 3 korda
        for (int i = 0; i < 3; i++)
        {
            // Roheline põleb
            GreenLight.Color =
                Colors.LimeGreen;


            GreenLight.Opacity =
                1;


            await Task.Delay(
                350,
                token);


            // Roheline kustub
            GreenLight.Color =
                offColor;


            await Task.Delay(
                350,
                token);
        }


        // Pärast vilkumist jääb roheline kustunuks
        GreenLight.Color =
            offColor;
    }


    // AUTOMAATREŽIIM STOP

    private void StopAutoMode()
    {
        isAutoMode =
            false;


        if (autoCancellation is not null)
        {
            autoCancellation.Cancel();

            autoCancellation.Dispose();

            autoCancellation =
                null;
        }


        AutoModeButton.Text =
            "AUTOMAATREŽIIM";
    }


    // LEHELT LAHKUMINE

    protected override void OnDisappearing()
    {
        StopNightMode();

        StopAutoMode();

        base.OnDisappearing();
    }
}