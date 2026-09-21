namespace TARge25;

public partial class TreePage : ContentPage
{
    private readonly Random random = new();

    private CancellationTokenSource? seasonAnimationCancellation;


    public TreePage()
    {
        InitializeComponent();

        ActionPicker.SelectedIndex = 0;

        DateTime selectedDate =
            SeasonDatePicker.Date ?? DateTime.Today;

        ApplySeason(selectedDate);
    }


    // =========================================================
    // KÄIVITA
    // =========================================================

    private async void OnActionClicked(
        object? sender,
        EventArgs e)
    {
        if (ActionPicker.SelectedItem is null)
        {
            InfoLabel.Text = "⚠️ Palun vali tegevus!";
            return;
        }

        string action =
            ActionPicker.SelectedItem.ToString() ?? "";

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


    // =========================================================
    // KASVA
    // =========================================================

    private async Task GrowTree()
    {
        ResetTreeTransform();

        uint speed = GetAnimationSpeed();

        // Juhuslik kasvamine
        double scale =
            1.15 + random.NextDouble() * 0.35;

        InfoLabel.Text =
            $"🌱 Puu kasvab! {scale:F1}x";

        await TreeContainer.ScaleToAsync(
            scale,
            speed);
    }


    // =========================================================
    // ÕITSE
    // =========================================================

    private async Task BloomTree()
    {
        ResetTreeTransform();

        uint speed = GetAnimationSpeed();

        InfoLabel.Text = "🌸 Puu õitseb!";

        Flower1.IsVisible = true;
        Flower2.IsVisible = true;
        Flower3.IsVisible = true;
        Flower4.IsVisible = true;

        Flower1.Opacity = 0;
        Flower2.Opacity = 0;
        Flower3.Opacity = 0;
        Flower4.Opacity = 0;

        await Task.WhenAll(
            Flower1.FadeToAsync(1, speed),
            Flower2.FadeToAsync(1, speed),
            Flower3.FadeToAsync(1, speed),
            Flower4.FadeToAsync(1, speed)
        );
    }


    // =========================================================
    // VÄRISE
    // =========================================================

    private async Task ShakeTree()
    {
        ResetTreeTransform();

        uint speed = GetAnimationSpeed();

        uint part =
            Math.Max(speed / 6, 50);

        InfoLabel.Text =
            "💨 Puu väriseb tuules!";

        await TreeContainer.TranslateToAsync(
            -22,
            0,
            part);

        await TreeContainer.TranslateToAsync(
            22,
            0,
            part);

        await TreeContainer.TranslateToAsync(
            -16,
            0,
            part);

        await TreeContainer.TranslateToAsync(
            16,
            0,
            part);

        await TreeContainer.TranslateToAsync(
            -8,
            0,
            part);

        await TreeContainer.TranslateToAsync(
            0,
            0,
            part);
    }


    // =========================================================
    // LANGETA
    // =========================================================

    private async Task CutTree()
    {
        DateTime selectedDate =
            SeasonDatePicker.Date ?? DateTime.Today;

        TimeSpan selectedTime =
            WorkTimePicker.Time ?? TimeSpan.FromHours(12);

        int month = selectedDate.Month;
        int hour = selectedTime.Hours;


        bool isWinter =
            month == 12 ||
            month == 1 ||
            month == 2;


        bool isDay =
            hour >= 8 &&
            hour <= 17;


        if (!isWinter && !isDay)
        {
            InfoLabel.Text =
                "❌ Pimedas ja väljaspool talve puid ei langetata!";
            return;
        }


        if (!isWinter)
        {
            InfoLabel.Text =
                "❌ Puid tohib langetada ainult talvel!";
            return;
        }


        if (!isDay)
        {
            InfoLabel.Text =
                "❌ Tööd tohib teha ainult kell 08:00–17:00!";
            return;
        }


        ResetTreeTransform();

        TreeContainer.AnchorX = 0.5;
        TreeContainer.AnchorY = 1;

        InfoLabel.Text =
            "🪓 Puu langeb!";

        await TreeContainer.RotateToAsync(
            90,
            GetAnimationSpeed());
    }


    // =========================================================
    // SLIDER
    // =========================================================

    private void OnOpacityChanged(
        object? sender,
        ValueChangedEventArgs e)
    {
        Foliage.Opacity = e.NewValue;

        OpacityLabel.Text =
            $"Opacity: {e.NewValue:F2}";
    }


    // =========================================================
    // STEPPER
    // =========================================================

    private void OnSpeedChanged(
        object? sender,
        ValueChangedEventArgs e)
    {
        SpeedLabel.Text =
            $"{(int)e.NewValue} ms";
    }


    // =========================================================
    // DATE PICKER
    // =========================================================

    private void OnDateSelected(
        object? sender,
        DateChangedEventArgs e)
    {
        DateTime date =
            e.NewDate ?? DateTime.Today;

        ApplySeason(date);
    }


    // =========================================================
    // AASTAAJAD
    // =========================================================

    private void ApplySeason(DateTime date)
    {
        StopSeasonAnimations();

        HideSeasonLayers();

        HideActionFlowers();

        ResetApples(false);

        int month = date.Month;


        // =====================================================
        // TALV
        // =====================================================

        if (month == 12 ||
            month == 1 ||
            month == 2)
        {
            TreeArea.BackgroundColor =
                Color.FromArgb("#B9C8D5");

            MainLayout.BackgroundColor =
                Color.FromArgb("#E9EFF3");

            GrassBack.Color =
                Color.FromArgb("#DDE8EC");

            GrassFront.Color =
                Color.FromArgb("#CBD8DE");

            SnowGround.IsVisible = true;

            WinterLayer.IsVisible = true;

            Sun.Opacity = 0.45;

            Cloud1.Opacity = 0.90;
            Cloud2.Opacity = 0.85;


            // Talvine hele lehestik
            SetCrownColors(
                "#78909C",
                "#90A4AE",
                "#78909C",
                "#B0BEC5",
                "#90A4AE",
                "#90A4AE"
            );

            InfoLabel.Text =
                "❄️ Talv – sajab lund";

            StartSnowAnimation();

            return;
        }


        // =====================================================
        // KEVAD
        // =====================================================

        if (month >= 3 &&
            month <= 5)
        {
            TreeArea.BackgroundColor =
                Color.FromArgb("#BFE8FF");

            MainLayout.BackgroundColor =
                Color.FromArgb("#E8F5E9");

            GrassBack.Color =
                Color.FromArgb("#9CCC65");

            GrassFront.Color =
                Color.FromArgb("#7CB342");

            SpringLayer.IsVisible = true;

            Sun.Opacity = 0.80;

            SetCrownColors(
                "#66BB6A",
                "#7CBF68",
                "#81C784",
                "#8BC34A",
                "#9CCC65",
                "#9CCC65"
            );

            InfoLabel.Text =
                "🌷 Kevad – loodus ärkab";

            return;
        }


        // =====================================================
        // SUVI
        // =====================================================

        if (month >= 6 &&
            month <= 8)
        {
            TreeArea.BackgroundColor =
                Color.FromArgb("#87CEEB");

            MainLayout.BackgroundColor =
                Color.FromArgb("#E3F2FD");

            GrassBack.Color =
                Color.FromArgb("#8BC34A");

            GrassFront.Color =
                Color.FromArgb("#689F38");

            SummerLayer.IsVisible = true;

            Sun.Opacity = 1;

            Cloud1.Opacity = 0.55;
            Cloud2.Opacity = 0.40;

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
                "☀️ Suvi – puu on roheline ja õunad valmivad";

            return;
        }


        // =====================================================
        // SÜGIS
        // =====================================================

        TreeArea.BackgroundColor =
            Color.FromArgb("#E9B872");

        MainLayout.BackgroundColor =
            Color.FromArgb("#FFF3E0");

        GrassBack.Color =
            Color.FromArgb("#A4A65A");

        GrassFront.Color =
            Color.FromArgb("#827C3C");

        AutumnLayer.IsVisible = true;

        Sun.Opacity = 0.60;

        SetCrownColors(
            "#E65100",
            "#EF6C00",
            "#F57C00",
            "#FB8C00",
            "#F9A825",
            "#FFB300"
        );

        InfoLabel.Text =
            "🍂 Sügis – lehed langevad";

        StartAutumnAnimation();
    }


    // =========================================================
    // KROONI VÄRVID
    // =========================================================

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


    // =========================================================
    // LUME ANIMATSIOON
    // =========================================================

    private void StartSnowAnimation()
    {
        seasonAnimationCancellation =
            new CancellationTokenSource();

        CancellationToken token =
            seasonAnimationCancellation.Token;

        _ = AnimateSnowflake(Snow1, 0, token);
        _ = AnimateSnowflake(Snow2, 250, token);
        _ = AnimateSnowflake(Snow3, 500, token);
        _ = AnimateSnowflake(Snow4, 750, token);
        _ = AnimateSnowflake(Snow5, 1000, token);
        _ = AnimateSnowflake(Snow6, 1250, token);
        _ = AnimateSnowflake(Snow7, 1500, token);
        _ = AnimateSnowflake(Snow8, 1750, token);
    }


    private async Task AnimateSnowflake(
        VisualElement snowflake,
        int startDelay,
        CancellationToken token)
    {
        try
        {
            await Task.Delay(startDelay, token);

            while (!token.IsCancellationRequested)
            {
                snowflake.TranslationY = 0;
                snowflake.TranslationX = 0;

                snowflake.Opacity =
                    0.55 + random.NextDouble() * 0.45;


                double x =
                    random.Next(-35, 36);

                uint duration =
                    (uint)random.Next(1800, 3200);


                await snowflake.TranslateToAsync(
                    x,
                    330,
                    duration,
                    Easing.Linear);


                if (token.IsCancellationRequested)
                    return;


                snowflake.TranslationY = 0;
                snowflake.TranslationX = 0;
            }
        }
        catch (TaskCanceledException)
        {
        }
    }


    // =========================================================
    // SÜGISLEHTEDE ANIMATSIOON
    // =========================================================

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
        int delay,
        CancellationToken token)
    {
        try
        {
            await Task.Delay(delay, token);

            while (!token.IsCancellationRequested)
            {
                leaf.TranslationX = 0;
                leaf.TranslationY = 0;
                leaf.Rotation = 0;

                double x =
                    random.Next(-70, 71);

                uint duration =
                    (uint)random.Next(1900, 3000);


                await Task.WhenAll(
                    leaf.TranslateToAsync(
                        x,
                        300,
                        duration,
                        Easing.Linear),

                    leaf.RotateToAsync(
                        360,
                        duration,
                        Easing.Linear)
                );


                if (token.IsCancellationRequested)
                    return;


                leaf.TranslationX = 0;
                leaf.TranslationY = 0;
                leaf.Rotation = 0;
            }
        }
        catch (TaskCanceledException)
        {
        }
    }


