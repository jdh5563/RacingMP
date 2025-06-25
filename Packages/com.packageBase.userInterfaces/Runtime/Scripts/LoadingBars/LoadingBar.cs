using packageBase.core;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace packageBase.userInterfaces
{
    public abstract class LoadingBar : InitableBase
    {
        [SerializeField]
        protected Image barImage;

        [SerializeField]
        protected Image borderImage;

        protected LoadingBarTypes loadingBarType;

        public LoadingBarTypes LoadingBarType
        {
            get { return loadingBarType; }
        }

        /// <summary>
        /// Function to handle this loading bars functionality.
        /// </summary>
        /// <param name="loadingTime">The amount of time the loading screen will be displayed.</param>
        /// <returns>Delays for the desired amount of time for this loading bar.</returns>
        public abstract IEnumerator HandleLoadingBar(float loadingTime);
    }
}
