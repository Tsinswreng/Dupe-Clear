using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
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
	protected byte _render(){
		System.Console.WriteLine("reader");
		return 0;
	}
}