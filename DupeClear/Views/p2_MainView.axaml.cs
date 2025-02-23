/* 
蠹:
= 頂工具欄 Help>About 無彈窗
= 頂工具欄 打勾圖標ˇʹ顯ˋ謬
= Locations 諸項 文件夾圖標過大
= 圈i ˉ圖標ˋ未顯
= Label之Target未設
= Filename Pattern之Target未顯
= Filename Pattern 與 Extensions 後之框 未對齊

 */

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;
using DupeClear.Converters;
using DupeClear.Helpers;
using DupeClear.Models;
using DupeClear.Models.MessageBox;
using DupeClear.Native;
using DupeClear.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#region alias
using Resx = DupeClear.Resources.Resources;
using Bd = Avalonia.Data.Binding;
using BdM = Avalonia.Data.BindingMode;
using GL = Avalonia.Controls.GridLength;
using ColDef = Avalonia.Controls.ColumnDefinition;
using RowDef = Avalonia.Controls.RowDefinition;
#endregion



namespace DupeClear.Views;
public partial class MainView{

	public MainViewModel? ctx{
		get{return DataContext as MainViewModel;}
		set{DataContext = value;}
	}

	protected byte _style(){
		var btn = new Style(x=>Selectors.Or(
			x.OfType<Button>(), x.OfType<DropDownButton>()
		));
		Styles.Add(btn);
		{
			btn.Setters.Add(new Setter(){
				Property = HorizontalAlignmentProperty
				,Value = Avalonia.Layout.HorizontalAlignment.Center
			});
		}
		
		var icon = new Style(x=>
			x.OfType<TextBlock>()
			.Class("icon")
		);
		Styles.Add(icon);
		{
			icon.Setters.Add(new Setter{
				Property = VerticalAlignmentProperty
				,Value = Avalonia.Layout.VerticalAlignment.Center
			});

			var ch = new Style(x=>
				x.Nesting().Class("regular")
			);
			icon.Children.Add(ch);
			{
				ch.Setters.Add(new Setter{
					Property = FontFamilyProperty
					,Value = this.FindResource("FontAwesomeRegular")
				});
			}
		}
		var menuItemDis = new Style(x=>
			x.OfType<MenuItem>().Class("disabled")
			.Descendant().OfType<TabControl>().Class("icon")
		);
		Styles.Add(menuItemDis);
		{
			menuItemDis.Setters.Add(new Setter{
				Property = ForegroundProperty//TODO ?
				,Value = new DynamicResourceExtension("DupeClearMenuItemIconDisabledBrush")
			});
		}

		var image1 = new Style(x=>Selectors.Or(
			x.OfType<Image>().Class("listbox-file-icon")
			,x.OfType<TextBlock>().Class("listbox-file-icon")
		));
		Styles.Add(image1);
		image1.Setters.Add(new Setter{
			Property = MarginProperty
			,Value = new Thickness(0,0,4,0)
		});

		var image2 = new Style(x=>
			x.OfType<Image>().Class("listbox-folder-icon")
		);
		Styles.Add(image2);
		{
			image2.Setters.Add(new Setter{
				Property = WidthProperty
				,Value = 16.0
			});
			image2.Setters.Add(new Setter{
				Property = HeightProperty
				,Value = 16.0
			});
			image2.Setters.Add(new Setter{
				Property = VerticalAlignmentProperty
				,Value = Avalonia.Layout.VerticalAlignment.Center
			});
		}

		var textFileIcon = new Style(x=>
			x.OfType<TextBlock>().Class("listbox-file-icon")
		);
		Styles.Add(textFileIcon);
		{
			textFileIcon.Setters.Add(new Setter{
				Property = FontSizeProperty
				,Value = 16.0
			});
		}

		return 0;
	}

	protected byte _render(){
		var MainGrid = new Grid();
		Content = MainGrid;
		MainGrid.Name = nameof(MainGrid);
		MainGrid.RowDefinitions.AddRange([
			new RowDef{Height = GL.Auto}
			,new RowDef{Height = GL.Star}
			,new RowDef{Height = GL.Auto}
		]);
		{{//MainGrid:Grid
			MainGrid.Children.Add(_WindowsTitleBar());
		}}//~MainGrid:Grid
		var MainTabControl = new TabControl(){};
		MainGrid.Children.Add(MainTabControl);
		Grid.SetRow(MainTabControl, 1);
		{{//MainTabControl:TabControl
			MainTabControl.Items.Add(_tab_location());
		}}//~MainTabControl:TabControl

		return 0;
	}


