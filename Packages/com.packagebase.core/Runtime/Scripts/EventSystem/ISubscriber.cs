namespace packageBase.core
{
	// Classes must implement one of these interfaces for each event they wish to sub to
	public interface ISubscriber<TEventType>
	{
		// This function will be called by the EventManager whenever it receives a message of that type
		void OnEventHandler(in TEventType e);
	}
}