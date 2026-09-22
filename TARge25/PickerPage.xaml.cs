namespace TARge25;

public partial class PickerPage : ContentPage
{
    public PickerPage()
    {
        InitializeComponent();

        FillGrid();
    }


    // PICKER
    private void OnPickerSelectedIndexChanged(
        object? sender,
        EventArgs e)
    {
        if (ImagePicker.SelectedIndex == -1)
            return;

        string selected =
            ImagePicker.Items[ImagePicker.SelectedIndex];


        if (selected == "Internetipilt")
        {
            MainImage.Source =
                ImageSource.FromUri(
                    new Uri("https://picsum.photos/500/300")
                );

            InfoLabel.Text =
                "Valitud internetipilt";
        }
        else
        {
            MainImage.Source =
                selected;

            InfoLabel.Text =
                $"Valitud pilt: {selected}";
        }
    }


    // PILDI SWITCH
    private void OnImageSwitchToggled(
        object? sender,
        ToggledEventArgs e)
    {
        MainImage.IsVisible =
            e.Value;

        InfoLabel.Text =
            e.Value
                ? "Pilt on nähtav"
                : "Pilt on peidetud";
    }


    // GRID SWITCH
    private void OnGridSwitchToggled(
        object? sender,
        ToggledEventArgs e)
    {
        ColorGrid.IsVisible =
            e.Value;

        InfoLabel.Text =
            e.Value
                ? "3x3 ruudustik on nähtav"
                : "3x3 ruudustik on peidetud";
    }


    // 3x3 GRID
    private void FillGrid()
    {
        Color[] colors =
        {
            Colors.Red,
            Colors.Orange,
            Colors.Gold,

            Colors.Green,
            Colors.LightBlue,
            Colors.Blue,

            Colors.Purple,
            Colors.Pink,
            Colors.Brown
        };

        int index = 0;


        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                int currentRow =
                    row;

                int currentColumn =
                    column;


#pragma warning disable CS0618

                Frame frame =
                    new Frame
                    {
                        BackgroundColor =
                            colors[index],

                        CornerRadius = 12,

                        Padding = 0,

                        HasShadow = true,

                        WidthRequest = 90,
                        HeightRequest = 90
                    };

#pragma warning restore CS0618


                Label text =
                    new Label
                    {
                        Text =
                            $"{row + 1},{column + 1}",

                        FontSize = 18,

                        FontAttributes =
                            FontAttributes.Bold,

                        TextColor =
                            Colors.White,

                        HorizontalOptions =
                            LayoutOptions.Center,

                        VerticalOptions =
                            LayoutOptions.Center
                    };


                frame.Content =
                    text;


                TapGestureRecognizer tap =
                    new TapGestureRecognizer();


                tap.Tapped +=
                    (sender, e) =>
                    {
                        InfoLabel.Text =
                            $"Vajutasid: rida {currentRow + 1}, " +
                            $"veerg {currentColumn + 1}";

                        frame.BackgroundColor =
                            Colors.DarkGray;
                    };


                frame.GestureRecognizers.Add(
                    tap);


                ColorGrid.Add(
                    frame,
                    column,
                    row);


                index++;
            }
        }
    }
}