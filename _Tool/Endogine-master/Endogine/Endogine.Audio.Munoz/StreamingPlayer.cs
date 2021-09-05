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
using System.IO;
using System.Windows.Forms;
using Microsoft.DirectX.DirectSound;
using System.Runtime.InteropServices;

namespace Endogine.Audio
{
	/// <summary>
	/// An audio streaming player using DirectSound
	/// </summary>
	public class StreamingPlayer : IDisposable, IAudioPlayer
	{
		private const int MaxLatencyMs = 300;

		private class PullStream : Stream 
		{
			public PullStream(PullAudioCallback pullAudio)
			{
				m_PullAudio = pullAudio;
			}

			public override bool CanRead { get { return true; } }
			public override bool CanSeek { get { return false; } }
			public override bool CanWrite { get { return false; } }
			public override long Length { get { return 0; } }
			public override long Position { get { return 0; } set {} }
			public override void Close() {}
			public override void Flush() {}
			public override int Read(byte[] buffer, int offset, int count)
			{
				if (m_PullAudio != null)
				{
					GCHandle h = GCHandle.Alloc(buffer, GCHandleType.Pinned);
					try
					{
						m_PullAudio(new IntPtr(h.AddrOfPinnedObject().ToInt64() + offset), count);
					}
					finally
					{
						h.Free();
					}
				}
				else
				{
					for (int i = offset; i < offset + count; i++)
						buffer[i] = 0;
				}
				return count;
			}
			public override long Seek(long offset, System.IO.SeekOrigin origin) { return 0; }
			public override void SetLength(long length) {}
			public override void Write(byte[] buffer, int offset, int count) {}
			public override void WriteByte(byte value) {}

			private PullAudioCallback m_PullAudio;
		}

		/// <summary>
		/// Helper function for creating WaveFormat instances
		/// </summary>
		/// <param name="sr">Sampling rate</param>
		/// <param name="bps">Bits per sample</param>
		/// <param name="ch">Channels</param>
		/// <returns></returns>
		public static Microsoft.DirectX.DirectSound.WaveFormat CreateWaveFormat(int sr, short bps, short ch)
		{
			Microsoft.DirectX.DirectSound.WaveFormat wfx = new Microsoft.DirectX.DirectSound.WaveFormat();

			wfx.FormatTag = WaveFormatTag.Pcm;
			wfx.SamplesPerSecond = sr;
			wfx.BitsPerSample = bps;
			wfx.Channels = ch;

			wfx.BlockAlign = (short)(wfx.Channels * (wfx.BitsPerSample / 8));
			wfx.AverageBytesPerSecond = wfx.SamplesPerSecond * wfx.BlockAlign;

			return wfx;
		}

		public StreamingPlayer(Control owner, int sr, short bps, short ch) : 
			this(owner, null, CreateWaveFormat(sr, bps, ch))
		{
		}

		public StreamingPlayer(Control owner, Microsoft.DirectX.DirectSound.WaveFormat format) : 
			this(owner, null, format)
		{
		}

		public StreamingPlayer(Control owner, Device device, int sr, short bps, short ch) : 
			this(owner, device, CreateWaveFormat(sr, bps, ch))
		{
		}

		public StreamingPlayer(Control owner, Device device, Microsoft.DirectX.DirectSound.WaveFormat format)
		{
			m_Device = device;
			if (m_Device == null)
			{
				m_Device = new Device();
				m_Device.SetCooperativeLevel(owner, CooperativeLevel.Normal);
				m_OwnsDevice = true;
			}

			BufferDescription desc = new BufferDescription(format);
			desc.BufferBytes = format.AverageBytesPerSecond;
			desc.ControlVolume = true;
			desc.GlobalFocus = true;

			m_Buffer = new SecondaryBuffer(desc, m_Device);
			m_BufferBytes = m_Buffer.Caps.BufferBytes;

			m_Timer = new System.Timers.Timer(BytesToMs(m_BufferBytes) / 6);
			m_Timer.Enabled = false;
			m_Timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
		}

		~StreamingPlayer()
		{
			Dispose();
		}

		public void Dispose()
		{
			Stop();
			if (m_Timer != null)
			{
				m_Timer.Dispose();
				m_Timer = null;
			}
			if (m_Buffer != null)
			{
				m_Buffer.Dispose();
				m_Buffer = null;
			}
			if (m_OwnsDevice && m_Device != null)
			{
				m_Device.Dispose();
				m_Device = null;
			}
			GC.SuppressFinalize(this);
		}

		// IAudioPlayer

		public int SamplingRate { get { return m_Buffer.Format.SamplesPerSecond; }  }
		public int BitsPerSample { get { return m_Buffer.Format.BitsPerSample; } }
		public int Channels { get { return m_Buffer.Format.Channels; } }

		public void Play(PullAudioCallback pullAudio)
		{
			Stop();

			m_PullStream = new PullStream(pullAudio);

			m_Buffer.SetCurrentPosition(0);
			m_NextWrite = 0;
			Feed(m_BufferBytes);
			m_Timer.Enabled = true;
			m_Buffer.Play(0, BufferPlayFlags.Looping);
		}

		public void Stop()
		{
			if (m_Timer != null)
				m_Timer.Enabled = false;
			if (m_Buffer != null)
				m_Buffer.Stop();
		}

		public int GetBufferedSize()
		{
			int played = GetPlayedSize();
			return played > 0 && played < m_BufferBytes ? m_BufferBytes - played : 0;
		}

		private Device m_Device;
		private bool m_OwnsDevice;
		private SecondaryBuffer m_Buffer;
		private System.Timers.Timer m_Timer;
		private int m_NextWrite;
		private int m_BufferBytes;
		private Stream m_PullStream;

		public Device Device
		{
			get { return m_Device; }
		}

		private int BytesToMs(int bytes)
		{
			return bytes * 1000 / m_Buffer.Format.AverageBytesPerSecond;
		}

		private int MsToBytes(int ms)
		{
			int bytes = ms * m_Buffer.Format.AverageBytesPerSecond / 1000;
			bytes -= bytes % m_Buffer.Format.BlockAlign;
			return bytes;
		}

		private void Feed(int bytes)
		{
			// limit latency to some milliseconds
			int tocopy = Math.Min(bytes, MsToBytes(MaxLatencyMs));

			if (tocopy > 0)
			{
				// restore buffer
				if (m_Buffer.Status.BufferLost)
					m_Buffer.Restore();

				// copy data to the buffer
				m_Buffer.Write(m_NextWrite, m_PullStream, tocopy, LockFlag.None);

				m_NextWrite += tocopy;
				if (m_NextWrite >= m_BufferBytes)
					m_NextWrite -= m_BufferBytes;
			}
		}

		private int GetPlayedSize()
		{
			int pos = m_Buffer.PlayPosition;
			return pos < m_NextWrite ? pos + m_BufferBytes - m_NextWrite : pos - m_NextWrite;
		}

		private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			Feed(GetPlayedSize());
		}
	}
}