    // =========================================================
    // ÕUNAD
    // =========================================================

    private async void OnAppleTapped(
        object? sender,
        TappedEventArgs e)
    {
        string? appleName =
            e.Parameter as string;


        Border? apple =
            appleName switch
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
            "🍎 Õun kukkus puult!";


        await Task.WhenAll(
            apple.TranslateToAsync(
                0,
                210,
                650,
                Easing.CubicIn),

            apple.RotateToAsync(
                300,
                650),

            apple.FadeToAsync(
                0,
                650)
        );


        apple.IsVisible = false;
    }


    private void ResetApples(bool visible)
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
        Border apple,
        bool visible)
    {
        apple.IsVisible = visible;

        apple.TranslationX = 0;
        apple.TranslationY = 0;

        apple.Rotation = 0;
        apple.Opacity = 1;
    }


    // =========================================================
    // KIHTIDE PEITMINE
    // =========================================================

    private void HideSeasonLayers()
    {
        WinterLayer.IsVisible = false;
        SpringLayer.IsVisible = false;
        SummerLayer.IsVisible = false;
        AutumnLayer.IsVisible = false;

        SnowGround.IsVisible = false;

        Cloud1.Opacity = 0.75;
        Cloud2.Opacity = 0.55;
    }


    private void HideActionFlowers()
    {
        Flower1.IsVisible = false;
        Flower2.IsVisible = false;
        Flower3.IsVisible = false;
        Flower4.IsVisible = false;
    }


    // =========================================================
    // ANIMATSIOONI PEATAMINE
    // =========================================================

    private void StopSeasonAnimations()
    {
        if (seasonAnimationCancellation is null)
            return;

        seasonAnimationCancellation.Cancel();

        seasonAnimationCancellation.Dispose();

        seasonAnimationCancellation = null;
    }


    // =========================================================
    // PUU TAASTAMINE
    // =========================================================

    private void ResetTreeTransform()
    {
        TreeContainer.Scale = 1;

        TreeContainer.Rotation = 0;

        TreeContainer.TranslationX = 0;
        TreeContainer.TranslationY = 0;

        TreeContainer.AnchorX = 0.5;
        TreeContainer.AnchorY = 1;
    }


    // =========================================================
    // KIIRUS
    // =========================================================

    private uint GetAnimationSpeed()
    {
        return (uint)SpeedStepper.Value;
    }


    // =========================================================
    // LEHELT LAHKUMINE
    // =========================================================

    protected override void OnDisappearing()
    {
        StopSeasonAnimations();

        base.OnDisappearing();
    }
}