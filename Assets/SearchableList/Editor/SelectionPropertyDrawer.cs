using UnityEditor;
using UnityEngine;

namespace FromList
{
    public abstract class SelectionPropertyDrawer : PropertyDrawer
    {
        private class State
        {
            public bool wasSelected; 
        }


        private static int _nextID;
        private int _drawerID;
        private int _controlID; 

        public SelectionPropertyDrawer()
        {
            _drawerID = _nextID;
            _nextID++; 
        }

        public bool isSelected
        {
            get
            {
                return GUIUtility.keyboardControl == _controlID;
            }
        }

        public int controlID
        {
            get { return _controlID; }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            State state = (State)GUIUtility.GetStateObject(typeof(State), _controlID); 

            if(isSelected != state.wasSelected)
            {
                if(isSelected)
                {
                    OnGainedFocus(property);
                }
                else
                {
                    OnLostFocus(property);
                }
                state.wasSelected = isSelected;
            }
        }

        protected virtual void OnGainedFocus(SerializedProperty property)
        {
        }

        protected virtual void OnLostFocus(SerializedProperty property)
        {
        }
    }
}
