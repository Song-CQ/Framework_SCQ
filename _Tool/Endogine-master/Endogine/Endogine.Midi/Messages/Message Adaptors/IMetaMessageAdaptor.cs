/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 08/01/2004
 */

namespace Endogine.Midi
{
	/// <summary>
	/// Represents the basic functionality of meta message adaptors.
	/// </summary>
	/// <remarks>
	/// The MetaMessage class only provides an indexer for accessing its 
	/// internal data. This can sometimes be inconvenient. Meta message 
	/// adaptors alleviate this inconvenience by providing an easy to use 
	/// interface for getting and setting meta message data. 
	/// </remarks>
	public interface IMetaMessageAdaptor
	{
        /// <summary>
        /// Returns a clone of the adapted meta message.
        /// </summary>
        /// <returns>
        /// A clone of the adapted meta message.
        /// </returns>
        MetaMessage ToMessage();
	}
}
