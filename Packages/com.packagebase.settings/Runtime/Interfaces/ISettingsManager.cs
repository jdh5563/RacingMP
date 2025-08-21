using packageBase.audio;
using packageBase.core;
using packageBase.userInterfaces;

namespace packageBase.settings
{
    public interface ISettingsManager : ISystem, ISubscriber<MenuSliderChangeEvent>, ISubscriber<PlaySoundEventStart>
    {
        
    }
}
