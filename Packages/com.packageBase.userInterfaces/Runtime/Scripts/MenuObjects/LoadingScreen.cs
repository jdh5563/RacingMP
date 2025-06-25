using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace packageBase.userInterfaces
{
    /// <summary>
    /// Class used to represent and handle specific functionality for the loading screen menu object.
    /// </summary>
    public class LoadingScreen : Menu
    {
        [SerializeField]
        private GameObject _dotHolder;

        [SerializeField]
        private GameObject _imageHolder;

        [SerializeField]
        private GameObject _loadingBarHolder;

        [SerializeField]
        private bool _showLoadingDots;

        [SerializeField]
        private bool _showImageSlides;

        [SerializeField]
        private bool _showLoadingBar;

        [SerializeField]
        private LoadingBarTypes _loadingBarType;

        [SerializeField, Range(0.05f, 0.5f)]
        private float _delayBetweenDots;

        [SerializeField, Range(1.0f, 5.0f)]
        private float _delayBetweenImages;

        private int _currentActiveDot = 0;
        private int _currentActiveImage = 0;
        private List<Image> _dots = new List<Image>();
        private List<MenuImage> _slideImages = new List<MenuImage>();
        private LoadingBar _loadingBar;

        private bool _doneLoading = false;

        private readonly float _loadingTime = 15.0f;

        private void OnEnable()
        {
            StartCoroutine(handleLoading());

            // Displaying or hiding loading dots.
            if (_dotHolder != null)
            {
                if (_showLoadingDots)
                {
                    _dotHolder.SetActive(true);
                    StartCoroutine(loadingDotsLogic());
                    return;
                }
                else
                {
                    _dotHolder.SetActive(false);
                }
            }

            // Displaying or hiding image slides.
            if (_imageHolder != null)
            {
                if (_showImageSlides)
                {
                    _imageHolder.SetActive(true);
                    StartCoroutine(imageSlideLogic());
                    return;
                }
                else
                {
                    _imageHolder.SetActive(false);
                }
            }

            // Displaying or hiding the loading bar.
            if (_loadingBarHolder != null)
            {
                if (_showLoadingBar)
                {
                    _loadingBarHolder.SetActive(true);
                    StartCoroutine(loadingBarLogic());
                    return;
                }
                else
                {
                    _loadingBarHolder.SetActive(false);
                }
            }
        }

        public override void DoInit()
        {
            base.DoInit();

            // Storing all the images that are children of the loading dot holder.
            if (_dotHolder != null)
            {
                for (int d = 0; d < _dotHolder.transform.childCount; d++)
                {
                    if (_dotHolder.transform.GetChild(d).TryGetComponent(out Image image))
                    {
                        _dots.Add(image);
                    }
                }
            }

            // Storing all the images that are children of the image slide holder.
            if (_imageHolder != null)
            {
                for (int i = 0; i < _imageHolder.transform.childCount; i++)
                {
                    if (_imageHolder.transform.GetChild(i).TryGetComponent(out MenuImage image))
                    {
                        _slideImages.Add(image);
                    }
                }
            }
        }

        public override void DoPostInit()
        {
            base.DoPostInit();

            // Storing all the loading bars that are children of the loading bar holder.
            if (_loadingBarHolder != null)
            {
                for (int i = 0; i < _loadingBarHolder.transform.childCount; i++)
                {
                    GameObject currentLoadingBar = _loadingBarHolder.transform.GetChild(i).gameObject;

                    if (currentLoadingBar.TryGetComponent(out LoadingBar loadingBar))
                    {
                        if (loadingBar.LoadingBarType == _loadingBarType)
                        {
                            _loadingBar = loadingBar;
                        }
                        else
                        {
                            currentLoadingBar.SetActive(false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Helper coroutine responsible for handling when the loading scene should finish.
        /// </summary>
        /// <returns>Delays until the end of each frame for every time increment.</returns>
        private IEnumerator handleLoading()
        {
            float currentTime = 0.0f;

            while (!_doneLoading)
            {
                currentTime += Time.deltaTime;

                // Breaking out of the loop if the loading screen has been going long enough.
                if (currentTime >= _loadingTime)
                {
                    _doneLoading = true;
                }

                yield return null;
            }
        }

        /// <summary>
        /// Helper coroutine responsible for making the loading dots light up.
        /// </summary>
        /// <returns>Delays for the delay between dots duration.</returns>
        private IEnumerator loadingDotsLogic()
        {
            while (!_doneLoading)
            {
                // Setting the color of the current dot to faded.
                Color currentDotColor = _dots[_currentActiveDot].color;
                _dots[_currentActiveDot].color = new Color(currentDotColor.r, currentDotColor.g, currentDotColor.b, 0.1f);

                // Increment the active dot index.
                _currentActiveDot = _currentActiveDot < _dots.Count -1 ? _currentActiveDot + 1 : 0;

                // Setting the color of the next dot to full.
                currentDotColor = _dots[_currentActiveDot].color;
                _dots[_currentActiveDot].color = new Color(currentDotColor.r, currentDotColor.g, currentDotColor.b, 1.0f);

                yield return new WaitForSeconds(_delayBetweenDots);
            }
        }

        /// <summary>
        /// Helper coroutine to handle image fade in and out.
        /// </summary>
        /// <returns>Delays for the duration of between image delays.</returns>
        private IEnumerator imageSlideLogic()
        {
            while (!_doneLoading)
            {
                // Getting the current menu image and starting the fade out for it.
                MenuImage currentImage = _slideImages[_currentActiveImage];
                StartCoroutine(currentImage.HandleFade(false, _delayBetweenImages));

                // Increment the active image index.
                _currentActiveImage = _currentActiveImage < _slideImages.Count - 1 ? _currentActiveImage + 1 : 0;

                // Getting the next menu image and starting the fade in for it.
                currentImage = _slideImages[_currentActiveImage];
                StartCoroutine(currentImage.HandleFade(true, _delayBetweenImages));

                yield return new WaitForSeconds(_delayBetweenImages);
            }
        }

        private IEnumerator loadingBarLogic()
        {
            yield return new WaitUntil(() => _loadingBar != null);

            StartCoroutine(_loadingBar.HandleLoadingBar(_loadingTime));
        }
    }
}
