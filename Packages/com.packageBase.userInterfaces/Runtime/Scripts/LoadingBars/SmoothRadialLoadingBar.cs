using System.Collections;
using UnityEngine;

namespace packageBase.userInterfaces
{
    public class SmoothRadialLoadingBar : LoadingBar
    {
        private void Awake()
        {
            loadingBarType = LoadingBarTypes.SmoothRadial;
        }

        public override IEnumerator HandleLoadingBar(float loadingTime)
        {
            // Delaying until the loading bar is full size.
            while (barImage.fillAmount < 1.0f)
            {
                barImage.fillAmount += Time.deltaTime * (1 / loadingTime);

                yield return null;
            }

            barImage.fillAmount = 1.0f;
        }
    }
}