	protected Control _tab_location(){
		//
		var Locations = new TabItem(){
			Header = "Locations"
		};
		{{//Locations:TabItem
			//
			var grid = new Grid(){};
			Locations.Content = grid;
			grid.ColumnDefinitions.AddRange([
				new ColDef{Width = GL.Star}
				,new ColDef{Width = new GL(8)}
				,new ColDef{Width = GL.Auto}
			]);
			{{//grid:Grid
				var grid2 = new Grid(){};
				grid.Children.Add(grid2);
				var idx_grid2 = 1;
				grid2.RowDefinitions.AddRange([
					new RowDef{Height = GL.Star}
					,new RowDef{Height = new GL(16)}
					,new RowDef{Height = GL.Auto}
				]);
				{{//grid2:Grid
					//
					var IncludedDirectoriesListBox = new ListBox(){
						SelectionMode = SelectionMode.Multiple
					};
					grid2.Children.Add(IncludedDirectoriesListBox);
					{//conf IncludedDirectoriesListBox:ListBox
						var o = IncludedDirectoriesListBox;
						idx_grid2++;
						o.Name = nameof(IncludedDirectoriesListBox);
						ScrollViewer.SetHorizontalScrollBarVisibility(
							o
							,Avalonia.Controls.Primitives.ScrollBarVisibility.Auto//可选值:Visible: 始终显示水平滚动条。Hidden: 始终隐藏水平滚动条。Auto: 根据内容的大小自动决定是否显示水平滚动条。
						);
						DragDrop.SetAllowDrop(o, true);//可從他處把文件夾拖至此
						o.Bind(
							ListBox.SelectedItemProperty
							,new Bd(nameof(ctx.SelectedIncludedDirectory)){Mode = BdM.TwoWay}
						);
						o.Bind(
							ListBox.SelectedItemsProperty
							,new Bd(nameof(ctx.SelectedIncludedDirectories))
						);
						o.Bind(
							ListBox.ItemsSourceProperty
							,new Bd(nameof(ctx.IncludedDirectories))
						);
						o.DoubleTapped += IncludedDirectoriesListBox_DoubleTapped;
						{//o.KeyBindings
							//Exception: Cannot find a DataContext to bind to.
							var key_space = new KeyBinding(){Gesture=KeyGesture.Parse("Space")};
							o.KeyBindings.Add(key_space);
							key_space.Bind(
								KeyBinding.CommandProperty
								,new Bd(nameof(ctx.InvertMakingOfSelectedIncludedDirectoryCommand)){
									Source=ctx
								}
							);
							//--
							var key_delete = new KeyBinding(){Gesture=KeyGesture.Parse("Delete")};
							o.KeyBindings.Add(key_delete);
							key_delete.Bind(
								KeyBinding.CommandProperty
								,new Bd(nameof(ctx.RemoveIncludedDirectoryCommand)){
									Source=ctx
								}
							);
						}//~IncludedDirectoriesListBox.KeyBindings
						o.ItemTemplate = new FuncDataTemplate<SearchDirectory>((history, _)=>{
							return _IncludedDirectoriesListBoxItemTemplate(history);
						});
					}//~conf IncludedDirectoriesListBox:ListBox
					{{//IncludedDirectoriesListBox:ListBox

					}}//~IncludedDirectoriesListBox:ListBox
					var border = new Border(){};
					grid2.Children.Add(border);
					{//conf border:Border
						var o = border;
						Grid.SetRow(o, idx_grid2++);
						o.Padding = new Thickness(8);
						o.CornerRadius = new CornerRadius(0);
						o.BorderThickness = new Thickness(1);
						o.Bind(
							Border.BorderBrushProperty
							,new DynamicResourceExtension("DupeClearBorderBrush")
						);
						//o.BorderBrush = Brushes.Yellow;
					}//~conf border:Border
					{{//border:Border
						var grid3 = new Grid(){};
						border.Child = grid3;
						{//conf grid3:Grid
							var o = grid3;
							o.RowDefinitions.AddRange([
								new RowDef(){Height = GL.Auto}
								,new RowDef(){Height = new GL(8)}
								,new RowDef(){Height = GL.Auto}
							]);
						}//~conf grid3:Grid
						{{//grid3:Grid
							grid3.Children.Add(_chkBoxs_matchConf());
							
							var additionalOptions = _additionalOptions();
							grid3.Children.Add(additionalOptions);
							Grid.SetRow(additionalOptions, 2);

						}}//~grid3:Grid
					}}//~border:Border
				}}//~grid2:Grid
			}}//~grid:Grid
			//
		}}//~Locations:TabItem
		return Locations;
	}

