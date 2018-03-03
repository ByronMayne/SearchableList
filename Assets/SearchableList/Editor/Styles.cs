using UnityEngine;

namespace FromList
{
    public class Styles
    {
        private GUIStyle _textField;
        private GUIStyle _label; 

        public GUIStyle textField
        {
            get { return _textField; }
        }

        public GUIStyle label
        {
            get { return _label; }
        }


        public Styles()
        {
            _textField = new GUIStyle(GUI.skin.textField);
            _label = new GUIStyle(GUI.skin.label);
        }
    }
}
