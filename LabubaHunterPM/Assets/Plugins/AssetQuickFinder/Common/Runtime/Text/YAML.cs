//------------------------------------------------------------//
// yanfei 2025.3.1
// black_qin@hotmail.com
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;

namespace QuickFinder.Text.YAML
{
    public struct StringSpan
    {
        string buffer;
        int start;
        int count;
        static readonly char[] KEYWORDS;
        public static StringSpan Null { get { return new StringSpan(null, -1, -1); } }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Invalid() { return buffer == null || start < 0 || count < 0; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Valid() { return !Invalid(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public char Head() { return buffer[start]; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]public char Tail() { return buffer[start + count - 1]; }

        static StringSpan()
        {
            KEYWORDS = new char[256];
            KEYWORDS[' '] = ' '; KEYWORDS['\t'] = '\t'; KEYWORDS['('] = '('; KEYWORDS[')'] = ')'; KEYWORDS['|'] = '|'; KEYWORDS['!'] = '!'; KEYWORDS['^'] = '^'; KEYWORDS['$'] = '$';
        }

        public StringSpan(string buffer, int start, int count)
        {
            this.buffer = buffer; this.start = start; this.count = count;
        }

        public bool Equals(string other)
        {
            if (other.Length != count)
                return false;

            for (int i = 0; i < other.Length; i++)
            {
                if (buffer[start + i] != other[i])
                { return false; }
            }
            return true;
        }

        public bool MatchOne(string other)
        {
            bool result = false;
            bool lastResult = false;
            int otherIndex = other.Length - 1;
            int rightBracket = 0;
            int bufferIndex = start + count - 1;
            for (; ;)
            {
                if(otherIndex < 0)
                { break; }

                var oc = other[otherIndex];
                if(oc == ' ' || oc == '\t' || oc == '^' || oc == '$')
                {
                    otherIndex--;
                    continue;  
                }

                if(oc == ')')
                {
                    rightBracket++;
                    otherIndex--;
                    continue; ; 
                }
                if(oc == '(')
                {
                    if(rightBracket == 0)
                    { return false; }

                    result = result || lastResult;
                    lastResult = false;
                    rightBracket--;
                    otherIndex--;
                    continue;
                }
                if(oc == '|')
                {
                    lastResult = result;
                    result = false;
                    otherIndex--;
                    continue;
                }
                if(oc == '!')
                {
                    result = !result;
                    otherIndex--;
                    continue;
                }

                for (; ; )
                {
                    if (buffer[bufferIndex] != oc)
                    {
                        result = false;
                        bufferIndex = start + count - 1;
                        for (; ; )
                        {
                            otherIndex--;
                            if (otherIndex < 0)
                            { break; }
                            var nextoc = other[otherIndex];
                            //if (nextoc == ' ' || nextoc == '\t' || nextoc == '!' || nextoc == '(' || nextoc == ')' || nextoc == '|' || oc == '^' || oc == '$')
                            if (KEYWORDS[nextoc] != 0)
                            {
                                break;
                            }
                        }
                        break;
                    }

                    bufferIndex--;

                    if(bufferIndex < start)
                    {
                        if(otherIndex <= 0)
                        {
                            result = true;
                            otherIndex--;
                            break;
                        }
                        else
                        {
                            var nextoc = other[otherIndex - 1];
                            //if(nextoc == ' ' || nextoc == '\t' || nextoc == '!' || nextoc == '(' || nextoc == ')' || nextoc == '|' || oc == '^' || oc == '$')
                            if (KEYWORDS[nextoc] != 0)
                            {
                                result = true;
                                bufferIndex = start + count - 1;
                                otherIndex--;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if(otherIndex > 0)
                        {
                            otherIndex--;
                            oc = other[otherIndex];
                            continue;
                        }
                    }

                    result = false;
                    break;
                }
            }

            return result || lastResult;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            if (!Valid())
            { return ""; }

            if(buffer[start] == '\'')
            {
                buffer.Substring(start + 1, count - 1);
            }
            return buffer.Substring(start, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ToInteger()
        {
            long sign = 1;
            long value = 0;
            int index = start;
            for (; index < start + count; index++)
            {
                var c = buffer[index];
                if (c == '-')
                {
                    sign = -1;
                    continue;
                }

                if (c >= 48 && c <= 57)
                {
                    value = value * 10 + (c - 48);
                    continue;
                }
                break;
            }
            value = sign * value;

            return value;
        }
    }

    public struct Stream
    {
        public enum EConsumeWhen
        { 
            Never,
            Always,
            Success,
        }

        string stream;
        int start;
        int count;
        int current;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Current() { { return current; } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char PrevToken() {{ return stream[current - 1]; } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char CurrentToken() { { return stream[current]; } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char NextToken(int len = 1)  { return stream[current + len];  }

        public string DebugText
        {
            get
            {
                if (current + 32 < start + count)
                {
                    return stream.Substring(current, 32);
                }
                return stream.Substring(current + 32);
            }
        }

        public override string ToString()
        {
            return DebugText;
        }

        public Stream(string s_, int start_, int count_)
        {
            stream = s_; start = start_; count = count_; current = start;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEndOfStream() { return current >= start + count; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEndOfLine() 
        { 
            return CurrentToken() == '\n' || (CurrentToken() == '\r' && NextToken() == '\n'); 
        }

        public bool NextIsEOS(int dis = 1) { return current + dis >= start + count; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Advance(int len)
        {
            if (NextIsEOS(len))
            { return false; }
            current += len;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(bool success, EConsumeWhen consume)
        {
            if((success && consume == EConsumeWhen.Success) || consume == EConsumeWhen.Always)
            { current++; }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int len, bool success, EConsumeWhen consume)
        {
            if ((success && consume == EConsumeWhen.Success) || consume == EConsumeWhen.Always)
            { current += len; }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ToNextLine()
        {
            while (stream[current] != '\n')
            {
                current++;
                if(IsEndOfStream())
                { return false; }
            }
            current++;

            return true;
        }
        public bool ToLastEndOfLine()
        {
            int index = current;
            while(stream[index] != '\n')
            { 
                if(index == 0)
                { return false; }
                index--; 
            }
            current = index;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ToBackOf(char c, out int indent, EConsumeWhen consume = EConsumeWhen.Success, bool toNextLine = false)
        {
            int index = current;
            indent = 0;
            for (; ; )
            {
                if (IsEndOfStream())
                { return false; }
                var t = stream[index];
                if(t == c)
                { break; }
                if((!toNextLine) && t == '\n')
                { return false; }
                index++;
            }
            index++;
            if (stream[index] == '\n' && !toNextLine)
            { return false; }
            indent = index - current;
            if (consume == EConsumeWhen.Always || consume == EConsumeWhen.Success)
            { current = index; }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ToFrontOf(char c, out int indent, EConsumeWhen consume = EConsumeWhen.Success, bool toNextLine = false)
        {
            int index = current;
            indent = 0;
            for (; ; )
            {
                if (IsEndOfStream())
                { return false; }
                var t = stream[index];
                if (t == c)
                { break; }
                if ((!toNextLine) && t == '\n')
                { return false; }
                index++;
            }
            index--;
            indent = index - current;
            if (consume == EConsumeWhen.Always || consume == EConsumeWhen.Success)
            { current = index; }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ToFirstOf(char c, out int indent, EConsumeWhen consume = EConsumeWhen.Success, bool toNextLine = false)
        {
            int index = current;
            indent = 0;
            for (; ; )
            {
                if (IsEndOfStream())
                { return false; }
                var t = stream[index];
                if (t == c)
                { break; }
                if ((!toNextLine) && t == '\n')
                { return false; }
                index++;
            }
            indent = index - current;
            if (consume == EConsumeWhen.Always || consume == EConsumeWhen.Success)
            { current = index; }
            return true;
        }

        public bool ToFirstOfNot(char target1, out int indent, EConsumeWhen consume, bool notToNextLine = true)
        {
            int index = current;
            indent = 0;
            for (; index < current + count; index++)
            {
                if (IsEndOfStream()) 
                { return false; }
                var c = stream[index];

                if ((!notToNextLine) && (c == '\r' || c == '\n'))
                { continue; }

                if (c != target1)
                {
                    if (consume == EConsumeWhen.Always || consume == EConsumeWhen.Success)
                    { current = index; }
                    indent = index - current;
                    return true;
                }
                if (notToNextLine && c == '\n')
                {
                    break;
                }
            }
            indent = 0;
            return false;
        }
        public bool ToFirstOfNot(char target1, char target2, out int indent, EConsumeWhen consume, bool notToNextLine = true)
        {
            int index = current;
            indent = 0;
            for (; index < current + count; index++)
            {
                if (IsEndOfStream())
                { return false; }

                var c = stream[index];

                if ((!notToNextLine) && (c == '\r' || c == '\n'))
                { continue; }

                if (c != target1 && c != target2)
                {
                    if (consume == EConsumeWhen.Always || consume == EConsumeWhen.Success)
                    { current = index; }
                    indent = index - current;
                    return true;
                }
                if (notToNextLine && c == '\n')
                {
                    break;
                }
            }
            indent = 0;
            return false;
        }

        public bool NextLineFirstOfNot(char target1, out int indent)
        {
            int index = current;
            indent = 0;

            while (stream[index] != '\n')
            {
                index++;
                if (IsEndOfStream())
                { return false; }
            }
            index++;

            for (; index < current + count; index++)
            {
                if (IsEndOfStream())
                { return false; }
                var c = stream[index];
                if (c != target1)
                {
                    indent = index - current;
                    return true;
                }
                if (c == '\n')
                {
                    break;
                }
            }
            indent = 0;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Match(char c, EConsumeWhen consume)
        {
            if (IsEndOfStream())
            {
                if(consume == EConsumeWhen.Always)
                Advance(false, consume);
                return false;
            }
            if (stream[current] == c)
            {
                Advance(true, consume);
                return true;
            }
            else
            {
                Advance(false, consume);
                return false;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Match(string text, EConsumeWhen consume)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (IsEndOfStream())
                {
                    Advance(text.Length, false, consume);
                    return false;
                }
                if (stream[current + i] != text[i])
                {
                    Advance(text.Length, false, consume);
                    return false;
                }
            }

            Advance(text.Length, true, consume);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ParseInteger(char end, EConsumeWhen consume, out long value)
        {
            bool match = false;
            long sign = 1;
            value = 0;
            int index = current;
            for (; ;index++)
            {
                var c = stream[index];
                if (c == end || c == '\n' || (c == '\r' && stream[index+1] == '\n'))
                { break; }

                if (c == '-')
                { 
                    sign = -1;
                    continue; 
                }

                if(c >= 48 && c <= 57)
                {
                    match = true;
                    value = value * 10 + (c - 48);
                    continue;
                }
                match = false;
                break;
            }
            value = sign * value;

            if(consume == EConsumeWhen.Always || (match && consume == EConsumeWhen.Success))
            { current = index; }

            return match;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ParseTextUntil(char end, EConsumeWhen consume, out string text, bool notToNextLine = true)
        {
            int index = current;
            int quota = 0;
            for (; index < start + count; index++)
            {
                var c = stream[index];
                if (c == '\'' && stream[index + 1] == '\'')
                {
                    if (quota > 0)
                    {
                        index++;
                        continue;
                    }
                    else
                    { quota = 2; }
                }

                if (c == '\'' && stream[index - 1] != '\'')
                { quota++; }

                if (quota == 2 || ((c == end || ((c == '\n' || c == '\r' && stream[index + 1] == '\n') && notToNextLine)) && (quota == 0 || quota == 2)))
                {
                    text = stream.Substring(current, index - current);
                    if (consume == EConsumeWhen.Always || (consume == EConsumeWhen.Success))
                    { current = index; }
                    return true;
                }
            }
            if (consume == EConsumeWhen.Always)
            { current = index; }
            text = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringSpan ParseTextUntil(char end, EConsumeWhen consume, bool notToNextLine = true)
        {
            int index = current;
            int quota = 0;
            for (; index < start + count; index++)
            {
                var c = stream[index];
                if (c == '\'' && stream[index + 1] == '\'')
                {
                    if (quota > 0)
                    {
                        index++;
                        continue;
                    }
                    else
                    { quota = 2; }
                }

                if (c == '\'' && stream[index - 1] != '\'')
                { quota++; }

                if (quota == 2 || ((c == end || ((c == '\n' || c == '\r' && stream[index + 1] == '\n') && notToNextLine)) && (quota == 0)))
                {
                    var text = new StringSpan(stream, current, index - current);
                    if (consume == EConsumeWhen.Always || (consume == EConsumeWhen.Success))
                    { current = index; }
                    return text;
                }
            }
            if (consume == EConsumeWhen.Always)
            { current = index; }
            return StringSpan.Null;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringSpan ParseTextUntil(char end1, char end2, EConsumeWhen consume)
        {
            int index = current;
            int quota = 0;
            for (; index < start + count; index++)
            {
                var c = stream[index];

                if (c == '\'' && stream[index + 1] == '\'')
                {
                    if (quota > 0)
                    { 
                        index++;
                        continue;
                    }
                    else 
                    { quota = 2; }
                }

                if (c == '\'' && stream[index - 1] != '\'')
                { quota++; }

                if (quota == 2 || ((c == end1 || c == end2) && (quota == 0)))
                {
                    var text = new StringSpan(stream, current, index - current);
                    if (consume == EConsumeWhen.Always || (consume == EConsumeWhen.Success))
                    { current = index; }
                    return text;
                }
            }
            if (consume == EConsumeWhen.Always)
            { current = index; }
            return StringSpan.Null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool LengthToNext(char end, out int lengh)
        {
            int index = current;
            for (; index < start + count; index++)
            {
                var c = stream[index];
                if (c == end)
                {
                    lengh = index - current;
                    return true;
                }
            }
            lengh = 0;
            return false;
        }
    }

    public enum EControl
    {
        FailAndConntinue,
        FailAndStop,
        SuccessAndContinue,
        SuccessAndStop,
    }

    public class ParsingCaller
    {
        public enum ERange
        {
            Any,
            Head,
            Tail,
        }
        public long typeId;
        public long objectId;
        private List<StringSpan> currentHierarchy;
        public StringSpan propertyName;
        public StringSpan propertyValue;


        public int proertyCounter { get; private set; } = 0;
        public bool isCurlyPair { get; set; } = false;

        //private long targetTypeId;
        private List<string> searchHierarchy;
        private List<string> skipHierarchy;
        private bool matchLeafMust = true;

        public Action<ParsingCaller> callback;

        public ParsingCaller() { }

        public ParsingCaller(List<string> targetHierarchy)
        {
            this.searchHierarchy = targetHierarchy;
            currentHierarchy = new List<StringSpan>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushToHierarchy(StringSpan propertyName)
        {
            if (currentHierarchy == null)
            { currentHierarchy = new List<StringSpan>(); }
            currentHierarchy.Add(propertyName);

            if(searchHierarchy == null || searchHierarchy.Count == 0)
            { return; }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopHierarchy()
        {
            if (currentHierarchy == null)
            { return; }

            if (currentHierarchy.Count > 0)
            {
                currentHierarchy.RemoveAt(currentHierarchy.Count - 1);
            }
        }
        //for outside caller
        public StringSpan FirstHierarchy 
        {
            get { return currentHierarchy[0]; }
        }
        public StringSpan ParentHierarchy()
        {
            if(currentHierarchy == null || currentHierarchy.Count <= 1)
            { return StringSpan.Null; }
            return currentHierarchy[currentHierarchy.Count - 2];
        }
        public bool CompareHierarchyReverse(string nodeName, int reversedIndex)
        {
            var index = currentHierarchy.Count - 1 - reversedIndex;
            if(index < 0 || index >= currentHierarchy.Count)
            { return false; }
            return currentHierarchy[index].Equals(nodeName);
        }

        public StringSpan Top()
        {
            if(currentHierarchy != null && currentHierarchy.Count > 0)
            { 
                return currentHierarchy[currentHierarchy.Count - 1];
            }
            return StringSpan.Null;
        }

        public ParsingCaller SearchHierarchy(params string[] checks)
        {
            if(checks == null || checks.Length == 0)
            { return this; } 
            if(searchHierarchy == null)
            { searchHierarchy = new List<string>(); }
            searchHierarchy.Clear();
            searchHierarchy.AddRange(checks);
            return this;
        }

        public ParsingCaller SkipHierarchy(params string[] checks)
        {
            if (checks == null || checks.Length == 0)
            { return this; }

            if(skipHierarchy == null)
            { skipHierarchy = new List<string>(); }

            skipHierarchy.Clear();
            skipHierarchy.AddRange(checks);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParsingCaller MatchLeafMust(bool leafOnly)
        {
            matchLeafMust = leafOnly;
            return this;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMatchLeafMust()
        {  return matchLeafMust; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckBranch()
        {
            return (!matchLeafMust) && CheckLeaf2Root();
        }

        public bool NeedSkip()
        {
            if(skipHierarchy == null)
            { return false; }

            return CheckRoot2Leaf();
        }

        public bool CheckLeaf2Root()
        {
            var currentIndex = currentHierarchy.Count - 1;
            for (int j = searchHierarchy.Count - 1; j >= 0;)
            {
                var searchPattern = searchHierarchy[j];
                if (currentIndex < 0)
                { return false; }
                var currentTarget = currentHierarchy[currentIndex];
                if (currentTarget.MatchOne(searchPattern) || searchPattern == "?")
                {
                    if (j == 0)
                    {
                        if (searchPattern.First() == '^' && currentIndex > 0)
                        { break; }

                        return true;
                    }
                    if (j == searchHierarchy.Count - 1 && searchPattern.Last() == '$' && currentIndex != currentHierarchy.Count - 1)
                    { break; }

                    j--;
                    currentIndex--;
                    continue;
                }
                if (searchPattern == "*")
                {
                    if (j <= 0)
                    { return true; }

                    var nextPattern = searchHierarchy[j - 1];
                    if (nextPattern == "*" || nextPattern == "?")
                    {
                        j--;
                        currentIndex--;
                        continue;
                    }
                    if (currentTarget.MatchOne(nextPattern))
                    {
                        if (searchPattern.First() == '^' && currentIndex > 0)
                        { break; }
                        return true;
                    }

                    currentIndex--;
                    continue;
                }

                if ((!matchLeafMust))
                {
                    if(j == searchHierarchy.Count - 1)
                    { currentIndex--; }
                    else
                    { j = searchHierarchy.Count - 1; }
                    continue;
                }
                return false;
            }

            return false;
        }

        public bool CheckRoot2Leaf()
        {
            var currentIndex = 0;
            for (int j = 0; j < skipHierarchy.Count;)
            {
                var searchPattern = skipHierarchy[j];
                if (currentIndex >= currentHierarchy.Count)
                { return false; }
                var currentTarget = currentHierarchy[currentIndex];
                if (currentTarget.MatchOne(searchPattern) || searchPattern == "?")
                {
                    if (j == skipHierarchy.Count - 1)
                    {
                        if (searchPattern.Last() == '$' && currentIndex < currentHierarchy.Count)
                        { return false; }

                        return true;
                    }
                    if (j == 0 && searchPattern.First() == '^' && currentIndex > 0)
                    { return false; }

                    j++;
                    currentIndex++;
                    continue;
                }
                if (searchPattern == "*")
                {
                    if (j >= skipHierarchy.Count)
                    { return true; }
                    var nextPattern = skipHierarchy[j + 1];
                    if (nextPattern == "*" || nextPattern == "?")
                    {
                        j++;
                        currentIndex++;
                        continue;
                    }
                    if (currentTarget.MatchOne(nextPattern))
                    {
                        if (searchPattern.Last() == '$' && currentIndex < currentHierarchy.Count)
                        { return false; }
                        return true;
                    }

                    currentIndex++;
                    continue;
                }

                if (j == 0)
                { currentIndex++; }
                else
                { j = 0; }
                continue;
            }

            return false;
        }

        public void CheckAndInvoke()
        {
            if (CheckLeaf2Root())
            {
                Invoke(); 
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Invoke() 
        {
            proertyCounter++;
            callback?.Invoke(this); 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetForNewObject()
        {
            typeId = -1; objectId = -1; currentHierarchy?.Clear(); proertyCounter = 0;
        }

        public void Clear()
        {
            ResetForNewObject();
            matchLeafMust = true;
            searchHierarchy.Clear();
        }
    }

    public class ManualParser
    {
        const char SPACE = ' ';
        const char AMPERSAND = '&';
        const char COLON = ':';
        const char COMMA = ',';
        const char HYPHEN = '-';
        const char CURLY_LEFT = '{';
        const char CURLY_RIGHT = '}';
        const char SQUARE_LEFT = '[';
        const char SQUARE_RIGHT = ']';
        const char APOSTROPHE = '\'';
        const char BREAKLINE = '\n';
        const char RETURN = '\r';
        const string ROOTOBJECT = "--- !u!";

        Stream stream;

        ParsingCaller caller;

        public ManualParser()
        {
            //var none_terminals = new byte[256];
            //none_terminals[(int)' '] = 1; none_terminals[(int)'%'] = 1; none_terminals[(int)'-'] = 1; none_terminals[(int)':'] = 1;
            //none_terminals[(int)'!'] = 1; none_terminals[(int)'&'] = 1; none_terminals[(int)'['] = 1; none_terminals[(int)']'] = 1;
            //none_terminals[(int)'{'] = 1; none_terminals[(int)'}'] = 1; none_terminals[(int)'"'] = 1;
            //none_terminals[(int)'\t'] = 1; none_terminals[(int)'\r'] = 1; none_terminals[(int)'\n'] = 1; none_terminals[(int)'\f'] = 1; none_terminals[(int)'\v'] = 1;
        }


        public bool ParseYAML(string input, ParsingCaller caller_)
        {
            this.caller = caller_;
            stream = new Stream(input, 0, input.Length);
            if(! stream.Match("%YAML ", Stream.EConsumeWhen.Success))
            { return false; }
            stream.ToNextLine();
            if (!stream.Match("%TAG ", Stream.EConsumeWhen.Success))
            { return false; }

            stream.ToNextLine();

            ParseObjects();

            DebugPrintLine("parsing yaml completed.");
            return true;
        }

        public EControl ParseObjects()
        {
            while (!stream.NextIsEOS())
            {
                var control = ParseObject();
                if(control == EControl.SuccessAndContinue || control == EControl.FailAndConntinue)
                {
                    continue;
                }
                else
                {
                    return control;
                }
            }
            return EControl.SuccessAndContinue;
        }

        public EControl ParseObject()
        {
            caller.ResetForNewObject();

            #region Object First Line: TyyeId, ObjectId
            if (!stream.Match("--- !u!", Stream.EConsumeWhen.Success))
            {
                return EControl.FailAndConntinue;
            }

            if (!stream.ParseInteger(SPACE, Stream.EConsumeWhen.Success, out var typeId))
            {
                return EControl.FailAndConntinue;
            }

            caller.typeId = typeId;

            stream.ToBackOf(AMPERSAND, out _);
            if (!stream.ParseInteger(SPACE, Stream.EConsumeWhen.Success, out var objectId))
            {
                return EControl.FailAndConntinue;
            }
            #endregion

            caller.objectId = objectId;

            stream.ToNextLine();

            var control = ParsePropertys(0);

            return control;
        }

        public EControl ParsePropertys(int depth)
        {
            while (stream.ToFirstOfNot(SPACE, out var indent, Stream.EConsumeWhen.Never))
            {
                if (indent < depth)
                {
                    break; 
                }

                if(indent > depth)
                {
                    stream.ToNextLine();
                    continue;
                }

                if (stream.Match("--- !u!", Stream.EConsumeWhen.Never))
                {
                    break;
                }

                var control = ParseProperty(depth);
                if (control == EControl.SuccessAndContinue || control == EControl.FailAndConntinue)
                {
                    continue;
                }
                else
                {
                    return control;
                }
            }

            return EControl.SuccessAndContinue;
        }

        public EControl ParseProperty(int depth)
        {
            if(stream.IsEndOfStream())
            {
                return EControl.FailAndStop;
            }

            if (!stream.ToFirstOfNot(SPACE, out int indent, Stream.EConsumeWhen.Never))
            {
                return EControl.FailAndConntinue;
            }

            EControl control;
            if (stream.NextToken(indent) == HYPHEN)
            {
                control = ParsePropertyArray(indent);
            }
            else
            {
                control= ParsePropertyKeyValue(indent);
            }

            return control;
        }

        public EControl ParsePropertyArray(int depth)
        {
            if (stream.IsEndOfStream())
            {
                return EControl.FailAndStop;
            }

            if(!stream.ToFirstOfNot(SPACE, HYPHEN, out int arrayElementIndent, Stream.EConsumeWhen.Never))
            {
                return EControl.FailAndConntinue;
            }

            if(arrayElementIndent <= depth)
            { 
                return EControl.FailAndConntinue; 
            }

            while(stream.ToFirstOfNot(SPACE, out int indent, Stream.EConsumeWhen.Never))
            {
                if ((indent < depth))
                {
                    return EControl.SuccessAndStop; 
                }

                if(indent > depth)
                { 
                    stream.ToNextLine();
                    continue;
                }

                if(indent == depth && stream.NextToken(indent) == HYPHEN)
                {
                    var control = ParsePropertyArrayElement(depth, arrayElementIndent);
                    if (control == EControl.SuccessAndContinue || control == EControl.FailAndConntinue)
                    {
                        continue;
                    }
                    else
                    {
                        return control;
                    }
                }
                else
                {
                    return EControl.SuccessAndStop;
                }
            }

            return EControl.SuccessAndContinue;
        }

        public EControl ParsePropertyArrayElement(int arrayDepth, int depth)
        {
            if(stream.IsEndOfStream())
            { return EControl.FailAndStop; }

            while(stream.ToFirstOfNot(SPACE, out var indent, Stream.EConsumeWhen.Never))
            {
                if (indent < arrayDepth)
                {
                    break; 
                }

                if(indent > depth)
                {
                    stream.ToNextLine();
                    continue; 
                }

                if(indent == arrayDepth && stream.NextToken(indent) != HYPHEN)
                {
                    break;
                }

                var control = ParsePropertyKeyValue(depth);
                if (control == EControl.SuccessAndContinue || control == EControl.FailAndConntinue)
                {
                    continue;
                }
                else
                {
                    return control;
                }
            }

            return EControl.SuccessAndContinue;
        }

        public EControl ParsePropertyKeyValue(int depth)
        {
            if (stream.IsEndOfStream())
            {
                return EControl.FailAndStop;
            }

            //HYPHEN: may be the first line of array element
            if (!stream.ToFirstOfNot(SPACE, HYPHEN, out int indent, Stream.EConsumeWhen.Never))
            {
                return EControl.FailAndConntinue;
            }

            if(indent < depth)
            { return EControl.FailAndConntinue; }

            stream.Advance(indent);

            bool isTupleOrArrayElement = false;
            if(stream.CurrentToken() != CURLY_LEFT && stream.CurrentToken() != SQUARE_LEFT) //1【propertyName: propertyValue】 2【propertyName:\n】  3【propertyName: {...}】  4【propertyName: [...]】  5【- {...}】  6【- [...]】
            {
                caller.propertyName = stream.ParseTextUntil(COLON, Stream.EConsumeWhen.Success);
                if (caller.propertyName.Invalid())
                {
                    return EControl.FailAndConntinue;
                }

                stream.Advance(1);
                if (stream.IsEndOfLine())
                {
                    stream.ToNextLine();
                    if (!stream.ToFirstOfNot(SPACE, out var subDepth, Stream.EConsumeWhen.Never))
                    {
                        return EControl.FailAndConntinue;
                    }
                    //DebugPrintLine(RepeatIndent(depth) + propertyName.ToString() + ":");
                    caller.PushToHierarchy(caller.propertyName);
                    var r = ParsePropertys(subDepth);
                    caller.PopHierarchy();
                    return EControl.SuccessAndContinue;
                }

                stream.Advance(1);
            }
            else
            {
                isTupleOrArrayElement = true;
            }

            EControl control = EControl.SuccessAndContinue;
            if(stream.CurrentToken() == CURLY_LEFT)
            {
                stream.Advance(1);
                if (isTupleOrArrayElement)
                {
                    caller.isCurlyPair = true;
                    control = ParseFlowStylePropetys(depth);
                    caller.isCurlyPair = false;
                }
                else
                {
                    caller.isCurlyPair = true;
                    caller.PushToHierarchy(caller.propertyName);
                    control = ParseFlowStylePropetys(depth);
                    caller.PopHierarchy();
                    caller.isCurlyPair = false;
                }

            }
            else if(stream.CurrentToken() == SQUARE_LEFT)
            {
                stream.Advance(1);

                stream.ToNextLine();
                return EControl.SuccessAndContinue;
                //if (isTupleOrArrayElement)
                //{
                //    control = ParseArrayRawValues(depth);
                //}
                //else
                //{
                //    caller.PushToHierarchy(caller.propertyName);
                //    control = ParseArrayRawValues(depth);
                //    caller.PopHierarchy();
                //}
            }
            else
            {
                caller.propertyValue = stream.ParseTextUntil(RETURN, BREAKLINE, Stream.EConsumeWhen.Success);
                if (caller.propertyValue.Invalid())
                {
                    return EControl.FailAndConntinue;
                }
                stream.ToNextLine();

                caller.PushToHierarchy(caller.propertyName);
                caller.CheckAndInvoke();
                caller.PopHierarchy();
                //DebugPrintLine(string.Format("{0}{1}: {2}", RepeatIndent(depth), propertyName.ToString(), propertyValue.ToString()));
            }
            return control;
        }

        public EControl ParseFlowStylePropetys(int parentDepth)
        {
            //DebugPrint(RepeatIndent(parentDepth) + caller.Top().ToString()); DebugPrint(": {");
            while (stream.CurrentToken() != CURLY_RIGHT)
            {
                caller.propertyName = stream.ParseTextUntil(COLON, Stream.EConsumeWhen.Success);
                //DebugPrint(proertyName.ToString()); DebugPrint(": ");
                stream.ToFirstOfNot(SPACE, COLON, out _, Stream.EConsumeWhen.Success, false);
                caller.propertyValue = stream.ParseTextUntil(COMMA, CURLY_RIGHT, Stream.EConsumeWhen.Success);
                //DebugPrint(proertyValue.ToString()); DebugPrint(", ");
                stream.ToFirstOfNot(SPACE, COMMA, out _, Stream.EConsumeWhen.Success, false);

                caller.PushToHierarchy(caller.propertyName);
                caller.CheckAndInvoke();
                caller.PopHierarchy();
            }
            //DebugPrint("}\n");
            stream.ToNextLine();
            return EControl.SuccessAndContinue;
        }
        public EControl ParseArrayRawValues(int parentDepth)
        {
            int icount = -1;
            //DebugPrint(RepeatIndent(parentDepth) + caller.Top().ToString()); DebugPrint(": [");
            while (stream.CurrentToken() != SQUARE_RIGHT)
            {
                icount++;
                var scount = icount.ToString();
                caller.propertyName = new StringSpan(scount, 0, scount.Length);
                caller.propertyValue = stream.ParseTextUntil(COMMA, SQUARE_RIGHT, Stream.EConsumeWhen.Success);
                //DebugPrint(propertyValue.ToString());DebugPrint(", ");
                stream.ToFirstOfNot(SPACE, out _, Stream.EConsumeWhen.Success, false);

                //caller.PushToHierarchy(caller.propertyName);
                caller.CheckAndInvoke();
                //caller.PopHierarchy();
            }
            //DebugPrint("]\n");
            stream.ToNextLine();
            return EControl.SuccessAndContinue;
        }

        public string RepeatIndent(int count_)
        {
            string n = "";
            for (int i = 0; i < count_; i++)
            {
                n += " ";
            }
            return n;
        }

        public void DebugPrint(string msg)
        {
            Console.Write(msg);
        }
        public void DebugPrintLine(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
