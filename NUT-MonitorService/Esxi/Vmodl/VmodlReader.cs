using System;
using System.Collections.Generic;
using System.Text;

namespace NUTMonitor.Esxi.Vmodl
{
    public class VmodlReader
    {
        public VmodlReader(string source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        private Stack<VmodlReaderFrame> _frames = new Stack<VmodlReaderFrame>();
        private VmodlReaderFrame _frame = new VmodlReaderFrame();
        private int _position = 0;

        private static readonly string _tokenUnsetString = @"<unset>";
        private static readonly string _tokenNullString = @"null";
        private static readonly string _tokenTrueString = @"true";
        private static readonly string _tokenFalsetString = @"false";

        public string Source { get; private set; }

        private bool _isLineBreak(char ch) => (ch == '\r') || (ch == '\n');

        public bool Parse()
        {
            bool reading = true;
            bool successfull = _position < Source.Length;

            for (; (_position < Source.Length) && reading && successfull; _position++)
            {
                char ch = Source[_position];
                char? prevCh = (_position > 0) ? (char?)Source[_position - 1] : null;
                bool skip = false;

                VmodlReaderFrame newFrame = null;

                switch (_frame.State)
                {
                    case VmodlReaderState.Initial:
                        newFrame = new VmodlReaderFrame();
                        newFrame.State = VmodlReaderState.ValueStart_15;
                        newFrame.Buffer = string.Empty;

                        _frame.State = VmodlReaderState.Finish_100;
                        _frames.Push(_frame);
                        _frame = newFrame;

                        _position--;

                        break;
                    case VmodlReaderState.TypeNameForObject_1:
                        bool isSpaceOrLinebreak = char.IsWhiteSpace(ch) || _isLineBreak(ch);


                        if (isSpaceOrLinebreak)
                        {
                            skip = true;
                            //if (!string.IsNullOrEmpty(_frame.Buffer))
                            //    reading = false;
                        }
                        else if (char.IsLetter(ch) || (!string.IsNullOrEmpty(_frame.Buffer) && char.IsDigit(ch)) || (ch == '.'))
                            _frame.Buffer += ch;
                        else if (ch == ')')
                        {
                            //_position--; //no longer required
                            reading = false;
                        }
                        else
                        {
                            successfull = false;
                            reading = false;
                        }

                        if (!reading && successfull)
                        {
                            if (_frame.CurrentTokenType == VmodlToken.TypeName)
                            {
                                RawValue = ch.ToString();
                                _frame.CurrentTokenType = VmodlToken.EndTypeName;
                                _frame.State = VmodlReaderState.WaitObjectBody_3;
                            }
                            else
                            {
                                _frame.CurrentTokenType = VmodlToken.TypeName;
                                RawValue = _frame.Buffer;
                                _position--;
                            }

                        }

                        break;
                    case VmodlReaderState.TypeNameForRef_2:
                        if (ch == '\'')
                        {
                            successfull = false;
                            reading = false;
                        }
                        else if (ch == ':')
                        {
                            reading = false;
                            RawValue = _frame.Buffer;
                            _frame.Buffer = string.Empty;

                            _frame.CurrentTokenType = VmodlToken.ReferenceType;
                            _frame.State = VmodlReaderState.RefId_11;
                        }
                        else
                            _frame.Buffer += ch;

                        break;
                    case VmodlReaderState.WaitObjectBody_3:
                        if (char.IsWhiteSpace(ch) || _isLineBreak(ch))
                            skip = true;
                        else if (ch == '{')
                        {
                            _frame.CurrentTokenType = VmodlToken.StartObject;
                            _frame.State = VmodlReaderState.ObjectBody_4;

                            RawValue = ch.ToString();
                            reading = false;
                        }
                        else if (ch == 'n')
                        {
                            _frame.CurrentTokenType = VmodlToken.Null;
                            _frame.State = VmodlReaderState.NullValue_14;

                            _frame.Buffer = ch.ToString();
                        }
                        else
                        {
                            successfull = false;
                            reading = false;
                        }

                        break;
                    case VmodlReaderState.ObjectBody_4:
                        if (char.IsWhiteSpace(ch) || _isLineBreak(ch))
                            skip = true;
                        else if (char.IsLetter(ch))
                        {
                            _frame.Buffer = ch.ToString();
                            _frame.State = VmodlReaderState.PropertyName_5;
                        }
                        else if (ch == '}')
                        {
                            if (_frame.CurrentTokenType == VmodlToken.EndObject)
                            {
                                VmodlReaderFrame oldFrame = _frames.Pop();
                                _frame = oldFrame;
                            }
                            else
                            {
                                RawValue = ch.ToString();
                                _frame.CurrentTokenType = VmodlToken.EndObject;
                                reading = false;
                                _position--;
                            }
                        }
                        else
                        {
                            successfull = false;
                            reading = false;
                        }
                        break;
                    case VmodlReaderState.PropertyName_5:
                        if (char.IsLetter(ch) || (!string.IsNullOrEmpty(_frame.Buffer) && char.IsDigit(ch)))
                            _frame.Buffer += ch;
                        else if (char.IsWhiteSpace(ch) || _isLineBreak(ch))
                            skip = true;
                        else if (ch == '=')
                        {
                            reading = false;

                            _frame.CurrentTokenType = VmodlToken.PropertyName;
                            _frame.State = VmodlReaderState.PropertyValue_6;

                            RawValue = _frame.Buffer;
                            _frame.Buffer = string.Empty;
                        }
                        break;
                    case VmodlReaderState.PropertyValue_6:

                        newFrame = new VmodlReaderFrame();
                        newFrame.State = VmodlReaderState.ValueStart_15;
                        newFrame.Buffer = string.Empty;

                        _frame.State = VmodlReaderState.PropertyEnd_7;
                        _frames.Push(_frame);
                        _frame = newFrame;

                        _position--;

                        break;
                    case VmodlReaderState.PropertyEnd_7:
                        if (char.IsWhiteSpace(ch) || _isLineBreak(ch))
                            skip = true;
                        else
                        {
                            if (ch != ',')
                                _position--;

                            _frame.State = VmodlReaderState.ObjectBody_4;
                            _frame.Buffer = string.Empty;
                            RawValue = string.Empty;
                        }

                        break;
                    case VmodlReaderState.StringValue_8:
                        if (ch == '"' && prevCh != '\\')
                        {
                            reading = false;
                            RawValue = _frame.Buffer;

                            _frame = _frames.Pop();
                            _frame.CurrentTokenType = VmodlToken.String;
                        }
                        else
                            _frame.Buffer += ch;

                        break;
                    case VmodlReaderState.NumberValue_9:
                        if (char.IsDigit(ch))
                            _frame.Buffer += ch;
                        else if (ch == '.')
                        {
                            _frame.Buffer += ch;
                            _frame.CurrentTokenType = VmodlToken.Decimal;
                        }
                        else
                        {
                            reading = false;
                            RawValue = _frame.Buffer;

                            VmodlReaderFrame cFrame = _frame;
                            _frame = _frames.Pop();
                            _frame.CurrentTokenType = cFrame.CurrentTokenType;
                        }

                        break;
                    case VmodlReaderState.UnsetValue_10:
                        switch (ch)
                        {
                            case 'u':
                            case 'n':
                            case 's':
                            case 'e':
                            case 't':
                                _frame.Buffer += ch;
                                break;
                            case '>':
                                _frame.Buffer += ch;
                                reading = false;
                                successfull = _frame.Buffer == _tokenUnsetString;

                                if (successfull)
                                {
                                    RawValue = _frame.Buffer;
                                    _frame = _frames.Pop();
                                    _frame.CurrentTokenType = VmodlToken.Unset;
                                }
                                break;
                            default:
                                reading = false;
                                successfull = false;
                                break;

                        }

                        break;
                    case VmodlReaderState.RefId_11:
                        if (ch == '\'')
                        {
                            if (_frame.CurrentTokenType == VmodlToken.ReferenceId)
                            {
                                RawValue = ch.ToString();
                                _frame = _frames.Pop();
                                _frame.CurrentTokenType = VmodlToken.EndReference;
                            }
                            else
                            {
                                RawValue = _frame.Buffer;
                                _frame.CurrentTokenType = VmodlToken.ReferenceId;
                                _position--;
                            }
                        }
                        else
                            _frame.Buffer += ch;

                        break;
                    case VmodlReaderState.NullValue_14:
                        if ((ch == 'u') || (ch == 'l'))
                            _frame.Buffer += ch;
                        else
                        {
                            reading = false;
                            RawValue = _frame.Buffer;
                            successfull = _frame.Buffer == _tokenNullString;

                            _frame = _frames.Pop();
                            if (successfull)
                                _frame.CurrentTokenType = VmodlToken.Null;

                            if (!(char.IsWhiteSpace(ch) || _isLineBreak(ch)))
                                _position--;
                        }

                        break;
                    case VmodlReaderState.ValueStart_15:

                        if (char.IsWhiteSpace(ch) || _isLineBreak(ch))
                            skip = true;
                        else if (char.IsDigit(ch) || (ch == '-') || (ch == '+'))
                        {
                            _frame.CurrentTokenType = VmodlToken.Integer;
                            _frame.State = VmodlReaderState.NumberValue_9;
                            _frame.Buffer = ch.ToString();
                        }
                        else
                            switch (ch)
                            {
                                case '(':
                                    _frame.CurrentTokenType = VmodlToken.StartTypeName;
                                    _frame.State = VmodlReaderState.TypeNameForObject_1;
                                    _frame.Buffer = string.Empty;

                                    RawValue = ch.ToString();
                                    reading = false;

                                    break;

                                case '\'':
                                    _frame.CurrentTokenType = VmodlToken.StartReference;
                                    _frame.State = VmodlReaderState.TypeNameForRef_2;
                                    _frame.Buffer = string.Empty;

                                    RawValue = ch.ToString();
                                    reading = false;

                                    break;

                                case '"':
                                    _frame.State = VmodlReaderState.StringValue_8;
                                    _frame.CurrentTokenType = VmodlToken.String;
                                    _frame.Buffer = string.Empty;

                                    break;

                                case '<':
                                    _frame.Buffer = ch.ToString();
                                    _frame.State = VmodlReaderState.UnsetValue_10;

                                    break;

                                case 't':
                                    _frame.Buffer = ch.ToString();
                                    _frame.CurrentTokenType = VmodlToken.True;
                                    _frame.State = VmodlReaderState.True_18;

                                    break;

                                case 'f':
                                    _frame.Buffer = ch.ToString();
                                    _frame.CurrentTokenType = VmodlToken.False;
                                    _frame.State = VmodlReaderState.False_19;

                                    break;

                                default:
                                    successfull = false;
                                    reading = false;
                                    break;
                            }

                        break;

                    case VmodlReaderState.NextArrayItem_16:
                        switch (ch)
                        {
                            case ',':
                                _frame.Buffer = string.Empty;
                                _frame.State = VmodlReaderState.WaitForArrayItemBody_17;
                                _frame.CurrentTokenType = VmodlToken.ArrayItemSeparator;
                                RawValue = ch.ToString();
                                reading = false;
                                successfull = true;

                                break;
                            case ']':
                                _frame = _frames.Pop();
                                _frame.CurrentTokenType = VmodlToken.EndArray;
                                RawValue = ch.ToString();
                                reading = false;
                                successfull = true;

                                break;
                            default:
                                if (char.IsWhiteSpace(ch) || _isLineBreak(ch))
                                    skip = true;
                                else
                                {
                                    reading = false;
                                    successfull = false;
                                }

                                break;
                        }
                        break;

                    case VmodlReaderState.WaitForArrayItemBody_17:
                        switch (ch)
                        {
                            case ']':
                                _frame = _frames.Pop();
                                _frame.CurrentTokenType = VmodlToken.EndArray;
                                RawValue = ch.ToString();
                                reading = false;
                                successfull = true;

                                break;
                            default:
                                if (char.IsWhiteSpace(ch) || _isLineBreak(ch))
                                    skip = true;
                                else
                                {
                                    _position--;
                                    _frame.State = VmodlReaderState.ValueStart_15;
                                }

                                break;
                        }

                        break;
                    case VmodlReaderState.True_18:
                        switch (ch)
                        {
                            case 'r':
                            case 'u':
                            case 'e':
                                _frame.Buffer += ch;
                                break;      
                            default:
                                if (char.IsWhiteSpace(ch) || _isLineBreak(ch) || ch == ',')
                                {
                                    reading = false;
                                    successfull = _frame.Buffer == _tokenTrueString;

                                    if (successfull)
                                    {
                                        RawValue = _frame.Buffer;
                                        _frame = _frames.Pop();
                                        _frame.CurrentTokenType = VmodlToken.True;
                                    }
                                }
                                else
                                {
                                    reading = false;
                                    successfull = false;
                                }
                                break;

                        }

                        break;
                    case VmodlReaderState.False_19:
                        switch (ch)
                        {
                            case 'a':
                            case 'l':
                            case 's':
                            case 'e':
                                _frame.Buffer += ch;
                                break;
                            default:
                                if (char.IsWhiteSpace(ch) || _isLineBreak(ch) || ch == ',')
                                {
                                    reading = false;
                                    successfull = _frame.Buffer == _tokenFalsetString;

                                    if (successfull)
                                    {
                                        RawValue = _frame.Buffer;
                                        _frame = _frames.Pop();
                                        _frame.CurrentTokenType = VmodlToken.False;
                                    }
                                }
                                else
                                {
                                    reading = false;
                                    successfull = false;
                                }
                                break;

                        }

                        break;

                                                                     
                    case VmodlReaderState.Finish_100:
                        reading = false;
                        _frame.CurrentTokenType = VmodlToken.EOF;
                        RawValue = string.Empty;

                        break;
                    default:
                        break;
                }
            }

            if ((successfull || (_position == Source.Length)) && reading && (_frame.State == VmodlReaderState.Finish_100))
            {
                _frame.CurrentTokenType = VmodlToken.EOF;
                RawValue = string.Empty;
                successfull = true;
            }

            return successfull;
        }


        public VmodlToken TokenType { get => _frame.CurrentTokenType; }
        public string RawValue { get; private set; }
    }
}
