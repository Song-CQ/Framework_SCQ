//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE.
//
//  This material may not be duplicated in whole or in part, except for 
//  personal use, without the express written consent of the author. 
//
//  web:    http://www.chronotron.com 
//  email:  ianier@hotmail.com
//
//  Copyright (C) 1999-2003 Ianier Munoz. All Rights Reserved.

using System;

namespace Endogine.Audio
{
	/// <summary>
	/// Delegate used to fill in a buffer
	/// </summary>
	public delegate void PullAudioCallback(IntPtr data, int count);

	/// <summary>
	/// Audio player interface
	/// </summary>
	public interface IAudioPlayer : IDisposable
	{
		int SamplingRate { get; }
		int BitsPerSample { get; }
		int Channels { get; }

		int GetBufferedSize();
		void Play(PullAudioCallback onAudioData);
		void Stop();
	}
}