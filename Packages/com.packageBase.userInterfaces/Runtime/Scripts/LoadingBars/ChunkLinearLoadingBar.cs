using System.Collections;
using UnityEngine;

namespace packageBase.userInterfaces
{
    public class ChunkLinearLoadingBar : LoadingBar
    {
        [SerializeField, Range(4, 20)]
        private int _segments;

        private void Awake()
        {
            loadingBarType = LoadingBarTypes.ChunkLinear;
        }

        public override IEnumerator HandleLoadingBar(float loadingTime)
        {
            float initialOffset = 1.0f;

            barImage.transform.localScale = new Vector3(barImage.transform.localScale.x / _segments, barImage.transform.localScale.y, barImage.transform.localScale.z);
            float imageWidth = barImage.GetComponent<RectTransform>().rect.width;

            yield return new WaitForSeconds(initialOffset);

            // Drawing a new chunk of the loading bar for each segment of time.
            for (int i = 0; i < _segments; i++)
            {
                float newXPosition = barImage.transform.position.x + (imageWidth / _segments) * i;
                Vector3 nextChunkPosition = new(newXPosition, barImage.transform.position.y, barImage.transform.position.z);

                Instantiate(barImage, nextChunkPosition, Quaternion.identity, transform).gameObject.SetActive(true);

                yield return new WaitForSeconds((loadingTime - initialOffset) / _segments);
            }
        }
    }
}
