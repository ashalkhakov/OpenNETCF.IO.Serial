//==========================================================================================
//
//		OpenNETCF.IO.Serial.GPS.Enums
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

	public enum CardinalDirection
	{
		/// <summary>
		/// North
		/// </summary>
		North = 0,
		/// <summary>
		/// East
		/// </summary>
		East = 1,
		/// <summary>
		/// South
		/// </summary>
		South = 2,
		/// <summary>
		/// West
		/// </summary>
		West = 4,
		/// <summary>
		/// Northwest
		/// </summary>
		NorthWest = 5,
		/// <summary>
		/// Northeast
		/// </summary>
		NorthEast = 6,
		/// <summary>
		/// Southwest
		/// </summary>
		SouthWest = 7,
		/// <summary>
		/// Southeast
		/// </summary>
		SouthEast = 8,
		/// <summary>
		/// Stationary
		/// </summary>
		Stationary = 9
	}

	public enum States
	{
		/// <summary>
		/// Auto-Discovery
		/// </summary>
		AutoDiscovery,
		/// <summary>
		/// Opening
		/// </summary>
		Opening,
		/// <summary>
		/// Running
		/// </summary>
		Running,
		/// <summary>
		/// Stopping
		/// </summary>
		Stopping,
		/// <summary>
		/// Stopped
		/// </summary>
		Stopped
	}

	public enum StatusType
	{
		NotSet,
		OK, //A
		Warning //V
	}

	public enum AutoDiscoverStates
	{
		Testing,
		FailedToOpen,
		Opened,
		Failed,
		NoGPSDetected
	}
	public enum Fix_Mode
	{
		Auto,
		Manual
	}
	public enum Fix_Indicator
	{
		NotSet,
		Mode2D,
		Mode3D
	}
	public enum Fix_Type
	{
		NotSet,
		NoAltitude,
		WithAltitude
	}
	public enum Fix_TypeMode
	{
		NotSet,
		SPS,
		DSPS,
		PPS,
		RTK
	}
	public enum Units
	{
		Kilometers,
		Miles,
		Knots
	}


}	

