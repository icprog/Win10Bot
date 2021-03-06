using cmRobot.Element.Internal;
using System;

using System.Threading;

namespace cmRobot.Element.Components
{

	/// <summary>
    /// An abstract base class for <c>ElementComponenets</c> that are queryable.
	/// </summary>
	/// <include file='Docs\remarks.xml' path='/remarks/remarks[@name="QueryableComponentBase"]'/>
	public abstract class QueryableComponentBase : ElementComponent, IQueryableComponent
	{

		#region Public constants

		/// <summary>
		/// The default value for the <c>UpdateFrequency</c> property.
		/// </summary>
		public const int UpdateFrequencyDefault = 300;

		/// <summary>
		/// The default value for the <c>Enabled</c> property.
		/// </summary>
		public const bool EnabledDefault = true;

		#endregion


		#region Ctors

		internal QueryableComponentBase()
		{
			commJob = new CommJob(this);
			timer = new Timer(new TimerCallback(timer_Elapsed), 
				null, Timeout.Infinite, Timeout.Infinite);
		}

		#endregion


		#region CommJobs

		private class CommJob : ICommunicationJob
		{
			private QueryableComponentBase owner;

			public CommJob(QueryableComponentBase owner)
			{
				this.owner = owner;
			}

			public string GenerateCommand()
			{
				return owner.GenerateCommand();
			}

			public void ProcessResponse(string response)
			{
				owner.ProcessResponse(response);
			}

		}

		#endregion


		#region Public Properties

		/// <summary>
        /// The frequency, in millisecinds, that the corresponding Element-board 
		/// component values are queried.
		/// </summary>
		public int UpdateFrequency
		{
			get { return freq; }

			set 
			{
				Toolbox.AssertInRange(value, 0, Int32.MaxValue);
				freq = value; 
			}
		}

		/// <summary>
        /// Turns querying of this <c>ElementComponent</c> on/off.
		/// </summary>
		public bool Enabled
		{
			get { return enabled; }

			set 
			{
				if (!enabled && value)
				{
					enabled = value;
					SetTimer();
				}
				else if (enabled && !value)
				{
					enabled = value;
					DisableTimer();
				}
			}
		}

		#endregion


		#region Protected Methods

		/// <summary>
		/// Overriden to start a timer.  When the timer has elapsed, the corresponding
        /// Element-board component values are queried.  The frequency of this timer
		/// is set by <c>UpdateFrequency</c>.
 		/// </summary>
		protected override void OnStartCommunication()
		{
			base.OnStopCommunication();

			SetTimer();
		}

		/// <summary>
		/// Overriden to start a timer.
		/// </summary>
		protected override void OnStopCommunication()
		{
			base.OnStopCommunication();

			DisableTimer();
		}

		/// <summary>
		/// Derived classes should provide an implementation that
        /// reads that generates a Element command string to query
		/// sensor value.
		/// </summary>
		protected abstract string GenerateCommand();

		/// <summary>
		/// Derived classes should provide an implementation that
		/// processes the reposnse string that will be sent in response
		/// to the command string generated by GenerateCommand().
		/// </summary>
		protected abstract void ProcessResponse(string response);

		#endregion

		#region Public Methods

		/// <summary>
		/// Allows a user to manually update the value of the component,
		/// instead of relying on it to be periodically polled for a value.
		/// </summary>
		public void Update()
		{
			DisableTimer();
			Element.CommunicationTask.EnqueueCommJobAndWait(Priority.High, commJob);
			SetTimer();
		}

		#endregion


		#region Private Methods

		private void SetTimer()
		{
			if (enabled)
			{
				timer.Change(freq, Timeout.Infinite);
			}
		}

		private void DisableTimer()
		{
			timer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		private void timer_Elapsed(object state)
		{
			DisableTimer();
			Element.CommunicationTask.EnqueueCommJob(Priority.Low, commJob);
			SetTimer();
		}

		#endregion 


		#region Privates

		private Timer timer;
		private int freq = UpdateFrequencyDefault;
		private bool enabled = EnabledDefault;
		private CommJob commJob;

		#endregion

	}

}
