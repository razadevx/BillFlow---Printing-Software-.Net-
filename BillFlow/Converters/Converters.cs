using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using BillFlow.Models;

namespace BillFlow.Converters;

public class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility.Collapsed;
    }
}

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is not true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is not true;
    }
}

public class IsSelectedConverter : IValueConverter
{
    public string TargetValue { get; set; } = "";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() == TargetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return System.Windows.Data.Binding.DoNothing;
    }
}

public class BooleanToOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? 1.0 : 0.0;
        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return System.Windows.Data.Binding.DoNothing;
    }
}

public class ActiveNavForegroundConverter : IValueConverter
{
    private static readonly SolidColorBrush ActiveBrush = new(Color.FromRgb(255, 255, 255));
    private static readonly SolidColorBrush InactiveBrush = new(Color.FromRgb(75, 85, 99));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? ActiveBrush : InactiveBrush;
        return InactiveBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return System.Windows.Data.Binding.DoNothing;
    }
}

/// <summary>Compares CurrentRoute with nav item Name for sidebar highlight.</summary>
public class RouteMatchConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2)
            return false;
        return string.Equals(values[0]?.ToString(), values[1]?.ToString(), StringComparison.Ordinal);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return new[] { System.Windows.Data.Binding.DoNothing };
    }
}

// Credit Risk Converters
public class RiskToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CreditRiskLevel risk)
        {
            return risk switch
            {
                CreditRiskLevel.Clear => new SolidColorBrush(Color.FromRgb(236, 253, 245)),     // Light green
                CreditRiskLevel.Moderate => new SolidColorBrush(Color.FromRgb(254, 243, 199)),  // Light yellow
                CreditRiskLevel.HighRisk => new SolidColorBrush(Color.FromRgb(254, 226, 226)),  // Light red
                _ => new SolidColorBrush(Colors.White)
            };
        }
        return new SolidColorBrush(Colors.White);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}

public class RiskToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CreditRiskLevel risk)
        {
            return risk switch
            {
                CreditRiskLevel.Clear => new SolidColorBrush(Color.FromRgb(22, 163, 74)),      // Green
                CreditRiskLevel.Moderate => new SolidColorBrush(Color.FromRgb(202, 138, 4)),   // Yellow/Orange
                CreditRiskLevel.HighRisk => new SolidColorBrush(Color.FromRgb(220, 38, 38)),  // Red
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}

public class RiskToDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CreditRiskLevel risk)
        {
            return risk switch
            {
                CreditRiskLevel.Clear => "Clear",
                CreditRiskLevel.Moderate => "Moderate",
                CreditRiskLevel.HighRisk => "High Risk",
                _ => "Unknown"
            };
        }
        return "Unknown";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
