using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace packageBase.userInterfaces
{
    public class MenuSlider : Slider, ISelectHandler, IDeselectHandler
    {
        [SerializeField]
        private SliderTypes _sliderType;

        public SliderTypes SliderType
        {
            get { return _sliderType; }
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }
}
