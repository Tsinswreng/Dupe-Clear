﻿// Copyright (C) 2024 Antik Mozib. All rights reserved.

using System.Collections.Generic;

namespace DupeClear.Models;

public class FinderResult
{
	public int DuplicateCount { get; set; }

	public List<DuplicateFile> Files { get; } = [];

	public List<SearchDirectory> ExcludedDirectories { get; } = [];

	public Dictionary<string, string> Errors { get; } = [];
}