	protected Control _UpDownButton(
		str bindCmd
		,str btnResKey
	){
		//
		var ans = new Button(){};
		{//conf up
			var o = ans;
			o.Bind(
				Button.IsVisibleProperty
				,new Bd(nameof(ListBoxItem.IsPointerOver)){//查找祖先类型为ListBoxItem的IsPointerOver属性。这意味着当鼠标悬停在ListBoxItem上时，按钮才会显示。
					RelativeSource = new RelativeSource{AncestorType= typeof(ListBoxItem)}
				}
			);
			o.Bind(
				Button.CommandProperty
				,new Bd(nameof(DataContext)+"."+bindCmd){//查找祖先元素如UserControl的DataContext
					RelativeSource = new RelativeSource{AncestorType= typeof(UserControl)}
				}
			);
			o.Bind(
				Button.CommandParameterProperty
				,new Bd("")
			);
			var text = new TextBlock(){};
			o.Content = text;
			{//conf text
				text.Classes.Add("icon");//TODO nameof
				text.Text = this.FindResource(btnResKey) as str;
			}//~conf text
		}//~conf up
		//
		return ans;

	}



	protected Control _WindowsTitleBar(){
		//
		var WindowsTitleBar = new Grid();//TODO 宜試用WrapPanel
		WindowsTitleBar.Name = nameof(WindowsTitleBar);
		WindowsTitleBar.ColumnDefinitions.AddRange([
			new ColDef{Width = GL.Auto}
			,new ColDef{Width = new GL(4)}
			,new ColDef{Width = GL.Auto}
			,new ColDef{Width = new GL(16)}
			,new ColDef{Width = GL.Auto}
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
						,new Bd(nameof(ctx.CloseCommand))
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
						,new Bd(nameof(ctx.CheckForUpdatesCommand))
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
						,new Bd(nameof(ctx.ShowAboutCommand)) //TODO 不效、宜察原ʹ叶
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
				,new Bd(nameof(ctx.ChangeThemeCommand))
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
					,new Bd(nameof(ctx.Theme)){
						Converter = new IntToTrueConverter()
						,ConverterParameter = commandParameter
					}
				);
			}
		}//~Auto:MenuItem
		return ans;
	}//~fn


	protected Control _IncludedDirectoriesListBoxItemTemplate(SearchDirectory history){
		var ans = new Grid(){};
		//var idx_ans = 1;
		ans.ColumnDefinitions.AddRange([
			new ColDef{Width = new GL(4)}
			,new ColDef{Width = GL.Auto}
			,new ColDef{Width = GL.Auto}
			,new ColDef{Width = GL.Auto}
			,new ColDef{Width = GL.Auto}
			,new ColDef{Width = GL.Star}
			,new ColDef{Width = GL.Auto}
			,new ColDef{Width = new GL(4)}
		]);
		{{//ans:Grid
			var CheckBox_IncludedDirectory = new CheckBox(){
				Focusable = false
			};
			ans.Children.Add(CheckBox_IncludedDirectory);
			{//conf IncludedDirectoryCheckBox
				var o = CheckBox_IncludedDirectory;
				Grid.SetColumn(o, 1);
				o.Name = nameof(CheckBox_IncludedDirectory);
				o.Bind(
					CheckBox.IsEnabledProperty
					,new Bd(nameof(DataContext)+"."+nameof(ctx.IsBusy)){
						RelativeSource = new RelativeSource{AncestorType= typeof(UserControl)}//TODO ?
						,Converter = BoolToInvertedBoolConverter.inst
					}
				);
				o.Bind(
					CheckBox.IsCheckedProperty
					,new Bd(nameof(history.IsMarked)){Mode=BdM.TwoWay}
				);
				o.Bind(
					CheckBox.CommandProperty
					,new Bd(nameof(DataContext)+"."+nameof(ctx.ApplyMarkingToSelectedIncludedDirectoriesCommand))
				);
				o.Bind(
					CheckBox.CommandParameterProperty
					,new Bd("")
				);
				o.Click += IncludedDirectoryCheckBox_Click;
			}//~conf IncludedDirectoryCheckBox
			
			var image = new Image(){};
			ans.Children.Add(image);
			{//conf image
				var o = image;
				Grid.SetColumn(o, 2);
				o.Classes.Add("listbox-file-icon");
				o.Bind(
					Image.IsVisibleProperty
					,new Bd(nameof(history.FolderIcon)){
						Converter = new NullToTrueConverter{Inverted=true}
					}
				);
				o.Bind(
					Image.SourceProperty
					,new Bd(nameof(history.FolderIcon))
				);
			}//~conf image
			var folder = new TextBlock(){};
			ans.Children.Add(folder);
			{//conf folder
				var o = folder;
				Grid.SetColumn(o, 2);
				o.Classes.Add("listbox-file-icon");
				o.Bind(
					TextBlock.IsVisibleProperty
					,new Bd(nameof(history.FolderIcon)){
						Converter = new NullToTrueConverter()
					}
				);
				o.Text = Application.Current?.FindResource("Folder") as str;
			}//~conf folder
			var fullName = new TextBlock(){};
			ans.Children.Add(fullName);
			{//conf fullName
				var o = fullName;
				Grid.SetColumn(o, 3);
				o.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
				o.Bind(
					TextBlock.TextProperty
					,new Bd(nameof(history.FullName))
				);
			}//~conf fullName
			var TriangleExclamation = new TextBlock(){};
			ans.Children.Add(TriangleExclamation);
			{//conf TriangleExclamation
				var o = TriangleExclamation;
				Grid.SetColumn(o, 4);
				o.Classes.Add("icon");
				o.Margin = new Thickness(2,0);
				ToolTip.SetTip(o, "Folder excluded from search");
				o.Bind(
					TextBlock.IsVisibleProperty
					,new Bd(nameof(history.IsExcluded))
				);
				o.Text = Application.Current?.FindResource("TriangleExclamation") as str;
			}//~conf TriangleExclamation
			
			var stackPanel = new StackPanel(){};
			ans.Children.Add(stackPanel);
			{
				var o = stackPanel;
				o.Orientation = Avalonia.Layout.Orientation.Horizontal;
				//System.Console.WriteLine(idx_ans);//7
				Grid.SetColumn(o, 6);
			}
			{{//stackPanel:StackPanel
				stackPanel.Children.Add(_UpDownButton(nameof(ctx.MoveDirectoryUpCommand), "ArrowUp"));
				stackPanel.Children.Add(_UpDownButton(nameof(ctx.MoveDirectoryDownCommand), "ArrowDown"));
			}}//~stackPanel:StackPanel
		}}//~ans:Grid
		return ans;
		//
	}

	protected Control _chkBoxs_matchConf(){
		//
		var wrapPanel = new WrapPanel();
		{
			
		}
		{{//wrapPanel:WrapPanel
			var MatchSameFileNameCheckBox = new CheckBox();
			wrapPanel.Children.Add(MatchSameFileNameCheckBox);
			{//conf MatchSameFileNameCheckBox:CheckBox
				var o = MatchSameFileNameCheckBox;
				o.Name = nameof(MatchSameFileNameCheckBox);
				o.Content = "Match Same Fi_lename";
				o.Margin = new Thickness(0, 0, 12, 0);
				o.Bind(
					CheckBox.IsEnabledProperty
					,new Bd(nameof(ctx.IsBusy)){
						Converter = BoolToInvertedBoolConverter.inst
					}
				);
				o.Bind(
					CheckBox.IsCheckedProperty
					,new Bd(nameof(ctx.MatchSameFileName)){
						Mode = BdM.TwoWay
					}
				);
			}//~conf MatchSameFileNameCheckBox:CheckBox

			var stackPanel = new StackPanel();
			wrapPanel.Children.Add(stackPanel);
			{//conf stackPanel:StackPanel
				var o = stackPanel;
				o.Orientation = Avalonia.Layout.Orientation.Horizontal;
				o.Margin = new Thickness(0, 0, 12, 0);
				o.Spacing = 4.0;
			}//~conf stackPanel:StackPanel
			{{//stackPanel:StackPanel
				var chkBox_matchSameContent = new CheckBox{};
				stackPanel.Children.Add(chkBox_matchSameContent);
				{//conf chkBox_matchSameContent:CheckBox
					var o = chkBox_matchSameContent;
					//var tip = Resources["MatchSameContentsToolTip"];
					var tip = Resx.MatchSameContentsToolTip;
					ToolTip.SetTip(o, tip);
					o.Bind(
						CheckBox.IsCheckedProperty
						,new Bd(nameof(ctx.MatchSameContents)){Mode=BdM.TwoWay}
					);
					o.Content = "Match Same _Content";
					// o.Bind(
					// 	CheckBox.IsEnabledProperty
					// 	,new MultiBinding{
					// 		Converter = AllTrueToTrueConverter.inst
					// 		,Bindings = [
					// 			new Bd(nameof(ctx.IsBusy))
					// 			,new Bd(nameof(ctx.IsSearching)){
					// 				//ElementName
					// 				//Source = this.FindControl<CheckBox>(nameof(MatchSameSizeCheckBox))
					// 				//Source = //TODO
					// 			}
					// 		]
					// 	}
					// );
				}//~conf chkBox_matchSameContent:CheckBox
				
				var icon = new TextBlock{};
				stackPanel.Children.Add(icon);
				{//conf icon:TextBlock
					icon.Classes.Add("icon");//TODO nameof
					icon.Text = this.FindResource("CircleInfo") as str;
					ToolTip.SetTip(icon, DupeClear.Resources.Resources.MatchSameContentsToolTip);
				}//~conf icon:TextBlock
			}}//~stackPanel:StackPanel

			var chkBox_matchSameType = new CheckBox{};
			wrapPanel.Children.Add(chkBox_matchSameType);
			{//conf chkBox_matchSameType:CheckBox
				var o = chkBox_matchSameType;
				o.Content = "Match Same _Type";
				o.Margin = new Thickness(0, 0, 12, 0);
				o.Bind(
					CheckBox.IsEnabledProperty
					,new Bd(nameof(ctx.IsBusy)){
						Converter = BoolToInvertedBoolConverter.inst
					}
				);
				o.Bind(
					CheckBox.IsCheckedProperty
					,new Bd(nameof(ctx.MatchSameType)){Mode=BdM.TwoWay}
				);
			}//~conf chkBox_matchSameType:CheckBox

			var MatchSameSizeCheckBox = new CheckBox{};
			wrapPanel.Children.Add(MatchSameSizeCheckBox);
			{//conf MatchSameSizeCheckBox:CheckBox
				var o = MatchSameSizeCheckBox;
				o.Name = nameof(MatchSameSizeCheckBox);
				o.Content = "Match Same Si_ze";
				o.Margin = new Thickness(0, 0, 12, 0);
				o.Bind(
					CheckBox.IsEnabledProperty
					,new Bd(nameof(ctx.IsBusy)){
						Converter = BoolToInvertedBoolConverter.inst
					}
				);
				o.Bind(
					CheckBox.IsCheckedProperty
					,new Bd(nameof(ctx.MatchSameSize)){Mode=BdM.TwoWay}
				);
			}//~conf MatchSameSizeCheckBox:CheckBox

			var chkBox_matchAcrossFolders = new CheckBox{};
			wrapPanel.Children.Add(chkBox_matchAcrossFolders);
			{//conf chkBox_matchAcrossFolders:CheckBox
				var o = chkBox_matchAcrossFolders;
				o.Content = "Match Across F_olders";
				o.Bind(
					CheckBox.IsEnabledProperty
					,new Bd(nameof(ctx.IsBusy)){
						Converter = BoolToInvertedBoolConverter.inst
					}
				);
				o.Bind(
					CheckBox.IsCheckedProperty
					,new Bd(nameof(ctx.MatchAcrossDirectories)){Mode=BdM.TwoWay}
				);
			}//~conf chkBox_matchAcrossFolders:CheckBox
		}}//~wrapPanel:WrapPanel
		return wrapPanel;
	}

	protected Control _additionalOptions(){
		var ans = new Expander();
		{//conf ans
			var o = ans;
			o.Header = "Additional Options";
			o.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;//TODO ?
			o.BorderThickness = new Thickness(1);
			o.Bind(
				Border.BackgroundProperty
				,new DynamicResourceExtension("DupeClearBorderBrush")
			);
			o.Bind(
				Expander.IsExpandedProperty
				,new Bd(nameof(ctx.AdditionalOptionsExpanded)){Mode=BdM.TwoWay}
			);
		}//~conf ans
		{{//ans:Expander
			var stackPanel = new StackPanel{
				Spacing = 8.0
			};
			ans.Content = stackPanel;
			{{//stackPanel:StackPanel

				var grid_FileNamePattern = new Grid();
				stackPanel.Children.Add(grid_FileNamePattern);
				{//conf grid_FileNamePattern:Grid
					var o = grid_FileNamePattern;
					o.ColumnDefinitions.AddRange([
						new ColDef{Width = GL.Auto, SharedSizeGroup = "LabelGroup"}
						,new ColDef{Width = new GL(8)}
						,new ColDef{Width = GL.Star}
						,new ColDef{Width = new GL(4)}

						,new ColDef{Width = GL.Auto}
					]);
				}//~conf grid_FileNamePattern:Grid
				{{//grid_FileNamePattern:Grid
					//
					var label = new Label{};
					grid_FileNamePattern.Children.Add(label);
					{
						var o = label;
						//Grid.SetColumn(o, 1);
						o.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
						o.Content = "Filename _Pattern:";
						//o.Target="FilenamePatternBox" //TODO
					}
					var FilenamePatternBox = new AutoCompleteBox();
					grid_FileNamePattern.Children.Add(FilenamePatternBox);
					{//conf FilenamePatternBox:AutoCompleteBox
						var o = FilenamePatternBox;
						o.Name = nameof(FilenamePatternBox);
						Grid.SetColumn(o, 2);
						ToolTip.SetTip(o, Resx.FileNamePatternToolTip);
						o.Bind(
							AutoCompleteBox.ItemsSourceProperty
							,new Bd(nameof(ctx.SavedFileNamePatterns))
						);
						o.Bind(
							AutoCompleteBox.TextProperty
							,new Bd(nameof(ctx.FileNamePattern)){Mode = BdM.TwoWay}
						);
						o.Bind(
							AutoCompleteBox.IsEnabledProperty
							,new MultiBinding{
								Converter = AllTrueToTrueConverter.inst
								,Bindings = [
									new Bd(nameof(ctx.IsBusy)){
										Converter = BoolToInvertedBoolConverter.inst
									}
									// ,new Bd(nameof(CheckBox.IsChecked)){
									// 	Source = //TODO
									// }
								]
							}
						);
					}//~conf FilenamePatternBox:AutoCompleteBox
					var icon = new TextBlock{};
					grid_FileNamePattern.Children.Add(icon);
					{//conf icon:TextBlock
						var o = icon;
						o.Classes.Add("icon");//TODO nameof
						Grid.SetColumn(o, 4);
						ToolTip.SetTip(o, Resx.FileNamePatternToolTip);
						o.Text = this.FindResource("CircleInfo") as str;
					}//~conf icon:TextBlock
				}}//~grid_FileNamePattern:Grid
				var grid_Extensions = new Grid();
				stackPanel.Children.Add(grid_Extensions);
				{//conf grid_Extensions:Grid
					var o = grid_Extensions;
					o.ColumnDefinitions.AddRange([
						new ColDef{Width = GL.Auto, SharedSizeGroup = "LabelGroup"}
						,new ColDef{Width = new GL(8)}
						,new ColDef{Width = GL.Star}
						,new ColDef{Width = new GL(4)}
						
						,new ColDef{Width = GL.Auto}
					]);
				}//~conf grid_Extensions:Grid
				{{//grid_Extensions:Grid
					var label = new Label{};
					grid_Extensions.Children.Add(label);
					{//conf label:Label
						var o = label;
						o.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
						o.Content = "_Extensions: ";
						//o.Target = //TODO
					}//~conf label:Label

					var IncludedExtensionsBox = new AutoCompleteBox();
					grid_Extensions.Children.Add(IncludedExtensionsBox);
					{//conf IncludedExtensionsBox:AutoCompleteBox
						var o = IncludedExtensionsBox;
						o.Name = nameof(IncludedExtensionsBox);
						Grid.SetColumn(o, 2);
						ToolTip.SetTip(o, Resx.IncludedExtensionsToolTip);
						o.Bind(
							AutoCompleteBox.IsEnabledProperty
							,new Bd(nameof(ctx.IsBusy))
						);
						o.Bind(
							AutoCompleteBox.ItemsSourceProperty
							,new Bd(nameof(ctx.SavedIncludedExtensions))
						);
						o.Bind(
							AutoCompleteBox.TextProperty
							,new Bd(nameof(ctx.IncludedExtensions)){Mode = BdM.TwoWay}
						);
					}//~conf IncludedExtensionsBox:AutoCompleteBox
					var icon = new TextBlock{};
					grid_Extensions.Children.Add(icon);
					{//conf icon:TextBlock
						var o = icon;
						Grid.SetColumn(o, 4);
						o.Classes.Add("icon");//TODO nameof
						ToolTip.SetTip(o, Resx.IncludedExtensionsToolTip);
						o.Text = this.FindResource("CircleInfo") as str;
					}//~conf icon:TextBlock
				}}//grid_Extensions:Grid
				var grid_minSize = new Grid();
				stackPanel.Children.Add(grid_minSize);
				{//conf grid_minSize:Grid
					var o = grid_minSize;
					o.ColumnDefinitions.AddRange([
						new ColDef{Width = GL.Auto, SharedSizeGroup = "LabelGroup"}
						,new ColDef{Width = new GL(8)}
						,new ColDef{Width = GL.Auto}
						,new ColDef{Width = new GL(8)}
						,new ColDef{Width = GL.Auto}
						,new ColDef{Width = GL.Star}
					]);

				}//~conf grid_minSize:Grid
				{{//grid_minSize:Grid
					var label = new Label{};
					grid_minSize.Children.Add(label);
					{//conf label:Label
						var o = label;
						o.Content = "Mi_nimum Size:";
						o.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
						//o.Target = //TODO
					}//~conf label:Label
					var MinSizeBox = new NumericUpDown();
					grid_minSize.Children.Add(MinSizeBox);
					{//conf MinSizeBox:NumericUpDown
						var o = MinSizeBox;
						o.Name = nameof(MinSizeBox);
						Grid.SetColumn(o, 2);
						o.MinWidth = 128.0;
						o.TextAlignment = Avalonia.Media.TextAlignment.Right;
						o.FormatString = "N0";
						o.Maximum = i64.MaxValue;
						o.Minimum = 0;
						o.Bind(
							NumericUpDown.IsEnabledProperty
							,new Bd(nameof(ctx.IsBusy)){
								Converter = BoolToInvertedBoolConverter.inst
							}
						);
						o.Bind(
							NumericUpDown.ValueProperty
							,new Bd(nameof(ctx.MinimumFileLength)){
								Mode = BdM.TwoWay
								,Converter = BytesToKilobytesConverter.inst
							}
						);
					}//~conf MinSizeBox:NumericUpDown
					var kb = new TextBlock();
					grid_minSize.Children.Add(kb);
					{
						var o = kb;
						Grid.SetColumn(o, 4);
						o.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
						o.Text = "KB";
					}
				}}//~grid_minSize:Grid
				var grid_dateCreated = _dateGrid(
					"Da_te Created:"
					,nameof(ctx.MatchDateCreated)
					,nameof(ctx.DateCreatedTo)
				);
				stackPanel.Children.Add(grid_dateCreated);

				var grid_dateModified = _dateGrid(
					"Date _Modified:"
					,nameof(ctx.MatchDateModified)
					,nameof(ctx.DateModifiedTo)
				);
				stackPanel.Children.Add(grid_dateModified);

				
			}}//~stackPanel:StackPanel
		}}//~ans:Expander
		return ans;
	}

	protected Control _dateGrid(
		str chkBoxContent
		,str chkBoxBd_IsChecked
		,str bd_datePicker_SelectedDate
	){
		var ans = new Grid();
		{//conf grid_dateCreated:Grid
			var o = ans;
			o.ColumnDefinitions.AddRange([
				new ColDef{Width = GL.Auto, SharedSizeGroup = "LabelGroup"}
				,new ColDef{Width = new GL(8)}
				,new ColDef{Width = GL.Auto}
				,new ColDef{Width = new GL(8)}

				,new ColDef{Width = GL.Auto}
				,new ColDef{Width = new GL(16)}
				,new ColDef{Width = GL.Auto}
				,new ColDef{Width = new GL(8)}

				,new ColDef{Width = GL.Auto}
				,new ColDef{Width = new GL(8)}
				,new ColDef{Width = GL.Auto}
				,new ColDef{Width = GL.Star}
			]);
		}//~conf grid_dateCreated:Grid
		{{//grid_dateCreated:Grid
			var DateCheckBox = new CheckBox();
			ans.Children.Add(DateCheckBox);
			{//conf DateCreatedCheckBox:CheckBox
				var o = DateCheckBox;
				//o.Name = nameof(DateCheckBox);
				o.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
				o.Content = chkBoxContent;
				o.Bind(
					CheckBox.IsEnabledProperty
					,new Bd(nameof(ctx.IsBusy)){
						Converter = BoolToInvertedBoolConverter.inst
					}
				);
				o.Bind(
					CheckBox.IsCheckedProperty
					,new Bd(chkBoxBd_IsChecked){Mode=BdM.TwoWay}
				);
			}//conf DateCreatedCheckBox:CheckBox
			var label_from = new Label();
			ans.Children.Add(label_from);
			{//conf label_from:Label
				var o = label_from;
				Grid.SetColumn(o, 2);
				o.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
				o.Content = "From";
			}//~conf label_from:Label
			var DateCreatedFromDatePicker = new CalendarDatePicker();
			ans.Children.Add(DateCreatedFromDatePicker);
			{//conf DateCreatedFromDatePicker:CalendarDatePicker
				var o = DateCreatedFromDatePicker;
				Grid.SetColumn(o, 4);
				o.Name = nameof(DateCreatedFromDatePicker);
				o.Bind(
					CalendarDatePicker.SelectedDateProperty
					,new Bd(nameof(ctx.DateCreatedFrom)){Mode=BdM.TwoWay}
				);
				o.Bind(
					CalendarDatePicker.IsEnabledProperty
					,new MultiBinding{
						Converter = AllTrueToTrueConverter.inst
						,Bindings = [
							new Bd(nameof(CheckBox.IsChecked)){
								Source = DateCheckBox
							}
							,new Bd(nameof(ctx.IsBusy)){
								Converter = BoolToInvertedBoolConverter.inst
							}
						]
					}
				);
			}//~conf DateCreatedFromDatePicker:CalendarDatePicker
			var label_to = new Label();
			ans.Children.Add(label_to);
			{//conf label_to:Label
				var o = label_to;
				Grid.SetColumn(o, 6);
				o.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
				o.Content = "To";
			}//~conf label_to:Label
			var DateCreatedToDatePicker = new CalendarDatePicker();
			ans.Children.Add(DateCreatedToDatePicker);
			{//conf DateCreatedToDatePicker:CalendarDatePicker
				var o = DateCreatedToDatePicker;
				Grid.SetColumn(o, 8);
				o.Bind(
					CalendarDatePicker.SelectedDateProperty
					,new Bd(bd_datePicker_SelectedDate){Mode=BdM.TwoWay}
				);
				o.Bind(
					CalendarDatePicker.IsEnabledProperty
					,new MultiBinding{
						Converter = AllTrueToTrueConverter.inst
						,Bindings = [
							new Bd(nameof(CheckBox.IsChecked)){
								Source = DateCheckBox
							}
							,new Bd(nameof(ctx.IsBusy)){
								Converter = BoolToInvertedBoolConverter.inst
							}
						]
					}
				);
			}
			//TODO toolTipIcon略
		}}//~grid_dateCreated:Grid
		return ans;
	}
}