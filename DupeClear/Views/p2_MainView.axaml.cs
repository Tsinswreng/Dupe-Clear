using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using DupeClear.Converters;
using DupeClear.Helpers;
using DupeClear.Models.MessageBox;
using DupeClear.Native;
using DupeClear.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DupeClear.Views;
public partial class MainView{

	public MainViewModel? ctx{
		get{return DataContext as MainViewModel;}
		set{DataContext = value;}
	}

	protected byte _render(){
		var MainGrid = new Grid();
		Content = MainGrid;
		MainGrid.Name = nameof(MainGrid);
		MainGrid.RowDefinitions.AddRange([
			new RowDefinition{Height = GridLength.Auto}
			,new RowDefinition{Height = GridLength.Star}
			,new RowDefinition{Height = GridLength.Auto}
		]);
		{{//MainGrid:Grid
			MainGrid.Children.Add(_WindowsTitleBar());
		}}//~MainGrid:Grid
		return 0;
	}


	protected Control _WindowsTitleBar(){
		//
		var WindowsTitleBar = new Grid();
		WindowsTitleBar.Name = nameof(WindowsTitleBar);
		WindowsTitleBar.ColumnDefinitions.AddRange([
			new ColumnDefinition{Width = GridLength.Auto}
			,new ColumnDefinition{Width = new GridLength(4)}
			,new ColumnDefinition{Width = GridLength.Auto}
			,new ColumnDefinition{Width = new GridLength(16)}
			,new ColumnDefinition{Width = GridLength.Auto}
		]);
		{{//WindowsTitleBar:Grid
			var AppIconImage = new Image(){
				Width = 24.0
				,Height=24.0
				,Source=new Bitmap(AssetLoader.Open(new Uri("avares://DupeClear/Assets/Icons/DupeClear.ico")))//似從項目根目錄
			};
			WindowsTitleBar.Children.Add(AppIconImage);
			AppIconImage.Name = nameof(AppIconImage);
			RenderOptions.SetBitmapInterpolationMode(
				AppIconImage
				,BitmapInterpolationMode.HighQuality
			);
			//--
			var AppTitleTextBlock = new TextBlock(){
				VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
				,Text = "Dupe Clear"
			};
			WindowsTitleBar.Children.Add(AppTitleTextBlock);
			AppTitleTextBlock.Name = nameof(AppTitleTextBlock);
			Grid.SetColumn(AppTitleTextBlock, 2);
			AppTitleTextBlock.Bind(
				TextBlock.ForegroundProperty
				,new DynamicResourceExtension("DupeClearTitleBarForegroundBrush")
			);
			//
			WindowsTitleBar.Children.Add(_menu());
		}}//~WindowsTitleBar:Grid
		return WindowsTitleBar;
	}

	protected Control _menu(){
		var MainMenu = new Menu(){
			VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
		};
		Grid.SetColumn(MainMenu, 4);
		{{//MainMenu:Menu
			var FileMenuItem = new MenuItem(){
				Header = "_File"
				,CornerRadius = new CornerRadius(0)//TODO 以css批量設
			};
			MainMenu.Items.Add(FileMenuItem);
			{{//FileMenuItem:MenuItem
				//
				var Theme = new MenuItem(){
					Header = "_Theme"
					//Header = new TextBlock(){Text = "_Theme"}//t
					,CornerRadius = new CornerRadius(0)//TODO 以css批量設
				};
				FileMenuItem.Items.Add(Theme);
				{{//Theme:MenuItem
					Theme.Items.Add(themeMenuItem("Auto",DupeClear.Models.Theme.Auto));
					Theme.Items.Add(themeMenuItem("Dark",DupeClear.Models.Theme.Dark));
					Theme.Items.Add(themeMenuItem("Light",DupeClear.Models.Theme.Light));
				}}//~Theme:MenuItem
				//
				var Exit = new MenuItem(){
					Header = "_Exit"
				};
				FileMenuItem.Items.Add(Exit);
				{//Exit:MenuItem
					Exit.Bind(
						MenuItem.CommandProperty
						,new Binding(nameof(ctx.CloseCommand))
					);
					var icon = new TextBlock(){};
					Exit.Icon = icon;
					{
						icon.Text = Application.Current?.FindResource("XMark") as str;
						icon.Classes.Add("icon");//TODO nameof
					}
				}//~Exit:MenuItem

			}}//~FileMenuItem:MenuItem
			//
			var HelpMenuItem = new MenuItem(){
				Header = "_Help"
			};
			MainMenu.Items.Add(HelpMenuItem);
			{{//HelpMenuItem:MenuItem
				//
				var CheckForUpdates = new MenuItem(){
					Header = "_Check for Updates"
				};
				HelpMenuItem.Items.Add(CheckForUpdates);
				{{//CheckForUpdates:MenuItem
					//
					CheckForUpdates.Bind(
						MenuItem.CommandProperty
						,new Binding(nameof(ctx.CheckForUpdatesCommand))
					);
					var icon = new TextBlock(){};
					CheckForUpdates.Icon = icon;
					{
						icon.Text = Application.Current?.FindResource("Download") as str;
					}
				}}//~CheckForUpdates:MenuItem
				HelpMenuItem.Items.Add(new Separator());

				var About = new MenuItem(){
					Header = "_About"
				};
				HelpMenuItem.Items.Add(About);
				{{//About:MenuItem
					About.Bind(
						MenuItem.CommandProperty
						,new Binding(nameof(ctx.ShowAboutCommand)) //TODO 不效、宜察原ʹ叶
					);
				}}//~About:MenuItem
			}}//~HelpMenuItem:MenuItem
		}}//~MainMenu:Menu
		return MainMenu;
	}

	protected MenuItem themeMenuItem (
		str header
		,object? commandParameter
	){
		//
		var ans = new MenuItem(){
			Header = header
			,CommandParameter =  commandParameter
		};
		{//Auto:MenuItem
			//
			ans.Bind(
				MenuItem.CommandParameterProperty
				,new Binding(nameof(ctx.ChangeThemeCommand))
			);
			//
			var icon = new TextBlock(){};
			ans.Icon = icon;
			{
				var res = Application.Current?.FindResource("CircleCheck");
				if(res is string s){
					icon.Text = s;
				}else{
					icon.Text = "error";
				}
				icon.Classes.Add("icon");//TODO nameof
				icon.Bind(
					TextBlock.IsVisibleProperty
					,new Binding(nameof(ctx.Theme)){
						Converter = new IntToTrueConverter()
						,ConverterParameter = commandParameter
					}
				);
			}
		}//~Auto:MenuItem
		return ans;
	}//~fn
}