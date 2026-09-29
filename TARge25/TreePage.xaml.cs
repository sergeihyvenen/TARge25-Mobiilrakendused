using System.ComponentModel;
using Microsoft.Maui.Controls.Shapes;

namespace TARge25;

public partial class TreePage : ContentPage
{
    private readonly Random random = new();

    private CancellationTokenSource? seasonAnimationCancellation;
    private CancellationTokenSource? nightSkyCancellation;


    public TreePage()
    {
        InitializeComponent();

        ActionPicker.SelectedIndex = 0;

        UpdateSpeedLabel();
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();

        DateTime date =
            SeasonDatePicker.Date
            ?? DateTime.Today;

        ApplySeason(date);
    }


    // KÄIVITA

    private async void OnActionClicked(
        object? sender,
        EventArgs e)
    {
        if (ActionPicker.SelectedItem is null)
        {
            InfoLabel.Text =
                "Palun vali tegevus!";

            return;
        }

        string action =
            ActionPicker.SelectedItem.ToString()
            ?? "";

        switch (action)
        {
            case "Kasva":
                await GrowTree();
                break;

            case "Õitse":
                await BloomTree();
                break;

            case "Värise":
                await ShakeTree();
                break;

            case "Langeta":
                await CutTree();
                break;
        }
    }


    // KASVA


    private async Task GrowTree()
    {
        ResetTreeTransform();

        uint duration =
            GetAnimationDuration();


        // Väike normaalne kasv
        double finalScale =
            1.06 +
            random.NextDouble() * 0.05;


        TreeContainer.Scale = 0.95;


        InfoLabel.Text =
            "Puu kasvab!";


        // Suvel kasvavad liblikad puuga samal ajal
        if (SummerLayer.IsVisible)
        {
            Butterfly1.Scale = 0.95;
            Butterfly2.Scale = 0.95;


            await Task.WhenAll(

                TreeContainer.ScaleToAsync(
                    finalScale,
                    duration,
                    Easing.CubicOut),

                Butterfly1.ScaleToAsync(
                    finalScale,
                    duration,
                    Easing.CubicOut),

                Butterfly2.ScaleToAsync(
                    finalScale,
                    duration,
                    Easing.CubicOut)
            );


            InfoLabel.Text =
                "Puu ja liblikad kasvasid!";
        }
        else
        {
            await TreeContainer.ScaleToAsync(
                finalScale,
                duration,
                Easing.CubicOut);


            InfoLabel.Text =
                "Puu kasvas!";
        }
    }


    // ÕITSE


    private async Task BloomTree()
    {
        ResetTreeTransform();


        uint duration =
            GetAnimationDuration();


        InfoLabel.Text =
            "Puu õitseb!";


        Flower1.IsVisible = true;
        Flower2.IsVisible = true;
        Flower3.IsVisible = true;
        Flower4.IsVisible = true;


        Flower1.Opacity = 0;
        Flower2.Opacity = 0;
        Flower3.Opacity = 0;
        Flower4.Opacity = 0;


        Flower1.Scale = 0.4;
        Flower2.Scale = 0.4;
        Flower3.Scale = 0.4;
        Flower4.Scale = 0.4;


        await Task.WhenAll(

            Flower1.FadeToAsync(1, duration),
            Flower2.FadeToAsync(1, duration),
            Flower3.FadeToAsync(1, duration),
            Flower4.FadeToAsync(1, duration),

            Flower1.ScaleToAsync(1, duration),
            Flower2.ScaleToAsync(1, duration),
            Flower3.ScaleToAsync(1, duration),
            Flower4.ScaleToAsync(1, duration)
        );
    }


    // VÄRISE

    private async Task ShakeTree()
    {
        ResetTreeTransform();


        uint duration =
            GetAnimationDuration();


        uint part =
            Math.Max(
                duration / 6,
                50u);


        InfoLabel.Text =
            "Puu väriseb tuules!";


        await TreeContainer.TranslateToAsync(
            -22,
            0,
            part,
            Easing.Linear);


        await TreeContainer.TranslateToAsync(
            22,
            0,
            part,
            Easing.Linear);


        await TreeContainer.TranslateToAsync(
            -16,
            0,
            part,
            Easing.Linear);


        await TreeContainer.TranslateToAsync(
            16,
            0,
            part,
            Easing.Linear);


        await TreeContainer.TranslateToAsync(
            -8,
            0,
            part,
            Easing.Linear);


        await TreeContainer.TranslateToAsync(
            0,
            0,
            part,
            Easing.Linear);
    }


    // LANGETA

