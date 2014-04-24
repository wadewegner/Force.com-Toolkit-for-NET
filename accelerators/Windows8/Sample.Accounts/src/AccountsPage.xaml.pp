<Page
    x:Class="$rootnamespace$.Pages.AccountsPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:$rootnamespace$.Pages"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d" Loaded="Page_Loaded">

    <Grid Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">

        <ListView x:Name="GridView1" Margin="10,50,10,10">
            <ListView.HeaderTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal">
                        <TextBlock Text="Name" Width="300" FontWeight="Bold" FontSize="20"></TextBlock>
                        <TextBlock Text="Description" FontWeight="Bold" FontSize="20"></TextBlock>
                    </StackPanel>
                </DataTemplate>

            </ListView.HeaderTemplate>
            <ListView.ItemTemplate>
                <DataTemplate>
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="300"/>
                            <ColumnDefinition Width="*"/>
                        </Grid.ColumnDefinitions>

                        <TextBlock Text="{Binding Name}" Grid.Column="0" TextWrapping="NoWrap"/>
                        <TextBlock Text="{Binding Description}" Grid.Column="1" />
                    </Grid>
                </DataTemplate>
            </ListView.ItemTemplate>

        </ListView>
        
    </Grid>
</Page>
