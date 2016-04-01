using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Twice.Models.Configuration;
using Twice.Resources;
using WPFLocalizeExtension.Engine;

namespace Twice.ViewModels.Settings
{
	internal class VisualSettings : ViewModelBaseEx, IVisualSettings
	{
		public VisualSettings( IConfig currentConfig )
		{
			var swatches = new SwatchesProvider().Swatches.ToArray();

			AvailableAccentColors = new List<ColorItem>( swatches.Where( a => a.IsAccented ).Select( a =>
				new ColorItem
				{
					Name = a.Name,
					ColorBrush = new SolidColorBrush( a.ExemplarHue.Color )
				}
				) );

			AvailablePrimaryColors = new List<ColorItem>( swatches.Select( a =>
				new ColorItem
				{
					Name = a.Name,
					ColorBrush = new SolidColorBrush( a.ExemplarHue.Color )
				}
				) );

			SelectedPrimaryColor = AvailablePrimaryColors.First( c => c.Name == currentConfig.Visual.PrimaryColor );
			SelectedAccentColor = AvailableAccentColors.First( c => c.Name == currentConfig.Visual.AccentColor );

			AvailableFontSizes = new List<FontSizeItem>();
			foreach( var kvp in new Dictionary<int, string>
			{
				{10, "Tiny"},
				{13, "Small"},
				{16, "Normal"},
				{18, "Large"},
				{20, "Huge"}
			} )
			{
				string name = Strings.ResourceManager.GetString( $"FontSize_{kvp.Value}", LocalizeDictionary.CurrentCulture );
				AvailableFontSizes.Add( new FontSizeItem
				{
					DisplayName = name,
					Size = kvp.Key
				} );
			}

			UseDarkTheme = currentConfig.Visual.UseDarkTheme;
			SelectedFontSize = AvailableFontSizes.FirstOrDefault( f => f.Size == currentConfig.Visual.FontSize );
			SelectedHashtagColor = AvailableAccentColors.FirstOrDefault( c => c.Name == currentConfig.Visual.HashtagColor );
			SelectedLinkColor = AvailableAccentColors.FirstOrDefault( c => c.Name == currentConfig.Visual.LinkColor );
			SelectedMentionColor = AvailableAccentColors.FirstOrDefault( c => c.Name == currentConfig.Visual.MentionColor );
			InlineMedias = currentConfig.Visual.InlineMedia;
		}

		public void SaveTo( IConfig config )
		{
			config.Visual.UseDarkTheme = UseDarkTheme;
			config.Visual.AccentColor = SelectedAccentColor.Name;
			config.Visual.PrimaryColor = SelectedPrimaryColor.Name;

			config.Visual.FontSize = SelectedFontSize.Size;
			config.Visual.HashtagColor = SelectedHashtagColor.Name;
			config.Visual.LinkColor = SelectedLinkColor.Name;
			config.Visual.MentionColor = SelectedMentionColor.Name;

			config.Visual.InlineMedia = InlineMedias;

			Application.Current.Resources["HashtagBrush"] = ThemeManager.GetAccent( config.Visual.HashtagColor ).Resources["HighlightBrush"];
			Application.Current.Resources["LinkBrush"] = ThemeManager.GetAccent( config.Visual.LinkColor ).Resources["HighlightBrush"];
			Application.Current.Resources["MentionBrush"] = ThemeManager.GetAccent( config.Visual.MentionColor ).Resources["HighlightBrush"];
			Application.Current.Resources["GlobalFontSize"] = (double)config.Visual.FontSize;

			var palette = new PaletteHelper();
			palette.SetLightDark( config.Visual.UseDarkTheme );
			palette.ReplaceAccentColor( config.Visual.AccentColor );
			palette.ReplacePrimaryColor( config.Visual.PrimaryColor );
		}

		public ICollection<ColorItem> AvailableAccentColors { get; }
		public ICollection<FontSizeItem> AvailableFontSizes { get; }
		public ICollection<ColorItem> AvailablePrimaryColors { get; }

		public bool InlineMedias
		{
			[DebuggerStepThrough] get { return _InlineMedias; }
			set
			{
				if( _InlineMedias == value )
				{
					return;
				}

				_InlineMedias = value;
				RaisePropertyChanged();
			}
		}

		public ColorItem SelectedAccentColor
		{
			[DebuggerStepThrough] get { return _SelectedAccentColor; }
			set
			{
				if( _SelectedAccentColor == value )
				{
					return;
				}

				_SelectedAccentColor = value;
				RaisePropertyChanged();
			}
		}

		public FontSizeItem SelectedFontSize
		{
			[DebuggerStepThrough] get { return _SelectedFontSize; }
			set
			{
				if( _SelectedFontSize == value )
				{
					return;
				}

				_SelectedFontSize = value;
				RaisePropertyChanged();
			}
		}

		public ColorItem SelectedHashtagColor
		{
			[DebuggerStepThrough] get { return _SelectedHashtagColor; }
			set
			{
				if( _SelectedHashtagColor == value )
				{
					return;
				}

				_SelectedHashtagColor = value;
				RaisePropertyChanged();
			}
		}

		public ColorItem SelectedLinkColor
		{
			[DebuggerStepThrough] get { return _SelectedLinkColor; }
			set
			{
				if( _SelectedLinkColor == value )
				{
					return;
				}

				_SelectedLinkColor = value;
				RaisePropertyChanged();
			}
		}

		public ColorItem SelectedMentionColor
		{
			[DebuggerStepThrough] get { return _SelectedMentionColor; }
			set
			{
				if( _SelectedMentionColor == value )
				{
					return;
				}

				_SelectedMentionColor = value;
				RaisePropertyChanged();
			}
		}

		public ColorItem SelectedPrimaryColor
		{
			[DebuggerStepThrough] get { return _SelectedPrimaryColor; }
			set
			{
				if( _SelectedPrimaryColor == value )
				{
					return;
				}

				_SelectedPrimaryColor = value;
				RaisePropertyChanged();
			}
		}

		public bool UseDarkTheme
		{
			[DebuggerStepThrough] get { return _UseDarkTheme; }
			set
			{
				if( _UseDarkTheme == value )
				{
					return;
				}

				_UseDarkTheme = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _InlineMedias;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private ColorItem _SelectedAccentColor;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private FontSizeItem _SelectedFontSize;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private ColorItem _SelectedHashtagColor;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private ColorItem _SelectedLinkColor;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private ColorItem _SelectedMentionColor;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private ColorItem _SelectedPrimaryColor;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _UseDarkTheme;
	}
}