    private async Task CutTree()
    {
        DateTime date =
            SeasonDatePicker.Date
            ?? DateTime.Today;


        TimeSpan time =
            WorkTimePicker.Time
            ?? TimeSpan.FromHours(12);


        int month =
            date.Month;


        int hour =
            time.Hours;


        bool isWinter =
            month == 12 ||
            month == 1 ||
            month == 2;


        bool isDay =
            hour >= 8 &&
            hour < 18;


        if (!isWinter && !isDay)
        {
            InfoLabel.Text =
                "Pimedas ja väljaspool talve puid ei langetata!";

            return;
        }


        if (!isWinter)
        {
            InfoLabel.Text =
                "Puid tohib langetada ainult talvel!";

            return;
        }


        if (!isDay)
        {
            InfoLabel.Text =
                "Tööd tohib teha ainult kell 08:00–17:00!";

            return;
        }


        ResetTreeTransform();


        TreeContainer.AnchorX = 0.5;
        TreeContainer.AnchorY = 1;


        InfoLabel.Text =
            "Puu langeb!";


        await TreeContainer.RotateToAsync(
            90,
            GetAnimationDuration(),
            Easing.CubicIn);
    }


    // ÕUNAD

    private async void OnAppleTapped(
        object? sender,
        TappedEventArgs e)
    {
        if (sender is not TapGestureRecognizer tap)
            return;


        string name =
            tap.CommandParameter?.ToString()
            ?? "";


        Ellipse? apple =
            name switch
            {
                "Apple1" => Apple1,
                "Apple2" => Apple2,
                "Apple3" => Apple3,
                _ => null
            };


        if (apple is null ||
            !apple.IsVisible)
        {
            return;
        }


        InfoLabel.Text =
            "Õun kukub puult!";


        uint duration =
            Math.Max(
                GetAnimationDuration(),
                500u);


        await Task.WhenAll(

            apple.TranslateToAsync(
                0,
                220,
                duration,
                Easing.CubicIn),

            apple.RotateToAsync(
                360,
                duration,
                Easing.Linear),

            apple.FadeToAsync(
                0,
                duration)
        );


        apple.IsVisible = false;
    }


    // SLIDER

    private void OnOpacityChanged(
        object? sender,
        ValueChangedEventArgs e)
    {
        Foliage.Opacity =
            e.NewValue;


        OpacityLabel.Text =
            $"Opacity: {e.NewValue:F2}";
    }


    // KIIRUS

    private void OnSpeedChanged(
        object? sender,
        ValueChangedEventArgs e)
    {
        UpdateSpeedLabel();
    }


    private void UpdateSpeedLabel()
    {
        int milliseconds =
            (int)SpeedStepper.Value;


        string text;


        if (milliseconds <= 750)
        {
            text = "kiire";
        }
        else if (milliseconds <= 1250)
        {
            text = "keskmine";
        }
        else
        {
            text = "aeglane";
        }


        SpeedLabel.Text =
            $"{milliseconds} ms – {text}";
    }


    private uint GetAnimationDuration()
    {
        double value =
            Math.Clamp(
                SpeedStepper.Value,
                500,
                2000);


        return (uint)value;
    }


    // KUUPÄEV

    private void OnDateSelected(
        object? sender,
        DateChangedEventArgs e)
    {
        DateTime date =
            e.NewDate
            ?? DateTime.Today;


        ApplySeason(date);
    }


    // KELLAAEG

