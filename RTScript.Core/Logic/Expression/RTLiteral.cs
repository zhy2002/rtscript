﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RTScript.Core.Logic.Execution;

namespace RTScript.Core.Logic.Expression
{
    internal class RTLiteral : RTExpression
    {
        private enum ParsingState
        {
            Neutral,
            CheckingMarker
        }

        private string _value;

        protected override void ParseInternal(string srcText, RTParsingContext context)
        {
            StringBuilder sb = new StringBuilder();
            int index = SourceStartIndex;
            ParsingState state = ParsingState.Neutral;

            while (index < srcText.Length)
            {
                char currentChar = srcText[index];
                switch (state)
                {
                    case ParsingState.Neutral:
                        if (currentChar == EXPR_MARKER) state = ParsingState.CheckingMarker;
                        else sb.Append(currentChar);
                        break;
                    case ParsingState.CheckingMarker:
                        if (currentChar == EXPR_START)
                        {
                            index--;
                            goto EXIT_LOOP;
                        }
                        else if (currentChar == EXPR_END)
                        {
                            if (context.MarkerDepth == 0)
                            {
                                sb.Append(EXPR_MARKER).Append(EXPR_END);
                                state = ParsingState.Neutral;
                            }
                            else
                            {
                                index--;
                                goto EXIT_LOOP;
                            }
                        }
                        else if (currentChar == EXPR_MARKER)
                        {
                            sb.Append(EXPR_MARKER);
                            state = ParsingState.Neutral;
                        }
                        else
                        {
                            sb.Append(EXPR_MARKER).Append(currentChar);
                            state = ParsingState.Neutral;
                        }
                        break;

                    default:
                        throw new NotSupportedException();
                }
                index++;
            }

            if (state == ParsingState.CheckingMarker)
            {
                sb.Append(EXPR_MARKER);
            }
        EXIT_LOOP:
            context.SourceEndIndex = index - 1;
            _value = sb.ToString();
        }

        public override object Execute(RTExecutionContext context)
        {
            return _value;
        }

        public override string ToString()
        {
            return _value;
        }

    }
}
