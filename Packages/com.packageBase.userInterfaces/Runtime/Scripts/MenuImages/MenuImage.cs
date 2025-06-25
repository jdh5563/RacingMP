using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace packageBase.userInterfaces
{
    public class MenuImage : Image
    {
        public IEnumerator HandleFade(bool fadeIn, float fadeTime)
        {
            float currentFadeTime = 0.0f;

            while (currentFadeTime < fadeTime)
            {
                color = new Color(color.r, color.g, color.b, fadeIn ? currentFadeTime : 1.0f - currentFadeTime);
                currentFadeTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
