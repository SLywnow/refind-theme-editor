using UnityEditor;

//based on https://github.com/madsbangh/EasyButtons
namespace SLywnow
{
    /// <summary>
    /// Custom inspector for Object including derived classes.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class ObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            this.DrawEasyButtons();

            // Draw the rest of the inspector as usual
            DrawDefaultInspector();
        }
    }
}
