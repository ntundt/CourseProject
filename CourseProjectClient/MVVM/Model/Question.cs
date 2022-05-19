using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProjectClient.MVVM.Model
{
    internal class Question : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Id)));
                Unsaved = true;
            }
        }

        private string _text;
        public string Text
        {
            get => _text; set
            {
                _text = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Text)));
                Unsaved = true;
            }
        }

        private int _index;
        public int Index 
        {
            get => _index;
            set
            {
                _index = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Index)));
                Unsaved = true;
            }
        }

        private int? _mark;
        public int? Mark 
        {
            get => _mark;
            set
            {
                _mark = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Mark)));
                Unsaved = true;
            }
        }

        private int? _maxMark;
        public int? MaxMark 
        { 
            get => _maxMark; 
            set
            {
                _maxMark = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(MaxMark)));
                Unsaved = true;
            } 
        }

        private QuestionType _questionType = QuestionType.SingleChoise;
        public QuestionType QuestionType 
        { 
            get => _questionType; 
            set
            {
                _questionType = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(QuestionType)));
                Unsaved = true;
            }
        }

        private CheckAlgorithm? _checkAlgorithm = DataTransferObject.CheckAlgorithm.FullMatch;
        public CheckAlgorithm? CheckAlgorithm 
        { 
            get => _checkAlgorithm; 
            set
            {
                _checkAlgorithm = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CheckAlgorithm)));
                Unsaved = true;
            } 
        }

        private ObservableCollection<AnswerOption> _answerOptions = new ObservableCollection<AnswerOption>();
        public ObservableCollection<AnswerOption> AnswerOptions 
        { 
            get => _answerOptions;
            set
            {
                _answerOptions = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AnswerOptions)));
                Unsaved = true;
                foreach (var item in _answerOptions)
                {
                    item.PropertyChanged += (_sender, _e) =>
                    {
                        Unsaved = true;
                    };
                }
            }
        }

        private AnswerOption _stringInputAnswerOption = new AnswerOption();
        public AnswerOption StringInputAnswerOption
        {
            get => _stringInputAnswerOption;
            set
            {
                _stringInputAnswerOption = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(StringInputAnswerOption)));
            }
        }

        private bool _unsaved = true;
        public bool Unsaved
        {
            get => _unsaved;
            set
            {
                _unsaved = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Unsaved)));
            }
        }

        public Question()
        {
            AnswerOptions.CollectionChanged += (sender, e) =>
            {
                Unsaved = true;
                foreach (var item in e.NewItems)
                {
                    (item as AnswerOption).PropertyChanged += (_sender, _e) =>
                    {
                        Unsaved = true;
                    };
                }
            };
        }
    }

    internal class AnswerOption : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Id)));
            }
        }

        private string _text;
        public string Text
        {
            get => _text; set
            {
                _text = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Text)));
            }
        }

        private bool _isCorrect;
        public bool IsCorrect 
        { 
            get => _isCorrect;
            set
            {
                _isCorrect = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsCorrect)));
            }
        }

        private bool _isChecked;
        public bool IsChecked 
        { 
            get => _isChecked; 
            set
            {
                _isChecked = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsChecked)));
            }
        }

        private bool _multipleChoiseSelected;
        public bool MultipleChoiseSelected
        {
            get => _multipleChoiseSelected;
            set
            {
                _multipleChoiseSelected = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(MultipleChoiseSelected)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsChecked)));
            }
        }

        private bool _singleChoiseSelected;
        public bool SingleChoiseSelected
        {
            get => _singleChoiseSelected;
            set
            {
                _singleChoiseSelected = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(SingleChoiseSelected)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsChecked)));
            }
        }
    }
}
