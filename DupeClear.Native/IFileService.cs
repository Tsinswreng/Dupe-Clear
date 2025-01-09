// Copyright (C) 2024 Antik Mozib. All rights reserved.
/* #
文件服務接口、于 win與linux 各有叶
 */
namespace DupeClear.Native;

public interface IFileService{
	/// <summary>
	/// Gets the platform-specific name of the Recycle Bin, e.g. "Trash" in Linux.
	/// </summary>
	string RecycleBinLabel { get; }
	//#
	/// <summary>
	/// 打開文件、windows用explorer.exe，linux用shell
	/// </summary>
	/// <param name="fileName"></param>
	void LaunchFile(string? fileName);

	//#
	/// <summary>
	/// 類于 LaunchFile
	/// </summary>
	/// <param name="url"></param>
	void LaunchUrl(string? url);

	//#
	/// <summary>
	/// 顯示指定檔案的所在資料夾，或者如果檔案不存在，則顯示指定路徑的資料夾。
	/// </summary>
	/// <param name="fileName"></param>
	void LaunchContainingDirectory(string? fileName);

	void MoveToRecycleBin(string? fileName);

	//#
	/// <summary>
	/// 取文件描述、常決于後綴。如".txt" -> "文本文件"
	/// </summary>
	/// <param name="fileName"></param>
	/// <returns></returns>
	string GetFileDescription(string? fileName);

	Avalonia.Media.Imaging.Bitmap? GetPreview(string? fileName);

	Avalonia.Media.Imaging.Bitmap? GetFolderIcon(string? directoryName);

	Avalonia.Media.Imaging.Bitmap? GetFileIcon(string? fileName);
}
