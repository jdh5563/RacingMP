using UnityEditor;

namespace packageBase.userInterfaces
{
    /// <summary>
    /// Class used to expose all serializable fields on a menu button (inherits from type Button).
    /// </summary>
    [CustomEditor(typeof(MenuButton))]
    public class CustomButtonInspector : Editor
    {

    }
}