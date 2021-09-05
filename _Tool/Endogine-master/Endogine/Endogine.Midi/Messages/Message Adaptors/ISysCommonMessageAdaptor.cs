/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 09/21/2004
 */

namespace Endogine.Midi
{
	/// <summary>
	/// Represents the basic functionality of system common message adaptors.
	/// </summary>
	public interface ISysCommonMessageAdaptor
	{
        /// <summary>
        /// Returns a clone of the adapted system common message.
        /// </summary>
        /// <returns>
        /// A clone of the adapted system common message.
        /// </returns>
        SysCommonMessage ToMessage();
	}
}
