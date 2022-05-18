using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseProjectClient.MVVM.UserContols
{
    /// <summary>
    /// Логика взаимодействия для TestCard.xaml
    /// </summary>
    public partial class TestCard : UserControl
    {
        public static readonly DependencyProperty TextProperty;
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                Title.Text = value;
            }
        }

        private bool _questionCountLabelVisible = true;
        public bool QuestionCountLabelVisible
        {
            get => _questionCountLabelVisible;
            set
            {
                _questionCountLabelVisible = value;
                QuestionCountLabel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static readonly DependencyProperty QuestionCountProperty;
        public int QuestionCount
        {
            get => (int)GetValue(QuestionCountProperty);
            set {
                SetValue(QuestionCountProperty, value);
                if (value >= 1)
                {
                    QuestionCountLabel.Content = Format(value);
                    QuestionCountLabelVisible = true;
                }
                else
                {
                    QuestionCountLabelVisible = false;
                }
            }
        }

        private static string Format(TimeSpan timeSpan)
        {
            if (timeSpan.TotalHours >= 1)
            {
                return $"{Math.Floor(timeSpan.TotalHours)} ч {Math.Floor(timeSpan.TotalMinutes)} мин";
            }
            return $"{Math.Floor(timeSpan.TotalMinutes)} мин";
        }

        private static string Format(int questionCount)
        {
            string result = $"{questionCount} вопрос";
            if (questionCount % 10 == 1 && questionCount % 100 != 11)
            {
                return result;
            }
            if (questionCount % 10 >= 2 && questionCount % 10 <= 4)
            {
                return result + "а";
            }
            return result + "ов";
        }

        private bool _timeLimitLabelVisible = true;
        public bool TimeLimitLabelVisible
        {
            get => _timeLimitLabelVisible;
            set
            {
                _timeLimitLabelVisible = value;
                TimeLimitLabel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static readonly DependencyProperty TimeLimitProperty;
        public TimeSpan TimeLimit
        {
            get => (TimeSpan)GetValue(TimeLimitProperty);
            set
            {
                SetValue(TimeLimitProperty, value);
                if (value > new TimeSpan(0, 0, 1)) 
                {
                    TimeLimitLabel.Content = Format(value);
                    TimeLimitLabelVisible = true;
                } 
                else
                {
                    TimeLimitLabelVisible = false;
                }
            }
        }

        public string TimeLimitFormatted
        {
            get => Format(TimeLimit);
        }

        public string QuestionCountFormatted
        {
            get => Format(QuestionCount);
        }

        static TestCard()
        {
            TextProperty = DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(TestCard),
                new FrameworkPropertyMetadata(
                    "",
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender));

            QuestionCountProperty = DependencyProperty.Register(
                "QuestionCount",
                typeof(int),
                typeof(TestCard),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender));

            TimeLimitProperty = DependencyProperty.Register(
                "TimeLimit",
                typeof(TimeSpan),
                typeof(TestCard),
                new FrameworkPropertyMetadata(
                    new TimeSpan(0, 0, 0),
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender));
        }

        public TestCard()
        {
            InitializeComponent();
        }
    }
}
