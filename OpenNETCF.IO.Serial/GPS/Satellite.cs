//==========================================================================================
//
//		OpenNETCF.IO.Serial.GPS
//		Copyright (c) 2003-2009, OpenNETCF Consulting, LLC
//      http://www.opennetcf.com
//
//      Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
//      associated documentation files (the "Software"), to deal in the Software without restriction, 
//      including without limitation the rights to use, copy, modify, merge, publish, distribute, 
//      sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
//      furnished to do so, subject to the following conditions:
//
//      The above copyright notice and this permission notice shall be included in all copies or 
//      substantial portions of the Software.
// 
//      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
//      BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//      NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
//      DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
//      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//
//==========================================================================================
using System;
using OpenNETCF.IO.Serial;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;
using System.Globalization;

namespace OpenNETCF.IO.Serial.GPS
{

	public class Satellite
	{
		#region Initialization
		int id=0;
		int snr=0;
		int elevation=0;
		int azimuth=0;
		bool active=false;
		int channel=0;
		#endregion

		#region properties
		public int ID
		{
			get
			{
				return id;
			}
			set
			{
				id=value;
			}
		}

		public int SNR
		{
			get
			{
				return snr;
			}
			set
			{
				snr=value;
			}
		}

		public int Elevation
		{
			get
			{
				return elevation;
			}
			set
			{
				elevation=value;
			}
		}
		public int Azimuth
		{
			get
			{
				return azimuth;
			}
			set
			{
				azimuth=value;
			}
		}

		public bool Active
		{
			get
			{
				return active;
			}
			set
			{
				active=value;
			}
		}

		public int Channel
		{
			get
			{
				return channel;
			}
			set
			{
				channel=value;
			}
		}

		#endregion
	}
}