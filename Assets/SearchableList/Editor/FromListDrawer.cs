using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FromList
{
    public class State
    {
        public int controlID;
        public Event current;
        public Rect position;
        public SerializedProperty property;
    }

    [CustomPropertyDrawer(typeof(FromListAttribute))]
    public class FromListDrawer : SelectionPropertyDrawer
    {
        public class ContentState
        {
            public string text;
        }

        private static GUIContent _tempContent;
        private static Styles _styles;
        private static TextEditor _textEdtior;
        private static bool _dragToPosition;
        private static bool _selectAllOnMouseUp;
        private static bool _dragged;
        private static bool _postPoneMove;
        private static string _orginalText;
        private static char[] _allowedletters = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        static FromListDrawer()
        {
            _textEdtior = new TextEditor();
            _tempContent = new GUIContent();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_styles == null)
            {
                _styles = new Styles();
                _textEdtior.style = _styles.textField;
            }

            base.OnGUI(position, property, label);

            ContentState contentState = (ContentState)GUIUtility.GetStateObject(typeof(ContentState), controlID);
            State state = new State();
            state.controlID = controlID;
            state.position = position;
            state.property = property;
            state.current = Event.current;

            if (contentState.text == null)
            {
                contentState.text = "";
            }

            ProcessEvent(ref state, label, contentState);

            DrawSearchArea(ref state, label, contentState);
        }


        private void ProcessEvent(ref State state, GUIContent label, ContentState contentState)
        {
            Event current = state.current;
            EventType eventType = state.current.GetTypeForControl(state.controlID);

            switch (eventType)
            {
                case EventType.MouseDown:
                    OnMouseDown(state, current);
                    break;
                case EventType.MouseUp:
                    OnMouseUp(state, current);
                    break;
                case EventType.ValidateCommand:
                    OnValidateCommand(current);
                    break;
                case EventType.ContextClick:
                    OnContextClick(state, current);
                    break;
                case EventType.KeyDown:
                    OnKeyDown(state, current);
                    break;
                case EventType.KeyUp:
                    OnKeyUp(state, current, contentState);
                    break;
                case EventType.Repaint:
                    OnPaint(state, label, contentState);
                    break;
            }
        }

        private void OnPaint(State state, GUIContent label, ContentState contentState)
        {
            if (GUIUtility.hotControl == 0)
            {
                EditorGUIUtility.AddCursorRect(state.position, MouseCursor.Text);
            }

            string text;

            if (isSelected)
            {
                text = _textEdtior.text;
            }
            else if (EditorGUI.showMixedValue)
            {
                text = "—";
            }
            else
            {
                text = contentState.text;
            }

            Rect pos = state.position;
            pos.width = EditorGUIUtility.labelWidth;
            _styles.label.Draw(pos, label, -1);
            pos.x += pos.width;
            pos.width = state.position.width - EditorGUIUtility.labelWidth;
            _tempContent.text = text;

            if (isSelected)
            {
                _textEdtior.position = pos;
                _textEdtior.DrawCursor(text);
            }
            else
            {
                _styles.textField.Draw(pos, _tempContent, state.controlID);
            }
        }

        private void OnKeyUp(State state, Event current, ContentState contentState)
        {
            if (GUIUtility.hotControl == controlID)
            {
                GUIUtility.hotControl = 0;

                char letter = (char)current.keyCode;

                if (letter >= 'a' && letter <= 'z')
                {
                    if (current.shift)
                    {
                        letter -= (char)('a' - 'A');
                    }

                    _textEdtior.Insert(letter);

                }
                contentState.text = _textEdtior.text;
                current.Use();
            }
        }

        private void OnKeyDown(State state, Event current)
        {
            if (GUIUtility.keyboardControl == controlID)
            {
                bool flag = false;
                char character = current.character;
                if (isSelected)
                {
                    current.Use();
                    flag = true;
                }
                else if (current.keyCode == KeyCode.Escape)
                {
                    _textEdtior.text = _orginalText;
                    flag = true;
                }
                else if (character == '\n' || character == '\u0003')
                {
                    if (!current.alt && !current.shift && !current.control)
                    {
                        _textEdtior.Insert(character);
                        flag = true;
                        return;
                    }
                    current.Use();
                }
                else if (character == '\t' || current.keyCode == KeyCode.Tab)
                {
                    bool flag2 = _allowedletters == null || Array.IndexOf(_allowedletters, character) != -1;
                    bool flag3 = !current.alt && !current.shift && !current.control && character == '\t';
                    if (flag3 && flag2)
                    {
                        _textEdtior.Insert(character);
                        flag = true;
                    }
                }
                else if (character != '\u0019' && character != '\u001b')
                {
                    bool flag4 = (_allowedletters == null || Array.IndexOf(_allowedletters, character) != -1) && character != '\0';
                    if (flag4)
                    {
                        _textEdtior.Insert(character);
                        flag = true;
                    }
                    else
                    {
                        if (Input.compositionString != "")
                        {
                            _textEdtior.ReplaceSelection("");
                            flag = true;
                        }
                        current.Use();
                    }
                }
            }
        }

        private void OnContextClick(State state, Event current)
        {
            if (state.position.Contains(current.mousePosition))
            {
                if (isSelected)
                {
                    GUIUtility.keyboardControl = controlID;
                    _textEdtior.MoveCursorToPosition(Event.current.mousePosition);
                }
                Event.current.Use();
            }
        }

        private void OnValidateCommand(Event current)
        {
            if (GUIUtility.keyboardControl == controlID)
            {
                string commandName = current.commandName;
                if (commandName != null)
                {
                    if (!(commandName == "Cut") && !(commandName == "Copy"))
                    {
                        if (!(commandName == "Paste"))
                        {
                            if (!(commandName == "SelectAll") && !(commandName == "Delete"))
                            {
                                if (commandName == "UndoRedoPerformed")
                                {
                                    _textEdtior.text = _tempContent.text;
                                    current.Use();
                                }
                            }
                            else
                            {
                                current.Use();
                            }
                        }
                        else if (_textEdtior.CanPaste())
                        {
                            current.Use();
                        }
                    }
                    else if (_textEdtior.hasSelection)
                    {
                        current.Use();
                    }
                }
            }
        }

        private void OnMouseDown(State state, Event current)
        {
            if (state.position.Contains(current.mousePosition))
            {
                if (Event.current.clickCount == 2 && GUI.skin.settings.doubleClickSelectsWord)
                {
                    _textEdtior.MoveCursorToPosition(Event.current.mousePosition);
                    _textEdtior.SelectCurrentWord();
                    _textEdtior.MouseDragSelectsWholeWords(true);
                    _textEdtior.DblClickSnap(TextEditor.DblClickSnapping.WORDS);
                    _dragToPosition = false;
                }
                else if (Event.current.clickCount == 3 && GUI.skin.settings.tripleClickSelectsLine)
                {
                    _textEdtior.MoveCursorToPosition(Event.current.mousePosition);
                    _textEdtior.SelectCurrentParagraph();
                    _textEdtior.MouseDragSelectsWholeWords(true);
                    _textEdtior.DblClickSnap(TextEditor.DblClickSnapping.PARAGRAPHS);
                    _dragToPosition = false;
                }
                else
                {
                    _textEdtior.MoveCursorToPosition(Event.current.mousePosition);
                    _selectAllOnMouseUp = false;
                }
                GUIUtility.keyboardControl = state.controlID;
                GUIUtility.hotControl = state.controlID;
                current.Use();
            }
        }

        private void OnMouseUp(State state, Event current)
        {
            if (GUIUtility.hotControl == state.controlID)
            {
                if (_dragged && _dragToPosition)
                {
                    _textEdtior.MoveSelectionToAltCursor();
                }
                else if (_postPoneMove)
                {
                    _textEdtior.MoveCursorToPosition(Event.current.mousePosition);
                }
                else if (_selectAllOnMouseUp)
                {
                    if (GUI.skin.settings.cursorColor.a > 0f)
                    {
                        _textEdtior.SelectAll();
                    }
                    _selectAllOnMouseUp = false;
                }
                _textEdtior.MouseDragSelectsWholeWords(false);
                _dragToPosition = true;
                _dragged = false;
                _postPoneMove = false;
                if (current.button == 0)
                {
                    GUIUtility.hotControl = 0;
                    current.Use();
                }
            }
        }



        private void DrawSearchArea(ref State state, GUIContent label, ContentState contentState)
        {

        }

        protected override void OnGainedFocus(SerializedProperty property)
        {
            _textEdtior.OnFocus();
        }

        protected override void OnLostFocus(SerializedProperty property)
        {
            _textEdtior.OnLostFocus();
        }
    }
}
