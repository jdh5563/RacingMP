using UnityEditor;

namespace packageBase.userInterfaces
{
    /// <summary>
    /// Class used to expose all serializable fields on a menu slider (inherits from type slider).
    /// </summary>
    [CustomEditor(typeof(MenuSlider))]
    public class CustomSliderInspector : Editor
    {

    }
}