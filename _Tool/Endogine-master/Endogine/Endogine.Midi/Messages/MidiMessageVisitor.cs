using System;

namespace Endogine.Midi
{
	/// <summary>
	/// Summary description for MidiMessageVisitor.
	/// </summary>
	public abstract class MidiMessageVisitor
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public virtual void Visit(ChannelMessage message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public virtual void Visit(MetaMessage message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public virtual void Visit(SysCommonMessage message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public virtual void Visit(SysExMessage message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public virtual void Visit(SysRealtimeMessage message)
        {
        }
	}
}
