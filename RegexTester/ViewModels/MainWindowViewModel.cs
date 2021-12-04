using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using RegexTester.Commands;

namespace RegexTester.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public bool IgnoreCase
        {
            get => App.Config.IgnoreCase;
            set => App.Config.IgnoreCase = value;
        }

        public bool Multiline
        {
            get => App.Config.Multiline;
            set => App.Config.Multiline = value;
        }

        public bool ExplicitCapture
        {
            get => App.Config.ExplicitCapture;
            set => App.Config.ExplicitCapture = value;
        }

        public bool Compiled
        {
            get => App.Config.Compiled;
            set => App.Config.Compiled = value;
        }

        public bool Singleline
        {
            get => App.Config.Singleline;
            set => App.Config.Singleline = value;
        }

        public bool IgnorePatternWhitespace
        {
            get => App.Config.IgnorePatternWhitespace;
            set => App.Config.IgnorePatternWhitespace = value;
        }

        public bool RightToLeft
        {
            get => App.Config.RightToLeft;
            set => App.Config.RightToLeft = value;
        }

        public bool ECMAScript
        {
            get => App.Config.ECMAScript;
            set => App.Config.ECMAScript = value;
        }

        public bool CultureInvariant
        {
            get => App.Config.CultureInvariant;
            set => App.Config.CultureInvariant = value;
        }

        public string Pattern { get; set; } = string.Empty;

        public Brush PatternForeground { get; set; } = Brushes.Black;
        public string? PatternError { get; set; }

        public MatchCollection? GetMatches(string input, TimeSpan timeout)
        {
            MatchCollection? result = null;
            try
            {
                if (!string.IsNullOrEmpty(Pattern))
                    result = Regex.Matches(input, Pattern, GetRegexOptions(), timeout);
                SetPatternError(null);
            }
            catch (Exception ex)
            {
                SetPatternError(ex.Message);
            }
            return result;
        }

        private void SetPatternError(string? error)
        {
            PatternError = error;
            if (string.IsNullOrEmpty(error))
                PatternForeground = Brushes.Black;
            else
                PatternForeground = Brushes.Red;
        }

        private RegexOptions GetRegexOptions()
        {
            var ro = RegexOptions.None;
            if (IgnoreCase)
                ro |= RegexOptions.IgnoreCase;
            if (Multiline)
                ro |= RegexOptions.Multiline;
            if (ExplicitCapture)
                ro |= RegexOptions.ExplicitCapture;
            if (Compiled)
                ro |= RegexOptions.Compiled;
            if (Singleline)
                ro |= RegexOptions.Singleline;
            if (IgnorePatternWhitespace)
                ro |= RegexOptions.IgnorePatternWhitespace;
            if (RightToLeft)
                ro |= RegexOptions.RightToLeft;
            if (ECMAScript)
                ro |= RegexOptions.ECMAScript;
            if (CultureInvariant)
                ro |= RegexOptions.CultureInvariant;
            return ro;
        }
    }
}
