﻿// Copyright (C) 2024 Antik Mozib. All rights reserved.

using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace DupeClear.Converters;

public class BoolToInvertedBoolConverter : IValueConverter {
	protected static BoolToInvertedBoolConverter? _inst = null;
	public static BoolToInvertedBoolConverter inst=> _inst??= new BoolToInvertedBoolConverter();
		
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is bool boolValue) {
			return !boolValue;
		}

		return null;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		throw new NotImplementedException();
	}
}