    private void OnTimePickerPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName ==
            nameof(TimePicker.Time))
        {
            UpdateDayNight();
        }
    }


    // PÄEV / ÖÖ

    private void UpdateDayNight()
    {
        TimeSpan time =
            WorkTimePicker.Time
            ?? TimeSpan.FromHours(12);


        int hour =
            time.Hours;


        bool isDay =
            hour >= 8 &&
            hour < 18;


        // PÄEV

        if (isDay)
        {
            TreeArea.BackgroundColor =
                GetDaySkyColor();


            Sun.IsVisible =
                true;


            Moon.IsVisible =
                false;


            MoonGlow.IsVisible =
                false;


            Cloud1.IsVisible =
                true;


            Cloud2.IsVisible =
                true;


            HideStars();

            StopNightSkyAnimation();


            TimeStatusLabel.Text =
                "Päev – on valge";


            TimeStatusLabel.TextColor =
                Color.FromArgb("#455A64");
        }

        // ÖÖ

        else
        {


            TreeArea.BackgroundColor =
                Color.FromArgb("#3E6FA8");


            Sun.IsVisible =
                false;


            MoonGlow.IsVisible =
                true;


            Moon.IsVisible =
                true;


            // Öösel pilved peidus
            Cloud1.IsVisible =
                false;


            Cloud2.IsVisible =
                false;


            ShowStars();

            StartNightSkyAnimation();


            TimeStatusLabel.Text =
                "Öö – sinine tähistaevas";


            TimeStatusLabel.TextColor =
                Color.FromArgb("#3E5F8A");
        }
    }


    // PÄEVASE TAEVA VÄRV

    private Color GetDaySkyColor()
    {
        DateTime date =
            SeasonDatePicker.Date
            ?? DateTime.Today;


        int month =
            date.Month;


        // Talv
        if (month == 12 ||
            month == 1 ||
            month == 2)
        {
            return Color.FromArgb(
                "#B8D1E5");
        }


        // Kevad
        if (month >= 3 &&
            month <= 5)
        {
            return Color.FromArgb(
                "#BFE8FF");
        }


        // Suvi
        if (month >= 6 &&
            month <= 8)
        {
            return Color.FromArgb(
                "#87CEEB");
        }


        // Sügis
        return Color.FromArgb(
            "#E9B872");
    }


    // TÄHED

    private void ShowStars()
    {
        Star1.IsVisible = true;
        Star2.IsVisible = true;
        Star3.IsVisible = true;
        Star4.IsVisible = true;
        Star5.IsVisible = true;
        Star6.IsVisible = true;
        Star7.IsVisible = true;
        Star8.IsVisible = true;
        Star9.IsVisible = true;
    }


    private void HideStars()
    {
        Star1.IsVisible = false;
        Star2.IsVisible = false;
        Star3.IsVisible = false;
        Star4.IsVisible = false;
        Star5.IsVisible = false;
        Star6.IsVisible = false;
        Star7.IsVisible = false;
        Star8.IsVisible = false;
        Star9.IsVisible = false;
    }


    private void StartNightSkyAnimation()
    {
        StopNightSkyAnimation();


        nightSkyCancellation =
            new CancellationTokenSource();


        CancellationToken token =
            nightSkyCancellation.Token;


        _ = AnimateStarAsync(
            Star1, 0, token);

        _ = AnimateStarAsync(
            Star2, 120, token);

        _ = AnimateStarAsync(
            Star3, 240, token);

        _ = AnimateStarAsync(
            Star4, 360, token);

        _ = AnimateStarAsync(
            Star5, 480, token);

        _ = AnimateStarAsync(
            Star6, 600, token);

        _ = AnimateStarAsync(
            Star7, 720, token);

        _ = AnimateStarAsync(
            Star8, 840, token);

        _ = AnimateStarAsync(
            Star9, 960, token);
    }


    private async Task AnimateStarAsync(
        VisualElement star,
        int startDelay,
        CancellationToken token)
    {
        try
        {
            await Task.Delay(
                startDelay,
                token);


            while (!token.IsCancellationRequested)
            {
                double bright =
                    0.60 +
                    random.NextDouble() * 0.40;


                uint brightTime =
                    (uint)random.Next(
                        400,
                        800);


                uint darkTime =
                    (uint)random.Next(
                        500,
                        1000);


                await star.FadeToAsync(
                    bright,
                    brightTime,
                    Easing.SinInOut);


                await star.FadeToAsync(
                    0.25,
                    darkTime,
                    Easing.SinInOut);
            }
        }
        catch (TaskCanceledException)
        {
        }
    }


    private void StopNightSkyAnimation()
    {
        if (nightSkyCancellation is null)
            return;


        nightSkyCancellation.Cancel();

        nightSkyCancellation.Dispose();

        nightSkyCancellation =
            null;
    }


    // AASTAAJAD

    private void ApplySeason(
        DateTime date)
    {
        StopSeasonAnimations();


        HideSeasonLayers();

        HideFlowers();

        ResetApples(false);

        ResetSeasonElements();


        int month =
            date.Month;


        // TALV

        if (month == 12 ||
            month == 1 ||
            month == 2)
        {
            MainLayout.BackgroundColor =
                Color.FromArgb("#E9EFF3");


            GrassBack.Color =
                Color.FromArgb("#DDE7EA");


            GrassFront.Color =
                Color.FromArgb("#CBD8DE");


            SnowGround.IsVisible =
                true;


            WinterLayer.IsVisible =
                true;


            Cloud1.Opacity =
                0.85;


            Cloud2.Opacity =
                0.75;


            SetCrownColors(
                "#78909C",
                "#8297A2",
                "#90A4AE",
                "#B0BEC5",
                "#90A4AE",
                "#90A4AE"
            );


            InfoLabel.Text =
                "Talv – sajab lund";


            StartSnowAnimation();
        }


        // KEVAD

        else if (month >= 3 &&
                 month <= 5)
        {
            MainLayout.BackgroundColor =
                Color.FromArgb("#E8F5E9");


            GrassBack.Color =
                Color.FromArgb("#9CCC65");


            GrassFront.Color =
                Color.FromArgb("#7CB342");


            SpringLayer.IsVisible =
                true;


            Cloud1.Opacity =
                0.65;


            Cloud2.Opacity =
                0.50;


            SetCrownColors(
                "#66BB6A",
                "#72BF70",
                "#81C784",
                "#8BC34A",
                "#9CCC65",
                "#9CCC65"
            );


            InfoLabel.Text =
                "Kevad – loodus ärkab";
        }

        // SUVI

        else if (month >= 6 &&
                 month <= 8)
        {
            MainLayout.BackgroundColor =
                Color.FromArgb("#E3F2FD");


            GrassBack.Color =
                Color.FromArgb("#8BC34A");


            GrassFront.Color =
                Color.FromArgb("#689F38");


            SummerLayer.IsVisible =
                true;


            Cloud1.Opacity =
                0.55;


            Cloud2.Opacity =
                0.40;


            SetCrownColors(
                "#2E7D32",
                "#388E3C",
                "#43A047",
                "#4CAF50",
                "#66BB6A",
                "#66BB6A"
            );


            ResetApples(true);


            InfoLabel.Text =
                "Suvi – õunad ja liblikad";
        }


        // SÜGIS

        else
        {
            MainLayout.BackgroundColor =
                Color.FromArgb("#FFF3E0");


            GrassBack.Color =
                Color.FromArgb("#AAA65D");


            GrassFront.Color =
                Color.FromArgb("#827C3C");


            AutumnLayer.IsVisible =
                true;


            Cloud1.Opacity =
                0.75;


            Cloud2.Opacity =
                0.65;


            SetCrownColors(
                "#E65100",
                "#EF6C00",
                "#F57C00",
                "#FB8C00",
                "#F9A825",
                "#FFB300"
            );


            InfoLabel.Text =
                "Sügis – lehed langevad";


            StartAutumnAnimation();
        }


        UpdateDayNight();
    }


    // KROONI VÄRVID

    private void SetCrownColors(
        string left,
        string right,
        string center,
        string top,
        string topLeft,
        string topRight)
    {
        CrownLeft.BackgroundColor =
            Color.FromArgb(left);


        CrownRight.BackgroundColor =
            Color.FromArgb(right);


        CrownCenter.BackgroundColor =
            Color.FromArgb(center);


        CrownTop.BackgroundColor =
            Color.FromArgb(top);


        CrownTopLeft.BackgroundColor =
            Color.FromArgb(topLeft);


        CrownTopRight.BackgroundColor =
            Color.FromArgb(topRight);
    }


    // LUMI

    private void StartSnowAnimation()
    {
        seasonAnimationCancellation =
            new CancellationTokenSource();


        CancellationToken token =
            seasonAnimationCancellation.Token;


        _ = AnimateSnowflake(
            Snow1, 0, token);

        _ = AnimateSnowflake(
            Snow2, 250, token);

        _ = AnimateSnowflake(
            Snow3, 500, token);

        _ = AnimateSnowflake(
            Snow4, 750, token);

        _ = AnimateSnowflake(
            Snow5, 1000, token);

        _ = AnimateSnowflake(
            Snow6, 1250, token);

        _ = AnimateSnowflake(
            Snow7, 1500, token);

        _ = AnimateSnowflake(
            Snow8, 1750, token);
    }


    private async Task AnimateSnowflake(
        VisualElement snowflake,
        int startDelay,
        CancellationToken token)
    {
        try
        {
            await Task.Delay(
                startDelay,
                token);


            while (!token.IsCancellationRequested)
            {
                snowflake.TranslationX =
                    0;


                snowflake.TranslationY =
                    0;


                snowflake.Opacity =
                    0.60 +
                    random.NextDouble() * 0.40;


                double moveX =
                    random.Next(
                        -35,
                        36);


                uint duration =
                    (uint)random.Next(
                        1800,
                        3200);


                await snowflake.TranslateToAsync(
                    moveX,
                    330,
                    duration,
                    Easing.Linear);


                snowflake.TranslationX =
                    0;


                snowflake.TranslationY =
                    0;
            }
        }
        catch (TaskCanceledException)
        {
        }
    }


    // SÜGISLEHED

    private void StartAutumnAnimation()
    {
        seasonAnimationCancellation =
            new CancellationTokenSource();


        CancellationToken token =
            seasonAnimationCancellation.Token;


        _ = AnimateAutumnLeaf(
            AutumnLeaf1,
            0,
            token);


        _ = AnimateAutumnLeaf(
            AutumnLeaf2,
            400,
            token);


        _ = AnimateAutumnLeaf(
            AutumnLeaf3,
            800,
            token);


        _ = AnimateAutumnLeaf(
            AutumnLeaf4,
            1200,
            token);
    }


    private async Task AnimateAutumnLeaf(
        VisualElement leaf,
        int startDelay,
        CancellationToken token)
    {
        try
        {
            await Task.Delay(
                startDelay,
                token);


            while (!token.IsCancellationRequested)
            {
                leaf.TranslationX =
                    0;


                leaf.TranslationY =
                    0;


                leaf.Rotation =
                    0;


                double moveX =
                    random.Next(
                        -70,
                        71);


                uint duration =
                    (uint)random.Next(
                        1800,
                        3000);


                await Task.WhenAll(

                    leaf.TranslateToAsync(
                        moveX,
                        300,
                        duration,
                        Easing.Linear),

                    leaf.RotateToAsync(
                        360,
                        duration,
                        Easing.Linear)
                );


                leaf.TranslationX =
                    0;


                leaf.TranslationY =
                    0;


                leaf.Rotation =
                    0;
            }
        }
        catch (TaskCanceledException)
        {
        }
    }


    // ÕUNAD RESET

    private void ResetApples(
        bool visible)
    {
        ResetApple(
            Apple1,
            visible);


        ResetApple(
            Apple2,
            visible);


        ResetApple(
            Apple3,
            visible);
    }


    private static void ResetApple(
        Ellipse apple,
        bool visible)
    {
        apple.IsVisible =
            visible;


        apple.TranslationX =
            0;


        apple.TranslationY =
            0;


        apple.Rotation =
            0;


        apple.Opacity =
            1;
    }


    // LIBLIKAD

    private void ResetButterflies()
    {
        Butterfly1.Scale =
            1;


        Butterfly2.Scale =
            1;


        Butterfly1.TranslationX =
            0;


        Butterfly1.TranslationY =
            0;


        Butterfly2.TranslationX =
            0;


        Butterfly2.TranslationY =
            0;
    }

    // PUU RESET

    private void ResetTreeTransform()
    {
        TreeContainer.Scale =
            1;


        TreeContainer.Rotation =
            0;


        TreeContainer.TranslationX =
            0;


        TreeContainer.TranslationY =
            0;


        TreeContainer.AnchorX =
            0.5;


        TreeContainer.AnchorY =
            1;
    }


    private void HideFlowers()
    {
        Flower1.IsVisible =
            false;


        Flower2.IsVisible =
            false;


        Flower3.IsVisible =
            false;


        Flower4.IsVisible =
            false;
    }


    private void HideSeasonLayers()
    {
        WinterLayer.IsVisible =
            false;


        SpringLayer.IsVisible =
            false;


        SummerLayer.IsVisible =
            false;


        AutumnLayer.IsVisible =
            false;


        SnowGround.IsVisible =
            false;
    }


    private void ResetSeasonElements()
    {
        ResetSnowflake(Snow1);
        ResetSnowflake(Snow2);
        ResetSnowflake(Snow3);
        ResetSnowflake(Snow4);
        ResetSnowflake(Snow5);
        ResetSnowflake(Snow6);
        ResetSnowflake(Snow7);
        ResetSnowflake(Snow8);


        ResetAutumnLeaf(
            AutumnLeaf1);

        ResetAutumnLeaf(
            AutumnLeaf2);

        ResetAutumnLeaf(
            AutumnLeaf3);

        ResetAutumnLeaf(
            AutumnLeaf4);


        ResetButterflies();
    }


    private static void ResetSnowflake(
        VisualElement snowflake)
    {
        snowflake.TranslationX =
            0;


        snowflake.TranslationY =
            0;


        snowflake.Opacity =
            1;
    }


    private static void ResetAutumnLeaf(
        VisualElement leaf)
    {
        leaf.TranslationX =
            0;


        leaf.TranslationY =
            0;


        leaf.Rotation =
            0;
    }


    // STOP

    private void StopSeasonAnimations()
    {
        if (seasonAnimationCancellation is null)
            return;


        seasonAnimationCancellation.Cancel();

        seasonAnimationCancellation.Dispose();

        seasonAnimationCancellation =
            null;
    }


    protected override void OnDisappearing()
    {
        StopSeasonAnimations();

        StopNightSkyAnimation();

        base.OnDisappearing();
    }
}