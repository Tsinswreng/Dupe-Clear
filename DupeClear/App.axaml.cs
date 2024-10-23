// Copyright (C) 2024 Antik Mozib. All rights reserved.

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using DupeClear.Native;
using DupeClear.ViewModels;
using DupeClear.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DupeClear;

public partial class App : Application{
	public override void Initialize(){
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted(){
		// Line below is needed to remove Avalonia data validation.
		// Without this line you will get duplicate validations from both Avalonia and CT
		BindingPlugins.DataValidators.RemoveAt(0);
		// 見note.md
		var collection = new ServiceCollection();
		if (OperatingSystem.IsWindows()){
			collection.AddSingleton<IWindowService, Native.Windows.WindowService>();
			collection.AddSingleton<IFileService, Native.Windows.FileService>();
		}
		else if (OperatingSystem.IsLinux()){
			collection.AddSingleton<IFileService, Native.Linux.FileService>();
		}

		collection.AddTransient<MainView>();
		collection.AddTransient<MainViewModel>();

		var services = collection.BuildServiceProvider();
		var mainView = services.GetRequiredService<MainView>();
		var mainViewModel = services.GetRequiredService<MainViewModel>();
		
		mainView.DataContext = mainViewModel;

		DupeClear.Resources.Resources.Culture = new System.Globalization.CultureInfo("en");
		//ApplicationLifetime 被定義在父類
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop){
			var mainWindow = new MainWindow();
			mainWindow.Content = mainView;
			desktop.MainWindow = mainWindow;

			if (desktop.Args != null && desktop.Args.Length > 0){
				Task.Run(async () => await mainViewModel.ImportAsync(desktop.Args.FirstOrDefault()));
			}
		}else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform){
			singleViewPlatform.MainView = mainView;
		}

		base.OnFrameworkInitializationCompleted();
	}
}

/* 

这段代码主要用于处理应用程序的生命周期，具体来说是针对不同类型的应用程序框架进行初始化。

1. **IClassicDesktopStyleApplicationLifetime**: 
   - 代码首先检查 `ApplicationLifetime` 是否为 `IClassicDesktopStyleApplicationLifetime` 类型。如果是，表示应用程序是一个经典桌面应用程序。
   - 在这个条件下，创建一个新的 `MainWindow` 实例，并将其内容设置为 `mainView`。然后，将这个主窗口设置为 `desktop.MainWindow`。
   - 接着，检查 `desktop.Args` 是否不为 `null` 并且长度大于0（意味着命令行参数存在）。如果满足条件，则异步运行一个任务，调用 `mainViewModel.ImportAsync` 方法，并传入第一个命令行参数。这是为了处理可能需要在启动应用时导入的数据。

2. **ISingleViewApplicationLifetime**:
   - 如果 `ApplicationLifetime` 不是 `IClassicDesktopStyleApplicationLifetime`，则检查是否为 `ISingleViewApplicationLifetime` 类型。这通常表示应用程序只有一个视图。
   - 在这种情况下，设置 `singleViewPlatform.MainView` 为 `mainView`，将主视图直接赋值。

总的来说，这段代码是为了在应用程序启动时，根据不同的应用程序类型进行适当的初始化和设置主视图或主窗口。
 */
